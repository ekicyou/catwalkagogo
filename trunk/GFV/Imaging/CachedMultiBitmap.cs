using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Threading;

namespace GFV.Imaging {
	public abstract class CachedMultiBitmap : MultiBitmap{
		protected BitmapSource[] Cache{get; private set;}
		protected BitmapSource Thumbnail{get; private set;}

		private int _FrameCount;
		public sealed override int FrameCount{
			get{
				return this._FrameCount;
			}
		}

		protected void SetFrameCount(int count){
			this._FrameCount = count;
		}

		public CachedMultiBitmap(int frameCount){
			this.Cache = new BitmapSource[frameCount];
			this._FrameCount = frameCount;
		}

		public sealed override BitmapSource this[int index]{
			get{
				if(this.Cache[index] == null){
					this.Cache[index] = this.LoadFrame(index);
				}
				return this.Cache[index];
			}
		}

		protected abstract BitmapSource LoadFrame(int index);

		public override void PreloadAllFrames(){
			for(var i = 0; i < this.FrameCount; i++){
				this.Cache[i] = this.LoadFrame(i);
			}
			this.GetThumbnail();
			this.DisposeWrappedDecoder();
		}

		protected virtual void DisposeWrappedDecoder(){
		}

		public sealed override BitmapSource GetThumbnail() {
			return this.Thumbnail ?? (this.Thumbnail = this.LoadThumbnail());
		}

		protected abstract BitmapSource LoadThumbnail();

		public sealed override IEnumerator<BitmapSource> GetEnumerator() {
			this.PreloadAllFrames();
			return this.Cache.ToList().GetEnumerator();
		}

		protected sealed override System.Collections.IEnumerator GetEnumeratorImpl() {
			this.PreloadAllFrames();
			return this.Cache.GetEnumerator();
		}
	}
}
