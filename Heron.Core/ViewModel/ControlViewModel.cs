using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk;
using CatWalk.Collections;
using CatWalk.Mvvm;

namespace CatWalk.Heron.ViewModel {
	public class ControlViewModel : AppViewModelBase, IHierarchicalViewModel<ControlViewModel>, IDisposable{
		public ControlViewModel() : this(null) {
		}

		public ControlViewModel(ControlViewModel parent) {
			this.Parent = parent;
		}

		#region Job

		public Job CreateJob(Action<Job> action){
			var job = new Job(action);
			this.AddJob(job);
			return job;
		}

		public Job CreateJob(Action<Job> action, TaskCreationOptions creationOptions) {
			var job = new Job(action, creationOptions);
			this.AddJob(job);
			return job;
		}

		public Job CreateJob(Action<Job, object> action, object state){
			var job = new Job(action, state);
			this.AddJob(job);
			return job;
		}

		public Job CreateJob(Action<Job, object> action, object state, TaskCreationOptions creationOptions) {
			var job = new Job(action, state, creationOptions);
			this.AddJob(job);
			return job;
		}

		public Job<TResult> CreateJob<TResult>(Func<Job<TResult>, TResult> action){
			var job = new Job<TResult>(action);
			this.AddJob(job);
			return job;
		}

		public Job<TResult> CreateJob<TResult>(Func<Job<TResult>, TResult> action, TaskCreationOptions creationOptions){
			var job = new Job<TResult>(action, creationOptions);
			this.AddJob(job);
			return job;
		}

		public Job<TResult> CreateJob<TResult>(Func<Job<TResult>, object, TResult> action, object state){
			var job = new Job<TResult>(action, state);
			this.AddJob(job);
			return job;
		}

		public Job<TResult> CreateJob<TResult>(Func<Job<TResult>, object, TResult> action, object state, TaskCreationOptions creationOptions){
			var job = new Job<TResult>(action, state, creationOptions);
			this.AddJob(job);
			return job;
		}

		public void AddJob(Job job) {
			job.ThrowIfNull("job");

			var site = Seq.Make(this).Concat(this.Ancestors).OfType<IJobManagerSite>().FirstOrDefault();
			if(site != null) {
				site.JobManager.Register(job);
			}
		}

		#endregion

		#region IHierarchicalViewModel<ControlViewModel> Members

		public IEnumerable<ControlViewModel> Ancestors {
			get {
				ControlViewModel vm = this.Parent;
				while(vm != null) {
					yield return vm;
					vm = vm.Parent;
				}
			}
		}

		private ControlViewModel _Parent;
		public ControlViewModel Parent {
			get {
				return this._Parent;
			}
			set {
				var parent = this._Parent;
				if(parent != null) {
					parent.Children.Remove(this);
				}
				this._Parent = value;
				if(value != null) {
					value.Children.Add(this);
				}
				this.OnPropertyChanged("Parent", "Ancestors");
			}
		}
		public ControlViewModelCollection Children { get; private set; }
		IEnumerable<ControlViewModel> IHierarchicalViewModel<ControlViewModel>.Children {
			get {
				return this.Children;
			}
		}

		public class ControlViewModelCollection : ObservableCollection<ControlViewModel> {

			public ControlViewModel ViewModel { get; private set; }

			public ControlViewModelCollection(ControlViewModel vm) : this(vm, new List<ControlViewModel>()) { }

			public ControlViewModelCollection(ControlViewModel vm, IList<ControlViewModel> collection)
				: base(collection) {
				this.ViewModel = vm;
			}

			protected override void InsertItem(int index, ControlViewModel item) {
				if(item == null) {
					throw new ArgumentNullException("item");
				}

				var parent = item._Parent;
				if(parent != null) {
					parent.Children.Remove(item);
				}
				item._Parent = this.ViewModel;
				base.InsertItem(index, item);
			}

			protected override void RemoveItem(int index) {
				var item = this[index];
				item._Parent = null;
				base.RemoveItem(index);
			}

			protected override void ClearItems() {
				foreach(var item in this) {
					item._Parent = null;
				}
				base.ClearItems();
			}

			protected override void SetItem(int index, ControlViewModel item) {
				if(item == null) {
					throw new ArgumentNullException("item");
				}
				var old = this[index];
				old._Parent = null;

					var parent = item._Parent;
				if(parent != null) {
					parent.Children.Remove(item);
				}

				item._Parent = this.ViewModel;
				base.SetItem(index, item);
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose() {
			if(this.Parent != null) {
				this.Parent.Children.Remove(this);
			}
		}

		#endregion
	}
}
