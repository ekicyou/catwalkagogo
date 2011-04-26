/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using CatWalk;
using GFV.Properties;
using System.Windows.Shell;

namespace GFV.ViewModel{
	using Gfl = GflNet;
	using IO = System.IO;

	public class ViewerWindowViewModel : ViewModelBase, IDisposable{
		public Gfl::Gfl Gfl{get; private set;}
		public ProgressManager ProgressManager{get; private set;}

		public ViewerWindowViewModel(Gfl::Gfl gfl){
			if(gfl == null){
				throw new ArgumentNullException("gfl");
			}
			this.Gfl = gfl;
			this.ProgressManager = new ProgressManager();
		}

		private ViewerViewModel _Viewer;
		public ViewerViewModel Viewer{
			get{
				if(this._Viewer == null){
					this._Viewer = new ViewerViewModel(this.Gfl, this.ProgressManager);
				}
				return this._Viewer;
			}
		}

		#region Property

		public string Title{
			get{
				//return "GFV " + this.Gfl.LoadedBitmapCount;
				return (this.CurrentFilePath != null) ? "GFV - " + this.CurrentFilePath : "GFV";
			}
		}

		private Gfl::Bitmap _Icon;
		public Gfl::Bitmap Icon{
			get{
				return this._Icon;
			}
			private set{
				this.OnPropertyChanging("Icon");
				this._Icon = value;
				this.OnPropertyChanged("Icon");
			}
		}

		#endregion

		#region OpenFile

		private string _CurrentFilePath;
		public string CurrentFilePath{
			get{
				return this._CurrentFilePath;
			}
			set{
				this.OnPropertyChanging("CurrentFilePath", "Title");
				this._CurrentFilePath = IO.Path.GetFullPath(value);
				this.OnPropertyChanged("CurrentFilePath", "Title");
				if(this._CurrentFilePath == null){
					this.Icon = null;
					var bmp = this.Viewer.SourceBitmap;
					if(bmp != null){
						bmp.Dispose();
					}
					this.Viewer.SourceBitmap = null;
				}else{
					this.ReadFile(this._CurrentFilePath);
				}
			}
		}

