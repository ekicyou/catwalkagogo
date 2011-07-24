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
using CatWalk.Mvvm;

namespace GFV.ViewModel{
	using Gfl = GflNet;
	using IO = System.IO;

	[RecieveMessage(typeof(SizeMessage))]
	[RecieveMessage(typeof(ScaleMessage))]
	[RecieveMessage(typeof(RequestScaleMessage))]
	public class ViewerViewModel : ViewModelBase, IDisposable{
		public Gfl::Gfl Gfl{get; private set;}
		public ProgressManager ProgressManager{get; private set;}

		public ViewerViewModel(Gfl::Gfl gfl) : this(gfl, null){}

		public ViewerViewModel(Gfl::Gfl gfl, ProgressManager pm){
			this.Gfl = gfl;
			this.ProgressManager = pm;

			Messenger.Default.Register<SizeMessage>(this.RecieveSizeMessage, this);
			Messenger.Default.Register<ScaleMessage>(this.RecieveScaleMessage, this);
			Messenger.Default.Register<RequestScaleMessage>(this.RecieveRequestScaleMessage, this);
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
				case ImageFittingMode.ShorterEdge:{
					if(currentBitmap.Width > currentBitmap.Height){
						scale = (viewerSize.Width / currentBitmap.Width);
					}else{
						scale = (viewerSize.Height / currentBitmap.Height);
					}
					break;
				}
				case ImageFittingMode.ShorterEdgeLargeOnly:{
					if(currentBitmap.Height <= viewerSize.Height && currentBitmap.Width <= viewerSize.Width){
						scale = 1.0;
					}else if(currentBitmap.Width > currentBitmap.Height){
						scale = (viewerSize.Width / currentBitmap.Width);
					}else{
						scale = (viewerSize.Height / currentBitmap.Height);
					}
					break;
				}
				case ImageFittingMode.LongerEdge:{
					if(currentBitmap.Width > currentBitmap.Height){
						scale = (viewerSize.Height / currentBitmap.Height);
					}else{
						scale = (viewerSize.Width / currentBitmap.Width);
					}
					break;
				}
				case ImageFittingMode.LongerEdgeLargeOnly:{
					if(currentBitmap.Height <= viewerSize.Height && currentBitmap.Width <= viewerSize.Width){
						scale = 1.0;
					}else if(currentBitmap.Width > currentBitmap.Height){
						scale = (viewerSize.Height / currentBitmap.Height);
					}else{
						scale = (viewerSize.Width / currentBitmap.Width);
					}
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
				this._DisplayBitmapSize = new Size(Math.Floor(currentBitmap.Width * scale), Math.Floor(currentBitmap.Height * scale));
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

		private Gfl::MultiBitmap _SourceBitmap = null;
		public Gfl::MultiBitmap SourceBitmap{
			get{
				return this._SourceBitmap;
			}
			set{
				this.OnPropertyChanging("SourceBitmap", "FrameIndex", "CurrentBitmap", "CurrentBitmapLoaded");
				this._SourceBitmap = value;
				this._FrameIndex = 0;
				this.OnPropertyChanged("SourceBitmap", "FrameIndex", "CurrentBitmap", "CurrentBitmapLoaded");
				this.RefreshDisplayBitmapSize();
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

		private Size _DisplayBitmapSize = new Size(0, 0);
		public Size DisplayBitmapSize{
			get{
				return this._DisplayBitmapSize;
			}
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
				this.OnPropertyChanging("FrameIndex", "CurrentBitmap", "CurrentBitmapLoaded");
				this._FrameIndex = value;
				this.OnPropertyChanged("FrameIndex", "CurrentBitmap", "CurrentBitmapLoaded");
				this.RefreshDisplayBitmapSize();
			}
		}

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
					this._SetFittingModeCommand = new DelegateCommand<ImageFittingMode>(this.SetFittingMode);
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

	#region SizeMessage

	public class SizeMessage : MessageBase{
		public Size Size{get; private set;}
		public SizeMessage(object sender, Size size) : base(sender){
			this.Size = size;
		}
	}

	#endregion

	#region Scale Message

	public class ScaleMessage : MessageBase{
		public double Scale{get; private set;}

		public ScaleMessage(object sender, double scale) : base(sender){
			this.Scale = scale;
		}
	}

	#endregion

	#region RequestScaleMessage

	public class RequestScaleMessage : MessageBase{
		public double Scale{get; set;}

		public RequestScaleMessage(object sender) : base(sender){
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
