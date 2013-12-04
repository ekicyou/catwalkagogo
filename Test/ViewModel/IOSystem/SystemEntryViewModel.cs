﻿using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Windows.Data;
using CatWalk;
using CatWalk.Collections;
using CatWalk.Mvvm;
using CatWalk.IOSystem;

namespace Test.ViewModel.IOSystem {
	public class SystemEntryViewModel : AppViewModelBase, IDisposable {
		private bool _Disposed = false;
		private IIOSystemWatcher _Watcher;

		public SystemEntryViewModel(SystemEntryViewModel parent, SystemProvider provider, ISystemEntry entry) {
			entry.ThrowIfNull("entry");
			provider.ThrowIfNull("provider");
			/*if(!parent.IsDirectory) {
				throw new ArgumentException("parent");
			}*/
			this.Parent = parent;
			this.Entry = entry;
			this.Provider = provider;
			this._Columns = new Lazy<ColumnDictionary>(() => new ColumnDictionary(this));
			this.ColumnOrder = new ColumnOrderDefinition(Seq.Make(new ColumnOrderSet(ColumnDefinition.NameColumn, ListSortDirection.Ascending)));
			if(this.IsDirectory) {
				this._Children = new ChildrenCollection();
				this._ChildrenView = new ChildrenCollectionView(this, this._Children);
				var watchable = entry as IWatchable;
				if(watchable != null) {
					this._Watcher = watchable.Watcher;
					this._Watcher.CollectionChanged += _Watcher_CollectionChanged;
				}
			}
		}

		#region IDisposable
		public void Dispose() {
			this.Dispose(true);

			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing) {
			if(!this._Disposed) {
				if(this._CancellationTokenSource.IsValueCreated && !this._CancellationTokenSource.Value.IsCancellationRequested) {
					this._CancellationTokenSource.Value.Cancel();
				}
				this._Disposed = true;
			}
		}

		~SystemEntryViewModel() {
			this.Dispose(false);
		}

		#endregion

		#region Cancellation

		private ResetLazy<CancellationTokenSource> _CancellationTokenSource = new ResetLazy<CancellationTokenSource>(() => new CancellationTokenSource());
		public CancellationToken CancellationToken {
			get {
				return this._CancellationTokenSource.Value.Token;
			}
		}

		public void CancelTokenProcesses() {
			this._CancellationTokenSource.Value.Cancel();
		}

		public void ResetCancellationToken() {
			this._CancellationTokenSource.Reset();
		}
		#endregion

		#region Properties
		public ISystemEntry Entry { get; private set; }
		public SystemProvider Provider { get; private set; }
		public IEnumerable<ColumnDefinition> ColumnDefinitions {
			get {
				return this.Provider.GetColumnDefinitions(this.Entry);
			}
		}

		private Lazy<ColumnDictionary> _Columns;
		public ColumnDictionary Columns {
			get {
				return this._Columns.Value;
			}
		}

		public string Name {
			get {
				return this.Entry.Name;
			}
		}
		public string Path {
			get {
				return this.Entry.Path;
			}
		}
		public string DisplayName {
			get {
				return this.Entry.DisplayName;
			}
		}
		public string DisplayPath {
			get {
				return this.Entry.DisplayPath;
			}
		}
		public bool IsDirectory {
			get {
				return this.Entry.IsDirectory;
			}
		}

		public SystemEntryViewModel Parent { get; private set; }

		#endregion

		#region ColumnDictionary

		public class ColumnDictionary : IReadOnlyDictionary<ColumnDefinition, ColumnViewModel> {
			private SystemEntryViewModel _this;
			private IDictionary<ColumnDefinition, ColumnViewModel> _columns = new Dictionary<ColumnDefinition, ColumnViewModel>();

			public ColumnDictionary(SystemEntryViewModel vm){
				this._this = vm;
				foreach(var definition in vm.ColumnDefinitions) {
					this._columns.Add(definition, null);
				}
			}

			public int Count {
				get {
					return this._columns.Count;
				}
			}

			public IEnumerator<KeyValuePair<ColumnDefinition, ColumnViewModel>> GetEnumerator() {
				this.CreateAll();
				return this._columns.GetEnumerator();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
				this.CreateAll();
				return this._columns.GetEnumerator();
			}