		private CancellationTokenSource _OpenFile_CancellationTokenSource;
		private void ReadFile(string file){
			var path = IO.Path.GetFullPath(file);

			// Add to history
			JumpList.AddToRecentCategory(file);
			Settings.Default.RecentFiles = Enumerable.Concat(Seq.Make(file), Settings.Default.RecentFiles.EmptyIfNull()).Distinct().Take(16).ToArray();

			// Load bitmap;
			if(this._OpenFile_CancellationTokenSource != null){
				this._OpenFile_CancellationTokenSource.Cancel();
			}
			this._OpenFile_CancellationTokenSource = new CancellationTokenSource();
			var ui = TaskScheduler.FromCurrentSynchronizationContext();
			var task1 = new Task<Tuple<Gfl::MultiBitmap, object>>(delegate{
				Gfl::MultiBitmap bitmap = null;
				var id = this.ProgressManager.AddJob();
				try{
					bitmap = this.Gfl.LoadMultiBitmap(path);
					bitmap.LoadParameters.BitmapType = Gfl::BitmapType.Bgra;
					bitmap.LoadParameters.Options = Gfl::LoadOptions.ForceColorModel | Gfl::LoadOptions.IgnoreReadError;
					bitmap.LoadParameters.ProgressChanged += Bitmap_LoadProgressChanged;
					bitmap.LoadParameters.WantCancel += Bitmap_WantCancel;
					bitmap.FrameLoading += this.Bitmap_FrameLoading;
					bitmap.FrameLoaded += this.Bitmap_FrameLoaded;
					this.ProgressManager.ReportProgress(id, 1);
					return new Tuple<Gfl::MultiBitmap, object>(bitmap, id);
				}catch(Exception ex){
					this.ProgressManager.Complete(id);
					throw ex;
				}
			}, this._OpenFile_CancellationTokenSource.Token);
			var task2 = task1.ContinueWith(delegate(Task<Tuple<Gfl::MultiBitmap, object>> t){
				var bitmap = t.Result.Item1;
				var id = t.Result.Item2;
				bitmap.LoadAllFrames();
				var bmp = bitmap[0];
				double scaleW = (32d / (double)bmp.Width);
				double scaleH = (32d / (double)bmp.Height);
				var scale = Math.Min(scaleW, scaleH);
				this.Icon = Gfl::Bitmap.Resize(bmp, (int)Math.Round(bmp.Width * scale), (int)Math.Round(bmp.Height * scale), GflNet.ResizeMethod.Lanczos);
				return new Tuple<Gfl::MultiBitmap, object>(bitmap, id);
			}, this._OpenFile_CancellationTokenSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
			var task3 = task2.ContinueWith(delegate(Task<Tuple<Gfl::MultiBitmap, object>> t){
				var bitmap = t.Result.Item1;
				var id = t.Result.Item2;
				try{
					this.SetViewerBitmap(bitmap);
				}finally{
					this.ProgressManager.Complete(id);
				}
			}, this._OpenFile_CancellationTokenSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, ui);
			task1.ContinueWith(this.Bitmap_LoadError, CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, ui);
			task2.ContinueWith(this.Bitmap_LoadError, CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, ui);

			task1.Start();
		}
		
		private void Bitmap_LoadError(Task task){
			this.OnBitmapLoadFailed(new BitmapLoadFailedEventArgs(task.Exception));
			this.Icon = null;
			this.SetViewerBitmap(null);
		}

		private void SetViewerBitmap(Gfl::MultiBitmap bitmap){
			var old = this.Viewer.SourceBitmap;
			this.Viewer.SourceBitmap = bitmap;
			if(old != null){
				old.Dispose();
			}
		}

		private void Bitmap_FrameLoading(object sender, EventArgs e){
			this.ProgressManager.Start(sender);
		}

		private void Bitmap_LoadProgressChanged(object sender, Gfl::ProgressEventArgs e){
			this.ProgressManager.ReportProgress(sender, (double)e.ProgressPercentage / 100d);
		}

		private void Bitmap_FrameLoaded(object sender, Gfl::FrameLoadedEventArgs e){
			this.ProgressManager.Complete(sender);
		}


		private void Bitmap_WantCancel(object sender, CancelEventArgs e){
			if((this._OpenFile_CancellationTokenSource != null) && (this._OpenFile_CancellationTokenSource.IsCancellationRequested)){
				e.Cancel = true;
				this.ProgressManager.Complete(sender);
			}
		}

		private IOpenFileDialog _OpenFileDialog;
		public IOpenFileDialog OpenFileDialog{
			get{
				return this._OpenFileDialog;
			}
			set{
				this.OnPropertyChanging("OpenFileDialog");
				this._OpenFileDialog = value;
				this.OnPropertyChanged("OpenFileDialog");
			}
		}

		private DelegateUICommand<string> _OpenFileInNewWindowCommand;
		public ICommand OpenFileInNewWindowCommand{
			get{
				if(this._OpenFileInNewWindowCommand == null){
					this._OpenFileInNewWindowCommand = new DelegateUICommand<string>(this.OpenFileInNewWindow);
				}
				return this._OpenFileInNewWindowCommand;
			}
		}

		private DelegateUICommand<string> _OpenFileCommand;
		public DelegateUICommand<string> OpenFileCommand{
			get{
				if(this._OpenFileCommand == null){
					this._OpenFileCommand = new DelegateUICommand<string>(this.OpenFile);
				}
				return this._OpenFileCommand;
			}
		}

		public void OpenFile(string path){this.OpenFile(path, false);}
		public void OpenFileInNewWindow(string path){this.OpenFile(path, true);}
		public void OpenFile(string path, bool newWindow){
			if(String.IsNullOrEmpty(path)){
				if(this.OpenFileDialog != null){
					var dlg = this.OpenFileDialog;
					dlg.Reset();
					var formats = this.Gfl.Formats.Where(fmt => fmt.Readable);
					dlg.Filters.Add(new FileDialogFilter(
						"All Images",
						String.Join(";", formats.Select(fmt => fmt.Extensions).Flatten().Select(ext => "*." + ext))));
					dlg.Filters.Add(new FileDialogFilter("All Files (*.*)", "*.*"));
					foreach(var format in formats.OrderBy(fmt => fmt.Description)){
						var mask = String.Join(";", format.Extensions.Select(ext => "*." + ext));
						dlg.Filters.Add(new FileDialogFilter(
							format.Description + " (" + mask + ")", mask));
					}
					dlg.IsCheckFileExists = dlg.IsCheckPathExists = dlg.IsMultiselect = dlg.IsValidNames = dlg.IsAddExtension = true;

					if(dlg.ShowDialog().Value){
						if(dlg.FileNames.Length > 0){
							var isFirst = true;
							foreach(var file in dlg.FileNames){
								if(this.Viewer.SourceBitmap == null){
									this.CurrentFilePath = file;
								}else{
									if(newWindow || !isFirst){
										var vwvwvm = Program.CurrentProgram.CreateViewerWindow();
										vwvwvm.View.Show();
										vwvwvm.ViewModel.CurrentFilePath = file;
									}else{
										this.CurrentFilePath = file;
									}
								}
								isFirst = false;
							}
						}
					}
				}
			}else{
				this.CurrentFilePath = path;
			}
		}

		#endregion

		#region BitmapLoadFailed

		public event BitmapLoadFailedEventHandler BitmapLoadFailed;

		protected void OnBitmapLoadFailed(BitmapLoadFailedEventArgs e){
			var eh = this.BitmapLoadFailed;
			if(eh != null){
				eh(this, e);
			}
		}

		#endregion

		#region Close

		private ICommand _CloseCommand;
		public ICommand CloseCommand{
			get{
				return this._CloseCommand ?? (this._CloseCommand = new DelegateUICommand(this.Close));
			}
		}

		public void Close(){
			Messenger.Default.Send<CloseMessage>(new CloseMessage(this), this);
		}

		#endregion

		#region Next / Prev File

		private ICommand _NextFileCommand;
		public ICommand NextFileCommand{
			get{
				return this._NextFileCommand ?? (this._NextFileCommand = new DelegateUICommand(this.NextFile, this.CanNextFile));
			}
		}

		public void NextFile(){
			this.CurrentFilePath = this.GetNextFile();
		}

		private string GetNextFile(){
			var exts = this.Gfl.Formats.Select(fmt => fmt.Extensions).Flatten().Select(ext => "." + ext);
			string firstFile = null;
			var isNext = false;
			foreach(var file in IO.Directory.EnumerateFiles(IO.Path.GetDirectoryName(this._CurrentFilePath))
				.Where(file => IO.Path.GetExtension(file)
					.Let(ext => exts.Where(e => e.Equals(ext, StringComparison.OrdinalIgnoreCase)).FirstOrDefault() != null))){
				if(firstFile == null){
					firstFile = file;
				}
				if(isNext){
					return file;
				}
				if(file.Equals(this._CurrentFilePath, StringComparison.OrdinalIgnoreCase)){
					isNext = true;
				}
			}
			return (isNext) ? firstFile : null;
		}

		public bool CanNextFile(){
			return !String.IsNullOrEmpty(this._CurrentFilePath);
		}

		private ICommand _PreviousFileCommand;
		public ICommand PreviousFileCommand{
			get{
				return this._PreviousFileCommand ?? (this._PreviousFileCommand = new DelegateUICommand(this.PreviousFile, this.CanPreviousFile));
			}
		}

		public void PreviousFile(){
			this.CurrentFilePath = this.GetPreviousFile();
		}
		private string GetPreviousFile(){
			var exts = this.Gfl.Formats.Select(fmt => fmt.Extensions).Flatten().Select(ext => "." + ext);
			string firstFile = null;
			var isNext = false;
			foreach(var file in IO.Directory.EnumerateFiles(IO.Path.GetDirectoryName(this._CurrentFilePath))
				.Where(file => IO.Path.GetExtension(file)
					.Let(ext => exts.Where(e => e.Equals(ext, StringComparison.OrdinalIgnoreCase)).FirstOrDefault() != null))
						.Reverse()){
				if(firstFile == null){
					firstFile = file;
				}
				if(isNext){
					return file;
				}
				if(file.Equals(this._CurrentFilePath, StringComparison.OrdinalIgnoreCase)){
					isNext = true;
				}
			}
			return (isNext) ? firstFile : null;
		}

		public bool CanPreviousFile(){
			return !String.IsNullOrEmpty(this._CurrentFilePath);
		}


		#endregion

		#region About

		private ICommand _AboutCommand;
		public ICommand AboutCommand{
			get{
				return this._AboutCommand ?? (this._AboutCommand = new DelegateUICommand(this.About));
			}
		}

		public void About(){
			Messenger.Default.Send<AboutMessage>(new AboutMessage(this), this);
		}

		#endregion

		#region Exit

		private DelegateUICommand _ExitCommand;
		public ICommand ExitCommand{
			get{
				return this._ExitCommand ?? (this._ExitCommand = new DelegateUICommand(this.Exit));
			}
		}

		public void Exit(){
			Application.Current.Shutdown();
		}

		#endregion

		#region OpenNewWindow

		private DelegateUICommand _OpenNewWindowCommand;
		public ICommand OpenNewWindowCommand{
			get{
				return this._OpenNewWindowCommand ?? (this._OpenNewWindowCommand = new DelegateUICommand(this.OpenNewWindow));
			}
		}

		public void OpenNewWindow(){
			var pair = Program.CurrentProgram.CreateViewerWindow();
			pair.ViewModel.CurrentFilePath = this.CurrentFilePath;
			pair.View.Show();
		}

		#endregion

		#region Show Menubar

		private ICommand _ShowMenubarCommand;
		public ICommand ShowMenubarCommand{
			get{
				return this._ShowMenubarCommand ?? (this._ShowMenubarCommand = new DelegateUICommand<bool?>(this.ShowMenubar));
			}
		}

		public void ShowMenubar(bool? visibility){
			if(Settings.Default.IsShowMenubar == null){
				Settings.Default.IsShowMenubar = false;
			}
			if(visibility == null){	// toggle
				Settings.Default.IsShowMenubar = !Settings.Default.IsShowMenubar;
			}else{
				Settings.Default.IsShowMenubar = visibility.Value;
			}
		}

		#endregion

		#region IDisposable Members

		~ViewerWindowViewModel(){
			this.Dispose(true);
		}

		public void Dispose(){
			this.Dispose(false);
			GC.SuppressFinalize(this);
		}

		private bool _Dispose = false;
		protected virtual void Dispose(bool disposing){
			if(!this._Dispose){
				if(this._Viewer != null){
					this._Viewer.Dispose();
				}
				this._Dispose = true;
			}
		}

		#endregion
	}

	public delegate void BitmapLoadFailedEventHandler(object sender, BitmapLoadFailedEventArgs e);

	public class BitmapLoadFailedEventArgs : EventArgs{
		public AggregateException Exception{get; private set;}

		public BitmapLoadFailedEventArgs(AggregateException ex){
			this.Exception = ex;
		}
	}

	public class CloseMessage : MessageBase{
		public CloseMessage(object sender) : base(sender){}
	}

	public class AboutMessage : MessageBase{
		public AboutMessage(object sender) : base(sender){}
	}
}
