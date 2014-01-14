using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CatWalk.Heron.ViewModel.IOSystem;
using CatWalk;
using CatWalk.Windows.Input;
using CatWalk.Threading;
using CatWalk.Collections;
using CatWalk.Heron.IOSystem;
using Codeplex.Reactive;
using Codeplex.Reactive.Extensions;
using Codeplex.Reactive.Helpers;
using System.Reactive;
using System.Reactive.Linq;
using CatWalk.IOSystem;

namespace CatWalk.Heron.ViewModel.Windows {
	public class ListViewModel : ViewViewModel{
		private SystemEntryViewModel _Current;
		private object _EntryViewModel;
		private Job _NavigateJob;
		private IHistoryStack<HistoryItem> _History = new HistoryStack<HistoryItem>();

		public ListViewModel(SystemEntryViewModel entry){
			this._Current = entry;


		}

		public SystemEntryViewModel CurrentEntry {
			get {
				return this._Current;
			}
			private set {
				value.ThrowIfNull("value");
				this._Current = value;
				this.OnPropertyChanged("CurrentEntry");

				var provider = value.Provider;
				this.EntryViewModel = provider.GetViewModel(this, value, this.EntryViewModel);
			}
		}

		public object EntryViewModel {
			get {
				return this._EntryViewModel;
			}
			private set {
				if(value != this._EntryViewModel) {
					var old = this._EntryViewModel;
					if(old != null) {
						var oldControl = old as ControlViewModel;
						if(oldControl != null) {
							this.Children.Remove(oldControl);
						}
					}
					this._EntryViewModel = value;
					var control = value as ControlViewModel;
					if(control != null) {
						this.Children.Add(control);
					}

					this.OnPropertyChanged("EntryViewModel");
				}
			}
		}

		#region SelectedItems

		public IEnumerable<SystemEntryViewModel> SelectedItems {
			get {
				return this.CurrentEntry.Children.Where(ent => ent.IsSelected);
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
			var entry = this.FocusedItem;
			if(entry.IsDirectory) {
				this.Navigate(entry);
			} else {
				var entries = this.SelectedItems.Select(ent => ent.Entry).ToArray();
				this.CreateJob(job => {
					this.Application.EntryOperator.Open(entries, job.CancellationToken, job);
				}).Start();
			}
		}

		public bool CanOpen(string name) {
			return this.FocusedItem != null || !name.IsNullOrEmpty();
		}

		private void Navigate(SystemEntryViewModel entry) {
			this.Navigate(entry, null);
		}

		private void Navigate(SystemEntryViewModel entry, string focusName) {
			if(this._NavigateJob != null) {
				this._NavigateJob.Cancel();
			}

			var old = this.CurrentEntry;
			if(old != null) {
				old.Exit();
			}
			entry.Enter();

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

			var job = this.CreateJob(_ => {
				entry.RefreshChildren(_.CancellationToken, _);
				SystemEntryViewModel focus;
				if(entry.Children.TryGetValue(focusName, out focus)) {
					//this.FocusedItem = focus;
					// TODO:
				}
			});
			this._NavigateJob = job;
			job.Start();
		}

		#endregion

		#region CopyTo

		private DelegateCommand _CopyToCommand;
		public DelegateCommand CopyToCommand {
			get {
				return this._CopyToCommand ?? (this._CopyToCommand = new DelegateCommand(this.CopyTo));
			}
		}

		public void CopyTo() {
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

		#region Copy

		private ReactiveCommand<ISystemEntry> _CopyCommand;

		public ReactiveCommand<ISystemEntry> CopyCommand {
			get {
				if(this._CopyCommand == null) {
					this.CurrentEntry.Children.CollectionChangedAsObservable();
					var cmd = new ReactiveCommand<ISystemEntry>();
					cmd.Subscribe(this.Copy);
					this._CopyCommand = cmd;
				}
				return this.CopyCommand;
			}
		}

		public void Copy(ISystemEntry dest) {
			if(dest == null) {

			}

			var entries = this.SelectedItems.Select(item => item.Entry).ToArray();
			this.CreateJob(job => {
				this.Application.EntryOperator.Copy(entries, dest, job.CancellationToken, job);
			}).Start();
		}

		#endregion
	}
}
