/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using CatWalk;

namespace GFV.ViewModel{
	using Gfl = GflNet;
	using IO = System.IO;

	public class ViewerWindowViewModel : ViewModelBase{
		public Gfl::Gfl Gfl{get; private set;}
		public ProgressManager ProgressManager{get; private set;}

		public ViewerWindowViewModel(Gfl::Gfl gfl){
			this.Gfl = gfl;
			this.ProgressManager = new ProgressManager();
		}

		private ViewerViewModel viewer;
		public ViewerViewModel Viewer{
			get{
				if(this.viewer == null){
					this.viewer = new ViewerViewModel(this.Gfl, this.ProgressManager);
				}
				return this.viewer;
			}
		}

		#region Property

		public string Title{
			get{
				//return "GFV " + this.Gfl.LoadedBitmapCount;
				return (this.Path != null) ? "GFV - " + this.Path : "GFV";
			}
		}

		private Gfl::Bitmap _Icon;
		public Gfl::Bitmap Icon{
			get{
				return this._Icon;
			}
		}

		#endregion

		#region OpenFile

		private string _Path;
		public string Path{
			get{
				return this._Path;
			}
			set{
				if(value == null){
					this._Icon = null;
					this.OnPropertyChanged("Icon");
					var bmp = this.Viewer.SourceBitmap;
					if(bmp != null){
						bmp.Dispose();
					}
					this.Viewer.SourceBitmap = null;
				}else{
					this.ReadFile(value);
				}
			}
		}

		private CancellationTokenSource _OpenFile_CancellationTokenSource;
		private void ReadFile(string file){
			var path = this._Path = IO.Path.GetFullPath(file);
			this.OnPropertyChanged("Path", "Title");

			// Load bitmap;
			if(this._OpenFile_CancellationTokenSource != null){
				this._OpenFile_CancellationTokenSource.Cancel();
			}
			this._OpenFile_CancellationTokenSource = new CancellationTokenSource();
			var ui = TaskScheduler.FromCurrentSynchronizationContext();
			var task1 = new Task<Gfl::MultiBitmap>(delegate{
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
					this.ProgressManager.ReportProgress(id, 0.5);
					return bitmap;
				}finally{
					this.ProgressManager.Complete(id);
				}
			}, this._OpenFile_CancellationTokenSource.Token);
			var task2 = task1.ContinueWith(delegate(Task<Gfl::MultiBitmap> t){
				var bitmap = t.Result;
				try{
					bitmap.LoadAllFrames();
					var bmp = bitmap[0];
					double scaleW = (32d / (double)bmp.Width);
					double scaleH = (32d / (double)bmp.Height);
					var scale = Math.Min(scaleW, scaleH);
					this._Icon = Gfl::Bitmap.Resize(bmp, (int)Math.Round(bmp.Width * scale), (int)Math.Round(bmp.Height * scale), this.Viewer.ResizeMethod);
					//this.ProgressManager.ReportProgress(bitmap, 1);
					return bitmap;
				}catch(Exception ex){
					this.ProgressManager.Complete(bitmap);
					throw ex;
				}
			}, this._OpenFile_CancellationTokenSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
			var task3 = task2.ContinueWith(delegate(Task<Gfl::MultiBitmap> t){
				var bitmap = t.Result;
				try{
					this.OnPropertyChanged("Icon");
					this.SetViewerBitmap(bitmap);
				}finally{
					//this.ProgressManager.Complete(bitmap);
				}
			}, this._OpenFile_CancellationTokenSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, ui);
			task1.ContinueWith(this.Bitmap_LoadError, CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, ui);
			task2.ContinueWith(this.Bitmap_LoadError, CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, ui);

			task1.Start();
		}
		
		private void Bitmap_LoadError(Task task){
			this.OnBitmapLoadFailed(new BitmapLoadFailedEventArgs(task.Exception));
			this._Icon = null;
			this.OnPropertyChanged("Icon");
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
				this._OpenFileDialog = value;
				this.OnPropertyChanged("OpenFileDialog");
			}
		}

