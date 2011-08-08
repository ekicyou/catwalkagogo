using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Threading;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Media;

namespace GFV.Imaging {
	using Gfl = GflNet;
	public class GflMultiBitmap : CachedMultiBitmap, IDisposable{

		private Gfl::MultiBitmap _MultiBitmap;
		public GflMultiBitmap(Gfl::MultiBitmap bmp) : base(bmp.FrameCount){
			this._MultiBitmap = bmp;
			bmp.LoadParameters.Origin = Gfl::Origin.TopLeft;
			bmp.LoadParameters.BitmapType = Gfl::BitmapType.Bgra;
			bmp.LoadParameters.Options = Gfl::LoadOptions.ForceColorModel | Gfl::LoadOptions.IgnoreReadError;
			bmp.FrameLoading += new EventHandler(MultiBitmap_FrameLoading);
			bmp.FrameLoaded += new Gfl.FrameLoadedEventHandler(MultiBitmap_FrameLoaded);
			bmp.FrameLoadFailed += new Gfl.FrameLoadFailedEventHandler(MultiBitmap_FrameLoadFailed);
			bmp.LoadParameters.ProgressChanged += new GflNet.ProgressEventHandler(MultiBitmap_ProgressChanged);
		}

		private void MultiBitmap_ProgressChanged(object sender, Gfl.ProgressEventArgs e) {
			this.OnProgressChanged(new ProgressEventArgs(e.ProgressPercentage / 100));
		}

		private void MultiBitmap_FrameLoadFailed(object sender, Gfl.FrameLoadFailedEventArgs e) {
			this._IsLoading = false;
			this.OnLoadFailed(new BitmapLoadFailedEventArgs(e.Exception));
		}

		private void MultiBitmap_FrameLoaded(object sender, Gfl.FrameLoadedEventArgs e) {
			this._IsLoading = false;
			this.OnLoadCompleted(e);
		}

		private void MultiBitmap_FrameLoading(object sender, EventArgs e) {
			this._IsLoading = true;
			this.OnLoadStarted(e);
		}

		private bool _IsLoading = false;
		public override bool IsLoading {
			get {
				return this._IsLoading;
			}
		}

		protected override void DisposeWrappedDecoder() {
			this.Dispose();
		}

		protected override BitmapSource LoadFrame(int index){
			using(var gflBitmap = this._MultiBitmap[index]){
				var length = gflBitmap.BytesPerLine * gflBitmap.Height;
				var pixels = new byte[length];
				Marshal.Copy(gflBitmap.Scan0, pixels, 0, length);
				var bmp = BitmapSource.Create(gflBitmap.Width, gflBitmap.Height, 96, 96, PixelFormats.Bgra32, null, pixels, gflBitmap.BytesPerLine);
				bmp.Freeze();
				return bmp;
			}
		}

		protected override BitmapSource LoadThumbnail() {
			using(var gflBitmap = this._MultiBitmap.GetThumbnail(256, 256)){
				var length = gflBitmap.BytesPerLine * gflBitmap.Height;
				var pixels = new byte[length];
				Marshal.Copy(gflBitmap.Scan0, pixels, 0, length);
				var bmp = BitmapSource.Create(gflBitmap.Width, gflBitmap.Height, 96, 96, PixelFormats.Bgra32, null, pixels, gflBitmap.BytesPerLine);
				bmp.Freeze();
				return bmp;
			}
		}

		#region IMultiBitmap Members

		public override bool IsAnimated {
			get { return false; }
		}

		public override int[] DelayTimes {
			get { throw new NotImplementedException(); }
		}

		public override int LoopCount {
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region IDisposable

		~GflMultiBitmap(){
			this.Dispose(false);
		}

		protected virtual void Dispose(bool disposing){
			if(this._MultiBitmap != null){
				this._MultiBitmap.Dispose();
				this._MultiBitmap = null;
			}
		}

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