			public IEnumerable<ColumnDefinition> Keys {
				get{
					return this._columns.Keys;
				}
			}

			public IEnumerable<ColumnViewModel> Values{
				get {
					this.CreateAll();
					return this._columns.Values;
				}
			}

			public ColumnViewModel this[ColumnDefinition column] {
				get{
					ColumnViewModel vm;
					if(this.TryGetValue(column, out vm)) {
						return vm;
					} else {
						throw new KeyNotFoundException();
					}
				}
			}

			public bool TryGetValue(ColumnDefinition column, out ColumnViewModel vm) {
				if(this._columns.TryGetValue(column, out vm)) {
					if(vm == null) {
						vm = this.CreateViewModel(column);
					}
					return true;
				} else {
					vm = null;
					return false;
				}
			}

			public bool ContainsKey(ColumnDefinition column) {
				return this._columns.ContainsKey(column);
			}

			private ColumnViewModel CreateViewModel(ColumnDefinition provider) {
				var vm = new ColumnViewModel(provider, _this);
				this._columns[provider] = vm;
				return vm;
			}

			private void CreateAll(){
				foreach(var entry in this.Keys) {
					if(this._columns[entry] == null) {
						this.CreateViewModel(entry);
					}
				}
			}
		}

		#endregion

		#region Children

		private ChildrenCollection _Children;
		public ChildrenCollection Children {
			get {
				if(!this.IsDirectory) {
					throw new InvalidOperationException();
				}
				return this._Children;
			}
		}

		private ChildrenCollectionView _ChildrenView;
		public ChildrenCollectionView ChildrenView {
			get {
				if(!this.IsDirectory) {
					throw new InvalidOperationException();
				}
				return this._ChildrenView;
			}
		}

		public void RefreshChildren(CancellationToken token) {
			if(!this.IsDirectory) {
				throw new InvalidOperationException();
			}

			this._Children.Clear();

			var children = this.Entry.GetChildren(token)
				.Select(child => this.Provider.GetEntryViewModel(this, child));

			foreach(var child in children) {
				this.Children.Add(child);
			}
		}

		public class ChildrenCollection : ObservableList<SystemEntryViewModel>{
			private IDictionary<String, int> nameMap = new Dictionary<string, int>();

			public ChildrenCollection() : base(new SkipList<SystemEntryViewModel>()){
				this.CollectionChanged += ChildrenCollection_CollectionChanged;
			}

			private void ChildrenCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
				lock(this.nameMap) {
					switch(e.Action) {
						case NotifyCollectionChangedAction.Add: {
								var i = e.NewStartingIndex;
								foreach(var item in e.NewItems.Cast<SystemEntryViewModel>()) {
									this.nameMap[item.Name] = i;
									i++;
								}
								break;
							}
						case NotifyCollectionChangedAction.Move: {
								foreach(var item in e.OldItems.Cast<SystemEntryViewModel>()) {
									this.nameMap.Remove(item.Name);
								}
								var i = e.NewStartingIndex;
								foreach(var item in e.NewItems.Cast<SystemEntryViewModel>()) {
									this.nameMap[item.Name] = i;
									i++;
								}
								break;
							}
						case NotifyCollectionChangedAction.Remove: {
								foreach(var item in e.OldItems.Cast<SystemEntryViewModel>()) {
									this.nameMap.Remove(item.Name);
								}
								break;
							}
						case NotifyCollectionChangedAction.Replace: {
								foreach(var item in e.OldItems.Cast<SystemEntryViewModel>()) {
									this.nameMap.Remove(item.Name);
								}
								var i = e.NewStartingIndex;
								foreach(var item in e.NewItems.Cast<SystemEntryViewModel>()) {
									this.nameMap[item.Name] = i;
									i++;
								}
								break;
							}
						case NotifyCollectionChangedAction.Reset: {
								this.nameMap.Clear();
								var i = e.NewStartingIndex;
								foreach(var item in e.NewItems.Cast<SystemEntryViewModel>()) {
									this.nameMap[item.Name] = i;
									i++;
								}
								break;
							}
					}
				}
			}