		private DelegateUICommand<string> _OpenFileInNewWindowCommand;
		public ICommand OpenFileInNewWindowCommand{
			get{
				if(this._OpenFileInNewWindowCommand == null){
					this._OpenFileInNewWindowCommand = new DelegateUICommand<string>(this.OpenFileInNewWindow, null, false, "OpenFileInNewWindow", typeof(ViewerWindowViewModel));
					this._OpenFileInNewWindowCommand.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
				}
				return this._OpenFileInNewWindowCommand;
			}
		}

		private DelegateUICommand<string> _OpenFileCommand;
		public ICommand OpenFileCommand{
			get{
				if(this._OpenFileCommand == null){
					this._OpenFileCommand = new DelegateUICommand<string>(this.OpenFile, null, false, "OpenFile", typeof(ViewerWindowViewModel));
					this._OpenFileCommand.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
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
									this.Path = file;
								}else{
									if(newWindow || !isFirst){
										var vwvwvm = Program.CreateViewerWindow();
										vwvwvm.Item1.Show();
										vwvwvm.Item2.Path = file;
									}else{
										this.Path = file;
									}
								}
								isFirst = false;
							}
						}
					}
				}
			}else{
				this.Path = path;
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

		public event EventHandler RequestClose;

		private ICommand _CloseCommand;
		public ICommand CloseCommand{
			get{
				if(this._CloseCommand == null){
					this._CloseCommand = new DelegateCommand(this.Close);
				}
				return this._CloseCommand;
			}
		}

		public void Close(){
			var handler = this.RequestClose;
			if(handler != null){
				handler(this, EventArgs.Empty);
			}
		}

		#endregion

		#region Next / Prev File

		private ICommand _NextFileCommand;
		public ICommand NextFileCommand{
			get{
				if(this._NextFileCommand == null){
					this._NextFileCommand = new DelegateCommand(this.NextFile, this.CanNextFile);
				}
				return this._NextFileCommand;
			}
		}

		public void NextFile(){
			this.Path = this.GetNextFile();
		}

		private string GetNextFile(){
			var exts = Program.Gfl.Formats.Select(fmt => fmt.Extensions).Flatten().Select(ext => "." + ext);
			string firstFile = null;
			var isNext = false;
			foreach(var file in IO.Directory.EnumerateFiles(IO.Path.GetDirectoryName(this._Path))
				.Where(file => IO.Path.GetExtension(file)
					.Let(ext => exts.Where(e => e.Equals(ext, StringComparison.OrdinalIgnoreCase)).FirstOrDefault() != null))){
				if(firstFile == null){
					firstFile = file;
				}
				if(isNext){
					return file;
				}
				if(file.Equals(this._Path, StringComparison.OrdinalIgnoreCase)){
					isNext = true;
				}
			}
			return (isNext) ? firstFile : null;
		}

		public bool CanNextFile(){
			return !String.IsNullOrEmpty(this._Path);
		}

		private ICommand _PreviousFileCommand;
		public ICommand PreviousFileCommand{
			get{
				if(this._PreviousFileCommand == null){
					this._PreviousFileCommand = new DelegateCommand(this.PreviousFile, this.CanPreviousFile);
				}
				return this._PreviousFileCommand;
			}
		}

		public void PreviousFile(){
			this.Path = this.GetPreviousFile();
		}
		private string GetPreviousFile(){
			var exts = Program.Gfl.Formats.Select(fmt => fmt.Extensions).Flatten().Select(ext => "." + ext);
			string firstFile = null;
			var isNext = false;
			foreach(var file in IO.Directory.EnumerateFiles(IO.Path.GetDirectoryName(this._Path))
				.Where(file => IO.Path.GetExtension(file)
					.Let(ext => exts.Where(e => e.Equals(ext, StringComparison.OrdinalIgnoreCase)).FirstOrDefault() != null))
						.Reverse()){
				if(firstFile == null){
					firstFile = file;
				}
				if(isNext){
					return file;
				}
				if(file.Equals(this._Path, StringComparison.OrdinalIgnoreCase)){
					isNext = true;
				}
			}
			return (isNext) ? firstFile : null;
		}

		public bool CanPreviousFile(){
			return !String.IsNullOrEmpty(this._Path);
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
}
