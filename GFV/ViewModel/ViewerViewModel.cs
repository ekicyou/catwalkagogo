/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using GFV.Properties;
using CatWalk;

namespace GFV.ViewModel{
	using Gfl = GflNet;
	using IO = System.IO;

	public class ViewerViewModel : ViewModelBase, IDisposable{
		public Gfl::Gfl Gfl{get; private set;}
		public ProgressManager ProgressManager{get; private set;}
		private bool _IsUpdateDisplayBitmap = true;

		public ViewerViewModel(Gfl::Gfl gfl) : this(gfl, null){}

		public ViewerViewModel(Gfl::Gfl gfl, ProgressManager pm){
			this.Gfl = gfl;
			this.ProgressManager = pm;
			this.FittingMode = Settings.Default.ImageFittingMode;
		}


		#region View

		private IViewerView _View;
		public IViewerView View{
			get{
				return this._View;
			}
			set{
				var old = this._View;
				this._View = value;
				if(old != null){
					old.SizeChanged -= this.View_SizeChanged;
				}
				this._View.SizeChanged += this.View_SizeChanged;
				this.ViewerSize = this._View.ViewerSize;
			}
		}

		private void View_SizeChanged(object sender, ViewerSizeChangedEventArgs e){
			this._ViewerSize = e.NewSize;
			this.OnPropertyChanged("ViewerSize");
			if(this.CurrentBitmap != null){
				this.RefreshDisplayBitmapSize();
			}
			if(e.IsUpdateDisplayBitmap){
				this.RefreshDisplayBitmap();
			}
		}

		#endregion

		#region Bitmap Properties

		private Gfl::MultiBitmap _SourceBitmap = null;
		public Gfl::MultiBitmap SourceBitmap{
			get{
				return this._SourceBitmap;
			}
			set{
				this._SourceBitmap = value;
				this._FrameIndex = 0;
				this.DisplayBitmap = null;
				this.OnPropertyChanged("SourceBitmap", "FrameIndex", "CurrentBitmap");
				this.RefreshDisplayBitmapSize();
				if(this._IsUpdateDisplayBitmap){
					this.RefreshDisplayBitmap();
				}
			}
		}

		public Gfl::Bitmap CurrentBitmap{
			get{
				if(this._SourceBitmap != null){
					return this._SourceBitmap[this._FrameIndex];
				}else{
					return null;
				}
			}
		}

		private Gfl::Bitmap _DisplayBitmap = null;
		public Gfl::Bitmap DisplayBitmap{
			get{
				return this._DisplayBitmap;
			}
			private set{
				if(this._DisplayBitmap != value){
					this._DisplayBitmap = value;
					this.OnPropertyChanged("DisplayBitmap");
				}
			}
		}

		private Size _DisplayBitmapSize = new Size(0, 0);
		public Size DisplayBitmapSize{
			get{
				return this._DisplayBitmapSize;
			}
		}

		#endregion

		#region Updating Bitmap

		/// <summary>
		/// 各パラメータの更新を開始します。
		/// EndUpdateが呼び出されるまでDisplayBitmapの更新が停止されます。
		/// </summary>
		public void BeginUpdate(){
			this._IsUpdateDisplayBitmap = false;
		}

		public void EndUpdate(){
			this._IsUpdateDisplayBitmap = true;
			this.RefreshDisplayBitmap();
		}

		private Size _OldDisplayBitmapSize;
		private WeakReference<Gfl::Bitmap> _OldCurrentBitmap = new WeakReference<Gfl.Bitmap>(null);
		private CancellationTokenSource _RefreshDisplayBitmap_CancellationTokenSource;
		/// <summary>
		/// DisplayBitmapを更新する。
		/// FittingModeがNoneの場合はScaleに、
		/// それ以外のときはFittingModeに従って処理する。
		/// </summary>
		private void RefreshDisplayBitmap(){
			if(this._RefreshDisplayBitmap_CancellationTokenSource != null){
				this._RefreshDisplayBitmap_CancellationTokenSource.Cancel();
			}

			var currentBitmap = this.CurrentBitmap;
			var displayBitmapSize = this._DisplayBitmapSize;
			//MessageBox.Show(new System.Diagnostics.StackTrace().ToString());

			// return if null
			if(currentBitmap == null){
				this.DisplayBitmap = null;
				return;
			}

			// return if new size is same
			if((this._OldCurrentBitmap.Target == currentBitmap) && (displayBitmapSize == this._OldDisplayBitmapSize)){
				return;
			}
			this._OldDisplayBitmapSize = displayBitmapSize;
			this._OldCurrentBitmap = new WeakReference<Gfl::Bitmap>(currentBitmap);

			// set plain bitmap if scale == 1
			if(displayBitmapSize.Width == currentBitmap.Width && displayBitmapSize.Height == currentBitmap.Height){
				this.DisplayBitmap = currentBitmap;
				return;
			}

			var ui = TaskScheduler.FromCurrentSynchronizationContext();
			this._RefreshDisplayBitmap_CancellationTokenSource = new CancellationTokenSource();

			var task = new Task(new Action<object>(delegate(object prm){
				var bitmap = (Gfl::Bitmap)prm;
				var displayBitmap = Gfl::Bitmap.Resize(bitmap, (int)displayBitmapSize.Width, (int)displayBitmapSize.Height, this._ResizeMethod);
				this.DisplayBitmap = displayBitmap;
			}), currentBitmap, this._RefreshDisplayBitmap_CancellationTokenSource.Token);
			task.ContinueWith(this.HandleTaskExceptions, CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, ui);
			if(this.ProgressManager != null){
				var jobId = new object();
				task.ContinueWith(delegate{
					this.ProgressManager.Complete(jobId);
				}, CancellationToken.None, TaskContinuationOptions.None, ui);
				this.ProgressManager.Start(jobId);
			}
			task.Start();
		}

