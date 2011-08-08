using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Threading;

namespace GFV.Imaging {
	public interface IMultiBitmap : IEnumerable<BitmapSource>{
		bool IsLoading{get;}
		int FrameCount{get;}
		BitmapSource this[int index]{get;}
		BitmapSource GetThumbnail();
		void PreloadAllFrames();
		event EventHandler LoadStarted;
		event ProgressEventHandler ProgressChanged;
		event EventHandler LoadCompleted;
		event BitmapLoadFailedEventHandler LoadFailed;

		bool IsAnimated{get;}
		int[] DelayTimes{get;}
		int LoopCount{get;}
	}

	public abstract class MultiBitmap : IMultiBitmap{
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

		public abstract bool IsLoading{get;}
		public abstract int FrameCount{get;}
		public abstract BitmapSource this[int index]{get;}
		public abstract BitmapSource GetThumbnail();
		public abstract void PreloadAllFrames();
		public abstract bool IsAnimated{get;}
		public abstract int[] DelayTimes{get;}
		public abstract int LoopCount{get;}

		public virtual IEnumerator<BitmapSource> GetEnumerator() {
			throw new NotImplementedException();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return this.GetEnumeratorImpl();
		}

		protected abstract System.Collections.IEnumerator GetEnumeratorImpl();
	}

	public delegate void BitmapLoadFailedEventHandler(object sender, BitmapLoadFailedEventArgs e);
	public class BitmapLoadFailedEventArgs : EventArgs{
		public Exception Exception{get; private set;}

		public BitmapLoadFailedEventArgs(Exception ex){
			this.Exception = ex;
		}
	}
}
