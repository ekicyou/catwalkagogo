using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Threading;
using System.IO;

namespace GFV.Imaging {
	public class WicImageLoader : IImageLoader{

		public IMultiBitmap Load(string file) {
			return this.Load(file, CancellationToken.None);
		}

		public IMultiBitmap Load(string file, CancellationToken token) {
			return new WicMultiBitmap(BitmapDecoder.Create(new Uri(file), BitmapCreateOptions.DelayCreation, BitmapCacheOption.Default));
		}

		public IMultiBitmap Load(Stream stream) {
			return this.Load(stream, CancellationToken.None);
		}

		public IMultiBitmap Load(Stream stream, CancellationToken token) {
			return new WicMultiBitmap(BitmapDecoder.Create(stream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.Default));
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
