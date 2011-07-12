using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;

namespace Nget {
	using Proc = System.Diagnostics.Process;
	public abstract class DownloaderResponse : IDisposable {
		#region Property

		public Process Process{get; private set;}

		public DownloaderResponse(Process process){
			if(process == null){
				throw new ArgumentNullException("progress");
			}
			this.Process = process;
		}

		/// <summary>
		/// Progress of this download.
		/// Negative value means unknown.
		/// </summary>
		public double Progress{
			get{
				return -1;
			}
		}

		/// <summary>
		/// Elapsed time of this download.
		/// Negative value means unknown.
		/// </summary>
		public virtual TimeSpan ElapsedTime{
			get{
				return TimeSpan.MinValue;
			}
		}

		/// <summary>
		/// Estimated time of this download.
		/// Negative value means unknown.
		/// </summary>
		public TimeSpan EstimatedTime{
			get{
				return TimeSpan.MinValue;
			}
		}

		#endregion

		#region Event

		public event EventHandler DownloadEnqueued;
		public event EventHandler DownloadStarted;
		public event EventHandler DownloadProgressChanged;
		public event EventHandler DownloadCompleted;
		public event EventHandler DownloadFailed;

		#endregion

		#region IDisposable

		~DownloaderResponse(){
			this.Process.Dispose();
		}

		public void Dispose(){
			GC.SuppressFinalize(this);
			this.Process.Dispose();
		}


		#endregion
	}

	public struct DownloadStatusEntry{
		public Uri Uri{get; private set;}
		public DownloadStatus Status{get; private set;}
		public double Progress{get; private set;}

		public DownloadStatusEntry(Uri uri, DownloadStatus status, double progress){
			if(uri == null){
				throw new ArgumentNullException("uri");
			}
			this.Uri = uri;
			this.Status = status;
			this.Progress = progress;
		}

		public override string ToString() {
			return this.Uri.ToString();
		}
	}

	public enum DownloadStatus{
		Waiting,
		Downloading,
		Completed,
	}
	/*
	public delegate void DownloadStartingEventHandler(object sender, DownloadStartingEventArgs e);
	public class DownloadStartingEventArgs : EventArgs{
		public Uri Uri{get; private set;}

		public DownloadStartingEventArgs(Uri uri){
			this.Uri = uri;
		}
	}

	public delegate void ResolvingUriEventHandler(object sender, ResolvingUriEventArgs e);
	public class ResolvingUriEventArgs : EventArgs{
		public Uri Host{get; private set;}

		public ResolvingUriEventArgs(Uri host){
			this.Host = host;
		}
	}

	public enum WgetState{
		Initialising,
		Starting,
		Resolving,
		Connecting,
		Requesting,
		Downloading,
		UnknownError,
		NetworkError,
	}
	 */
}
