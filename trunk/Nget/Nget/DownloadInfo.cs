using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Nget {
	[Serializable]
	public struct DownloadInfo {
		public Uri Uri{get; set;}
		public string OutputPath{get; set;}
		public string OutputDirectory{get; set;}
		public Uri Refferer{get; set;}
		public bool IsResume{get; set;}
		public bool IsOverwrite{get; set;}
		public bool IsTimestamping{get; set;}
	}
}
