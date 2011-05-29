using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CatWalk.IOSystem {
	[ChildSystemEntryTypes(typeof(ProcessSystemEntry))]
	public class ProcessSystemEntry : SystemDirectory, IDisposable{
		public ProcessSystemEntry(ISystemDirectory parent, int id) : base(parent, id){
			this.Initialize();
		}

		public ProcessSystemEntry(ISystemDirectory parent, Process proc) : base(parent, proc.Id){
			this.Initialize();
		}

		private void Initialize(){
			this._Process = new RefreshableLazy<Process>(this.GetProcess);
			this._ParentProcess = new RefreshableLazy<Process>(this.GetParentProcess);
		}

		public override void Refresh() {
			if(this._ParentProcess.IsValueCreated){
				this._ParentProcess.Value.Dispose();
			}
			this._ParentProcess.Refresh();
			if(this._Process.IsValueCreated){
				this._Process.Value.Dispose();
			}
			this._Process.Refresh();
			this.OnPropertyChanged("Process", "ParentProcess", "DisplayName");
			base.Refresh();
		}

		private Process GetProcess(){
			try{
				return Process.GetProcessById((int)this.Id);
			}catch(ArgumentException){
			}catch(InvalidOperationException){
			}
			return null;
		}

		private RefreshableLazy<Process> _Process;
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
			var id = ProcessUtility.GetParentProcessId((int)this.Id);
			if(id != 0){
				try{
					return Process.GetProcessById(id);
				}catch(ArgumentException){
				}catch(InvalidOperationException){
				}
			}
			return null;
		}

		private RefreshableLazy<Process> _ParentProcess;
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

		#region IDisposable Members
		/*
		protected void ThrowIfDisposed(){
			if(this._IsDisposed){
				throw new ObjectDisposedException("this");
			}
		}
		*/
		~ProcessSystemEntry(){
			this.Dispose();
		}

		//private bool _IsDisposed = false;
		public void Dispose() {
			if(this._Process.IsValueCreated){
				this._Process.Value.Dispose();
				this._Process.Refresh();
			}
			if(this._ParentProcess.IsValueCreated){
				this._ParentProcess.Value.Dispose();
				this._ParentProcess.Refresh();
			}
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ISystemDirectory Members

		public override IEnumerable<ISystemEntry> Children {
			get {
				return ProcessUtility.GetChildProcessIds((int)this.Id).Select(id => new ProcessSystemEntry(this, id));
			}
		}

		public override ISystemDirectory GetChildDirectory(object id) {
			return (ISystemDirectory)this.Children.FirstOrDefault(proc => proc.Id == id);
		}

		public override bool Contains(object id) {
			return this.Children.Any(proc => proc.Id == id);
		}

		public override string ConcatDisplayPath(string name) {
			return this.DisplayPath + '\\' + name;
		}

		#endregion
	}
}
