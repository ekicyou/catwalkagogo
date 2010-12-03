/*
	$Id$
*/
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace CatWalk.Collections{
	[Serializable]
	public class SkipList<T> : IList<T>{
		#region フィールド
		
		private const int P = Int32.MaxValue / 2;
		
		private SkipListNodeHeader head;
		private SkipListNodeHeader foot;
		private int count = 0;
		private const int maxLevel = Int32.MaxValue;
		private static Random random = new Random();
		
		#endregion
		
		#region コンストラクタ
		
		public SkipList(){
			this.Initialize();
		}
		
		private void Initialize(){
			this.count = 0;
			this.head = new SkipListNodeHeader();
			this.foot = new SkipListNodeHeader();
			this.head.Links.Add(new SkipListNodeLink(this.foot, 1));
			this.foot.Links.Add(new SkipListNodeLink(null, 0));
		}

		public SkipList(IEnumerable<T> collection){
			this.Initialize();
			foreach(var item in collection){
				this.Add(item);
			}
		}
		
		#endregion
		
		#region ロジック
		
		protected virtual int GetRandomLevel(){
			int newLevel = 0;
			while(random.Next() < P){
				newLevel++;
			}
			return Math.Min(newLevel, maxLevel);
		}
		
		private void SetLevel(int level){
			// 上位レベルの構築
			int newLevelCount = level - this.head.Links.Count;
			while(newLevelCount > 0){
				this.head.Links.Add(new SkipListNodeLink(this.foot, this.count + 1));
				this.foot.Links.Add(new SkipListNodeLink(null, 0));
				newLevelCount--;
			}
		}
		
		protected SkipListNode GetNodeAt(int index){
			// Headerの分インクリメント
			index++;
			
			SkipListNode node = this.head;
			int level = this.head.Links.Count - 1;
			int d = 0;
			while(true){
				int t = d + node.Links[level].Distance;
				if(t == index){
					return node.Links[level].Node;
				}else if(t < index){
					d = t;
					node = node.Links[level].Node;
				}else{
					level--;
				}
			}
		}
		
		#endregion
		
		#region 関数
		
		protected void ClearEmptyLevels(){
			for(int level = this.head.Links.Count - 1; level > 0; level--){
				if(this.head.Links[level].Node == this.foot){
					this.head.Links.RemoveAt(level);
				}else{
					break;
				}
			}
		}
		
		public virtual void Add(T value){
			this.Insert(this.count, value);
		}
		
		public virtual void Insert(int index, T value){
			if((index < 0) || (this.count < index)){
				throw new ArgumentOutOfRangeException("index");
			}
			index++;
			
			int newLevel = GetRandomLevel();
			this.SetLevel(newLevel);
			
			var newNode = new SkipListNode(value);
			newNode.Links = new List<SkipListNodeLink>(newLevel + 1);
			for(int i = 0; i <= newLevel; i++){
				newNode.Links.Add(new SkipListNodeLink(null, 0));
			}
			
			SkipListNode node = this.head;
			var nodeIndex = 0;
			var level = this.head.Links.Count - 1;
			//var cost = 0;
			while(level >= 0){
				// 挿入位置検索
				var link = node.Links[level];
				while(link.Node != null){
					var nextIndex = nodeIndex + link.Distance;
					if(nextIndex < index){
						nodeIndex = nextIndex;
						node = link.Node;
						link = node.Links[level];
						//cost++;
					}else{
						break;
					}
				}
				
				if(level > newLevel){
					node.Links[level].Distance++;
				}else{
					// currentの後ろに繋げる
					var next = node.Links[level].Node;
					newNode.Links[level].Node = next;
					node.Links[level].Node = newNode;
					if(level >= 0){
						newNode.Links[level].Distance = nodeIndex + node.Links[level].Distance - index + 1;
						node.Links[level].Distance = index - nodeIndex;
					}
				}
				level--;
			}
			
			this.count++;
			//Console.WriteLine("added node cost:" + cost + " level:" + this.head.Links.Count + " newlevel:" + newLevel + " count:" + this.Count);
			//this.DebugPrint("added node:" + cost + " level:" + this.head.Links.Count);
		}
		
		public virtual bool Contains(T value){
			return (this.IndexOf(value) >= 0);
		}
		
		public virtual bool Remove(T item){
			int index = this.IndexOf(item);
			if(index >= 0){
				this.RemoveAt(index);
				return true;
			}else{
				return false;
			}
		}
		
		public virtual void RemoveAt(int index){
			if(index < 0 || this.count <= index){
				throw new ArgumentOutOfRangeException("index");
			}
			
			index++;
			
			SkipListNode node = this.head;
			var nodeIndex = 0;
			var level = this.head.Links.Count - 1;
			while(level >= 0){
				// 削除位置検索
				var link = node.Links[level];
				while(link.Node != null){
					var nextIndex = nodeIndex + link.Distance;
					if(nextIndex < index){
						nodeIndex = nextIndex;
						node = link.Node;
						link = node.Links[level];
					}else{
						break;
					}
				}
				
				link = node.Links[level];
				if((nodeIndex + link.Distance) == index){
					link.Distance += link.Node.Links[level].Distance - 1;
					link.Node = link.Node.Links[level].Node;
				}else{
					link.Distance--;
				}
				level--;
			}
			this.count--;
			this.ClearEmptyLevels();
			//Console.WriteLine("Removed level:" + this.Head.Links.Count + " count:" + this.Count);
			//this.DebugPrint("Removed level:" + this.Head.Count);
		}
		
		public virtual int IndexOf(T value){
			var index = 0;
			//var cost = 0;
			foreach(var node in this.Nodes){
				if(node.Value.Equals(value)){
					//Console.WriteLine("Found: Cost:" + cost);
					return index;
				}
				//cost++;
				index++;
			}
			//Console.WriteLine("Found: Cost:" + cost);
			return -1;
		}
		
		public virtual void Clear(){
			this.Initialize();
		}
		
		IEnumerator IEnumerable.GetEnumerator(){
			return this.GetEnumerator();
		}
		
		public virtual IEnumerator<T> GetEnumerator(){
			foreach(var node in this.Nodes){
				yield return node.Value;
			}
		}
		
		public virtual void CopyTo(T[] array, int arrayIndex){
			if(array == null){
				throw new ArgumentNullException();
			}
			if(arrayIndex < 0){
				throw new ArgumentOutOfRangeException();
			}
			if((array.Rank > 1) || (array.Length <= arrayIndex) || (this.Count > (array.Length - arrayIndex))){
				throw new ArgumentException();
			}
			int i = arrayIndex;
			foreach(T value in this){
				array[i] = value;
				i++;
			}
		}
		
#if DEBUG
		public void DebugPrint(string m){
			Console.WriteLine(m);
			int level = this.head.Links.Count - 1;
			while(level >= 0){
				SkipListNode node = this.head;
				while(node != null){
					if(node == this.head){
						Console.Write("head");
					}else if(node == this.foot){
						Console.Write("foot");
					}else{
						Console.Write("{0,4}", node.Value);
					}
					if(node.Links[level].Distance > 1){
						Console.Write("-{0,2}{1}", node.Links[level].Distance, new String(' ', (node.Links[level].Distance - 1) * 4 + node.Links[level].Distance - 3));
					}else{
						Console.Write(" ");
					}
					node = node.Links[level].Node;
				}
				Console.Write("\n");
				level--;
			}
		}
#endif
		#endregion
		
		#region プロパティ
		
		public virtual T this[int index]{
			get{
				return this.GetNodeAt(index).Value;
			}
			set{
				this.GetNodeAt(index).Value = value;
			}
		}
		
		public int Count{
			get{
				return this.count;
			}
			protected set{
				this.count = value;
			}
		}
		
		public virtual bool IsReadOnly{
			get{
				return false;
			}
		}
		
		protected SkipListNodeHeader Head{
			get{
				return this.head;
			}
			set{
				this.head = value;
			}
		}

		protected SkipListNodeHeader Foot{
			get{
				return this.foot;
			}
			set{
				this.foot = value;
			}
		}

		protected IEnumerable<SkipListNode> Nodes{
			get{
				SkipListNode node = this.head.Links[0].Node;
				while(node != this.foot){
					yield return node;
					node = node.Links[0].Node;
				}
			}
		}

		#endregion
		
		#region クラス
		
		[Serializable]
		protected class SkipListNode{
			public T Value{get; set;}
			public virtual IList<SkipListNodeLink> Links{get; set;}
			
			public SkipListNode(){
				this.Links = new List<SkipListNodeLink>();
			}
			
			public SkipListNode(T value){
				this.Value = value;
			}
		}
		
		[Serializable]
		protected class SkipListNodeLink{
			public SkipListNode Node{get; set;}
			public int Distance{get; set;}
			
			public SkipListNodeLink(SkipListNode node, int distance){
				this.Node = node;
				this.Distance = distance;
			}
		}
		
		protected class SkipListNodeHeader : SkipListNode{
		}
		
		#endregion
	}
	
	public class SortedSkipList<T> : SkipList<T>{
		private IComparer<T> comparer;
		public bool IsAllowDuplicates{get; private set;}
		
		public SortedSkipList() : this(Comparer<T>.Default, false){
		}
		
		public SortedSkipList(IComparer<T> comparer) : this(comparer, false){
		}
		
		public SortedSkipList(IComparer<T> comparer, bool isAllowDuplicates){
			this.comparer = comparer;
			this.IsAllowDuplicates = isAllowDuplicates;
		}
		
		public override void Insert(int index, T item){
			throw new NotSupportedException();
		}
		
		protected virtual void BaseInsert(int index, T item){
			base.Insert(index, item);
		}
		
		public override void Add(T item){
			var idx = this.IndexOf(item);
			if(idx >= 0){
				base.Insert(idx, item);
			}else{
				base.Insert(~idx, item);
			}
		}
		
		public override int IndexOf(T item){
			SkipListNode node = this.Head;
			var level = this.Head.Links.Count - 1;
			var index = 0;
			var link = node.Links[level];
			//var cost = 0;
			while(link.Node != this.Foot){
				//cost++;
				int d = this.comparer.Compare(item, link.Node.Value);
				if(d < 0){ // item is smaller
					if(level > 0){
						level--;
						link = node.Links[level];
					}else{
						//Console.WriteLine("Found: Cost:" + cost);
						return ~index;
					}
				}else if(d > 0){ // item is bigger
					// find next
					node = link.Node;
					index += link.Distance;
					link = node.Links[level];
					
					// if next is footer find below
					while((level > 0) && (link.Node == this.Foot)){
						level--;
						link = node.Links[level];
					}
				}else{
					node = link.Node;
					index += link.Distance;
					link = node.Links[level];
					//Console.WriteLine("Found: Cost:" + cost);
					return index;
				}
			}
			//Console.WriteLine("Found: Cost:" + cost);
			return ~index;
		}
	}
	
	public class SkipListDictionary<TKey, TValue> : SortedSkipList<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>{
		public SkipListDictionary() : this(Comparer<TKey>.Default){
		}
		
		public SkipListDictionary(IComparer<TKey> comparer) : base(new KeyComparer(comparer), false){
		}
		
		public void Add(TKey key, TValue value){
			this.Add(new KeyValuePair<TKey, TValue>(key, value));
		}
		
		public bool Remove(TKey key){
			return this.Remove(new KeyValuePair<TKey, TValue>(key, default(TValue)));
		}
		
		public bool ContainsKey(TKey key){
			return this.Contains(new KeyValuePair<TKey, TValue>(key, default(TValue)));
		}
		
		public bool TryGetValue(TKey key, out TValue value){
			int index = this.IndexOf(new KeyValuePair<TKey, TValue>(key, default(TValue)));
			if(index < 0){
				value = default(TValue);
				return false;
			}else{
				value = this[index].Value;
				return true;
			}
		}
		
		public TValue this[TKey key]{
			get{
				var index = this.IndexOf(new KeyValuePair<TKey, TValue>());
				if(index < 0){
					throw new KeyNotFoundException();
				}else{
					return this.GetNodeAt(index).Value.Value;
				}
			}
			set{
				var index = this.IndexOf(new KeyValuePair<TKey, TValue>());
				if(index < 0){
					throw new KeyNotFoundException();
				}else{
					this.GetNodeAt(index).Value = new KeyValuePair<TKey, TValue>(key, value);
				}
			}
		}
		
		public ICollection<TKey> Keys{
			get{
				var list = new List<TKey>();
				foreach(var node in this.Nodes){
					list.Add(node.Value.Key);
				}
				return list;
			}
		}
		
		public ICollection<TValue> Values{
			get{
				var list = new List<TValue>();
				foreach(var node in this.Nodes){
					list.Add(node.Value.Value);
				}
				return list;
			}
		}
		
		private class KeyComparer : IComparer<KeyValuePair<TKey, TValue>>{
			private IComparer<TKey> comparer;
			
			public KeyComparer(IComparer<TKey> comparer){
				this.comparer = comparer;
			}
			
			public int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y){
				return this.comparer.Compare(x.Key, y.Key);
			}
		}
	}

	[Serializable]
	public class ObservableSortedSkipList<T> : SortedSkipList<T>, INotifyCollectionChanged, INotifyPropertyChanged{
		public ObservableSortedSkipList() : base(Comparer<T>.Default){
		}
		
		public ObservableSortedSkipList(IComparer<T> comparer) : base(comparer){
		}
		
		#region Reentrancy
		
		private SimpleMonitor monitor = new SimpleMonitor();
		
		protected IDisposable BlockReentrancy(){
			this.monitor.Enter();
			return this.monitor;
		}
		
		protected void CheckReentrancy(){
			if((this.monitor.Busy && (this.CollectionChanged != null)) && (this.CollectionChanged.GetInvocationList().Length > 1)){
				throw new InvalidOperationException();
			}
		}
		
		[Serializable]
		private class SimpleMonitor : IDisposable{
			private int busyCount = 0;
			
			public bool Busy{
				get{
					return this.busyCount > 0;
				}
			}
			
			public void Enter(){
				this.busyCount++;
			}
			
			public void Dispose(){
				this.busyCount--;
			}
		}
		#endregion
		
		#region IList
		
		public override void Add(T item){
			int index = this.IndexOf(item);
			if(index < 0){
				this.CheckReentrancy();
				this.BaseInsert(~index, item);
				this.OnPropertyChanged("Count");
				this.OnPropertyChanged("Item[]");
				this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item, ~index);
			}else{
				this.CheckReentrancy();
				this.BaseInsert(index, item);
				this.OnPropertyChanged("Count");
				this.OnPropertyChanged("Item[]");
				this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
			}
		}
		
		public override void Insert(int index, T item){
			this.Add(item);
		}
		
		public override void RemoveAt(int index){
			this.CheckReentrancy();
			T item = base[index];
			base.RemoveAt(index);
			this.OnPropertyChanged("Count");
			this.OnPropertyChanged("Item[]");
			this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
		}
		
		public override void Clear(){
			if(this.Count > 0){
				this.CheckReentrancy();
				base.Clear();
				this.OnPropertyChanged("Count");
				this.OnPropertyChanged("Item[]");
				this.OnCollectionChanged(NotifyCollectionChangedAction.Reset);
			}
		}
		
		public override bool Remove(T item){
			int index = this.IndexOf(item);
			if(index >= 0){
				this.CheckReentrancy();
				base.RemoveAt(index);
				this.OnPropertyChanged("Count");
				this.OnPropertyChanged("Item[]");
				this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
				return true;
			}else{
				return false;
			}
		}
		
		public override T this[int index]{
			get{
				return base[index];
			}
			set{
				T item = base[index];
				base[index] = value;
				this.OnPropertyChanged("Item[]");
				this.OnCollectionChanged(NotifyCollectionChangedAction.Replace, value, item, index);
			}
		}
		
		#endregion
		
		#region INotifyCollectionChanged
		
		public event NotifyCollectionChangedEventHandler CollectionChanged;
		private void OnCollectionChanged(NotifyCollectionChangedAction action, IList<T> list){
			if(this.CollectionChanged != null){
				using(this.BlockReentrancy()){
					this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, list));
				}
			}
		}
		private void OnCollectionChanged(NotifyCollectionChangedAction action, T newItem, T oldItem, int index){
			if(this.CollectionChanged != null){
				using(this.BlockReentrancy()){
					this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
				}
			}
		}
		
		private void OnCollectionChanged(NotifyCollectionChangedAction action, T item, int index){
			if(this.CollectionChanged != null){
				using(this.BlockReentrancy()){
					this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, item, index));
				}
			}
		}
		
		private void OnCollectionChanged(NotifyCollectionChangedAction action){
			if(this.CollectionChanged != null){
				using(this.BlockReentrancy()){
					this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(action));
				}
			}
		}
		
		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e){
			if(this.CollectionChanged != null){
				using(this.BlockReentrancy()){
					this.CollectionChanged(this, e);
				}
			}
		}
		
		#endregion
		
		#region INotifyPropertyChanged
		
		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string prop){
			if(this.PropertyChanged != null){
				this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
			}
		}
		
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e){
			if(this.PropertyChanged != null){
				this.PropertyChanged(this, e);
			}
		}
		
		#endregion
	}
}