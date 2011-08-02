using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.IO;

namespace GFV.Imaging {
	using Gfl = GflNet;
	public class GflImageLoader : IImageLoader{
		public Gfl::Gfl Gfl{get; private set;}
		public GflImageLoader(Gfl::Gfl gfl){
			this.Gfl = gfl;
		}

		public IMultiBitmap Load(string file) {
			return this.Load(file, CancellationToken.None);
		}
		public IMultiBitmap Load(string file, CancellationToken token) {
			var bmp = this.Gfl.LoadMultiBitmap(file);
			bmp.FrameLoaded += new GflNet.FrameLoadedEventHandler(MultiBitmap_FrameLoaded);
			bmp.FrameLoading += new EventHandler(MultiBitmap_FrameLoading);
			bmp.FrameLoadFailed += new GflNet.FrameLoadFailedEventHandler(MultiBitmap_FrameLoadFailed);
			bmp.LoadParameters.ProgressChanged += new GflNet.ProgressEventHandler(MultiBitmap_ProgressChanged);
			bmp.LoadParameters.WantCancel += delegate(object sender, CancelEventArgs e){
				if(token.IsCancellationRequested){
					e.Cancel = true;
				}
			};
			return new GflMultiBitmap(bmp);
		}

		private void MultiBitmap_FrameLoadFailed(object sender, Gfl.FrameLoadFailedEventArgs e) {
			this.OnLoadFailed(new BitmapLoadFailedEventArgs(e.Exception));
		}

		public void MultiBitmap_ProgressChanged(object sender, Gfl.ProgressEventArgs e) {
			this.OnProgressChanged(new ProgressEventArgs(e.ProgressPercentage / 100));
		}

		private void MultiBitmap_FrameLoading(object sender, EventArgs e) {
			this.OnLoadStarted(e);
		}

		private void MultiBitmap_FrameLoaded(object sender, Gfl.FrameLoadedEventArgs e) {
			this.OnLoadCompleted(e);
		}

		public IMultiBitmap Load(System.IO.Stream stream) {
			throw new NotImplementedException();
		}

		public IMultiBitmap Load(System.IO.Stream stream, CancellationToken token) {
			throw new NotImplementedException();
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
	}
}
