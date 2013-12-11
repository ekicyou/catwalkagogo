using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk;
using CatWalk.Collections;

namespace Test.ViewModel {
	public class ControlViewModel : AppViewModelBase, IHierarchicalViewModel<ControlViewModel>, IDisposable{
		private WeakLinkedList<ControlViewModel> _Children = new WeakLinkedList<ControlViewModel>();

		public ControlViewModel Parent {
			get;
			private set;
		}

		public IEnumerable<ControlViewModel> Children {
			get {
				foreach(var c in this._Children){
					yield return c;
				}
			}
		}

		public ControlViewModel(ControlViewModel parent) {
			if(parent != null) {
				parent._Children.AddLast(this);
			}
			this.Parent = parent;
		}

		public Job NewJob() {
			var job = new Job();
			this.AddJob(job);
			return job;
		}

		public void AddJob(Job job) {
			job.ThrowIfNull("job");

			ControlViewModel vm = this;
			while(vm != null) {
				var site = vm as IJobManagerSite;
				if(site != null) {
					site.JobManager.AddJob(job);
					break;
				} else {
					vm = vm.Parent;
				}
			}
		}

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
			if(this.Parent != null) {
				this.Parent._Children.Remove(this);
			}
		}

		~ControlViewModel() {
			this.Dispose(false);
		}
	}
}
