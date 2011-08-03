using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;

namespace GFV.Imaging {
	public class GifMultiBitmap : CachedMultiBitmap{
		private Bitmap _Bitmap;

		public GifMultiBitmap(Bitmap bitmap) : base(GetFrameCount(bitmap)){
			this._Bitmap = bitmap;
		}

		private static int GetFrameCount(Bitmap bitmap){
			if(bitmap.FrameDimensionsList.Contains(FrameDimension.Time.Guid)){
				return bitmap.GetFrameCount(FrameDimension.Time);
			}else{
				return 1;
			}
		}
		
		protected override BitmapSource LoadFrame(int index) {
			this.OnLoadStarted(EventArgs.Empty);
			this.OnProgressChanged(new ProgressEventArgs(Double.NaN));
			try{
				this._Bitmap.SelectActiveFrame(FrameDimension.Time, index);
				var bmp = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(this._Bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
				bmp.Freeze();
				this.OnLoadCompleted(EventArgs.Empty);
				return bmp;
			}catch(Exception ex){
				this.OnLoadFailed(new BitmapLoadFailedEventArgs(ex));
				throw ex;
			}
		}

		public override bool IsLoading {
			get {
				return false;
			}
		}

		public override BitmapSource GetThumbnail() {
			return this[0];
		}

		public override bool IsAnimated {
			get {
				return this.FrameCount > 1;
			}
		}

		public override int[] DelayTimes {
			get { throw new NotImplementedException(); }
		}
	}
}
