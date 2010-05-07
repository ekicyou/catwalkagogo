using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Nekome{
	[Serializable]
	public class ExternalTool{
		public string Name{get; set;}
		public string Verb{get; set;}
		public string FileName{get; set;}
		public string Arguments{get; set;}
		public string WorkingDirectory{get; set;}
		public ProcessWindowStyle WindowStyle{get; set;}
		public Key Key{get; set;}
		public ModifierKeys Modifiers{get; set;}
		
		public ProcessStartInfo GetProcessStartInfo(){
			var info = new ProcessStartInfo();
			info.Verb = this.Verb;
			info.FileName = this.FileName;
			info.Arguments = this.Arguments;
			info.WorkingDirectory = this.WorkingDirectory;
			info.WindowStyle = this.WindowStyle;
			return info;
		}
		
		public Process Start(){
			return Process.Start(this.GetProcessStartInfo());
		}
	}
}