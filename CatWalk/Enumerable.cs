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
		/// <returns>各組み合わせの列挙子</returns>
		public static IEnumerable<T[]> CombinationAll<T>(this IEnumerable<T> source){
			source.ThrowIfNull("source");
			var array = source.ToArray();
			return CombinationInternal(array, new List<T>(), 0, array.Length);
		}

		public static IEnumerable<T[]> Combination<T>(this IEnumerable<T> source, int n){
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

		public static IEnumerable<T[]> Permutation<T>(this IEnumerable<T> source, int n){
			foreach(var comb in source.Combination(n)){
				foreach(var perm in PermutationAllInternal(new LinkedList<T>(comb), new T[comb.Length], 0)){
					yield return perm;
				}
			}
		}

		public static IEnumerable<T[]> PermutationAll<T>(this IEnumerable<T> source){
			var list = new LinkedList<T>(source);
			return PermutationAllInternal(list, new T[list.Count], 0);
		}
		/*
		public static IEnumerable<T[]> PermutationAllParallel<T>(this IEnumerable<T> source){
			return PermutationAllParallel(source.ToArray(), new T[0]);
		}

		private static IEnumerable<T[]> PermutationAllParallel<T>(T[] source, T[] part){
			if(source.Length > 0){
				var result = new List<T[]>();
				Parallel.For(0, source.Length, delegate(int i){
					var newPart = new T[part.Length + 1];
					part.CopyTo(newPart, 0);
					newPart[part.Length] = source[i];

					var newSource = new T[source.Length - 1];
					Array.Copy(source, 0, newSource, 0, i);
					var len = source.Length - (i + 1);
					if(len > 0){
						Array.Copy(source, i + 1, newSource, i, len);
					}

					foreach(var v in PermutationAll(newSource, newPart)){
						result.Add(v);
					}
				});
				return result;
			}else{
				return new T[0][];
			}
		}
		*/
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


		private static void Permutation_Swap<T>(T[] array, T[] temp, int n){
			var last = array.Length - 1;
			// back
			for(var i = 0; i < n; i++){
				Permutation_Swap(array, last - i, temp, i);
			}
			// exchenge
			Permutation_Swap(array, last - n, temp, 0);
			// return
			for(var i = 0; i < n; i++){
				Permutation_Swap(array, last - (n - i - 1), temp, i);
			}
		}

		public static void Permutation_Swap<T>(this T[] array, int i, T[] temp, int j){
			var t = array[i];
			array[i] = temp[j];
			temp[j] = t;
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
			foreach(T value in source){
				action(value);
			}
		}
	}
}