using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Nget {
	public abstract class DownloaderRequest {
		public DownloadInfo DownloadInfo{get; private set;}
		public ThreadPriority ThreadPriority{get; set;}

		public DownloaderRequest(){
			this.DownloadInfo = new DownloadInfo();
			this.ThreadPriority = ThreadPriority.Normal;
		}

		public DownloaderRequest(DownloadInfo info){
			this.DownloadInfo = info;
			this.ThreadPriority = ThreadPriority.Normal;
		}

		public abstract DownloaderResponse GetResponse();
	}
}
