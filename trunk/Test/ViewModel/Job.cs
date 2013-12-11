using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Test.ViewModel {
	public class Job : AppViewModelBase, IProgress<double>{
		private double _Progress;
		private CancellationTokenSource _TokenSource = new CancellationTokenSource();
		private JobStatus _Status;

		public Job() {
			this._Status = JobStatus.Pending;
		}

		#region Properties
		public double Progress {
			get {
				return this._Progress;
			}
			set {
				this.OnPropertyChanging("Progress");
				this._Progress = value;
				this.OnPropertyChanged("Progress");
			}
		}

		public JobStatus Status {
			get {
				return this._Status;
			}
			private set {
				this.OnPropertyChanging("Status");
				this._Status = value;
				this.OnPropertyChanged("Status");

				if(value == JobStatus.Running) {
					this.OnStarted(EventArgs.Empty);
				} else if(value == JobStatus.Completed) {
					this.OnCompleted(EventArgs.Empty);
				} else if(value == JobStatus.Cancelled) {
					this.OnCancelled(EventArgs.Empty);
				} else if(value == JobStatus.Failed) {
					this.OnFailed(EventArgs.Empty);
				}
			}
		}

		public CancellationToken Token {
			get {
				return this._TokenSource.Token;
			}
		}

		public bool IsCancellationRequested {
			get {
				return this._TokenSource.IsCancellationRequested;
			}
		}
		#endregion

		#region Events

		public event EventHandler Started;
		protected virtual void OnStarted(EventArgs e){
			var handler = this.Started;
			if(handler != null) {
				handler(this, e);
			}		
		}
		public event EventHandler Completed;
		protected virtual void OnCompleted(EventArgs e){
			var handler = this.Completed;
			if(handler != null) {
				handler(this, e);
			}		
		}
		public event EventHandler Cancelled;
		protected virtual void OnCancelled(EventArgs e){
			var handler = this.Cancelled;
			if(handler != null) {
				handler(this, e);
			}		
		}
		public event EventHandler Failed;
		protected virtual void OnFailed(EventArgs e) {
			var handler = this.Failed;
			if(handler != null) {
				handler(this, e);
			}
		}

		#endregion

		#region Methods

		public void Start() {
			this.Status = JobStatus.Running;
		}

		public void Report(double value) {
			this.Progress = value;
		}

		public void Complete() {
			this.Status = JobStatus.Completed;
		}

		public void Cancel() {
			if(this.Status == JobStatus.Pending || this.Status == JobStatus.Running) {
				this._TokenSource.Cancel();
				this.Status = JobStatus.Cancelled;
			}
		}

		public void Fail() {
			this.Status = JobStatus.Failed;
		}

		#endregion
	}

	public enum JobStatus {
		Pending,
		Running,
		Cancelled,
		Completed,
		Failed,
	}
}
