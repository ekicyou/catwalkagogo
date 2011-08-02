using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace GFV.Imaging {
	public class CombinedImageLoader : IImageLoader{
		private IImageLoader[] _Loaders;

		public CombinedImageLoader(IEnumerable<IImageLoader> loaders){
			this._Loaders = loaders.ToArray();
		}

		public IMultiBitmap Load(string file) {
			return this.Load(file, CancellationToken.None);
		}

		public IMultiBitmap Load(string file, CancellationToken token) {
			var exs = new List<Exception>();
			foreach(var loader in this._Loaders){
				try{
					return loader.Load(file, token);
				}catch(Exception ex){
					exs.Add(ex);
				}
			}
			throw new AggregateException(exs);
		}

		public IMultiBitmap Load(Stream stream) {
			throw new NotImplementedException();
		}

		public IMultiBitmap Load(Stream stream, CancellationToken token) {
			var exs = new List<Exception>();
			foreach(var loader in this._Loaders){
				try{
					return loader.Load(stream, token);
				}catch(Exception ex){
					exs.Add(ex);
				}
			}
			throw new AggregateException(exs);
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