			#region IReadOnlyDictionary<string, SystemEntryViewModel>

			public bool ContainsKey(string key) {
				return this.nameMap.ContainsKey(key);
			}

			public IEnumerable<string> Keys {
				get {
					return this.nameMap.Keys;
				}
			}

			public bool TryGetValue(string key, out SystemEntryViewModel value) {
				int i;
				if(this.nameMap.TryGetValue(key, out i)){
					value = this[i];
					return true;
				}else{
					value = null;
					return false;
				}
			}

			public IEnumerable<SystemEntryViewModel> Values {
				get {
					return this;
				}
			}

			public SystemEntryViewModel this[string key] {
				get {
					return this[this.nameMap[key]];
				}
			}

			#endregion

			public override int IndexOf(SystemEntryViewModel item) {
				item.ThrowIfNull();
				return this.IndexOf(item.Name);
			}

			public int IndexOf(ISystemEntry entry) {
				entry.ThrowIfNull();
				return this.IndexOf(entry.Name);
			}

			public int IndexOf(string name) {
				name.ThrowIfNull();
				int idx;
				if(this.nameMap.TryGetValue(name, out idx)) {
					return idx;
				} else {
					return -1;
				}
			}

			public bool RemoveByName(string name){
				int i;
				if(this.nameMap.TryGetValue(name, out i)){
					this.RemoveAt(i);
					return true;
				}else{
					return false;
				}
			}
		}

		public class ChildrenCollectionView : ListCollectionView {
			public SystemEntryViewModel SourceEntry { get; private set; }

			internal ChildrenCollectionView(SystemEntryViewModel source, System.Collections.IList collection) : base(collection){
				source.ThrowIfNull("source");
				this.SourceEntry = source;
			}

			protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs args) {
				if(this.SourceEntry.SynchronizeInvoke.InvokeRequired) {
					this.SourceEntry.SynchronizeInvoke.BeginInvoke(new Action<NotifyCollectionChangedEventArgs>(this.InvokeOnCollectionChanged), new object[]{args});
				} else {
					base.OnCollectionChanged(args);
				}
			}

			private void InvokeOnCollectionChanged(NotifyCollectionChangedEventArgs e) {
				base.OnCollectionChanged(e);
			}

		}

		#endregion

		#region Watcher

		public bool IsWatcherEnabled {
			get {
				return this._Watcher.IsEnabled;
			}
			set {
				this._Watcher.IsEnabled = value;
			}
		}

		private void _Watcher_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			switch(e.Action) {
				case NotifyCollectionChangedAction.Add: {
						foreach(var item in e.NewItems.Cast<ISystemEntry>()) {
							this._Children.Add(this.Provider.GetEntryViewModel(this, item));
						}
						break;
					}
				case NotifyCollectionChangedAction.Remove: {
						foreach(var item in e.OldItems.Cast<ISystemEntry>()) {
							this._Children.RemoveByName(item.Name);
						}
						break;
					}
				case NotifyCollectionChangedAction.Replace: {
						for(var i = 0; i < e.OldItems.Count; i++) {
							var oldItem = e.OldItems[i] as ISystemEntry;
							var newItem = e.NewItems[i] as ISystemEntry;

							var idx = this._Children.IndexOf(oldItem);
							if(idx >= 0) {
								this._Children[idx] = this.Provider.GetEntryViewModel(this, newItem);
							}
						}
						break;
					}
				case NotifyCollectionChangedAction.Reset: {
						this._Children.Clear();
						foreach(var item in e.NewItems.Cast<ISystemEntry>()) {
							this._Children.Add(this.Provider.GetEntryViewModel(this, item));
						}
						break;
					}
			}
		}

		#endregion

		#region Ordering

		private ColumnOrderDefinition _ColumnOrder;

		public ColumnOrderDefinition ColumnOrder {
			get {
				return this._ColumnOrder;
			}
			set {
				if(!this.IsDirectory) {
					throw new InvalidOperationException();
				}
				this._ColumnOrder = value;

				this.ChildrenView.CustomSort = new SystemEntryViewModelComparer(value);
			}
		}

		#endregion
	}
}