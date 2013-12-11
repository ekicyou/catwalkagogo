using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Test.ViewModel.IOSystem;
using CatWalk;
using CatWalk.Windows.Input;
using CatWalk.Threading;
using CatWalk.Collections;

namespace Test.ViewModel {
	public class ListViewModel : ControlViewModel{
		private SystemEntryViewModel _Current;
		private Job _NavigateJob;
		private IHistoryStack<HistoryItem> _History = new HistoryStack<HistoryItem>();

		public ListViewModel(ControlViewModel parent)
			: base(parent) {

		}

		public SystemEntryViewModel CurrentEntry {
			get {
				return this._Current;
			}
			private set {
				this.OnPropertyChanging("Current");
				this._Current = value;
				this.OnPropertyChanged("Current");
			}
		}

		#region SelectedItem

		private object _SelectedItem;
		public object SelectedItem {
			get {
				return this._SelectedItem;
			}
			set {
				this.OnPropertyChanging("SelectedItem");
				this._SelectedItem = value;
				this.OnPropertyChanged("SelectedItem");
			}
		}

		public SystemEntryViewModel SelectedEntry {
			get {
				return this.SelectedItem as SystemEntryViewModel;
			}
		}

		#endregion

		#region FocusedItem

		public SystemEntryViewModel FocusedItem {
			get {
				return (SystemEntryViewModel)this._Current.ChildrenView.CurrentItem;
			}
		}

		#endregion

		#region Open

		private DelegateCommand<string> _OpenCommand;
		public DelegateCommand<string> OpenCommand {
			get {
				return this._OpenCommand ?? (this._OpenCommand = new DelegateCommand<string>(this.Open, this.CanOpen));
			}
		}

		public void Open(string name){
			var entry = this.SelectedEntry;
			if(entry.IsDirectory) {
				this.Navigate(entry);
			}
		}

		public bool CanOpen(string name) {
			return this.SelectedEntry != null || !name.IsNullOrEmpty();
		}

		private void Navigate(SystemEntryViewModel entry) {
			this.Navigate(entry, null);
		}

		private void Navigate(SystemEntryViewModel entry, string focusName) {
			if(this._NavigateJob != null) {
				this._NavigateJob.Cancel();
			}
			var job = this.NewJob();
			this._NavigateJob = job;

			var old = this.CurrentEntry;
			if(old != null) {
				this.ExitEntry(old);
			}
			this.EnterEntry(entry);

			// change current
			this.CurrentEntry = entry;

			// add history
			{
				var focused = this.FocusedItem;
				var focusedName = focused != null ? focused.Name : null;
				this._History.Add(new HistoryItem(
					old.DisplayName,
					() => {
						this.Navigate(old, focusedName);
					}
				));
			}

			Task.Factory.StartNew(new Action(delegate {
				job.Start();
				entry.RefreshChildren(job.Token, job);
				SystemEntryViewModel focus;
				if(entry.Children.TryGetValue(focusName, out focus)) {
					this.SelectedItem = focus;
				}
				job.Complete();
			}), job.Token)
			.ContinueWith(delegate(Task task) {
				job.Fail();
			}, TaskContinuationOptions.OnlyOnFaulted);
		}

		private void EnterEntry(SystemEntryViewModel vm) {
			vm.ResetCancellationToken();
			vm.IsWatcherEnabled = true;
		}

		private void ExitEntry(SystemEntryViewModel vm) {
			vm.IsWatcherEnabled = false;
			vm.CancelTokenProcesses();
		}


		#endregion

		#region GoUp

		private DelegateCommand _GoUpCommand;
		public DelegateCommand GoUpCommand {
			get {
				return this._GoUpCommand ?? (this._GoUpCommand = new DelegateCommand(this.GoUp, this.CanGoUp));
			}
		}

		private void GoUp() {
			this.Navigate(this.CurrentEntry.Parent);
		}

		private bool CanGoUp() {
			return this.CurrentEntry != null && this.CurrentEntry.Parent != null;
		}

		#endregion

		#region History
		private class HistoryItem {
			public string Text { get; private set; }
			public Action _Back;

			public HistoryItem(string text, Action back) {
				this.Text = text;
				back.ThrowIfNull("back");
				this._Back = back;
			}

			public void Back() {
				this._Back();
			}

		}
		#endregion
	}
}
