using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using CatWalk;
using CatWalk.Collections;
using CatWalk.Mvvm;
using CatWalk.IOSystem;
using CatWalk.Heron.IOSystem;

namespace CatWalk.Heron.ViewModel.IOSystem {
	public class SystemEntryViewModel : AppViewModelBase, IHierarchicalViewModel<SystemEntryViewModel>, IDisposable {
		private bool _Disposed = false;
		private readonly IIOSystemWatcher _Watcher;
		private readonly ResetLazy<ChildrenCollection> _Children;
		private readonly ResetLazy<ColumnDictionary> _Columns;
		private readonly ResetLazy<ChildrenCollectionView> _ChildrenView;
		private readonly ILookup<string, EntryGroupDescription> _Groupings;
		public SystemEntryViewModel Parent { get; private set; }
		public ISystemEntry Entry { get; private set; }
		public ISystemProvider Provider { get; private set; }

		#region Constructor

		public SystemEntryViewModel(SystemEntryViewModel parent, ISystemProvider provider, ISystemEntry entry) {
			entry.ThrowIfNull("entry");
			provider.ThrowIfNull("provider");
			/*if(!parent.IsDirectory) {
				throw new ArgumentException("parent");
			}*/
			this.Parent = parent;
			this.Entry = entry;
			this.Provider = provider;
			this._Columns = new ResetLazy<ColumnDictionary>(() => new ColumnDictionary(this));
			if(this.IsDirectory) {
				this._Children = new ResetLazy<ChildrenCollection>(() => new ChildrenCollection());
				this._ChildrenView = new ResetLazy<ChildrenCollectionView>(this.ChildrenViewFactory);
				var watchable = entry as IWatchable;
				if(watchable != null) {
					this._Watcher = watchable.Watcher;
					this._Watcher.IsEnabled = false;
					this._Watcher.CollectionChanged += _Watcher_CollectionChanged;
				}
				this._Groupings = provider.GetGroupings(entry).ToLookup(grp => grp.ColumnName);
			}
		}

		#endregion

		#region Enter / Exit

		public void Enter() {
			this.IsWatcherEnabled = true;
		}

		public void Exit() {
			this.IsWatcherEnabled = false;
			this.CancelTokenProcesses();
			this._Columns.Reset();
			this._Children.Reset();
			this._ChildrenView.Reset();
		}

		#endregion

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
			this._CancellationTokenSource.Reset();
		}

		#endregion

		#region Properties
		public IEnumerable<ColumnDefinition> ColumnDefinitions {
			get {
				return this.Provider.GetColumnDefinitions(this.Entry);
			}
		}

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

		#endregion

		#region ColumnDictionary

		public class ColumnDictionary : IReadOnlyDictionary<string, ColumnViewModel> {
			private SystemEntryViewModel _this;
			private IDictionary<string, Tuple<ColumnDefinition, ColumnViewModel>> _columns = new Dictionary<string, Tuple<ColumnDefinition, ColumnViewModel>>();

			public ColumnDictionary(SystemEntryViewModel vm){
				this._this = vm;
				foreach(var definition in vm.ColumnDefinitions) {
					this._columns.Add(definition.Name, new Tuple<ColumnDefinition, ColumnViewModel>(definition, null));
				}
			}

			public int Count {
				get {
					return this._columns.Count;
				}
			}

			public IEnumerator<KeyValuePair<string, ColumnViewModel>> GetEnumerator() {
				this.CreateAll();
				return this._columns.Select(pair => new KeyValuePair<string, ColumnViewModel>(pair.Key, pair.Value.Item2)).GetEnumerator();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
				this.CreateAll();
				return this._columns.GetEnumerator();
			}

			public IEnumerable<string> Keys {
				get{
					return this._columns.Keys;
				}
			}

			public IEnumerable<ColumnViewModel> Values{
				get {
					this.CreateAll();
					return this._columns.Values.Select(v => v.Item2);
				}
			}

			public ColumnViewModel this[string column] {
				get{
					ColumnViewModel vm;
					if(this.TryGetValue(column, out vm)) {
						return vm;
					} else {
						throw new KeyNotFoundException();
					}
				}
			}

			public bool TryGetValue(string column, out ColumnViewModel vm) {
				Tuple<ColumnDefinition, ColumnViewModel> v;
				if(this._columns.TryGetValue(column, out v)) {
					vm = v.Item2;
					if(vm == null) {
						vm = this.CreateViewModel(v.Item1);
						this._columns[column] = new Tuple<ColumnDefinition, ColumnViewModel>(v.Item1, vm);
					}
					return true;
				} else {
					vm = null;
					return false;
				}
			}

			public bool ContainsKey(string column) {
				return this._columns.ContainsKey(column);
			}

			private ColumnViewModel CreateViewModel(ColumnDefinition provider) {
				var vm = new ColumnViewModel(provider, _this);
				this._columns[provider.Name] = new Tuple<ColumnDefinition,ColumnViewModel>(provider, vm);
				vm.PropertyChanged += vm_PropertyChanged;
				return vm;
			}

