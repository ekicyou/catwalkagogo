/*
	$Id$
*/

using System;
using System.Collections;
using System.Collections.Generic;

namespace CatWalk.Collections{
	[Serializable]
	public class SelectEqualityComparer<T> : IEqualityComparer<T>{
		private Func<T, object> selector;
		public SelectEqualityComparer(Func<T, object> selector){
			selector.ThrowIfNull();
			this.selector = selector;
		}
		
		public bool Equals(T x, T y){
			return this.selector(x) == this.selector(y);
		}
		
		public int GetHashCode(T obj){
			return this.selector(obj).GetHashCode();
		}
	}
	
	[Serializable]
	public class ReversedComparer<T> : IComparer<T>{
		private IComparer<T> comparer;
		
		public ReversedComparer(IComparer<T> comparer){
			comparer.ThrowIfNull();
			this.comparer = comparer;
		}
		
		public int Compare(T x, T y){
			return -(this.comparer.Compare(x, y));
		}
		
		public IComparer<T> BaseComparer{
			get{
				return this.comparer;
			}
		}
	}
	
	[Serializable]
	public class LambdaComparer<T> : IComparer<T>, IComparer{
		private Func<T, T, int> compare;
		
		public LambdaComparer(Func<T, T, int> compare){
			compare.ThrowIfNull();
			this.compare = compare;
		}
		
		public int Compare(object x, object y){
			return this.compare((T)x, (T)y);
		}
		
		public int Compare(T x, T y){
			return this.compare(x, y);
		}
	}
	
	[Serializable]
	public class KeyValuePairComparer<TKey, TValue> : IComparer<KeyValuePair<TKey, TValue>>{
		private IComparer<TKey> comparer;
		public KeyValuePairComparer() : this(Comparer<TKey>.Default){
		}
		
		public KeyValuePairComparer(IComparer<TKey> comparer){
			comparer.ThrowIfNull();
			this.comparer = comparer;
		}
		
		public int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y){
			return this.comparer.Compare(x.Key, y.Key);
		}
	}
	
	[Serializable]
	public class CharIgnoreCaseComparer : IComparer<char>{
		public int Compare(char x, char y){
			const char toSmall = (char)('a' - 'A');
			bool xIsLarge = ('A' <= x) && (x <= 'Z');
			bool yIsLarge = ('A' <= y) && (y <= 'Z');
			if(xIsLarge){
				x = (char)(x + toSmall);
			}
			if(yIsLarge){
				y = (char)(y + toSmall);
			}
			return x.CompareTo(y);
		}

		private static CharIgnoreCaseComparer comparer = null;
		public static CharIgnoreCaseComparer Comparer{
			get{
				return (comparer == null) ? (comparer = new CharIgnoreCaseComparer()) : comparer;
			}
		}
	}
}