		private void HandleTaskExceptions(Task prev){
			MessageBox.Show(prev.Exception.Message);
			prev.Exception.Handle(ex => true);
		}

		private double CalculateScale(){
			return this.CalculateScale(this.CurrentBitmap);
		}

		private double CalculateScale(Gfl::Bitmap currentBitmap){
			var viewerSize = this._ViewerSize;
			double scale = this._Scale;
			if(currentBitmap == null){
				return scale;
			}
			switch(this._FittingMode){
				case ImageFittingMode.Window:{
					double scaleW = (viewerSize.Width / currentBitmap.Width);
					double scaleH = (viewerSize.Height / currentBitmap.Height);
					scale = Math.Min(scaleW, scaleH);
					break;
				}
				case ImageFittingMode.WindowLargeOnly:{
					double scaleW = (currentBitmap.Width > viewerSize.Width) ? (viewerSize.Width / currentBitmap.Width) : 1.0;
					double scaleH = (currentBitmap.Height > viewerSize.Height) ? (viewerSize.Height / currentBitmap.Height) : 1.0;
					scale = Math.Min(scaleW, scaleH);
					break;
				}
				case ImageFittingMode.WindowWidth:{
					scale = (viewerSize.Width / currentBitmap.Width);
					break;
				}
				case ImageFittingMode.WindowWidthLargeOnly:{
					scale = (currentBitmap.Width > viewerSize.Width) ? (viewerSize.Width / currentBitmap.Width) : 1.0;
					break;
				}
				case ImageFittingMode.WindowHeight:{
					scale = (viewerSize.Height / currentBitmap.Height);
					break;
				}
				case ImageFittingMode.WindowHeightLargeOnly:{
					scale = (currentBitmap.Height > viewerSize.Height) ? (viewerSize.Height / currentBitmap.Height) : 1.0;
					break;
				}
			}
			return scale;
		}

		private void RefreshDisplayBitmapSize(){
			this.RefreshDisplayBitmapSize(this.CurrentBitmap);
		}

		private void RefreshDisplayBitmapSize(Gfl::Bitmap currentBitmap){
			double scale = 1;
			if(currentBitmap == null){
				this._DisplayBitmapSize = new Size(0, 0);
			}else{
				scale = this.CalculateScale(currentBitmap);
				this._DisplayBitmapSize = new Size(Math.Floor(currentBitmap.Width * scale), Math.Floor(currentBitmap.Height * scale));
			}
			this.OnPropertyChanged("DisplayBitmapSize");
		}

		#endregion

		#region Display Properties

		private int _FrameIndex = 0;
		public int FrameIndex{
			get{
				return this._FrameIndex;
			}
			set{
				if(this._SourceBitmap == null){
					throw new InvalidOperationException();
				}
				if(value < 0 || this.SourceBitmap.FrameCount <= value){
					throw new ArgumentOutOfRangeException();
				}
				this._FrameIndex = value;
				this.OnPropertyChanged("FrameIndex", "CurrentBitmap");
				this.RefreshDisplayBitmapSize();
				if(this._IsUpdateDisplayBitmap){
					this.RefreshDisplayBitmap();
				}
			}
		}

		private Size _ViewerSize;
		public Size ViewerSize{
			get{
				return this._ViewerSize;
			}
			set{
				this._ViewerSize = value;
				this.OnPropertyChanged("ViewerSize");
				this.RefreshDisplayBitmap();
			}
		}

		private double _Scale = 1;
		/// <summary>
		/// DisplayBitmapの縮尺を取得・設定する。自動設定時はNaN。
		/// </summary>
		public double Scale{
			get{
				return this._Scale;
			}
			set{
				if(value < 0){
					throw new ArgumentOutOfRangeException();
				}
				this._Scale = value;
				if(this._FittingMode != ImageFittingMode.None){
					this._FittingMode = ImageFittingMode.None;
					this.OnPropertyChanged("Scale", "FittingMode");
				}
				this.RefreshDisplayBitmapSize();
				if(this._IsUpdateDisplayBitmap){
					this.RefreshDisplayBitmap();
				}
			}
		}

