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
		private BitmapSource[] _Cache;

		private Gfl::MultiBitmap _MultiBitmap;
		public GflMultiBitmap(Gfl::MultiBitmap bmp){
			this._MultiBitmap = bmp;
			bmp.LoadParameters.BitmapType = Gfl::BitmapType.Bgra;
			bmp.LoadParameters.Options = Gfl::LoadOptions.ForceColorModel | Gfl::LoadOptions.IgnoreReadError;
			bmp.FrameLoading += new EventHandler(MultiBitmap_FrameLoading);
			bmp.FrameLoaded += new Gfl.FrameLoadedEventHandler(MultiBitmap_FrameLoaded);
			bmp.FrameLoadFailed += new Gfl.FrameLoadFailedEventHandler(MultiBitmap_FrameLoadFailed);
			bmp.LoadParameters.ProgressChanged += new GflNet.ProgressEventHandler(MultiBitmap_ProgressChanged);
			this._Cache = new BitmapSource[this._MultiBitmap.FrameCount];
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
				if(this._Cache[index] == null){
					using(var gflBitmap = this._MultiBitmap[index]){
						var length = gflBitmap.BytesPerLine * gflBitmap.Height;
						var pixels = new byte[length];
						Marshal.Copy(gflBitmap.Scan0, pixels, 0, length);
						var bmp = BitmapSource.Create(gflBitmap.Width, gflBitmap.Height, 96, 96, PixelFormats.Bgra32, null, pixels, gflBitmap.BytesPerLine);
						bmp.Freeze();
						this._Cache[index] = bmp;
						return bmp;
					}
				}else{
					return this._Cache[index];
				}
			}
		}

		public BitmapSource GetThumbnail() {
			return this[0];
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

		public event EventHandler LoadStarted;
		protected virtual void OnLoadStarted(EventArgs e){
			var handler = this.LoadStarted;
			if(handler != null){
				handler(this, e);
			}
		}

		public event ProgressEventHandler ProgressChanged;
		protected virtual void OnProgressChanged(ProgressEventArgs e){
			var handler = this.ProgressChanged;
			if(handler != null){
				handler(this, e);
			}
		}

		public event EventHandler LoadCompleted;
		protected virtual void OnLoadCompleted(EventArgs e){
			var handler = this.LoadCompleted;
			if(handler != null){
				handler(this, e);
			}
		}

		public event BitmapLoadFailedEventHandler LoadFailed;
		protected virtual void OnLoadFailed(BitmapLoadFailedEventArgs e){
			var handler = this.LoadFailed;
			if(handler != null){
				handler(this, e);
			}
		}

		public bool CanReportProgress{
			get{
				return false;
			}
		}

		#region IMultiBitmap Members

		public bool IsAnimated {
			get { return false; }
		}

		public int[] DelayTimes {
			get { throw new NotImplementedException(); }
		}

		#endregion
	}
}
