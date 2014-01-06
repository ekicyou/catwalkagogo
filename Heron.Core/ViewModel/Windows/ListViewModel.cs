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
				this.EntryViewModel = provider.GetViewModel(this, value);
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

		#region SelectedItem

		private object _SelectedItem;
		public object SelectedItem {
			get {
				return this._SelectedItem;
			}
			set {
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
				entry.RefreshChildren(_.Token, _);
				SystemEntryViewModel focus;
				if(entry.Children.TryGetValue(focusName, out focus)) {
					this.SelectedItem = focus;
				}
			});
			this._NavigateJob = job;
			job.Start();
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