		public double ActualScale{
			get{
				if(this._DisplayBitmap == null || this.CurrentBitmap == null){
					return Double.NaN;
				}else{
					return (double)this._DisplayBitmap.Width / (double)this.CurrentBitmap.Width;
				}
			}
		}

		private ImageFittingMode _FittingMode;
		public ImageFittingMode FittingMode{
			get{
				return this._FittingMode;
			}
			set{
				this._FittingMode = value;
				Settings.Default.ImageFittingMode = value;
				this.OnPropertyChanged("FittingMode");
				if(this._FittingMode == ImageFittingMode.None){
					if(Double.IsNaN(this._Scale)){
						this._Scale = 1;
						this.OnPropertyChanged("Scale");
					}
				}else{
					if(!Double.IsNaN(this._Scale)){
						this._Scale = Double.NaN;
						this.OnPropertyChanged("Scale");
					}
				}
				this.RefreshDisplayBitmapSize();
				if(this._IsUpdateDisplayBitmap){
					this.RefreshDisplayBitmap();
				}
			}
		}

		private Gfl::ResizeMethod _ResizeMethod = Settings.Default.ResizeMethod;
		public Gfl::ResizeMethod ResizeMethod{
			get{
				return this._ResizeMethod;
			}
			set{
				this._ResizeMethod = value;
				Settings.Default.ResizeMethod = value;
				this.OnPropertyChanged("ResizeMethod");
				this.RefreshDisplayBitmapSize();
				if(this._IsUpdateDisplayBitmap){
					this.RefreshDisplayBitmap();
				}
			}
		}

		#endregion

		#region SetFittingMode

		private ICommand _SetFittingModeCommand;
		public ICommand SetFittingModeCommand{
			get{
				if(this._SetFittingModeCommand == null){
					this._SetFittingModeCommand = new DelegateCommand<ImageFittingMode>(this.SetFittingMode);
				}
				return this._SetFittingModeCommand;
			}
		}

		private void SetFittingMode(ImageFittingMode mode){
			this.FittingMode = mode;
		}

		#endregion

		#region SetResizeMethod

		private ICommand _SetResizeMethodCommand;
		public ICommand SetResizeMethodCommand{
			get{
				if(this._SetResizeMethodCommand == null){
					this._SetResizeMethodCommand = new DelegateCommand<Gfl::ResizeMethod>(this.SetResizeMethod);
				}
				return this._SetResizeMethodCommand;
			}
		}

		private void SetResizeMethod(Gfl::ResizeMethod method){
			this.ResizeMethod = method;
		}

		#endregion

		#region IDisposable

		public void Dispose(){
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		~ViewerViewModel(){
			this.Dispose(false);
		}
		
		private bool disposed = false;
		protected virtual void Dispose(bool disposing){
			if(!(this.disposed)){
				if(this.View != null){
					this.View.SizeChanged -= this.View_SizeChanged;
				}
				this.disposed = true;
			}
		}

		#endregion
	}
	
	#region enum

	public enum ImageFittingMode{
		None = 0,
		Window = 1,
		WindowLargeOnly = 2,
		WindowWidth = 3,
		WindowWidthLargeOnly = 4,
		WindowHeight = 5,
		WindowHeightLargeOnly = 6,
	}

	#endregion

	#region IViewerView

	public interface IViewerView{
		Size ViewerSize{get;}
		event ViewerSizeChangedEventHandler SizeChanged;
	}

	public delegate void ViewerSizeChangedEventHandler(object sender, ViewerSizeChangedEventArgs e);

	public class ViewerSizeChangedEventArgs : EventArgs{
		public Size NewSize{get; private set;}
		public bool IsUpdateDisplayBitmap{get; private set;}
		public ViewerSizeChangedEventArgs(Size newSize) : this(newSize, true){}
		public ViewerSizeChangedEventArgs(Size newSize, bool isUpdateDisplayBitmap){
			this.NewSize = newSize;
			this.IsUpdateDisplayBitmap = isUpdateDisplayBitmap;
		}
	}

	#endregion

	#region Converters

	public class ImageFittingModeCheckConverter : IValueConverter{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var v = (ImageFittingMode)value;
			var mode = (ImageFittingMode)parameter;
			return (v == mode);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var b = (bool)value;
			var mode = (ImageFittingMode)parameter;
			if(b){
				return mode;
			}else{
				return ImageFittingMode.None;
			}
		}
	}

	public class ResizeMethodCheckConverter : IValueConverter{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var v = (Gfl::ResizeMethod)value;
			var mode = (Gfl::ResizeMethod)parameter;
			return (v == mode);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var b = (bool)value;
			var mode = (Gfl::ResizeMethod)parameter;
			if(b){
				return mode;
			}else{
				return Gfl::ResizeMethod.Quick;
			}
		}
	}

	#endregion
}
