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

namespace GFV.ViewModel{
	using Gfl = GflNet;
	using IO = System.IO;

	public class ViewerViewModel : ViewModelBase, IDisposable{
		public Gfl::Gfl Gfl{get; private set;}
		private bool _IsUpdateDisplayBitmap = true;

		public ViewerViewModel(Gfl::Gfl gfl){
			this.Gfl = gfl;
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
				this.OnPropertyChanged("View");
			}
		}

		private void View_SizeChanged(object sender, ViewerSizeChangedEventArgs e){
			this.ViewerSize = e.NewSize;
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
				this._DisplayBitmap = this.CurrentBitmap;
				this.OnPropertyChanged("SourceBitmap", "FrameIndex", "CurrentBitmap", "DisplayBitmap");
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

			// return is null
			if(currentBitmap == null){
				this._DisplayBitmap = null;
				this.OnPropertyChanged("DisplayBitmap");
				return;
			}

			var scale = this.CalculateScale(currentBitmap);
			var newSize = new Size(Math.Floor(currentBitmap.Width * scale), Math.Floor(currentBitmap.Height * scale));

			// return if new size is same
			if(newSize != this._DisplayBitmapSize){
				this._DisplayBitmapSize = newSize;
				this._Scale = scale;
				this.OnPropertyChanged("Scale", "DisplayBitmapSize");
			}else{
				return;
			}

			// set plain bitmap if scale == 1
			if(this._Scale == 1){
				this._DisplayBitmap = currentBitmap;
				this.OnPropertyChanged("DisplayBitmap", "Scale", "DisplayBitmapSize");
				return;
			}

			var resizeMethod = this._ResizeMethod;
			var ui = TaskScheduler.FromCurrentSynchronizationContext();
			this._RefreshDisplayBitmap_CancellationTokenSource = new CancellationTokenSource();
			var task = new Task(new Action(delegate{
				Thread.Sleep(100);
			}), this._RefreshDisplayBitmap_CancellationTokenSource.Token);
			var task2 = task.ContinueWith(delegate{
				Gfl::Bitmap displayBitmap = null;
				displayBitmap = currentBitmap.Resize((int)this._DisplayBitmapSize.Width, (int)this._DisplayBitmapSize.Height, resizeMethod);
				this._DisplayBitmap = displayBitmap;
				this.OnPropertyChanged("DisplayBitmap");
			}, this._RefreshDisplayBitmap_CancellationTokenSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
			task2.ContinueWith(new Action<Task>(delegate(Task prev){
				prev.Exception.Handle(ex => true);
			}), CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, ui);
			task.Start();
		}

		private void CalculateScale(){
			this.CalculateScale(this.CurrentBitmap);
		}

		private double CalculateScale(Gfl::Bitmap currentBitmap){
			var viewerSize = this._ViewerSize;
			double scale = this._Scale;
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
		/// DisplayBitmapの縮尺を取得・設定する。
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
				this._FittingMode = ImageFittingMode.None;
				this.OnPropertyChanged("Scale", "FittingMode");
				if(this._IsUpdateDisplayBitmap){
					this.RefreshDisplayBitmap();
				}
			}
		}

		private ImageFittingMode _FittingMode = Settings.Default.ImageFittingMode;
		public ImageFittingMode FittingMode{
			get{
				return this._FittingMode;
			}
			set{
				this._FittingMode = value;
				Settings.Default.ImageFittingMode = value;
				this.OnPropertyChanged("FittingMode");
				if(this._FittingMode == ImageFittingMode.None){
					this._Scale = 1;
					this.OnPropertyChanged("Scale");
				}
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
				this.View.SizeChanged -= this.View_SizeChanged;
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
		public ViewerSizeChangedEventArgs(Size newSize){
			this.NewSize = newSize;
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
