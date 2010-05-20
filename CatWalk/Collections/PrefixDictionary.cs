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
	public class PrefixDictionary<T> : IDictionary<string, T>{
		#region フィールド
		
		private PrefixTreeNode<T> root = new PrefixTreeNode<T>(default(char), default(T), false);
		private IComparer<char> comparer = null;
		private int count = 0;
		
		#endregion
		
		#region コンストラクタ
		
		public PrefixDictionary(){
			this.comparer = Comparer<char>.Default;
		}
		
		public PrefixDictionary(IDictionary<string, T> dict) : this(Comparer<char>.Default, dict){
		}
		
		public PrefixDictionary(IComparer<char> comparer) : this(comparer, null){
		}
		
		public PrefixDictionary(IComparer<char> comparer, IDictionary<string, T> dict){
			if(comparer == null){
				throw new ArgumentNullException();
			}
			this.comparer = comparer;
			if(dict != null){
				foreach(var pair in dict){
					this.Add(pair.Key, pair.Value);
				}
			}
		}
		
		#endregion
		
		#region ロジック
		
		private bool FindNode(PrefixTreeNode<T> node, string key, out PrefixTreeNode<T> oNode){
			CheckKey(key);
			oNode = node;
			if(key.Length == 0){
				return true;
			}

			int index = 0;
			int lastIndex = key.Length - 1;
			while(oNode != null){
				PrefixTreeNode<T> found = null;
				LinkedListNode<PrefixTreeNode<T>> llNode = oNode.Children.First;
				char c = key[index];
				while(llNode != null){
					PrefixTreeNode<T> child = llNode.Value;
					int d = this.comparer.Compare(c, child.Char);
					if(d == 0){
						found = child;
						break;
					}else if(d > 0){
						break;
					}
					llNode = llNode.Next;
				}
				if(found != null){
					oNode = found;
					if(index == lastIndex){
						return true;
					}
					index++;
				}else{
					break;
				}
			}
			return false;
		}
		
		protected static void CheckKey(string key){
			if(key == null){
				throw new ArgumentNullException();
			}
		}
		
		#endregion
		
		#region 関数
		
		public IEnumerable<KeyValuePair<string,T>> Search(string key){
			return this.Search(key, false);
		}

		public IEnumerable<KeyValuePair<string,T>> Search(string key, bool includeEmptyEntry){
			PrefixTreeNode<T> result;
			if(this.FindNode(this.root, key, out result)){ // found
				if(includeEmptyEntry || result.HasValue){
					yield return result.Entry;
				}
				
				// サブノード追加
				foreach(PrefixTreeNode<T> node in result.SubNodes){
					if(includeEmptyEntry || node.HasValue){
						yield return node.Entry;
					}
				}
			}
			yield break;
		}
		
		public void Add(KeyValuePair<string, T> pair){
			this.Add(pair.Key, pair.Value);
		}
		
		public void Add(string key, T value){
			CheckKey(key);
			if(key.Length == 0){
				if(this.root.HasValue){
					throw new ArgumentException("key");
				}else{
					this.root.HasValue = true;
					this.root.Value = value;
					this.count++;
					return;
				}
			}
			
			PrefixTreeNode<T> node = null;
			int index = 0;
			int lastIndex = key.Length - 1;
			while(true){
				PrefixTreeNode<T> found = null;
				LinkedListNode<PrefixTreeNode<T>> right = null;
				LinkedListNode<PrefixTreeNode<T>> llNode = node.Children.First;
				while(llNode != null){
					PrefixTreeNode<T> child = llNode.Value;
					int d = this.comparer.Compare(key[index], child.Char);
					if(d == 0){
						found = child;
						break;
					}else if(d > 0){
						right = llNode;
						break;
					}
					llNode = llNode.Next;
				}
				
				if(found != null){
					if(index == lastIndex){
						if(found.HasValue){
							throw new ArgumentException("key");
						}else{
							found.HasValue = true;
							found.Value = value;
							this.count++;
							break;
						}
					}else{
						node = found;
					}
				}else{
					PrefixTreeNode<T> newNode = new PrefixTreeNode<T>(node, key[index]);
					if(right != null){
						node.Children.AddBefore(right, newNode);
					}else{
						node.Children.AddLast(newNode);
					}
					if(index == lastIndex){
						newNode.Value = value;
						newNode.HasValue = true;
						this.count++;
						break;
					}else{
						node = newNode;
					}
				}
				index++;
			}
		}
		
		public bool Remove(KeyValuePair<string, T> pair){
			return this.Remove(pair.Key);
		}
		
		public bool Remove(string key){
			CheckKey(key);
			PrefixTreeNode<T> node;
			if(FindNode(this.root, key, out node)){
				if(node.Children.Count > 0){
					node.HasValue = false;
					node.Value = default(T);
				}else{
					node.Parent.Children.Remove(node);
				}
				this.count--;
				return true;
			}else{
				return false;
			}
		}
		
		public bool Contains(KeyValuePair<string, T> pair){
			T item;
			if(this.TryGetValue(pair.Key, out item)){
				return item.Equals(pair.Value);
			}else{
				return false;
			}
		}
		
		public bool ContainsKey(string key){
			PrefixTreeNode<T> node;
			if(this.FindNode(this.root, key, out node)){
				return node.HasValue;
			}else{
				return false;
			}
		}
		
		public bool TryGetValue(string key, out T item){
			PrefixTreeNode<T> node;
			if(this.FindNode(this.root, key, out node)){
				if(node.HasValue){
					item = node.Value;
					return true;
				}
			}
			item = default(T);
			return false;
		}
		
		public void Clear(){
			this.root.Children.Clear();
			this.root.HasValue = false;
			this.root.Value = default(T);
			this.count = 0;
		}
		
		IEnumerator IEnumerable.GetEnumerator(){
			return this.GetEnumerator();
		}
		
		public IEnumerator<KeyValuePair<string,T>> GetEnumerator(){
			foreach(PrefixTreeNode<T> node in this.Nodes){
				if(node.HasValue){
					yield return node.Entry;
				}
			}
		}
		
		public void CopyTo(KeyValuePair<string,T>[] array, int arrayIndex){
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
			foreach(PrefixTreeNode<T> node in this.Nodes){
				if(node.HasValue){
					array[i] = node.Entry;
					i++;
				}
			}
		}
		
		#endregion
		
		#region プロパティ
		
		public int Count{
			get{
				return this.count;
			}
		}
		
		public T this[string key]{
			get{
				CheckKey(key);
				PrefixTreeNode<T> node;
				if(this.FindNode(this.root, key, out node)){
					return node.Value;
				}else{
					throw new KeyNotFoundException();
				}
			}
			set{
				CheckKey(key);
				PrefixTreeNode<T> node;
				if(this.FindNode(this.root, key, out node)){
					node.HasValue = true;
					node.Value = value;
				}else{
					this.Add(key, value);
				}
			}
		}
		
		public ICollection<string> Keys{
			get{
				var list = new Collection<string>();
				foreach(PrefixTreeNode<T> node in this.Nodes){
					if(node.HasValue){
						list.Add(node.Key);
					}
				}
				return list;
			}
		}
		
		public ICollection<T> Values{
			get{
				var list = new Collection<T>();
				foreach(PrefixTreeNode<T> node in this.Nodes){
					if(node.HasValue){
						list.Add(node.Value);
					}
				}
				return list;
			}
		}
		
		public bool IsReadOnly{
			get{
				return false;
			}
		}
		
		private IEnumerable<PrefixTreeNode<T>> Nodes{
			get{
				if(this.root.HasValue){
					yield return this.root;
				}
				foreach(var node in this.root.SubNodes){
					yield return node;
				}
			}
		}
		
		#endregion

		#region 内部クラス

		private class PrefixTreeNode<T> {
			#region フィールド

			public char Char {
				get;
				private set;
			}
			public T Value {
				get;
				set;
			}
			public PrefixTreeNode<T> Parent {
				get;
				set;
			}
			public LinkedList<PrefixTreeNode<T>> Children {
				get;
				set;
			}
			public bool HasValue {
				get;
				set;
			}

			#endregion

			#region コンストラクタ

			public PrefixTreeNode(PrefixTreeNode<T> parent, char key)
				: this(parent, key, default(T), false) {
			}

			public PrefixTreeNode(PrefixTreeNode<T> parent, char key, T value)
				: this(parent, key, default(T), true) {
			}

			public PrefixTreeNode(PrefixTreeNode<T> parent, char key, T value, bool HasValue)
				: this(key, value, HasValue) {
				if(parent == null) {
					throw new ArgumentNullException();
				}
				this.Parent = parent;
				this.Children = new LinkedList<PrefixTreeNode<T>>();
			}

			public PrefixTreeNode(char key, T value, bool HasValue) {
				this.Char = key;
				this.Value = value;
				this.HasValue = HasValue;
				this.Children = new LinkedList<PrefixTreeNode<T>>();
			}

			#endregion

			#region プロパティ

			/// <summary>
			/// キー文字列を取得する。
			/// </summary>
			public string Key {
				get {
					PrefixTreeNode<T>[] nodes = new List<PrefixTreeNode<T>>(this.PrefixNodes).ToArray();
					if(nodes.Length > 0) {
						char[] chars = new char[nodes.Length + 1];
						for(int i = nodes.Length - 1, j = 0; j < nodes.Length; i--, j++) {
							chars[j] = nodes[i].Char;
						}
						chars[nodes.Length] = this.Char;
						return new String(chars);
					} else {
						if(this.Char == '\0') {
							return String.Empty;
						} else {
							return this.Char.ToString();
						}
					}
				}
			}

			/// <summary>
			/// プレフィックスとなる上位ノードを取得する。
			/// 自分自身を含まない。
			/// <summary>
			public IEnumerable<PrefixTreeNode<T>> PrefixNodes {
				get {
					PrefixTreeNode<T> node = this;
					PrefixTreeNode<T> parent = this.Parent;
					while((parent != null) && (parent.Parent != null)) {
						yield return parent;
						parent = parent.Parent;
					}
				}
			}

			/// <summary>
			/// 同じ長さで同じプレフィックスを持つノードを取得する。
			/// 自分自身を含む。
			/// <summary>
			public IEnumerable<PrefixTreeNode<T>> ColumnNodes {
				get {
					PrefixTreeNode<T> parent = this.Parent;
					if(parent != null) {
						foreach(PrefixTreeNode<T> node in parent.Children) {
							yield return node;
						}
					}
				}
			}

			/// <summary>
			/// 自分をプレフィックスとするノードを取得する。
			/// 自分自身を含まない。
			/// </summary>
			public IEnumerable<PrefixTreeNode<T>> SubNodes {
				get {
					foreach(PrefixTreeNode<T> node in this.Children) {
						yield return node;
						foreach(PrefixTreeNode<T> node2 in node.SubNodes) {
							yield return node2;
						}
					}
				}
			}

			public KeyValuePair<string, T> Entry {
				get {
					return new KeyValuePair<string, T>(this.Key, this.Value);
				}
			}

			#endregion
		}
		#endregion
	}
	
	interface ISplitter<TSrc, TDst>{
		TDst[] Split(TSrc src);
		TSrc Join(TDst[] src);
	}
	
	public class StringSplitter : ISplitter<string, char>{
		public char[] Split(string src){
			return src.ToCharArray();
		}
		
		public string Join(char[] src){
			return new String(src);
		}
	}
}