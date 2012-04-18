/*
	$Id: ProcessSystemDirectory.cs 217 2011-06-21 14:16:53Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CatWalk.IOSystem.Environment {
	[ChildSystemEntryTypes(typeof(ProcessSystemEntry))]
	public class ProcessSystemDirectory : SystemDirectory{
		public string MachineName{get; private set;}
		public bool EnumAllProcesses{get; private set;}

		public ProcessSystemDirectory(ISystemDirectory parent, string name) : this(parent, name, null, false){
		}

		public ProcessSystemDirectory(ISystemDirectory parent, string name, bool enumAllProcesses) : this(parent, name, null, enumAllProcesses){
		}

		public ProcessSystemDirectory(ISystemDirectory parent, string name, string machineName) : this(parent, machineName, false){
		}

		public ProcessSystemDirectory(ISystemDirectory parent, string name, string machineName, bool enumAllProcesses) : base(parent, name){
			this.MachineName = machineName;
			this.EnumAllProcesses = enumAllProcesses;
		}

		#region ISystemDirectory Members

		private IEnumerable<ISystemEntry> GetChildren(){
			if(this.EnumAllProcesses){
				return ((String.IsNullOrEmpty(this.MachineName)) ? Process.GetProcesses() : Process.GetProcesses(this.MachineName))
					.Select(proc => new ProcessSystemEntry(this, proc.Id.ToString(), proc));
			}else{
				return ((String.IsNullOrEmpty(this.MachineName)) ? Process.GetProcesses() : Process.GetProcesses(this.MachineName))
					.Select(proc => new ProcessSystemEntry(this, proc.Id.ToString(), proc))
					.Where(entry => entry.ParentProcess == null);
			}
		}

		public override IEnumerable<ISystemEntry> Children{
			get{
				return this.GetChildren();
			}
		}

		#endregion
	}
}
