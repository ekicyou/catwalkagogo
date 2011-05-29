using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CatWalk.IOSystem {
	[ChildSystemEntryTypes(typeof(ProcessSystemEntry))]
	public class ProcessSystemDirectory : SystemDirectory{
		private const string ProcessSystemDirectoryId = "Process";
		public string MachineName{get; private set;}
		public bool EnumAllProcesses{get; private set;}

		public ProcessSystemDirectory(ISystemDirectory parent) : this(parent, false){
		}

		public ProcessSystemDirectory(ISystemDirectory parent, bool enumAllProcesses) : base(parent, ProcessSystemDirectoryId){
			this.MachineName = null;
			this.EnumAllProcesses = enumAllProcesses;
			this._Children = new RefreshableLazy<ISystemEntry[]>(this.GetChildren);
		}

		public ProcessSystemDirectory(ISystemDirectory parent, string machineName) : this(parent, machineName, false){
		}

		public ProcessSystemDirectory(ISystemDirectory parent, string machineName, bool enumAllProcesses) : base(parent, machineName){
			this.MachineName = machineName;
			this.EnumAllProcesses = enumAllProcesses;
			this._Children = new RefreshableLazy<ISystemEntry[]>(this.GetChildren);
		}

		public override void Refresh() {
			this._Children.Refresh();
			base.Refresh();
		}

		#region ISystemDirectory Members

		private ISystemEntry[] GetChildren(){
			if(this.EnumAllProcesses){
				return ((String.IsNullOrEmpty(this.MachineName)) ? Process.GetProcesses() : Process.GetProcesses(this.MachineName))
					.Select(proc => new ProcessSystemEntry(this, proc))
					.ToArray();
			}else{
				return ((String.IsNullOrEmpty(this.MachineName)) ? Process.GetProcesses() : Process.GetProcesses(this.MachineName))
					.Select(proc => new ProcessSystemEntry(this, proc))
					.Where(entry => entry.ParentProcess == null)
					.ToArray();
			}
		}

		private RefreshableLazy<ISystemEntry[]> _Children;
		public override IEnumerable<ISystemEntry> Children{
			get{
				return this._Children.Value;
			}
		}

		public override ISystemDirectory GetChildDirectory(object id) {
			return null;
		}

		public override bool Contains(object id) {
			try{
				return this.Children.Cast<ProcessSystemEntry>().Any(proc => proc.Id == id);
			}catch(ArgumentException){
			}catch(InvalidOperationException){
			}
			return false;
		}

		#endregion
	}
}
