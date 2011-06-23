﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk.Collections {
	[Serializable]
	public class SortedSkipList<T> : SkipList<T>{
		private IComparer<T> comparer;
		public bool IsAllowDuplicates{get; private set;}
		
		public SortedSkipList() : this(new T[0], Comparer<T>.Default, false){}
		public SortedSkipList(bool isAllowDuplicates) : this(new T[0], Comparer<T>.Default, isAllowDuplicates){}
		public SortedSkipList(IComparer<T> comparer) : this(new T[0], comparer, false){}
		public SortedSkipList(IComparer<T> comparer, bool isAllowDuplicates) : this(new T[0], comparer, isAllowDuplicates){}
		public SortedSkipList(IEnumerable<T> source) : this(source, Comparer<T>.Default, false){}
		public SortedSkipList(IEnumerable<T> source, bool isAllowDuplicates) : this(source, Comparer<T>.Default, isAllowDuplicates){}
		public SortedSkipList(IEnumerable<T> source, IComparer<T> comparer) : this(source, comparer, false){}
		public SortedSkipList(IEnumerable<T> source, IComparer<T> comparer, bool isAllowDuplicates){
			if(source == null){
				throw new ArgumentNullException("source");
			}
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
					//node = link.Node;
					index += link.Distance;
					//link = node.Links[level];
					//Console.WriteLine("Found: Cost:" + cost);
					return index - 1;
				}
			}
			//Console.WriteLine("Found: Cost:" + cost);
			return ~index;
		}
	}
	
	[Serializable]
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
}