			private void vm_PropertyChanged(object sender, PropertyChangedEventArgs e) {
				var column = (ColumnViewModel)sender;
				if(e.PropertyName == "Value") {
					var v = column.Value;
					var c = column.Definition.Name;
					var grps = _this.Parent._Groupings[c]
						.EmptyIfNull()
						.Select(grp => (IEntryGroup)grp.GroupNameFromItem(_this.Entry, 0, Thread.CurrentThread.CurrentUICulture))
						.LazyCache();
					column.Groups = grps;
				}
			}

			private void CreateAll(){
				foreach(var entry in this.Keys) {
					var v = this._columns[entry];
					if(v.Item2 == null) {
						this.CreateViewModel(v.Item1);
					}
				}
			}
		}

		#endregion

		#region Children

		private void ThrowIfNotDirectory() {
			if(!this.IsDirectory) {
				throw new InvalidOperationException("This entry is not a directory");
			}
		}

		public ChildrenCollection Children {
			get {
				this.ThrowIfNotDirectory();
				return this._Children.Value;
			}
		}

		IEnumerable<SystemEntryViewModel> IHierarchicalViewModel<SystemEntryViewModel>.Children {
			get {
				if(this.IsDirectory) {
					return this._Children.Value;
				} else {
					return new SystemEntryViewModel[0];
				}
			}
		}

		public void RefreshChildren() {
			this.RefreshChildren(this.CancellationToken, null);
		}
		public void RefreshChildren(CancellationToken token) {
			this.RefreshChildren(token, null);
		}
		public void RefreshChildren(CancellationToken token, IProgress<double> progress) {
			this.ThrowIfNotDirectory();

			this._Children.Value.Clear();

			var children = this.Entry.GetChildren(token, progress)
				.Select(child => GetViewModel(this, child));

			foreach(var child in children) {
				this._Children.Value.Add(child);
			}
		}

		#endregion

		#region ChildrenCollection
		public class ChildrenCollection : WrappedObservableList<SystemEntryViewModel>{
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

		#endregion

		#region ChildrenCollectionView

		private ChildrenCollectionView ChildrenViewFactory() {
			var view = new ChildrenCollectionView(this, this._Children.Value);
			using(view.DeferRefresh()) {
				view.ColumnOrder = new ColumnOrderDefinition(Seq.Make(new ColumnOrderSet(ColumnDefinition.NameColumn, ListSortDirection.Ascending)));
			}
			return view;
		}

		public ChildrenCollectionView ChildrenView {
			get {
				this.ThrowIfNotDirectory();
				return this._ChildrenView.Value;
			}
		}

		public class ChildrenCollectionView : ListCollectionView {
			public SystemEntryViewModel SourceEntry { get; private set; }
			public EntryFilterCollection Filters { get; private set; }
			private ColumnOrderDefinition _ColumnOrder;

			internal ChildrenCollectionView(SystemEntryViewModel source, System.Collections.IList collection) : base(collection){
				source.ThrowIfNull("source");
				this.SourceEntry = source;
				this.Filters = new EntryFilterCollection();
				this.Filter += this.FilterPredicate;
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

			private bool FilterPredicate(object obj) {
				if(this.Filters.Count > 0) {
					return true;
				} else {
					var entry = (SystemEntryViewModel)obj;
					return this.Filters.Filter(entry);
				}
			}

			public ColumnOrderDefinition ColumnOrder {
				get {
					return this._ColumnOrder;
				}
				set {
					this._ColumnOrder = value;

					this.CustomSort = new SystemEntryViewModelComparer(value);
				}
			}
		}

		#endregion

		#region Watcher

		private bool IsWatcherEnabled {
			get {
				return this._Watcher != null ? this._Watcher.IsEnabled : false;
			}
			set {
				if(this._Watcher != null) {
					this._Watcher.IsEnabled = value;
				}
			}
		}

		private void _Watcher_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			var children = this._Children.Value;
			switch(e.Action) {
				case NotifyCollectionChangedAction.Add: {
						foreach(var item in e.NewItems.Cast<ISystemEntry>()) {
							children.Add(GetViewModel(this, item));
						}
						break;
					}
				case NotifyCollectionChangedAction.Remove: {
						foreach(var item in e.OldItems.Cast<ISystemEntry>()) {
							children.RemoveByName(item.Name);
						}
						break;
					}
				case NotifyCollectionChangedAction.Replace: {
						for(var i = 0; i < e.OldItems.Count; i++) {
							var oldItem = e.OldItems[i] as ISystemEntry;
							var newItem = e.NewItems[i] as ISystemEntry;

							var idx = children.IndexOf(oldItem);
							if(idx >= 0) {
								children[idx] = GetViewModel(this, newItem);
							}
						}
						break;
					}
				case NotifyCollectionChangedAction.Reset: {
						children.Clear();
						foreach(var item in e.NewItems.Cast<ISystemEntry>()) {
							children.Add(GetViewModel(this, item));
						}
						break;
					}
			}
		}

		#endregion

		#region Static

		private static SystemEntryViewModel GetViewModel(SystemEntryViewModel parent, ISystemEntry child) {
			return new SystemEntryViewModel(parent, parent.Provider, child);
		}

		#endregion
	}
}
