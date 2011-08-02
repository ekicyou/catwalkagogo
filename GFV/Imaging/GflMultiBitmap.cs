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
	public class GflMultiBitmap : IMultiBitmap{
		private Gfl::MultiBitmap _MultiBitmap;
		public GflMultiBitmap(Gfl::MultiBitmap bmp){
			this._MultiBitmap = bmp;
			bmp.FrameLoading += new EventHandler(FrameLoading);
			bmp.FrameLoaded += new Gfl.FrameLoadedEventHandler(FrameLoaded);
			bmp.FrameLoadFailed += new Gfl.FrameLoadFailedEventHandler(FrameLoadFailed);
		}

		private void FrameLoadFailed(object sender, Gfl.FrameLoadFailedEventArgs e) {
			this._IsLoading = false;
		}

		private void FrameLoaded(object sender, Gfl.FrameLoadedEventArgs e) {
			this._IsLoading = false;
		}

		private void FrameLoading(object sender, EventArgs e) {
			this._IsLoading = true;
		}

		private bool _IsLoading = false;
		public bool IsLoading {
			get {
				return this._IsLoading;
			}
		}

		public int FrameCount {
			get {
				return this._MultiBitmap.FrameCount;
			}
		}

		public BitmapSource this[int index]{
			get{
				if(index < 0 || this.FrameCount <= index){
					throw new ArgumentOutOfRangeException("index");
				}
				using(var gflBitmap = this._MultiBitmap[index]){
					var length = gflBitmap.BytesPerLine * gflBitmap.Height;
					var pixels = new byte[length];
					Marshal.Copy(gflBitmap.Scan0, pixels, 0, length);
					var bmp = BitmapSource.Create(gflBitmap.Width, gflBitmap.Height, 96, 96, PixelFormats.Bgra32, null, pixels, gflBitmap.BytesPerLine);
					bmp.Freeze();
					return bmp;
				}
			}
		}

		public BitmapSource GetThumbnail() {
			throw new NotImplementedException();
		}

		#region IEnumerable<BitmapSource> Members

		public IEnumerator<BitmapSource> GetEnumerator() {
			throw new NotImplementedException();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			throw new NotImplementedException();
		}

		#endregion
	}
}
