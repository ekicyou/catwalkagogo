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
		protected bool IsDecoderDisposed{get; private set;}

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
				if(!this.IsDecoderDisposed && this.Thumbnail != null && !this.Cache.Any(c => c == null)){
					this.DisposeWrappedDecoder();
				}
				return this.Cache[index];
			}
		}

		protected abstract BitmapSource LoadFrame(int index);

		public override void PreloadAllFrames(){
			if(this.IsDecoderDisposed){
				return;
			}

			for(var i = 0; i < this.FrameCount; i++){
				this.Cache[i] = this.LoadFrame(i);
			}
			this.GetThumbnail();
		}

		protected virtual void DisposeWrappedDecoder(){
			this.IsDecoderDisposed = true;
		}

		public sealed override BitmapSource GetThumbnail() {
			var thumb = this.Thumbnail ?? (this.Thumbnail = this.LoadThumbnail());
			if(!this.IsDecoderDisposed && this.Thumbnail != null && !this.Cache.Any(c => c == null)){
				this.DisposeWrappedDecoder();
			}
			return thumb;
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
