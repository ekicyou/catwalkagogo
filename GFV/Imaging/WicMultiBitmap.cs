using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace GFV.Imaging {
	public class WicMultiBitmap : IMultiBitmap{
		public BitmapDecoder Decoder{get; private set;}
		public WicMultiBitmap(BitmapDecoder dec){
			this.Decoder = dec;
		}

		public int FrameCount {
			get {
				return this.Decoder.Frames.Count;
			}
		}

		public BitmapSource this[int index]{
			get{
				this.OnLoadStarted(EventArgs.Empty);
				try{
					var frame = this.Decoder.Frames[index];
					this.OnLoadCompleted(EventArgs.Empty);
					return frame;
				}catch(Exception ex){
					this.OnLoadFailed(new BitmapLoadFailedEventArgs(ex));
					throw ex;
				}
			}
		}

		private void BitmapFrame_DownloadProgress(object sender, DownloadProgressEventArgs e) {
			this.OnProgressChanged(new ProgressEventArgs(e.Progress));
		}

		private void BitmapFrame_DownloadCompleted(object sender, EventArgs e) {
			this.OnLoadCompleted(e);
		}

		private void BitmapFrame_DecodeFailed(object sender, ExceptionEventArgs e) {
			this.OnLoadFailed(new BitmapLoadFailedEventArgs(e.ErrorException));
		}

		public BitmapSource GetThumbnail() {
			return this.Decoder.Thumbnail;
		}

		public bool IsLoading {
			get {
				return false;
			}
		}

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

		public IEnumerator<BitmapSource> GetEnumerator() {
			return this.Decoder.Frames.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return this.Decoder.Frames.GetEnumerator();
		}

		public bool CanReportProgress{
			get{
				return false;
			}
		}
	}
}
