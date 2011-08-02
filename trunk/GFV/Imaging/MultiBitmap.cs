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
		bool CanReportProgress{get;}
		event EventHandler LoadStarted;
		event ProgressEventHandler ProgressChanged;
		event EventHandler LoadCompleted;
		event BitmapLoadFailedEventHandler LoadFailed;
	}

	public delegate void BitmapLoadFailedEventHandler(object sender, BitmapLoadFailedEventArgs e);
	public class BitmapLoadFailedEventArgs : EventArgs{
		public Exception Exception{get; private set;}

		public BitmapLoadFailedEventArgs(Exception ex){
			this.Exception = ex;
		}
	}
}
