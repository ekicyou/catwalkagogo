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
		private readonly object _SyncObject = new object();

		public GifMultiBitmap(Bitmap bitmap, Stream stream) : base(GetFrameCount(bitmap)){
			this._Bitmap = bitmap;
			this._Stream = stream;
			this._DelayTimes = new int[this.FrameCount];
			foreach(var prop in bitmap.PropertyItems){
				switch(prop.Id){
					case 0x5100:{
						for(var i = 0; i < this.FrameCount; i++){
							this.DelayTimes[i] = BitConverter.ToInt32(prop.Value, i * 4) * 10; // ms
						}
						break;
					}
					case 0x5101:{
						this._LoopCount = BitConverter.ToInt16(prop.Value, 0);
						break;
					}
				}
			}
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
			base.DisposeWrappedDecoder();
		}

		public override void PreloadAllFrames() {
			try{
				base.PreloadAllFrames();
			}catch(Exception ex){
				if(this._IsAnyFrameLoaded){
					var count = this.Cache.TakeWhile(b => b != null).Count();
					this.SetFrameCount(count);
				}else{
					throw ex;
				}
			}
		}

		[DllImport("gdi32.dll")]
		private static extern bool DeleteObject(IntPtr hObject);

		private bool _IsAnyFrameLoaded = false;
		protected override BitmapSource LoadFrame(int index) {
			lock(this._SyncObject){
				this.OnLoadStarted(EventArgs.Empty);
				this.OnProgressChanged(new ProgressEventArgs(Double.NaN));
				try{
					this._Bitmap.SelectActiveFrame(FrameDimension.Time, index);
					this._IsAnyFrameLoaded = true;
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
					if(!this._IsAnyFrameLoaded){
						this.OnLoadFailed(new BitmapLoadFailedEventArgs(ex));
					}
					throw ex;
				}
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

		private int[] _DelayTimes;
		public override int[] DelayTimes {
			get {
				return this._DelayTimes;
			}
		}

		private int _LoopCount;
		public override int LoopCount {
			get {
				return this._LoopCount;
			}
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
