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

namespace GFV.ViewModel{
	using Gfl = GflNet;
	using IO = System.IO;

	public class ViewerViewModel : ViewModelBase{
		public Gfl::Gfl Gfl{get; private set;}
		private bool isUpdateDisplayBitmap = true;

		public ViewerViewModel(Gfl::Gfl gfl){
			this.Gfl = gfl;
		}

		private Gfl::MultiBitmap _SourceBitmap = null;
		public Gfl::MultiBitmap SourceBitmap{
			get{
				return this._SourceBitmap;
			}
			set{
				this._SourceBitmap = value;
				this._FrameIndex = 0;
				this.OnPropertyChanged("MultiBitmap", "FrameIndex", "CurrentBitmap");
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

		/// <summary>
		/// 各パラメータの更新を開始します。
		/// EndUpdateが呼び出されるまでDisplayBitmapの更新が停止されます。
		/// </summary>
		public void BeginUpdate(){
			this.isUpdateDisplayBitmap = false;
		}

		public void EndUpdate(){
			this.isUpdateDisplayBitmap = true;
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
			if(currentBitmap == null){
				return;
			}
			int imageWidth = currentBitmap.Width;
			int imageHeight = currentBitmap.Height;

			var displaySize = this._DisplaySize;
			var ui = TaskScheduler.FromCurrentSynchronizationContext();
			this._RefreshDisplayBitmap_CancellationTokenSource = new CancellationTokenSource();
			var task = new Task(new Action(delegate{
				switch(this._FittingMode){
					case ImageFittingMode.None:
						imageWidth = (int)Math.Round(currentBitmap.Width * this._Scale);
						imageHeight = (int)Math.Round(currentBitmap.Height * this._Scale);
						break;
					case ImageFittingMode.Window:{
						var dw = Math.Abs(displaySize.Width - currentBitmap.Width);
						var dh = Math.Abs(displaySize.Height - currentBitmap.Height);
						if(dw < dh){ // fit to width
							imageWidth = (int)Math.Floor(this._DisplaySize.Width);
							imageHeight = (int)Math.Floor(currentBitmap.Height * ((double)imageWidth / (double)currentBitmap.Width));
						}else{
							imageHeight = (int)Math.Floor(this._DisplaySize.Height);
							imageWidth = (int)Math.Floor(currentBitmap.Width * ((double)imageHeight / (double)currentBitmap.Height));
						}
						break;
					}
					case ImageFittingMode.WindowLargeOnly:{
						var dw = displaySize.Width - currentBitmap.Width;
						var dh = displaySize.Height - currentBitmap.Height;
						if(dw < 0 || dh < 0){
							dw = Math.Abs(dw);
							dh = Math.Abs(dh);
							if(dw < dh){ // fit to width
								imageWidth = (int)Math.Floor(this._DisplaySize.Width);
								imageHeight = (int)Math.Floor(currentBitmap.Height * ((double)imageWidth / (double)currentBitmap.Width));
							}else{
								imageHeight = (int)Math.Floor(this._DisplaySize.Height);
								imageWidth = (int)Math.Floor(currentBitmap.Width * ((double)imageHeight / (double)currentBitmap.Height));
							}
						}
						break;
					}
					case ImageFittingMode.WindowHeight:{
						imageHeight = (int)Math.Floor(this._DisplaySize.Height);
						imageWidth = (int)Math.Floor(currentBitmap.Width * ((double)imageHeight / (double)currentBitmap.Height));
						break;
					}
					case ImageFittingMode.WindowHeightLargeOnly:{
						if(currentBitmap.Width > displaySize.Width || currentBitmap.Height > displaySize.Height){
							imageHeight = (int)Math.Floor(this._DisplaySize.Height);
							imageWidth = (int)Math.Floor(currentBitmap.Width * ((double)imageHeight / (double)currentBitmap.Height));
						}
						break;
					}
					case ImageFittingMode.WindowWidth:{
						imageWidth = (int)Math.Floor(this._DisplaySize.Width);
						imageHeight = (int)Math.Floor(currentBitmap.Height * ((double)imageWidth / (double)currentBitmap.Width));
						break;
					}
					case ImageFittingMode.WindowWidthLargeOnly:{
						if(currentBitmap.Width > displaySize.Width || currentBitmap.Height > displaySize.Height){
							imageWidth = (int)Math.Floor(this._DisplaySize.Width);
							imageHeight = (int)Math.Floor(currentBitmap.Height * ((double)imageWidth / (double)currentBitmap.Width));
						}
						break;
					}
				}

				Gfl::Bitmap displayBitmap = null;
				try{
					displayBitmap = currentBitmap.Resize(imageWidth, imageHeight, this._ResizeMethod);
				}catch{
				}

				this._DisplayBitmap = displayBitmap;
				this._Scale = (double)imageWidth / (double)currentBitmap.Width;
			}), this._RefreshDisplayBitmap_CancellationTokenSource.Token);
			task.ContinueWith(delegate{
				this.OnPropertyChanged("DisplayBitmap", "Scale");
			}, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, ui);
			task.Start();
		}

		private Size _DisplaySize;
		public Size DisplaySize{
			get{
				return this._DisplaySize;
			}
			set{
				this._DisplaySize = value;
				this.OnPropertyChanged("DisplaySize");
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
				if(this.isUpdateDisplayBitmap){
					this.RefreshDisplayBitmap();
				}
			}
		}

		private ImageFittingMode _FittingMode = ImageFittingMode.None;
		public ImageFittingMode FittingMode{
			get{
				return this._FittingMode;
			}
			set{
				this._FittingMode = value;
				this.OnPropertyChanged("FittingMode");
				if(this.isUpdateDisplayBitmap){
					this.RefreshDisplayBitmap();
				}
			}
		}

		private Gfl::ResizeMethod _ResizeMethod;
		public Gfl::ResizeMethod ResizeMethod{
			get{
				return this._ResizeMethod;
			}
			set{
				this._ResizeMethod = value;
				this.OnPropertyChanged("ResizeMethod");
				if(this.isUpdateDisplayBitmap){
					this.RefreshDisplayBitmap();
				}
			}
		}

		public void LoadFile(string file){
			file = IO.Path.GetFullPath(file);
			this.SourceBitmap = this.Gfl.LoadMultiBitmap(file);
		}

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
				if(this.isUpdateDisplayBitmap){
					this.RefreshDisplayBitmap();
				}
			}
		}
	}

	public enum ImageFittingMode{
		None = 0,
		Window = 1,
		WindowLargeOnly = 2,
		WindowWidth = 3,
		WindowWidthLargeOnly = 4,
		WindowHeight = 5,
		WindowHeightLargeOnly = 6,
	}
}
