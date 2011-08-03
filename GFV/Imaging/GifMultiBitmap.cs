using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;

namespace GFV.Imaging {
	/*
	public class GifMultiBitmap : IMultiBitmap{
		public GifMultiBitmap(Bitmap bitmap){
			if(bitmap.FrameDimensionsList.Contains(FrameDimension.Time.Guid)){
				this.FrameCount = bitmap.GetFrameCount(FrameDimension.Time);
			}
		}

		#region IMultiBitmap Members

		public bool IsLoading {
			get {
				return false;
			}
		}

		public int FrameCount {get; private set;}

		public BitmapSource this[int index] {
			get { throw new NotImplementedException(); }
		}

		public BitmapSource GetThumbnail() {
			throw new NotImplementedException();
		}

		public bool CanReportProgress {
			get { throw new NotImplementedException(); }
		}

		public event EventHandler LoadStarted;

		public event ProgressEventHandler ProgressChanged;

		public event EventHandler LoadCompleted;

		public event BitmapLoadFailedEventHandler LoadFailed;

		#endregion

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
	}
	 * */
}
