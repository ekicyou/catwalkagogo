using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace GFV.Imaging {
	public interface IImageLoader {
		IMultiBitmap Load(string file);
		IMultiBitmap Load(string file, CancellationToken token);
		IMultiBitmap Load(Stream stream);
		IMultiBitmap Load(Stream stream, CancellationToken token);
		event EventHandler LoadStarted;
		event ProgressEventHandler ProgressChanged;
		event EventHandler LoadCompleted;
		event BitmapLoadFailedEventHandler LoadFailed;
	}

	public delegate void ProgressEventHandler(object sender, ProgressEventArgs e);

	public class ProgressEventArgs : EventArgs{
		public double Progress{get; private set;}

		public ProgressEventArgs(double percent){
			this.Progress = percent;
		}
	}

	public delegate void BitmapLoadFailedEventHandler(object sender, BitmapLoadFailedEventArgs e);
	public class BitmapLoadFailedEventArgs : EventArgs{
		public Exception Exceptions{get; private set;}

		public BitmapLoadFailedEventArgs(Exception ex){
			this.Exceptions = ex;
		}
	}
}
