/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CatWalk.IOSystem {
	[ChildSystemEntryTypes(typeof(ProcessSystemEntry))]
	public class ProcessSystemEntry : SystemDirectory{
		public int ProcessId{get; private set;}

		public ProcessSystemEntry(ISystemDirectory parent, string name, int pid) : base(parent, name){
			this.ProcessId = pid;
			this.Initialize();
		}

		public ProcessSystemEntry(ISystemDirectory parent, string name, Process proc) : base(parent, name){
			this.ProcessId = proc.Id;
			this.Initialize();
		}

		private void Initialize(){
			this._Process = new Lazy<Process>(this.GetProcess);
			this._ParentProcess = new Lazy<Process>(this.GetParentProcess);
		}

		private Process GetProcess(){
			try{
				return Process.GetProcessById(this.ProcessId);
			}catch(ArgumentException){
			}catch(InvalidOperationException){
			}
			return null;
		}

		private Lazy<Process> _Process;
		public Process Process{
			get{
				return this._Process.Value;
			}
		}

		public override string DisplayName {
			get {
				return this.Process.ProcessName;
			}
		}

		private Process GetParentProcess(){
			var id = ProcessUtility.GetParentProcessId(this.ProcessId);
			if(id != 0){
				try{
					return Process.GetProcessById(id);
				}catch(ArgumentException){
				}catch(InvalidOperationException){
				}
			}
			return null;
		}

		private Lazy<Process> _ParentProcess;
		public Process ParentProcess{
			get{
				return this._ParentProcess.Value;
			}
		}

		public override bool Exists {
			get {
				return this.Process != null || !this.Process.HasExited;
			}
		}

		#region ISystemDirectory Members

		public override IEnumerable<ISystemEntry> Children {
			get {
				return ProcessUtility.GetChildProcessIds(this.ProcessId)
					.Select(id => Process.GetProcessById(id))
					.Select(proc => new ProcessSystemEntry(this, proc.Id.ToString(), proc));
			}
		}

		#endregion
	}
}
