/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatWalk{
	public static class EnumerableExtension{
		/// <summary>
		/// シーケンスの先頭から隣り合う二つのアイテムを順に返します。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns>二つのアイテムが入った<see cref="Tuple"/></returns>
		public static IEnumerable<Tuple<T, T>> Pairwise<T>(this IEnumerable<T> source){
			source.ThrowIfNull("source");
			var enumerator = source.GetEnumerator();
			if(enumerator.MoveNext()){
				var item1 = enumerator.Current;
				while(enumerator.MoveNext()){
					yield return new Tuple<T, T>(item1, enumerator.Current);
					item1 = enumerator.Current;
				}
			}else{
				yield break;
			}
		}
		
		/// <summary>
		/// let句の関数版
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="R"></typeparam>
		/// <param name="var">変数</param>
		/// <param name="func">関数</param>
		/// <returns>関数の戻り値</returns>
		/// <remarks>
		/// <code>
		/// Enumerable.Range(1, 5).Let(e => {
		/// 	...
		/// });
		/// </code>
		/// </remarks>
		public static R Let<T, R>(this T var, Func<T, R> func){
			func.ThrowIfNull("func");
			return func(var);
		}

		public static IEnumerable<R2> Let<T, R1, R2>(this IEnumerable<T> source, Func<T, R1> selector, Func<T, R1, R2> func){
			source.ThrowIfNull("source");
			selector.ThrowIfNull("selector");
			func.ThrowIfNull("func");
			foreach(var item in source){
				yield return func(item, selector(item));
			}
		}
		
		/// <summary>
		/// 与えられたシーケンスを無限に繰り返す。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static IEnumerable<T> Cycle<T>(params T[] source){
			source.ThrowIfNull("source");
			while(true){
				foreach(var item in source) {
					yield return item;
				}
			}
		}

		/// <summary>
		/// 全ての組み合わせを列挙する
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns>各組み合わせ</returns>
		public static IEnumerable<T[]> Combination<T>(this IEnumerable<T> source){
			source.ThrowIfNull("source");
			var array = source.ToArray();
			return CombinationInternal(array, new List<T>(), 0, array.Length);
		}

		/// <summary>
		/// 組み合わせを列挙する
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="n">選ぶ個数</param>
		/// <returns>各組み合わせ</returns>
		public static IEnumerable<T[]> Combination<T>(this IEnumerable<T> source, int n){
			source.ThrowIfNull("source");
			if(n < 0){
				throw new ArgumentOutOfRangeException();
			}
			var array = source.ToArray();
			return CombinationInternal(array, new List<T>(array.Length), 0, n);
		}

		private static IEnumerable<T[]> CombinationInternal<T>(T[] source, List<T> part, int index, int n){
			if(source.Length > index){
				if(part.Count < n){
					part.Add(source[index]);
					foreach(var comb in CombinationInternal(source, part, index + 1, n)){
						yield return comb;
					}
					part.RemoveAt(part.Count - 1);
				}
				foreach(var comb in CombinationInternal(source, part, index + 1, n)){
					yield return comb;
				}
			}else if(source.Length == index){
				yield return part.ToArray();
			}
		}

		/// <summary>
		/// 順列を列挙する
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="n">並べる個数</param>
		/// <returns></returns>
		public static IEnumerable<T[]> Permutation<T>(this IEnumerable<T> source, int n){
			source.ThrowIfNull("source");
			foreach(var comb in source.Combination(n)){
				foreach(var perm in PermutationAllInternal(new LinkedList<T>(comb), new T[comb.Length], 0)){
					yield return perm;
				}
			}
		}

		/// <summary>
		/// 順列を列挙する
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static IEnumerable<T[]> Permutation<T>(this IEnumerable<T> source){
			source.ThrowIfNull("source");
			var list = new LinkedList<T>(source);
			return PermutationAllInternal(list, new T[list.Count], 0);
		}

		private static IEnumerable<T[]> PermutationAllInternal<T>(LinkedList<T> source, T[] part, int index){
			if(source.Count > 0){
				var node = source.First;
				LinkedListNode<T> last = null;
				while(node != null){
					part[index] = node.Value;
					source.Remove(node);

					foreach(var perm in PermutationAllInternal(source, part, index + 1)){
						yield return perm;
					}
					if(last != null){
						source.AddAfter(last, node);
					}else{
						source.AddFirst(node);
					}

					last = node;
					node = node.Next;
				}
			}else{
				yield return part;
			}
		}

		public static T Sum<T>(this IEnumerable<T> source, Func<T, T, T> func){
			source.ThrowIfNull("source");
			func.ThrowIfNull("func");
			T sum = default(T);
			foreach(var v in source){
				sum = func(sum, v);
			}
			return sum;
		}

		public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source){
			if(source == null){
				return new T[0];
			}else{
				return source;
			}
		}

		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action){
			source.ThrowIfNull("source");
			action.ThrowIfNull("action");
			foreach(T value in source){
				action(value);
			}
		}

		public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action){
			source.ThrowIfNull("source");
			action.ThrowIfNull("action");
			var index = 0;
			foreach(T value in source){
				action(value, index++);
			}
		}

		public static T TryThese<T>(this IEnumerable<Func<T>> funcs, Predicate<T> isSuccess){
			return TryThese(funcs, isSuccess, default(T));
		}

		public static T TryThese<T>(this IEnumerable<Func<T>> funcs, Predicate<T> isSuccess, T defValue){
			isSuccess.ThrowIfNull("isSuccess");
			funcs.ThrowIfNull("funcs");
			foreach(var func in funcs){
				var result = func();
				if(isSuccess(result)){
					return result;
				}
			}
			return defValue;
		}

		public static T TryThese<T>(this IEnumerable<Func<T>> funcs, Predicate<T> isSuccess, Func<T> defFunc){
			isSuccess.ThrowIfNull("isSuccess");
			funcs.ThrowIfNull("funcs");
			foreach(var func in funcs){
				var result = func();
				if(isSuccess(result)){
					return result;
				}
			}
			defFunc.ThrowIfNull("defFunc");
			return defFunc();
		}

		public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector){
			source.ThrowIfNull("source");
			keySelector.ThrowIfNull("keySelector");
			var hash = new HashSet<TKey>();
			foreach(var item in source){
				if(hash.Add(keySelector(item))){
					yield return item;
				}
			}
		}
	}
}