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
using System.Windows.Threading;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using GFV.Properties;
using GFV.Imaging;
using GFV.Messaging;
using CatWalk;
using CatWalk.Mvvm;

namespace GFV.ViewModel{
	using IO = System.IO;

	[RecieveMessage(typeof(SizeMessage))]
	[RecieveMessage(typeof(ScaleMessage))]
	[RecieveMessage(typeof(RequestScaleMessage))]
	[RecieveMessage(typeof(FrameIndexMessage))]
	[SendMessage(typeof(AnimationMessage))]
	public class ViewerViewModel : ViewModelBase, IDisposable{
		private readonly object _SyncObject = new object();
		public IImageLoader Loader{get; private set;}
		public ProgressManager ProgressManager{get; private set;}

		public ViewerViewModel(IImageLoader loader) : this(loader, null){}
		public ViewerViewModel(IImageLoader loader, ProgressManager pm){
			this.Loader = loader;
			this.ProgressManager = pm;

			Messenger.Default.Register<SizeMessage>(this.RecieveSizeMessage, this);
			Messenger.Default.Register<ScaleMessage>(this.RecieveScaleMessage, this);
			Messenger.Default.Register<RequestScaleMessage>(this.RecieveRequestScaleMessage, this);
			Messenger.Default.Register<FrameIndexMessage>(this.RecieveFrameIndexMessage, this);
		}

		#region View

		private void RecieveSizeMessage(SizeMessage message){
			if(this._ViewerSize != message.Size){
				this.OnPropertyChanging("ViewerSize");
				this._ViewerSize = message.Size;
				this.OnPropertyChanged("ViewerSize");
				if(this.CurrentBitmap != null){
					this.RefreshDisplayBitmapSize();
				}
			}
		}

		private void RecieveScaleMessage(ScaleMessage message){
			if(this._Scale != message.Scale){
				this.OnPropertyChanging("Scale");
				this._Scale = message.Scale;
				this.OnPropertyChanged("Scale");
				if(this._FittingMode != ImageFittingMode.None){
					this.OnPropertyChanging("FittingMode");
					this._FittingMode = ImageFittingMode.None;
					this.OnPropertyChanged("FittingMode");
				}
				if(this.CurrentBitmap != null){
					this.RefreshDisplayBitmapSize();
				}
			}
		}

		private void RecieveRequestScaleMessage(RequestScaleMessage message){
			message.Scale = this.Scale;
		}

		#endregion

		#region Scale / DisplayBitmapSize

		private double CalculateScale(){
			return this.CalculateScale(this.CurrentBitmap);
		}

		private double CalculateScale(BitmapSource currentBitmap){
			var viewerSize = this._ViewerSize;
			double scale = this._Scale;
			if(currentBitmap == null){
				return scale;
			}
			switch(this._FittingMode){
				case ImageFittingMode.Window:{
					double scaleW = (viewerSize.Width / currentBitmap.PixelWidth);
					double scaleH = (viewerSize.Height / currentBitmap.PixelHeight);
					scale = Math.Min(scaleW, scaleH);
					break;
				}
				case ImageFittingMode.WindowLargeOnly:{
					double scaleW = (currentBitmap.PixelWidth > viewerSize.Width) ? (viewerSize.Width / currentBitmap.PixelWidth) : 1.0;
					double scaleH = (currentBitmap.PixelHeight > viewerSize.Height) ? (viewerSize.Height / currentBitmap.PixelHeight) : 1.0;
					scale = Math.Min(scaleW, scaleH);
					break;
				}
				case ImageFittingMode.WindowWidth:{
					scale = (viewerSize.Width / currentBitmap.PixelWidth);
					break;
				}
				case ImageFittingMode.WindowWidthLargeOnly:{
					scale = (currentBitmap.PixelWidth > viewerSize.Width) ? (viewerSize.Width / currentBitmap.PixelWidth) : 1.0;
					break;
				}
				case ImageFittingMode.WindowHeight:{
					scale = (viewerSize.Height / currentBitmap.PixelHeight);
					break;
				}
				case ImageFittingMode.WindowHeightLargeOnly:{
					scale = (currentBitmap.PixelHeight > viewerSize.Height) ? (viewerSize.Height / currentBitmap.PixelHeight) : 1.0;
					break;
				}
				case ImageFittingMode.ShorterEdge:{
					if(currentBitmap.PixelWidth > currentBitmap.PixelHeight){
						scale = (viewerSize.Width / currentBitmap.PixelWidth);
					}else{
						scale = (viewerSize.Height / currentBitmap.PixelHeight);
					}
					break;
				}
				case ImageFittingMode.ShorterEdgeLargeOnly:{
					if(currentBitmap.PixelHeight <= viewerSize.Height && currentBitmap.PixelWidth <= viewerSize.Width){
						scale = 1.0;
					}else if(currentBitmap.PixelWidth > currentBitmap.PixelHeight){
						scale = (viewerSize.Width / currentBitmap.PixelWidth);
					}else{
						scale = (viewerSize.Height / currentBitmap.PixelHeight);
					}
					break;
				}
				case ImageFittingMode.LongerEdge:{
					if(currentBitmap.PixelWidth > currentBitmap.PixelHeight){
						scale = (viewerSize.Height / currentBitmap.PixelHeight);
					}else{
						scale = (viewerSize.Width / currentBitmap.PixelWidth);
					}
					break;
				}
				case ImageFittingMode.LongerEdgeLargeOnly:{
					if(currentBitmap.PixelHeight <= viewerSize.Height && currentBitmap.PixelWidth <= viewerSize.Width){
						scale = 1.0;
					}else if(currentBitmap.PixelWidth > currentBitmap.PixelHeight){
						scale = (viewerSize.Height / currentBitmap.PixelHeight);
					}else{
						scale = (viewerSize.Width / currentBitmap.PixelWidth);
					}
					break;
				}
			}
			return scale;
		}

