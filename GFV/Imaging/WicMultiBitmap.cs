using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace GFV.Imaging {
	public class WicMultiBitmap : IMultiBitmap, IDisposable{
		private BitmapSource[] _Cache;
		public BitmapDecoder Decoder{get; private set;}
		public WicMultiBitmap(BitmapDecoder dec){
			this.Decoder = dec;
			this._Cache = new BitmapSource[dec.Frames.Count];
		}

		public int FrameCount {
			get {
				return this.Decoder.Frames.Count;
			}
		}

		public BitmapSource this[int index]{
			get{
				//return (BitmapSource)this.Decoder.Dispatcher.Invoke(new Func<int, BitmapSource>(this.GetFrame), index);
				if(this._Cache[index] == null){
					this.OnLoadStarted(EventArgs.Empty);
					try{
						var frame = this.Decoder.Frames[index];
						frame.Freeze();
						//this._Cache[index] = frame;
						//return frame;
						var cache = new WriteableBitmap(frame);
						this._Cache[index] = cache;
						cache.Freeze();
						this.Decoder.Dispatcher.InvokeShutdown();
						this.OnLoadCompleted(EventArgs.Empty);
						return cache;
					}catch(Exception ex){
						this.OnLoadFailed(new BitmapLoadFailedEventArgs(ex));
						throw ex;
					}
				}else{
					return this._Cache[index];
				}
			}
		}
		/*
		private BitmapSource GetFrame(int index){
			return this.Decoder.Frames[index];
		}
		*/
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

		#region IMultiBitmap Members


		public bool IsAnimated {
			get { return false; }
		}

		public int[] DelayTimes {
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region IDisposable Members

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private bool _IsDisposed = false;
		protected virtual void Dispose(bool disposing){
			if(!this._IsDisposed){
				this.Decoder.Dispatcher.InvokeShutdown();
				this._IsDisposed = true;
			}
		}

		~WicMultiBitmap(){
			this.Dispose(false);
		}

		#endregion
	}
}
