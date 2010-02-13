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
	public class SkipList<T> : IList<T>{
		#region フィールド
		
		private const int P = Int32.MaxValue / 2;
		
		private SkipListNodeHeader topLeft;
		private SkipListNodeHeader bottomLeft;
		private SkipListNodeFooter topRight;
		private SkipListNodeFooter bottomRight;
		private int levels = 1;
		private int count = 0;
		private const int maxLevels = Int32.MaxValue;
		private Random random = new Random();
		
		#endregion
		
		#region コンストラクタ
		
		public SkipList(){
			this.Initialize();
		}
		
		private void Initialize(){
			this.levels = 1;
			this.count = 0;
			this.topLeft = GetHeaderNode();
			this.bottomLeft = this.topLeft;
			this.topRight = this.bottomRight = (SkipListNodeFooter)this.topLeft.Next;
		}
		
		#endregion
		
		#region ロジック
		
		private static SkipListNodeHeader GetHeaderNode(){
			SkipListNodeHeader negativeInfinity = new SkipListNodeHeader();
			SkipListNodeFooter positiveInfinity = new SkipListNodeFooter();
			
			negativeInfinity.Next = positiveInfinity;
			positiveInfinity.Previous = negativeInfinity;
			
			return negativeInfinity;
		}
		
		protected virtual int GetRandomLevel(){
			int newLevels = 1;
			int max = Math.Min(this.levels + 1, maxLevels);
			while(random.Next() < P){
				newLevels++;
			}
			return newLevels;
		}
		
		protected virtual void ClearEmptyLevels(){
			if(this.levels > 1){
				SkipListNode current = this.topLeft;
				while(current != this.bottomLeft){
					if(current.IsHeader && current.Next.IsFooter){
						SkipListNodeHeader belowLeft = (SkipListNodeHeader)current.Below;
						SkipListNodeFooter belowRight = (SkipListNodeFooter)current.Next.Below;
						this.topLeft = belowLeft;
						this.topRight = belowRight;
						this.levels--;
						current = belowLeft;
					}else{
						break;
					}
				}
			}
		}
		
		protected virtual SkipListNode GetNodeAt(int index){
			if((index < 0) || (this.count <= index)){
				throw new ArgumentOutOfRangeException("index");
			}
			// Headerの分インクリメント
			index++;
			
			SkipListNode node = this.topLeft;
			int d = 0;
			while(true){
				if(d < index){
					int t = d + node.NextDistance;
					if(t == index){
						return node.Next;
					}else if(t < index){
						d = t;
						node = node.Next;
					}else{
						if((index - d) > (t - index)){
							d = t;
							node = node.Next.Below;
						}else{
							node = node.Below;
						}
					}
				}else{
					int t = d - node.PreviousDistance;
					if(t == index){
						if(node.IsHeader){
							throw new Exception();
						}
						return node.Previous;
					}else if(t > index){
						d = t;
						if(node.IsHeader){
							throw new Exception();
						}
						node = node.Previous;
					}else{
						if((d - index) > (index - t)){
							d = t;
							node = node.Previous.Below;
						}else{
							node = node.Below;
						}
					}
				}
			}
			throw new SystemException();
		}
		
		private void SetLevels(int levels){
			// 上位レベルの構築
			int newLevelCount = levels - this.levels;
			while(newLevelCount > 0){
				SkipListNodeHeader newLevel = GetHeaderNode();
				
				// 距離計算
				int d = 0;
				SkipListNode node = this.topLeft;
				while(!node.IsFooter){
					d += node.NextDistance;
					node = node.Next;
				}
				newLevel.NextDistance = newLevel.Next.PreviousDistance = d;
				
				this.topLeft.Above = newLevel;
				this.topRight.Above = newLevel.Next;
				newLevel.Below = this.topLeft;
				newLevel.Next.Below = this.topRight;
				this.topLeft = newLevel;
				this.topRight = (SkipListNodeFooter)newLevel.Next;
				newLevelCount--;
				this.levels++;
			}
		}
		
		#endregion
		
		#region 関数
		
		public void TrimExcess(){
			SkipListNode left = this.topLeft;
			SkipListNode right = this.topRight;
			while(left.Below != null){
				SkipListNode node = left;
				bool remove = true;
				while(!node.IsFooter){
					remove &= (node.NextDistance == node.Below.NextDistance);
					node = node.Next;
				}
				if(remove){
					if(left.Above == null){ // 最上位ノードの削除
						this.topLeft = (SkipListNodeHeader)left.Below;
						this.topRight = (SkipListNodeFooter)right.Below;
						SkipListNode node2 = this.topLeft;
						while(node2 != null){
							node2.Above = null;
							node2 = node2.Next;
						}
					}else{
						SkipListNode node2 = left.Below;
						while(node2 != null){
							node2.Above.Below = node2.Below;
							node2.Below.Above = node2.Above;
							node2 = node2.Next;
						}
					}
				}
				left = left.Below;
				right = right.Below;
			}
		}
		
		public virtual void Add(T value){
			this.Insert(this.Count, value);
		}
		
		public virtual void Insert(int index, T value){
			if((index < 0) || (this.count < index)){
				throw new ArgumentOutOfRangeException("index");
			}
			// Headerの分インクリメント
			index++;
			
			int levels = GetRandomLevel();
			this.SetLevels(levels);
			//this.DebugPrint("added header");
			
			// トップダウンにノードを構築
			SkipListNode current = this.topLeft;
			SkipListNode lastAbove = null;
			int currentLevel = this.levels;
			int currentDistance = 0;
			while(currentLevel > 0){
				// 挿入位置を検索
				while(!(current.IsFooter)){
					int nextIndex = currentDistance + current.NextDistance;
					if(nextIndex >= index){
						break;
					}else{
						currentDistance = nextIndex;
						current = current.Next;
					}
				}
				
				if(currentLevel > levels){
					current.NextDistance++;
					current.Next.PreviousDistance++;
				}else{
					// currentの後ろに繋げる
					SkipListNode newNode = (currentLevel == 1) ? new SkipListNode(value) : new SkipListNode();
					SkipListNode next = current.Next;
					newNode.Next = next;
					newNode.Previous = current;
					next.Previous = newNode;
					current.Next = newNode;
					if(currentLevel > 0){
						newNode.NextDistance = newNode.Next.PreviousDistance = currentDistance + current.NextDistance - index + 1;
						current.NextDistance = newNode.PreviousDistance = index - currentDistance;
					}
					
					if(lastAbove != null){
						lastAbove.Below = newNode;
						newNode.Above = lastAbove;
					}
					lastAbove = newNode;
				}
				
				current = current.Below;
				currentLevel--;
			}
			this.count++;
			//this.DebugPrint("added node");
		}
		
		protected virtual void AddAfter(SkipListNode node, T value){
			if(node == null){
				throw new ArgumentNullException("node");
			}else if(node.IsHeader || node.IsFooter){
				throw new ArgumentException("node");
			}
			
			this.Insert(node.Index + 1, value);
		}
		
		protected virtual void AddBefore(SkipListNode node, T value){
			if(node == null){
				throw new ArgumentNullException("node");
			}else if(node.IsHeader || node.IsFooter){
				throw new ArgumentException("node");
			}
			
			this.Insert(node.Index, value);
		}
		
		public virtual bool Contains(T value){
			SkipListNode node = this.bottomLeft.Next;
			while(!(node.IsFooter)){
				if(node.Value.Equals(value)){
					return true;
				}
				node = node.Next;
			}
			return false;
		}
		
		public virtual bool Remove(T value){
			SkipListNode node = this.bottomLeft.Next;
			while(!(node.IsFooter)){
				if(node.Value.Equals(value)){
					this.Remove(node);
					return true;
				}
				node = node.Next;
			}
			return false;
		}
		
		public virtual void RemoveAt(int index){
			this.Remove(this.GetNodeAt(index));
		}
		
		protected virtual void Remove(SkipListNode node){
			if(node == null){
				throw new ArgumentNullException("node");
			}else if(node.IsHeader || node.IsFooter){
				throw new ArgumentException("node");
			}
			
			SkipListNode prev = node.HighestNode.Previous;
			while(prev != null){
				SkipListNode above = prev.Above;
				while(above != null){
					prev = above;
					above.NextDistance--;
					above.Next.PreviousDistance--;
					if(above.Above != null){
						above = above.Above;
					}else{
						break;
					}
				}
				prev = prev.Previous;
			}
			
			node = node.LowestNode;
			while(node != null){
				node.Previous.Next = node.Next;
				node.Next.Previous = node.Previous;
				node.Previous.NextDistance += node.NextDistance - 1;
				node.Next.PreviousDistance += node.PreviousDistance - 1;
				
				node = node.Above;
			}
			
			this.count--;
			this.ClearEmptyLevels();
		}
		
		public virtual int IndexOf(T value){
			int idx = 0;
			SkipListNode node = this.bottomLeft.Next;
			while(!(node.IsFooter)){
				if(node.Value.Equals(value)){
					return idx;
				}
				node = node.Next;
				idx++;
			}
			return -1;
		}
		
		public virtual void Clear(){
			this.Initialize();
		}
		
		IEnumerator IEnumerable.GetEnumerator(){
			return this.GetEnumerator();
		}
		
		public virtual IEnumerator<T> GetEnumerator(){
			SkipListNode node = this.bottomLeft.Next;
			while(!(node.IsFooter)){
				yield return node.Value;
				node = node.Next;
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
			SkipListNode col = this.topLeft;
			Console.WriteLine(m);
			while(col != null){
				SkipListNode node = col;
				while(node != null){
					if(node.IsHeader){
						Console.Write("head");
					}else if(node.IsFooter){
						Console.Write("foot");
					}else{
						Console.Write("{0,4}", node.Value);
					}
					if(node.NextDistance > 1){
						Console.Write("-{0,2}{1}", node.NextDistance, new String(' ', (node.NextDistance - 1) * 4 + node.NextDistance - 3));
					}else{
						Console.Write(" ");
					}
					node = node.Next;
				}
				Console.Write("\n");
				col = col.Below;
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
		
		public int Levels{
			get{
				return this.levels;
			}
		}
		
		protected SkipListNodeHeader TopLeft{
			get{
				return this.topLeft;
			}
			set{
				this.topLeft = value;
			}
		}
		
		protected SkipListNodeFooter TopRight{
			get{
				return this.topRight;
			}
			set{
				this.topRight = value;
			}
		}
		
		protected SkipListNodeHeader BottomLeft{
			get{
				return this.bottomLeft;
			}
			set{
				this.bottomLeft = value;
			}
		}
		
		protected SkipListNodeFooter BottomRight{
			get{
				return this.bottomRight;
			}
			set{
				this.bottomRight = value;
			}
		}
		
		#endregion
		
		#region クラス
		
		/// <summary>
		/// 最下層のノードのみValueを持つ。
		/// </summary>
		protected class SkipListNode{
			public SkipListNode Next{get; set;}
			public SkipListNode Previous{get; set;}
			public SkipListNode Above{get; set;}
			public SkipListNode Below{get; set;}
			public int NextDistance{get; set;}
			public int PreviousDistance{get; set;}
			private ValueHolder valueHolder;
			
			public SkipListNode(){
				this.NextDistance = 1;
				this.PreviousDistance = 1;
			}
			
			public SkipListNode(T value) : this(){
				this.valueHolder = new ValueHolder(value);
			}
			
			public virtual bool IsHeader{
				get{
					return false;
				}
			}
			
			public virtual bool IsFooter{
				get{
					return false;
				}
			}
			
			public SkipListNode LowestNode{
				get{
					SkipListNode node = this;
					while(node.Below != null){
						node = node.Below;
					}
					return node;
				}
			}
			
			public SkipListNode HighestNode{
				get{
					SkipListNode node = this;
					while(node.Above != null){
						node = node.Above;
					}
					return node;
				}
			}
			
			public int Index{
				get{
					SkipListNode node = this.HighestNode;
					int idx = 0;
					while(!(node.IsHeader)){
						idx += node.PreviousDistance;
						node = node.Previous.HighestNode;
					}
					return idx;
				}
			}
			
			public int CollectionIndex{
				get{
					return this.Index - 1;
				}
			}
			
			public T Value{
				get{
					SkipListNode node = this;
					while(node.Below != null){
						node = node.Below;
					}
					return node.valueHolder.Value;
				}
				set{
					SkipListNode node = this;
					while(node.Below != null){
						node = node.Below;
					}
					node.valueHolder.Value = value;
				}
			}
			
			private class ValueHolder{
				public T Value{get; set;}
				
				public ValueHolder(){
				}
				
				public ValueHolder(T value){
					this.Value = value;
				}
			}
		}
		
		protected class SkipListNodeHeader : SkipListNode{
			public SkipListNodeHeader() : base(){
			}
			
			public override bool IsHeader{
				get{
					return true;
				}
			}
		}
		
		protected class SkipListNodeFooter : SkipListNode{
			public SkipListNodeFooter() : base(){
			}
			
			public override bool IsFooter{
				get{
					return true;
				}
			}
		}
		
		#endregion
	}
	
	public class OrderedSkipList<T> : SkipList<T>{
		private IComparer<T> comparer;
		public bool IsAllowDuplicates{get; private set;}
		
		public OrderedSkipList() : this(Comparer<T>.Default, true){
		}
		
		public OrderedSkipList(IComparer<T> comparer) : this(comparer, true){
		}
		
		public OrderedSkipList(IComparer<T> comparer, bool isAllowDuplicates){
			this.comparer = comparer;
			this.IsAllowDuplicates = isAllowDuplicates;
		}
		
		#region 関数
		
		public override void Insert(int index, T item){
			this.Add(item);
		}
		
		protected void BaseInsert(int index, T item){
			base.Insert(index, item);
		}
		
		public override void Add(T item){
			SkipListNode node;
			if(this.Search(item, out node)){
				base.Insert(node.Index, item);
			}else{
				if(this.IsAllowDuplicates){
					base.Insert(node.Index, item);
				}else{
					throw new ArgumentException();
				}
			}
		}
		
		public int Search(T item){
			SkipListNode node;
			if(this.Search(item, out node)){
				return node.CollectionIndex;
			}else{
				return ~node.Index;
			}
		}
		
		/// <param name="node">見つかったノード、もしくは挿入位置のノード</param>
		/// <returns>ノードが見つかったかどうか</returns>
		protected bool Search(T item, out SkipListNode node){
			node = this.TopLeft;
			while(!node.Next.IsFooter){
				int d = this.comparer.Compare(item, node.Next.Value);
				if(d < 0){ // item is smaller
					if(node.Below != null){
						node = node.Below;
					}else{
						return false;
					}
				}else if(d > 0){ // item is bigger
					// find next
					node = node.Next;
					
					// if next is footer find below
					while((node.Below != null) && (node.Next.IsFooter)){
						node = node.Below;
					}
				}else{
					node = node.Next;
					return true;
				}
			}
			return false;
		}
		
		public override int IndexOf(T item){
			return this.Search(item);
		}
		
		public override bool Remove(T item){
			int index = this.Search(item);
			if(index >= 0){
				base.RemoveAt(index);
				return true;
			}else{
				return false;
			}
		}
		
		public override bool Contains(T item){
			SkipListNode node;
			return this.Search(item, out node);
		}
		
		#endregion
	}
	
	public class ObservableOrderedSkipList<T> : OrderedSkipList<T>, INotifyCollectionChanged, INotifyPropertyChanged{
		public ObservableOrderedSkipList() : base(Comparer<T>.Default, true){
		}
		
		public ObservableOrderedSkipList(IComparer<T> comparer) : base(comparer, true){
		}
		
		public ObservableOrderedSkipList(IComparer<T> comparer, bool isAllowDuplicates) : base(comparer, isAllowDuplicates){
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
			int index = this.Search(item);
			if(index < 0){
				this.CheckReentrancy();
				this.BaseInsert(~index, item);
				this.OnPropertyChanged("Count");
				this.OnPropertyChanged("Item[]");
				this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item, ~index);
			}else{
				if(this.IsAllowDuplicates){
					this.CheckReentrancy();
					this.BaseInsert(index, item);
					this.OnPropertyChanged("Count");
					this.OnPropertyChanged("Item[]");
					this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
				}else{
					throw new ArgumentException();
				}
			}
		}
		/*
		public virtual void AddRange(IEnumerable<T> items){
			this.CheckReentrancy();
			List<T> list = new List<T>(items);
			if(list.Count > 0){
				foreach(T item in list){
					int index = this.Search(item);
					if(index < 0){
						this.BaseInsert(~index, item);
					}else{
						if(this.IsAllowDuplicates){
							this.BaseInsert(index, item);
						}else{
							throw new ArgumentException();
						}
					}
				}
				this.OnPropertyChanged("Count");
				this.OnPropertyChanged("Item[]");
				this.OnCollectionChanged(NotifyCollectionChangedAction.Add, list);
			}
		}
		*/
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
			int index = this.Search(item);
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