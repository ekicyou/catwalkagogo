/*
	$Id: ViewerWindowViewModel.cs 297 2011-11-22 20:54:41Z catwalkagogo@gmail.com $
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
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using CatWalk;
using CatWalk.Windows;
using CatWalk.Mvvm;
using CatWalk.Text;
using GFV.Properties;
using GFV.Imaging;
using GFV.Messaging;
using GFV.Windows;

namespace GFV.ViewModel{
	using IO = System.IO;
	using Win32 = CatWalk.Win32;

	[SendMessage(typeof(CloseMessage))]
	[SendMessage(typeof(AboutMessage))]
	[SendMessage(typeof(ArrangeWindowsMessage))]
	[SendMessage(typeof(ErrorMessage))]
	[ReceiveMessage(typeof(OpenFileMessage))]
	public class ViewerWindowViewModel : ViewModelBase, IDisposable{
		public IImageLoader Loader{get; private set;}
		public FileInfoComparer FileInfoComparer{get; private set;}
		public MainWindowViewModel Owner{get; internal set;}

		public ViewerWindowViewModel(MainWindowViewModel owner, IImageLoader loader){
			this.Owner = owner;
			this.Loader = loader;
			this.FileInfoComparer = GetFileInfoComparer();

			Messenger.Default.Register<OpenFileMessage>(this.ReceiveOpenFileMessage, this);

			Settings.Default.PropertyChanged += new PropertyChangedEventHandler(Settings_PropertyChanged);
		}

		private void ReceiveOpenFileMessage(OpenFileMessage message){
			this.ReadFile(message.File);
		}

		private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			switch(e.PropertyName){
				case "PrimarySortKey":
				case "PrimarySortOrder":
				case "SecondarySortKey":
				case "SecondarySortOrder":
					this.FileInfoComparer = GetFileInfoComparer();
					break;
			}
		}

		private ViewerViewModel _Viewer;
		public ViewerViewModel Viewer{
			get{
				if(this._Viewer == null){
					this._Viewer = new ViewerViewModel(this.Loader, this.Owner.ProgressManager);
				}
				return this._Viewer;
			}
		}

		private static FileInfoComparer GetFileInfoComparer(){
			return new FileInfoComparer(){
				PrimaryKey = Settings.Default.PrimarySortKey,
				PrimaryOrder = Settings.Default.PrimarySortOrder,
				SecondaryKey = Settings.Default.SecondarySortKey,
				SecondaryOrder = Settings.Default.SecondarySortOrder,
			};
		}

		#region Property

		public string Title{
			get{
				//return "GFV " + this.Gfl.LoadedBitmapCount;
				return (this.CurrentFilePath != null) ? "GFV - " + this.CurrentFilePath : "GFV";
			}
		}

		private BitmapSource _Icon;
		public BitmapSource Icon{
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

		#region CurrentFilePath

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
			if(path != null){
				path = IO.Path.GetFullPath(path);
				this._CurrentFilePath = path;
			}
			this.OnPropertyChanged("CurrentFilePath", "Title");
			if(this._CurrentFilePath == null){
				this.Icon = null;
				var bmp = this.Viewer.SourceBitmap;
				this.Viewer.SourceBitmap = null;
			}else{
				if(addHistory){
					// Add to history
					try{
						JumpList.AddToRecentCategory(path);
					}catch(ArgumentException){
					}
					Settings.Default.RecentFiles = Enumerable.Concat(Seq.Make(path), Settings.Default.RecentFiles.EmptyIfNull()).Distinct().Take(16).ToArray();
				}
				this.ReadFile(this._CurrentFilePath);
			}
		}

		#endregion

		#region ReadFile

		private struct ReadFileTaskParam{
			public IMultiBitmap Bitmap{get; private set;}
			public string Path{get; private set;}
			public ReadFileTaskParam(IMultiBitmap bitmap, string path) : this(){
				this.Bitmap = bitmap;
				this.Path = path;
			}
		}

		private bool _OpenFile_IsBusy = false;
		private bool OpenFile_IsBusy{
			get{
				return this._OpenFile_IsBusy;
			}
			set{
				this._OpenFile_IsBusy = value;
				CommandManager.InvalidateRequerySuggested();
			}
		}
		private CancellationTokenSource _OpenFile_CancellationTokenSource;
		private void ReadFile(string file){
			var path = IO.Path.GetFullPath(file);
			
			this.OpenFile_IsBusy = true;
			// Load bitmap;
			if(this._OpenFile_CancellationTokenSource != null){
				this._OpenFile_CancellationTokenSource.Cancel();
			}
			this._OpenFile_CancellationTokenSource = new CancellationTokenSource();
			var ui = TaskScheduler.FromCurrentSynchronizationContext();
			var token = this._OpenFile_CancellationTokenSource.Token;
			// Load bitmap from file
			var task1 = new Task<ReadFileTaskParam>(delegate{
				try{
					token.ThrowIfCancellationRequested();

					var stream = new IO::BufferedStream(IO::File.OpenRead(path), 1024 * 16);
					//var stream = IO::File.OpenRead(path);
					var bitmap = this.Loader.Load(stream, this._OpenFile_CancellationTokenSource.Token);
					bitmap.ProgressChanged += this.Bitmap_LoadProgressChanged;
					bitmap.LoadStarted += this.Bitmap_FrameLoading;
					bitmap.LoadFailed += new Imaging.BitmapLoadFailedEventHandler(MultiBitmap_LoadFailed);
					bitmap.LoadCompleted += this.Bitmap_FrameLoaded;
				
					token.ThrowIfCancellationRequested();

					if(bitmap.IsPreloadRequired){
						bitmap.PreloadAllFrames();
						token.ThrowIfCancellationRequested();
					}

					return new ReadFileTaskParam(bitmap, path);
				}catch(Win32Exception ex){ // Unknown Exception とりあえず握りつぶし
					this.OpenFile_IsBusy = false;
					throw new OperationCanceledException(ex.Message, ex, token);
				}catch(OperationCanceledException ex){
					this.OpenFile_IsBusy = false;
					throw ex;
				}catch(AggregateException ex){
					var message = String.Join("\n", ex.InnerExceptions.Select(ex2 => ex2.Message));
					this.OpenFile_IsBusy = false;
					Messenger.Default.Send(new ErrorMessage(this, message + "\n\n" + path, ex), this);
					throw ex;
				}catch(Exception ex){
					this.OpenFile_IsBusy = false;
					Messenger.Default.Send(new ErrorMessage(this, ex.Message + "\n\n" + path, ex), this);
					throw ex;
				}
			}, this._OpenFile_CancellationTokenSource.Token);
			var task2 = task1.ContinueWith(delegate(Task<ReadFileTaskParam> t){
				try{
					this.SetViewerBitmap(t.Result.Bitmap);
				}finally{
					this.OpenFile_IsBusy = false;
				}
			}, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, ui);
			var task3 = task1.ContinueWith(delegate(Task<ReadFileTaskParam> t){
				try{
					var bitmap = t.Result.Bitmap;
					var bmp = bitmap[0];
					double scaleW = (32d / (double)bmp.PixelWidth);
					double scaleH = (32d / (double)bmp.PixelHeight);
					var scale = Math.Min(scaleW, scaleH);
					this.Icon = bitmap.GetThumbnail();
				}catch{
				}
			}, this._OpenFile_CancellationTokenSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
			task1.ContinueWith(this.Bitmap_LoadError, CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, ui);

			task1.Start();
		}

		private void MultiBitmap_LoadFailed(object sender, BitmapLoadFailedEventArgs e) {
			Messenger.Default.Send(new ErrorMessage(this, e.Exception.Message, e.Exception), this);
			this.Owner.ProgressManager.Complete(sender);
			//this.OnBitmapLoadFailed(e);
		}
		
		private void Bitmap_LoadError(Task task){
			//Messenger.Default.Send(new ErrorMessage(this, task.Exception.Message, task.Exception));
			this.Icon = null;
			this.SetViewerBitmap(null);
		}

		private void SetViewerBitmap(IMultiBitmap bitmap){
			var old = this.Viewer.SourceBitmap;
			this.Viewer.SourceBitmap = bitmap;
		}

		private void Bitmap_FrameLoading(object sender, EventArgs e){
			//lock(this.Owner.ProgressManager){
			//	if(!this.Owner.ProgressManager.Contains(sender)){
					this.Owner.ProgressManager.Start(sender);
			//	}
			//}
		}

		private void Bitmap_LoadProgressChanged(object sender, ProgressEventArgs e){
			this.Owner.ProgressManager.ReportProgress(sender, e.Progress);
		}

		private void Bitmap_FrameLoaded(object sender, EventArgs e){
			this.Owner.ProgressManager.Complete(sender);
		}

		private void Bitmap_WantCancel(object sender, CancelEventArgs e){
			if((this._OpenFile_CancellationTokenSource != null) && (this._OpenFile_CancellationTokenSource.IsCancellationRequested)){
				e.Cancel = true;
				this.Owner.ProgressManager.Complete(sender);
			}
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
			try{
				var files = IO.Directory.EnumerateFiles(IO.Path.GetDirectoryName(this._CurrentFilePath))
					.Select(path => new IO::FileInfo(path))
					.OrderBy(info => info, this.FileInfoComparer)
					.Where(fn => Program.CurrentProgram.IsSupportedFormat(fn.Extension));
				var first = files.FirstOrDefault();
				if(first == null){
					return null;
				}
				var file = files.Concat(Seq.Make(first))
					.SkipWhile(info => !info.FullName.Equals(this._CurrentFilePath, StringComparison.OrdinalIgnoreCase))
					.Skip(1)
					.FirstOrDefault();
				return (file ?? first).FullName;
			}catch(Exception){
			}
			return null;
		}

		public bool CanNextFile(){
			return !String.IsNullOrEmpty(this._CurrentFilePath) && !this._OpenFile_IsBusy;
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
			try{
				var files = IO.Directory.EnumerateFiles(IO.Path.GetDirectoryName(this._CurrentFilePath))
					.Select(path => new IO::FileInfo(path))
					.OrderBy(info => info, this.FileInfoComparer)
					.Reverse()
					.Where(fn => Program.CurrentProgram.IsSupportedFormat(fn.Extension));
				var first = files.FirstOrDefault();
				if(first == null){
					return null;
				}
				var file = files.Concat(Seq.Make(first))
					.SkipWhile(info => !info.FullName.Equals(this._CurrentFilePath, StringComparison.OrdinalIgnoreCase))
					.Skip(1)
					.FirstOrDefault();
				return (file ?? first).FullName;
			}catch(Exception){
			}
			return null;
		}

		public bool CanPreviousFile(){
			return !String.IsNullOrEmpty(this._CurrentFilePath) && !this._OpenFile_IsBusy;
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

	public class FileInfoComparer : IComparer<IO::FileInfo>{
		public FileInfoSortKey PrimaryKey{get; set;}
		public SortOrder PrimaryOrder{get; set;}
		public FileInfoSortKey SecondaryKey{get; set;}
		public SortOrder SecondaryOrder{get; set;}

		public FileInfoComparer(){}

		public int Compare(IO::FileInfo x, IO::FileInfo y) {
			var d = this.CompareInternal(x, y, this.PrimaryKey, this.PrimaryOrder);
			return (d != 0) ? d : this.CompareInternal(x, y, this.SecondaryKey, this.SecondaryOrder);
		}

		private int CompareInternal(IO::FileInfo x, IO::FileInfo y, FileInfoSortKey key, SortOrder order){
			var d = this.CompareInternal(x, y, key);
			return (order == SortOrder.Descending) ? -d : d;
		}

		private int CompareInternal(IO::FileInfo x, IO::FileInfo y, FileInfoSortKey key){
			switch(key){
				case FileInfoSortKey.FileName: return UnsafeLogicalStringComparer.Comparer.Compare(x.Name, y.Name);
				case FileInfoSortKey.Extension: return UnsafeLogicalStringComparer.Comparer.Compare(x.Extension, y.Extension);
				case FileInfoSortKey.CreationTime: return Comparer<DateTime>.Default.Compare(x.CreationTime, y.CreationTime);
				case FileInfoSortKey.LastAccessTime: return Comparer<DateTime>.Default.Compare(x.LastAccessTime, y.LastAccessTime);
				case FileInfoSortKey.LastWriteTime: return Comparer<DateTime>.Default.Compare(x.LastWriteTime, y.LastWriteTime);
				default: return 0;
			}
		}
	}

	public enum FileInfoSortKey{
		FileName = 0,
		Extension = 1,
		LastAccessTime = 2,
		LastWriteTime = 3,
		CreationTime = 4,
	}

	public enum SortOrder{
		Ascending = 0,
		Descending = 1,
	}
}
