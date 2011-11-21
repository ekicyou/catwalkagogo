using System;
using System.Diagnostics;
using System.Windows.Input;
using CatWalk.Mvvm;
using System.ComponentModel;
using Nekome.Windows;
using System.Xml.Serialization;

namespace Nekome{
	[Serializable]
	public class ExternalTool : IMenuItem{
		public string Name{get; set;}
		public string Verb{get; set;}
		public string FileName{get; set;}
		public string Arguments{get; set;}
		public string WorkingDirectory{get; set;}
		public ProcessWindowStyle WindowStyle{get; set;}
		public Key Key{get; set;}
		public ModifierKeys Modifiers{get; set;}
		
		public ExternalTool(){
			this.Verb = "open";
		}

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

		#region IMenuItem

		[XmlIgnore]
		public string HeaderText {
			get {
				return this.Name;
			}
		}

		[XmlIgnore]
		public ICommand Command {
			get {
				return NekomeCommands.ExecuteExternalTool;
			}
		}

		[XmlIgnore]
		public object CommandParameter {
			get {
				return this;
			}
		}

		[XmlIgnore]
		public string InputGestureText{
			get{
				var gesture = new KeyGesture(this.Key, this.Modifiers);
				return TypeDescriptor.GetConverter(typeof(KeyGesture)).ConvertToString(gesture);
			}
		}

		#endregion
	}
}