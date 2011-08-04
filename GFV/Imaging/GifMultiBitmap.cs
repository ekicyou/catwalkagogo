using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Runtime.InteropServices;
using System.IO;

namespace GFV.Imaging {
	public class GifMultiBitmap : CachedMultiBitmap, IDisposable{
		private Bitmap _Bitmap;
		private Stream _Stream;

		public GifMultiBitmap(Bitmap bitmap, Stream stream) : base(GetFrameCount(bitmap)){
			this._Bitmap = bitmap;
			this._Stream = stream;
		}

		private static int GetFrameCount(Bitmap bitmap){
			if(bitmap.FrameDimensionsList.Contains(FrameDimension.Time.Guid)){
				return bitmap.GetFrameCount(FrameDimension.Time);
			}else{
				return 1;
			}
		}

		protected override void DisposeWrappedDecoder() {
			this.Dispose();
		}

		[DllImport("gdi32.dll")]
		private static extern bool DeleteObject(IntPtr hObject);

		protected override BitmapSource LoadFrame(int index) {
			this.OnLoadStarted(EventArgs.Empty);
			this.OnProgressChanged(new ProgressEventArgs(Double.NaN));
			try{
				this._Bitmap.SelectActiveFrame(FrameDimension.Time, index);
				var hBitmap = this._Bitmap.GetHbitmap();
				try{
					var bmp = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
					bmp.Freeze();
					this.OnLoadCompleted(EventArgs.Empty);
					return bmp;
				}finally{
					DeleteObject(hBitmap);
				}
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

		protected override BitmapSource LoadThumbnail() {
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

		#region IDisposable

		~GifMultiBitmap(){
			this.Dispose(false);
		}

		protected virtual void Dispose(bool disposing){
			if(this._Bitmap != null){
				this._Stream.Dispose();
				this._Bitmap.Dispose();
				this._Bitmap = null;
				this._Stream = null;
			}
		}

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
