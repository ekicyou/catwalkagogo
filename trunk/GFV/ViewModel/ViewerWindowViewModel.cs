/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shell;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using CatWalk;
using CatWalk.Windows;
using CatWalk.Windows.ViewModel;
using GFV.Properties;

namespace GFV.ViewModel{
	using Gfl = GflNet;
	using IO = System.IO;
	using Win32 = CatWalk.Win32;

	[SendMessage(typeof(CloseMessage))]
	[SendMessage(typeof(AboutMessage))]
	[SendMessage(typeof(SetRectMessage))]
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
				this.SetCurrentFilePath(value, true);
			}
		}

		private void SetCurrentFilePath(string path, bool addHistory){
			this.OnPropertyChanging("CurrentFilePath", "Title");
			this._CurrentFilePath = IO.Path.GetFullPath(path);
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

		private CancellationTokenSource _OpenFile_CancellationTokenSource;
		private Semaphore _OpenFile_Semaphore = new Semaphore(1, 1);
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
			var token = this._OpenFile_CancellationTokenSource.Token;
			// Load bitmap from file
			var task1 = new Task<Tuple<Gfl::MultiBitmap, object>>(delegate{
				this._OpenFile_Semaphore.WaitOne();
				var id = this.ProgressManager.AddJob();
				try{
					token.ThrowIfCancellationRequested();

					Gfl::MultiBitmap bitmap = null;
					bitmap = this.Gfl.LoadMultiBitmap(path);
					bitmap.LoadParameters.BitmapType = Gfl::BitmapType.Bgra;
					bitmap.LoadParameters.Options = Gfl::LoadOptions.ForceColorModel | Gfl::LoadOptions.IgnoreReadError;
					bitmap.LoadParameters.ProgressChanged += Bitmap_LoadProgressChanged;
					bitmap.LoadParameters.WantCancel += Bitmap_WantCancel;
					bitmap.FrameLoading += this.Bitmap_FrameLoading;
					bitmap.FrameLoaded += this.Bitmap_FrameLoaded;
				
					token.ThrowIfCancellationRequested();

					var bmp = bitmap[0];

					token.ThrowIfCancellationRequested();

					this.ProgressManager.ReportProgress(id, 0.75);
					return new Tuple<Gfl::MultiBitmap, object>(bitmap, id);
				}catch(Win32Exception ex){ // Unknown Exception とりあえず握りつぶし
					throw new OperationCanceledException(ex.Message, ex, token);
				}catch(Exception ex){
					this.ProgressManager.Complete(id);
					throw ex;
				}finally{
					this._OpenFile_Semaphore.Release();
				}
			}, this._OpenFile_CancellationTokenSource.Token);
			var task2 = task1.ContinueWith(delegate(Task<Tuple<Gfl::MultiBitmap, object>> t){
				var bitmap = t.Result.Item1;
				var id = t.Result.Item2;
				try{
					this.SetViewerBitmap(bitmap);
				}finally{
					this.ProgressManager.Complete(id);
				}
			}, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, ui);
			var task3 = task1.ContinueWith(delegate(Task<Tuple<Gfl::MultiBitmap, object>> t){
				this._OpenFile_Semaphore.WaitOne();
				try{
					var bitmap = t.Result.Item1;
					var id = t.Result.Item2;
					var bmp = bitmap[0];
					double scaleW = (32d / (double)bmp.Width);
					double scaleH = (32d / (double)bmp.Height);
					var scale = Math.Min(scaleW, scaleH);
					this.Icon = Gfl::Bitmap.Resize(bmp, (int)Math.Round(bmp.Width * scale), (int)Math.Round(bmp.Height * scale), GflNet.ResizeMethod.Lanczos);
				}catch(ObjectDisposedException){
				}finally{
					this._OpenFile_Semaphore.Release();
				}
			}, this._OpenFile_CancellationTokenSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
			task1.ContinueWith(this.Bitmap_LoadError, CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, ui);
			task3.ContinueWith(this.Bitmap_LoadError, CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, ui);

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
			this.ProgressManager.ReportProgress(sender, (double)e.ProgressPercentage / 100d / 2d);
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
			this.SetCurrentFilePath(this.GetNextFile(), false);
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
			this.SetCurrentFilePath(this.GetPreviousFile(), false);
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

		#region Arrange Window

		private DelegateUICommand<ArrangeMode> _ArrangeWindowsCommand;
		public ICommand ArrangeWindowsCommand{
			get{
				return this._ArrangeWindowsCommand ?? (this._ArrangeWindowsCommand = new DelegateUICommand<ArrangeMode>(this.ArrangeWindows));
			}
		}

		public void ArrangeWindows(ArrangeMode mode){
			Arranger arranger = null;
			switch(mode){
				case ArrangeMode.Cascade: arranger = new CascadeArranger(); break;
				case ArrangeMode.TileHorizontal: arranger = new TileHorizontalArranger(); break;
				case ArrangeMode.TileVertical: arranger = new TileVerticalArranger(); break;
				case ArrangeMode.StackHorizontal: arranger = new StackHorizontalArranger(); break;
				case ArrangeMode.StackVertical: arranger = new StackVerticalArranger(); break;
			}

			var window = Program.CurrentProgram.ViewerWindows.First(pair => pair.ViewModel == this).View;
			var screen = Win32::Screen.GetCurrentMonitor(new CatWalk.Int32Rect((int)window.Left, (int)window.Top, (int)window.Width, (int)window.Height));
			if(screen == null){
				return;
			}

			var windows = SortWindowsTopToBottom(Program.CurrentProgram.ViewerWindows)
				.Where(pair => Win32::Screen.GetCurrentMonitor(new CatWalk.Int32Rect((int)pair.View.Left, (int)pair.View.Top, (int)pair.View.Width, (int)pair.View.Height)) == screen).ToArray();
			var size = new Size(screen.WorkingArea.Width, screen.WorkingArea.Height);

			var i = 0;
			foreach(var rect in arranger.Arrange(size, windows.Length)){
				rect.Offset(screen.WorkingArea.Left, screen.WorkingArea.Top);
				Messenger.Default.Send(new SetRectMessage(this, rect), windows[i].ViewModel);
				i++;
			}
		}

		private IEnumerable<ViewViewModelPair<GFV.Windows.ViewerWindow, ViewerWindowViewModel>> SortWindowsTopToBottom(IEnumerable<ViewViewModelPair<GFV.Windows.ViewerWindow, ViewerWindowViewModel>> unsorted) {
			var byHandle = unsorted.ToDictionary(win => ((HwndSource)PresentationSource.FromVisual(win.View)).Handle);

			for(IntPtr hWnd = GetTopWindow(IntPtr.Zero); hWnd != IntPtr.Zero; hWnd = GetNextWindow(hWnd, GW_HWNDNEXT)){
				ViewViewModelPair<GFV.Windows.ViewerWindow, ViewerWindowViewModel> v;
				if(byHandle.TryGetValue(hWnd, out v)){
					yield return v;
				}
			}
		}

		private const uint GW_HWNDNEXT = 2;
		[DllImport("User32")]
		private static extern IntPtr GetTopWindow(IntPtr hWnd);
		[DllImport("User32", EntryPoint="GetWindow")]
		private static extern IntPtr GetNextWindow(IntPtr hWnd, uint wCmd);

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

	public enum ArrangeMode{
		Cascade,
		TileHorizontal,
		TileVertical,
		StackHorizontal,
		StackVertical,
	}

	public class SetRectMessage : MessageBase{
		public Rect Rect{get; private set;}

		public SetRectMessage(object sender, Rect rect) : base(sender){
			this.Rect = rect;
		}
	}
}
