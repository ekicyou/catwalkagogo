using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

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
				return this.Decoder.Frames[index];
			}
		}

		public BitmapSource GetThumbnail() {
			return this.Decoder.Thumbnail;
		}

		public bool IsLoading {
			get {
				return false;
			}
		}
	}
}