		private void RefreshDisplayBitmapSize(){
			this.RefreshDisplayBitmapSize(this.CurrentBitmap);
		}

		private void RefreshDisplayBitmapSize(BitmapSource currentBitmap){
			double scale = 1;
			this.OnPropertyChanging("DisplayBitmapSize");
			if(currentBitmap == null){
				this._DisplayBitmapSize = new Size(0, 0);
				if(this._Scale != 1){
					this.OnPropertyChanging("Scale");
					this._Scale = 1;
					this.OnPropertyChanged("Scale");
				}
			}else{
				scale = this.CalculateScale(currentBitmap);
				this._DisplayBitmapSize = new Size(Math.Floor(currentBitmap.PixelWidth * scale), Math.Floor(currentBitmap.PixelHeight * scale));
				if(scale != this._Scale){
					this.OnPropertyChanging("Scale");
					this._Scale = scale;
					this.OnPropertyChanged("Scale");
				}
			}
			this.OnPropertyChanged("DisplayBitmapSize");
		}

		#endregion

		#region Bitmap Properties

		public bool CurrentBitmapLoaded{
			get{
				return this.CurrentBitmap != null;
			}
		}

		private IMultiBitmap _SourceBitmap = null;
		public IMultiBitmap SourceBitmap{
			get{
				return this._SourceBitmap;
			}
			set{
				lock(this._SyncObject){
					this.OnPropertyChanging("SourceBitmap", "FrameIndex", "CurrentBitmap", "CurrentBitmapLoaded");
					this._SourceBitmap = value;
					this._FrameIndex = 0;
					if(value != null){
						this.IsAnimationEnabled = value.IsAnimated;
					}
					this.OnPropertyChanged("SourceBitmap", "FrameIndex", "CurrentBitmap", "CurrentBitmapLoaded");
					this.RefreshDisplayBitmapSize();
				}
			}
		}

