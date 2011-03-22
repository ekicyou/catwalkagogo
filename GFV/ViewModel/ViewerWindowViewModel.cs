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
					this.viewer = new ViewerViewModel(this.Gfl);
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
			var task = new Task<Gfl::MultiBitmap>(delegate{
				var bitmap = this.Gfl.LoadMultiBitmap(path);
				bitmap.LoadParameters.BitmapType = Gfl::BitmapType.Bgra;
				bitmap.LoadParameters.Options = Gfl::LoadOptions.ForceColorModel | Gfl::LoadOptions.IgnoreReadError;
				bitmap.LoadParameters.ProgressChanged += Bitmap_LoadProgressChanged;
				bitmap.LoadParameters.WantCancel += Bitmap_WantCancel;
				return bitmap;
			}, this._OpenFile_CancellationTokenSource.Token);
			var task2 = task.ContinueWith(delegate(Task<Gfl::MultiBitmap> t){
				var bitmap = t.Result;
				bitmap.LoadAllFrames();
				var bmp = bitmap[0];
				double scaleW = (32d / (double)bmp.Width);
				double scaleH = (32d / (double)bmp.Height);
				var scale = Math.Min(scaleW, scaleH);
				this._Icon = bmp.Resize((int)Math.Round(bmp.Width * scale), (int)Math.Round(bmp.Height * scale), this.Viewer.ResizeMethod);
				return bitmap;
			}, this._OpenFile_CancellationTokenSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
			task2.ContinueWith(delegate(Task<Gfl::MultiBitmap> t){
				var bitmap = t.Result;
				this.OnPropertyChanged("Icon");
				this.SetViewerBitmap(bitmap);
			}, this._OpenFile_CancellationTokenSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, ui);
			task.ContinueWith(this.Bitmap_LoadError, CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, ui);
			task2.ContinueWith(this.Bitmap_LoadError, CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, ui);
			task.Start();
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

		private void Bitmap_LoadProgressChanged(object sender, Gfl::ProgressEventArgs e){
		}

		private void Bitmap_WantCancel(object sender, CancelEventArgs e){
			if((this._OpenFile_CancellationTokenSource != null) && (this._OpenFile_CancellationTokenSource.IsCancellationRequested)){
				e.Cancel = true;
			}
		}

		public IOpenFileDialog OpenFileDialog{get; set;}

		private ICommand _OpenFileCommand;
		public ICommand OpenFileCommand{
			get{
				if(this._OpenFileCommand == null){
					this._OpenFileCommand = new DelegateCommand<string>(this.OpenFile);
				}
				return this._OpenFileCommand;
			}
		}

		public void OpenFile(string path){
			if(String.IsNullOrEmpty(path)){
				if(this.OpenFileDialog != null){
					var formats = this.Gfl.Formats.Where(fmt => fmt.Readable);
					var dlg = this.OpenFileDialog;
					dlg.Filters.Add(new FileDialogFilter(
						"All Images",
						String.Join(";", formats.Select(fmt => "*." + fmt.DefaultSuffix))));
					dlg.Filters.Add(new FileDialogFilter("All Files (*.*)", "*.*"));
					foreach(var filter in formats.OrderBy(fmt => fmt.Description)){
						var mask = "*." + filter.DefaultSuffix;
						dlg.Filters.Add(new FileDialogFilter(
							filter.Description + " (" + mask + ")", mask));
					}
					if(dlg.ShowDialog().Value){
						var file = dlg.FileName;
						if(!String.IsNullOrEmpty(file)){
							if(this.Viewer.SourceBitmap == null){
								this.Path = file;
							}else{
								Program.CreateViewerWindow(file).Item1.Show();
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
