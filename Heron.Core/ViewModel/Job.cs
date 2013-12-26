﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CatWalk.Heron.ViewModel {
	public class Job : AppViewModelBase, IJob{
		private JobStatus _Status;
		private double _Progress;
		private Task _Task;
		private CancellationTokenSource _TokenSource;
		private string _Name;
		private readonly DateTime _CreatedTime = DateTime.Now;
		private DateTime _StartedTime = DateTime.MinValue;
		private DateTime _FinishedTime = DateTime.MinValue;

		#region Constructor

		public Job(Action<Job> action)
			: this(action, TaskCreationOptions.None) {

		}

		public Job(Action<Job> action, TaskCreationOptions creationOptions) {
			this._TokenSource = new CancellationTokenSource();
			this._Task = new Task(this.CreateInvoker(action), this._TokenSource.Token, creationOptions);
			this.Initialize();
		}

		public Job(Action<Job, object> action, object state)
			: this(action, state, TaskCreationOptions.None) {
		}

		public Job(Action<Job, object> action, object state, TaskCreationOptions creationOptions) {
			this._TokenSource = new CancellationTokenSource();
			this._Task = new Task(this.CreateInvoker(action), state, this._TokenSource.Token, creationOptions);

			this.Initialize();
		}

		protected Job(Task task, CancellationTokenSource source) {
			task.ThrowIfNull("task");
			source.ThrowIfNull("source");
			this._TokenSource = source;
			this._Task = task;

			this.Initialize();
		}

		private void Initialize() {

			this._Task.ContinueWith((task) => {
				this._Status = JobStatus.Cancelled;
			}, TaskContinuationOptions.OnlyOnCanceled);

			this._Task.ContinueWith((task) => {
				this._Status = JobStatus.Failed;
			}, TaskContinuationOptions.OnlyOnFaulted);

			this._Task.ContinueWith((task) => {
				this._Status = JobStatus.Completed;
			}, TaskContinuationOptions.OnlyOnRanToCompletion);
		}

		#endregion

		#region CreateInvoker

		protected Action CreateInvoker(Action<Job> action) {
			action.ThrowIfNull("action");
			return new Action(delegate {
				action(this);
			});
		}

		protected Action<object> CreateInvoker(Action<Job, object> action) {
			action.ThrowIfNull("action");
			return new Action<object>(delegate(object state) {
				action(this, state);
			});
		}

		protected Action<Task> CreateTaskInvoker(Action<Job> action) {
			action.ThrowIfNull("action");
			return new Action<Task>((task) => {
				action(this);
			});
		}

		protected Func<Task, TResult> CreateTaskInvoker<TResult>(Func<Job, TResult> action) {
			action.ThrowIfNull("action");
			return new Func<Task, TResult>((task) => {
				return action(this);
			});
		}

		protected Action<Task, object> CreateTaskInvoker(Action<Job, object> action) {
			action.ThrowIfNull("action");
			return new Action<Task, object>((task, state) => {
				action(this, state);
			});
		}

		protected Func<Task, object, TResult> CreateTaskInvoker<TResult>(Func<Job, object, TResult> action) {
			action.ThrowIfNull("action");
			return new Func<Task, object, TResult>((task, state) => {
				return action(this, state);
			});
		}

		#endregion

		#region Task

		public void Start() {
			this._Task.Start();
			this.Status = JobStatus.Running;
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

		#region ContinueWith

		public Job ContinueWith(Action<Job> continuationAction) {
			var source = new CancellationTokenSource();
			return new Job(this._Task.ContinueWith(this.CreateTaskInvoker(continuationAction), source.Token), source);
		}
		public Job<TResult> ContinueWith<TResult>(Func<Job, TResult> continuationFunction) {
			var source = new CancellationTokenSource();
			return new Job<TResult>(this._Task.ContinueWith(this.CreateTaskInvoker(continuationFunction), source.Token), source);
		}
		public Job ContinueWith(Action<Job, object> continuationAction, object state) {
			var source = new CancellationTokenSource();
			return new Job(this._Task.ContinueWith(this.CreateTaskInvoker(continuationAction), state, source.Token), source);
		}
		public Job ContinueWith(Action<Job> continuationAction, TaskContinuationOptions continuationOptions) {
			var source = new CancellationTokenSource();
			return new Job(this._Task.ContinueWith(this.CreateTaskInvoker(continuationAction), source.Token, continuationOptions, TaskScheduler.Current), source);
		}
		public Job ContinueWith(Action<Job> continuationAction, TaskScheduler scheduler) {
			var source = new CancellationTokenSource();
			return new Job(this._Task.ContinueWith(this.CreateTaskInvoker(continuationAction), source.Token, TaskContinuationOptions.None, TaskScheduler.Current), source);
		}
		public Job<TResult> ContinueWith<TResult>(Func<Job, object, TResult> continuationFunction, object state) {
			var source = new CancellationTokenSource();
			return new Job<TResult>(this._Task.ContinueWith(this.CreateTaskInvoker(continuationFunction), state, source.Token), source);
		}
		public Job<TResult> ContinueWith<TResult>(Func<Job, TResult> continuationFunction, TaskContinuationOptions continuationOptions) {
			var source = new CancellationTokenSource();
			return new Job<TResult>(this._Task.ContinueWith(this.CreateTaskInvoker(continuationFunction), source.Token, continuationOptions, TaskScheduler.Current), source);
		}
		public Job<TResult> ContinueWith<TResult>(Func<Job, TResult> continuationFunction, TaskScheduler scheduler) {
			var source = new CancellationTokenSource();
			return new Job<TResult>(this._Task.ContinueWith(this.CreateTaskInvoker(continuationFunction), source.Token, TaskContinuationOptions.None, scheduler), source);
		}
		public Job ContinueWith(Action<Job, object> continuationAction, object state, TaskContinuationOptions continuationOptions) {
			var source = new CancellationTokenSource();
			return new Job(this._Task.ContinueWith(this.CreateTaskInvoker(continuationAction), state, source.Token, continuationOptions, TaskScheduler.Current), source);
		}
		public Job ContinueWith(Action<Job, object> continuationAction, object state, TaskScheduler scheduler) {
			var source = new CancellationTokenSource();
			return new Job(this._Task.ContinueWith(this.CreateTaskInvoker(continuationAction), state, source.Token, TaskContinuationOptions.None, scheduler), source);
		}
		public Job<TResult> ContinueWith<TResult>(Func<Job, object, TResult> continuationFunction, object state, TaskContinuationOptions continuationOptions) {
			var source = new CancellationTokenSource();
			return new Job<TResult>(this._Task.ContinueWith(this.CreateTaskInvoker(continuationFunction), state, source.Token, continuationOptions, TaskScheduler.Current), source);
		}
		public Job<TResult> ContinueWith<TResult>(Func<Job, object, TResult> continuationFunction, object state, TaskScheduler scheduler) {
			var source = new CancellationTokenSource();
			return new Job<TResult>(this._Task.ContinueWith(this.CreateTaskInvoker(continuationFunction), state, source.Token, TaskContinuationOptions.None, scheduler), source);
		}
		public Job ContinueWith(Action<Job> continuationFunction, TaskContinuationOptions continuationOptions, TaskScheduler scheduler) {
			var source = new CancellationTokenSource();
			return new Job(this._Task.ContinueWith(this.CreateTaskInvoker(continuationFunction), source.Token, continuationOptions, scheduler), source);
		}
		public Job ContinueWith(Action<Job, object> continuationFunction, object state, TaskContinuationOptions continuationOptions, TaskScheduler scheduler) {
			var source = new CancellationTokenSource();
			return new Job(this._Task.ContinueWith(this.CreateTaskInvoker(continuationFunction), state, source.Token, continuationOptions, scheduler), source);
		}
		public Job<TResult> ContinueWith<TResult>(Func<Job, TResult> continuationFunction, TaskContinuationOptions continuationOptions, TaskScheduler scheduler) {
			var source = new CancellationTokenSource();
			return new Job<TResult>(this._Task.ContinueWith(this.CreateTaskInvoker(continuationFunction), source.Token, continuationOptions, scheduler), source);
		}
		public Job<TResult> ContinueWith<TResult>(Func<Job, object, TResult> continuationFunction, object state, TaskContinuationOptions continuationOptions, TaskScheduler scheduler) {
			var source = new CancellationTokenSource();
			return new Job<TResult>(this._Task.ContinueWith(this.CreateTaskInvoker(continuationFunction), state, source.Token, continuationOptions, scheduler), source);
		}

		#endregion

		#region IProgress<double> Members

		public void Report(double value) {
			this._Progress = value;
		}

		public double Progress {
			get {
				return this._Progress;
			}
			set {
				this._Progress = value;
				this.OnProgressChanged(EventArgs.Empty);
				this.OnPropertyChanged("Progress");
			}
		}

		public void Cancel() {
			this._TokenSource.Cancel();
		}

		public event EventHandler ProgressChanged;
		protected virtual void OnProgressChanged(EventArgs e) {
			var handler = this.ProgressChanged;
			if(handler != null) {
				handler(this, e);
			}
		}
		
		#endregion

		#region JobStatus

		public JobStatus Status {
			get {
				return this._Status;
			}
			protected set {
				this._Status = value;
				this.OnPropertyChanged("Status");
				this.OnStatusChanged(EventArgs.Empty);

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

		public event EventHandler StatusChanged;
		protected virtual void OnStatusChanged(EventArgs e) {
			var handler = this.StatusChanged;
			if(handler != null) {
				handler(this, e);
			}
		}

		public event EventHandler Started;
		protected virtual void OnStarted(EventArgs e) {
			this.StartedTime = DateTime.Now;
			var handler = this.Started;
			if(handler != null) {
				handler(this, e);
			}
		}
		public event EventHandler Completed;
		protected virtual void OnCompleted(EventArgs e) {
			this.FinishedTime = DateTime.Now;
			var handler = this.Completed;
			if(handler != null) {
				handler(this, e);
			}
		}
		public event EventHandler Cancelled;
		protected virtual void OnCancelled(EventArgs e) {
			this.FinishedTime = DateTime.Now;
			var handler = this.Cancelled;
			if(handler != null) {
				handler(this, e);
			}
		}
		public event EventHandler Failed;
		protected virtual void OnFailed(EventArgs e) {
			this.FinishedTime = DateTime.Now;
			var handler = this.Failed;
			if(handler != null) {
				handler(this, e);
			}
		}

		#endregion

		#region Job

		public string Name {
			get {
				return this._Name;
			}
			set {
				this._Name = value;
				this.OnPropertyChanged("Name");
			}
		}

		public DateTime CreatedTime {
			get {
				return this._CreatedTime;
			}
		}

		public DateTime StartedTime {
			get {
				return this._StartedTime;
			}
			private set{
				this._StartedTime = value;
				this.OnPropertyChanged("StartedTime");
			}
		}

		public DateTime FinishedTime {
			get {
				return this._FinishedTime;
			}
			private set {
				this._FinishedTime = value;
				this.OnPropertyChanged("FinishedTime");
			}
		}

	
		#endregion

		#region IJob Members

		public void ReportCancelled() {
			this._TokenSource.Cancel();
		}

		#endregion
	}

	public class Job<TResult> : Job {
		public TResult Result { get; set; }

		#region Constructor

		public Job(Func<Job<TResult>, TResult> action) : base(CreateInvoker(action)) {
		}

		public Job(Func<Job<TResult>, TResult> action, TaskCreationOptions creationOptions)
			: base(CreateInvoker(action), creationOptions) {
		}

		public Job(Func<Job<TResult>, object, TResult> action, object state)
			: base(CreateInvoker(action), state) {
		}

		public Job(Func<Job<TResult>, object, TResult> action, object state, TaskCreationOptions creationOptions)
			: base(CreateInvoker(action), state, creationOptions) {
		}

		internal Job(Task<TResult> task, CancellationTokenSource source) : base(task, source){
			task.ContinueWith(new Action<Task<TResult>>(t2 => {
				this.Result = t2.Result;
			}));
		}

		private static Action<Job> CreateInvoker(Func<Job<TResult>, TResult> action) {
			action.ThrowIfNull("action");
			return new Action<Job>(job => {
				var job2 = ((Job<TResult>)job);
				job2.Result = action(job2);
			});
		}

		private static Action<Job, object> CreateInvoker(Func<Job<TResult>, object, TResult> action) {
			action.ThrowIfNull("action");
			return new Action<Job, object>((job, state) => {
				var job2 = ((Job<TResult>)job);
				job2.Result = action(job2, state);
			});
		}

		private static Action<Job> CreateTaskInvoker(Action<Job<TResult>> action) {
			action.ThrowIfNull("action");
			return new Action<Job>(job => {
				action((Job<TResult>)job);
			});
		}

		private static Func<Job, TNewResult> CreateTaskInvoker<TNewResult>(Func<Job<TResult>, TNewResult> action) {
			action.ThrowIfNull("action");
			return new Func<Job, TNewResult>(job => {
				return action((Job<TResult>)job);
			});
		}

		#endregion

		#region ContinueWith

		public Job ContinueWith(Action<Job<TResult>> continuationAction) {
			return base.ContinueWith(CreateTaskInvoker(continuationAction));
		}

		public Job ContinueWith(Action<Job<TResult>> continuationAction, TaskContinuationOptions continuationOptions) {
			return base.ContinueWith(CreateTaskInvoker(continuationAction), continuationOptions);
		}

		public Job ContinueWith(Action<Job<TResult>> continuationAction, TaskScheduler scheduler) {
			return base.ContinueWith(CreateTaskInvoker(continuationAction), scheduler);
		}

		public Job ContinueWith(Action<Job<TResult>> continuationAction, TaskContinuationOptions continuationOptions, TaskScheduler scheduler){
			return base.ContinueWith(CreateTaskInvoker(continuationAction), continuationOptions, scheduler);
		}

		public Job<TNewResult> ContinueWith<TNewResult>(Func<Job<TResult>, TNewResult> continuationFunction) {
			return base.ContinueWith<TNewResult>(CreateTaskInvoker(continuationFunction));
		}

		public Job<TNewResult> ContinueWith<TNewResult>(Func<Job<TResult>, TNewResult> continuationFunction, TaskContinuationOptions continuationOptions) {
			return base.ContinueWith<TNewResult>(CreateTaskInvoker(continuationFunction), continuationOptions);
		}

		public Job<TNewResult> ContinueWith<TNewResult>(Func<Job<TResult>, TNewResult> continuationFunction, TaskScheduler scheduler) {
			return base.ContinueWith<TNewResult>(CreateTaskInvoker(continuationFunction), scheduler);
		}

		public Job<TNewResult> ContinueWith<TNewResult>(Func<Job<TResult>, TNewResult> continuationFunction, TaskContinuationOptions continuationOptions, TaskScheduler scheduler) {
			return base.ContinueWith<TNewResult>(CreateTaskInvoker(continuationFunction), continuationOptions, scheduler);
		}
		#endregion

	}
}