		public BitmapSource CurrentBitmap{
			get{
				if(this._SourceBitmap != null){
					return this._SourceBitmap[this._FrameIndex];
				}else{
					return null;
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

		#region FrameIndex

		private int _FrameIndex = 0;
		public int FrameIndex{
			get{
				return this._FrameIndex;
			}
			set{
				lock(this._SyncObject){
					if(this._SourceBitmap == null){
						throw new InvalidOperationException();
					}
					if(value < 0 || this.SourceBitmap.FrameCount <= value){
						throw new ArgumentOutOfRangeException();
					}
					this.OnPropertyChanging("FrameIndex", "CurrentBitmap", "CurrentBitmapLoaded");
					this._FrameIndex = value;
					this.OnPropertyChanged("FrameIndex", "CurrentBitmap", "CurrentBitmapLoaded");
					this._NextPageCommand.RaiseCanExecuteChanged();
					this._PreviousPageCommand.RaiseCanExecuteChanged();
					this.RefreshDisplayBitmapSize();
				}
			}
		}

		private void RecieveFrameIndexMessage(FrameIndexMessage message){
			lock(this._SyncObject){
				if(this.SourceBitmap != null && message.FrameIndex < this.SourceBitmap.FrameCount){
					this.FrameIndex = message.FrameIndex;
				}
			}
		}

		#endregion

		#region Display Properties

		private Size _ViewerSize;
		public Size ViewerSize{
			get{
				return this._ViewerSize;
			}
			set{
				this.OnPropertyChanging("ViewerSize");
				this._ViewerSize = value;
				this.OnPropertyChanged("ViewerSize");
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
				if(this._Scale != value){
					this.OnPropertyChanging("Scale");
					this._Scale = value;
					this.OnPropertyChanged("Scale");
					if(this._FittingMode != ImageFittingMode.None){
						this.OnPropertyChanging("FittingMode");
						this._FittingMode = ImageFittingMode.None;
						this.OnPropertyChanged("FittingMode");
					}
					this.RefreshDisplayBitmapSize();
				}
			}
		}

		private ImageFittingMode _FittingMode = Settings.Default.ImageFittingMode;
		public ImageFittingMode FittingMode{
			get{
				return this._FittingMode;
			}
			set{
				if(this._FittingMode != value){
					this.OnPropertyChanging("FittingMode");
					Settings.Default.ImageFittingMode = this._FittingMode = value;
					this.OnPropertyChanged("FittingMode");
					if(this._FittingMode == ImageFittingMode.None){
						this.OnPropertyChanging("Scale");
						this._Scale = 1;
						this.OnPropertyChanged("Scale");
					}
					this.RefreshDisplayBitmapSize();
				}
			}
		}

		#endregion

		#region SetFittingMode

		private ICommand _SetFittingModeCommand;
		public ICommand SetFittingModeCommand{
			get{
				if(this._SetFittingModeCommand == null){
					this._SetFittingModeCommand = new DelegateUICommand<ImageFittingMode>(this.SetFittingMode);
				}
				return this._SetFittingModeCommand;
			}
		}

		private void SetFittingMode(ImageFittingMode mode){
			this.FittingMode = mode;
		}

		#endregion

		#region Zoom In / Zoom Out

		#endregion

		#region Next / Previous Page

		private DelegateCommand _NextPageCommand;
		public ICommand NextPageCommand{
			get{
				return this._NextPageCommand ?? (this._NextPageCommand = new DelegateUICommand(this.NextPage, this.CanNextPage));
			}
		}

		private void NextPage(){
			this.FrameIndex++;
		}

		private bool CanNextPage(){
			return (this._SourceBitmap != null && this._FrameIndex < this._SourceBitmap.FrameCount - 1);
		}

		private DelegateCommand _PreviousPageCommand;
		public ICommand PreviousPageCommand{
			get{
				return this._PreviousPageCommand ?? (this._PreviousPageCommand = new DelegateUICommand(this.PreviousPage, this.CanPreviousPage));
			}
		}

		private void PreviousPage(){
			this.FrameIndex--;
		}

		private bool CanPreviousPage(){
			return (this._SourceBitmap != null && this._FrameIndex > 0);
		}

		#endregion

		#region Animation

		private bool _IsAnimationEnabled = false;
		public bool IsAnimationEnabled{
			get{
				return this._IsAnimationEnabled;
			}
			set{
				this.OnPropertyChanging("IsAnimationEnabled", "Animation");
				this._IsAnimationEnabled = value;
				this.Animation = this.CreateAnimation();
				Messenger.Default.Send(new AnimationMessage(this, value, this.Animation), this);
				this.OnPropertyChanged("IsAnimationEnabled", "Animation");
			}
		}

		public Storyboard Animation{get; private set;}

		private Storyboard CreateAnimation(){
			var storyboard = new Storyboard();
			if(this.SourceBitmap.IsAnimated){
				var keyframe = new Int32AnimationUsingKeyFrames();
				storyboard.Children.Add(keyframe);

				var last = this.SourceBitmap.FrameCount - 1;
				var time = 0;
				for(var i = 0; i < last; i++){
					keyframe.KeyFrames.Add(new DiscreteInt32KeyFrame(i + 1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(time))));
					var delay = this.SourceBitmap.DelayTimes[i];
					if(delay <= 50){
						delay = 100;
					}
					time += delay;
				}
				if(last != 0){
					keyframe.KeyFrames.Add(new DiscreteInt32KeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(time))));
					var delay = this.SourceBitmap.DelayTimes[last];
					if(delay <= 50){
						delay = 100;
					}
					time += delay;
				}
				keyframe.Duration = new Duration(TimeSpan.FromMilliseconds(time));
				storyboard.RepeatBehavior = (this.SourceBitmap.LoopCount <= 0) ? RepeatBehavior.Forever : new RepeatBehavior(this.SourceBitmap.LoopCount);
			}
			return storyboard;
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
				Messenger.Default.Unregister<SizeMessage>(this.RecieveSizeMessage, this);
				Messenger.Default.Unregister<ScaleMessage>(this.RecieveScaleMessage, this);
				Messenger.Default.Unregister<RequestScaleMessage>(this.RecieveRequestScaleMessage, this);
				Messenger.Default.Unregister<FrameIndexMessage>(this.RecieveFrameIndexMessage, this);
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
		ShorterEdge = 7,
		ShorterEdgeLargeOnly = 8,
		LongerEdge = 9,
		LongerEdgeLargeOnly = 10,
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

	#endregion
}
