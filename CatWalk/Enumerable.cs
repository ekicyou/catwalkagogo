/*
	$Id$
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CatWalk{
	public static partial class Seq{
		#region Basic

		public static IEnumerable<T> Make<T>(params T[] values){
			return values;
		}

		public static IEnumerable<T> Repeat<T>(T value){
			while(true){
				yield return value;
			}
		}

		public static IEnumerable<T> Repeat<T>(Func<T> init){
			init.ThrowIfNull("init");
			var v = init();
			while(true){
				yield return v;
			}
		}

		public static IEnumerable<T> Repeat<T>(T value, Func<T, T> func){
			func.ThrowIfNull("func");
			while(true){
				yield return value;
				value = func(value);
			}
		}

		/// <summary>
		/// シーケンスがnullなら空のシーケンスを返す。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source){
			return source ?? new T[0];
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

		#endregion

		#region Pairwise

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

		#endregion

		#region Alternate

		public static IEnumerable<T> Alternate<T>(IEnumerable<T> source1, IEnumerable<T> source2){
			source1.ThrowIfNull("source1");
			source2.ThrowIfNull("source2");
			using(IEnumerator<T> enumerator1 = source1.GetEnumerator(), enumerator2 = source2.GetEnumerator()){
				while(true){
					if(!enumerator1.MoveNext()){
						yield break;
					}
					yield return enumerator1.Current;
					if(!enumerator2.MoveNext()){
						yield break;
					}
					yield return enumerator2.Current;
				}
			}
		}

		#endregion

		#region Unfold

		/// <summary>
		/// 畳み込みの反対
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="initial">初期値</param>
		/// <param name="func">関数</param>
		/// <returns></returns>
		/// <example>
		/// <code>
		/// var raising = 1.Unfold(n => n *= n).Take(10);
		/// </code>
		/// </example>
		public static IEnumerable<T> Unfold<T>(this T initial, Func<T, T> func){
			func.ThrowIfNull("func");
			yield return initial;
			while(true){
				yield return (initial = func(initial));
			}
		}

		#endregion

		#region Let

		/// <summary>
		/// let句の関数版
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="R"></typeparam>
		/// <param name="var">変数</param>
		/// <param name="func">関数</param>
		/// <returns>関数の戻り値</returns>
		/// <example>
		/// <code>
		/// var req = Enumerable.Range(1, 5).Let(e =>{
		/// 	...
		/// });
		/// </code>
		/// </example>
		public static R Let<T, R>(this T var, Func<T, R> func){
			func.ThrowIfNull("func");
			return func(var);
		}
		/*
		/// <summary>
		/// let句の関数版
		/// </summary>
		/// <typeparam name="T">シーケンスの要素の型</typeparam>
		/// <typeparam name="R">返すシーケンスの要素の型</typeparam>
		/// <param name="source">入力シーケンス</param>
		/// <param name="func">入力シーケンスの要素と代入された変数から出力シーケンスへの写像関数</param>
		/// <returns>出力シーケンス</returns>
		public static IEnumerable<R> Let<T, R>(this IEnumerable<T> source, Func<T, R> func){
			source.ThrowIfNull("source");
			func.ThrowIfNull("func");
			foreach(var item in source){
				yield return func(item);
			}
		}
		
		/// <summary>
		/// let句の関数版
		/// </summary>
		/// <typeparam name="T">シーケンスの要素の型</typeparam>
		/// <typeparam name="R1">代入する変数の型</typeparam>
		/// <typeparam name="R2">返すシーケンスの要素の型</typeparam>
		/// <param name="source">入力シーケンス</param>
		/// <param name="selector">入力シーケンスに適用して変数に代入する写像関数</param>
		/// <param name="func">入力シーケンスの要素と代入された変数から出力シーケンスへの写像関数</param>
		/// <returns>出力シーケンス</returns>
		/// <example>
		/// <code>
		/// var req = Enumerable.Range(1, 5).Let(n => (char)n, (n, c) => String.Format("{0}=>{1}", n, c));
		/// </code>
		/// </example>
		public static IEnumerable<R2> Let<T, R1, R2>(this IEnumerable<T> source, Func<T, R1> selector, Func<T, R1, R2> func){
			source.ThrowIfNull("source");
			selector.ThrowIfNull("selector");
			func.ThrowIfNull("func");
			foreach(var item in source){
				yield return func(item, selector(item));
			}
		}
		*/
		#endregion

		#region Cycle

		/// <summary>
		/// 与えられたシーケンスを無限に繰り返す。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static IEnumerable<T> Cycle<T>(this IEnumerable<T> source){
			source.ThrowIfNull("source");
			while(true){
				foreach(var item in source){
					yield return item;
				}
			}
		}

		public static IEnumerable<T> Cycle<T>(T item1, T item2){
			while(true){
				yield return item1;
				yield return item2;
			}
		}
		public static IEnumerable<T> Cycle<T>(T item1, T item2, T item3){
			while(true){
				yield return item1;
				yield return item2;
				yield return item3;
			}
		}
		public static IEnumerable<T> Cycle<T>(T item1, T item2, T item3, T item4){
			while(true){
				yield return item1;
				yield return item2;
				yield return item3;
				yield return item4;
			}
		}

		#endregion

		#region Combination

		/// <summary>
		/// 全ての組み合わせを列挙する
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns>各組み合わせ</returns>
		public static IEnumerable<T[]> Combination<T>(this IEnumerable<T> source){
			source.ThrowIfNull("source");
			var array = source.ToArray();
			var n = array.Length;
			var part = new T[n];
			return CombinationImpl(array, n);
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
			return CombinationImpl(array, n);
		}

		/// <summary>
		/// Combinationの実装
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">ソース</param>
		/// <param name="part">組合せの部分配列</param>
		/// <param name="index">注目位置</param>
		/// <param name="n">選ぶ個数</param>
		/// <param name="count"></param>
		/// <returns>各組合せのシーケンス</returns>
		private static IEnumerable<T[]> CombinationImpl<T>(T[] source, int n) {
			var ret = new T[n];
			var N = source.Length;
			var R = n;
			var current = new int[n];
			int i;
			for(i = 0; i < R; i++) {
				current[i] = i;
				ret[i] = source[i];
			}
			yield return ret;
			while(NextCombination(N, R, current)) {
				for(i = 0; i < R; i++) {
					ret[i] = source[current[i]];
				}
				yield return ret;
			}
		}

		private static bool NextCombination(int N, int R, int[] current) {
			for(var i = R - 1; i >= 0; i--) {
				if(current[i] < N - R + i) {
					current[i]++;
					for(var j = 1; i + j < R; j++) {
						current[i + j] = current[i] + j;
					}
					break;
				}
				if(i == 0) {
					return false;
				}
			}
			return true;
		}


		#endregion

		#region Permutation

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
				foreach(var perm in PermutationImpl(new LinkedList<T>(comb), new T[comb.Length], 0)){
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
			return PermutationImpl(list, new T[list.Count], 0);
		}

		/// <summary>
		/// 順列を列挙する
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">ソース</param>
		/// <param name="part">並び</param>
		/// <param name="index">partへの追加位置</param>
		/// <returns>各順列のシーケンス</returns>
		private static IEnumerable<T[]> PermutationImpl<T>(LinkedList<T> source, T[] part, int index){
			if(source.Count > 0){
				var node = source.First;
				LinkedListNode<T> last = null;
				while(node != null){
					part[index] = node.Value;
					source.Remove(node);

					foreach(var perm in PermutationImpl(source, part, index + 1)){
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

		#endregion

		#region ForEach / TryThese

		/// <summary>
		/// ForEach for IEnumerable
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="action"></param>
		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action){
			source.ThrowIfNull("source");
			action.ThrowIfNull("action");
			foreach(T value in source){
				action(value);
			}
		}

		/// <summary>
		/// ForEach for IEnumerable with index
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="action"></param>
		public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action){
			source.ThrowIfNull("source");
			action.ThrowIfNull("action");
			var index = 0;
			foreach(T value in source){
				action(value, index++);
			}
		}

		#endregion

		#region Flatten

		/// <summary>
		/// ネストされたシーケンスを一段平坦化する。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> source){
			source.ThrowIfNull("source");
			foreach(var sub in source){
				foreach(var item in sub){
					yield return item;
				}
			}
		}

		/// <summary>
		/// ネストされたシーケンスを一段平坦化する。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static IEnumerable<R> Flatten<T, R>(this IEnumerable<IEnumerable<T>> source, Func<T, R> resultSelector){
			source.ThrowIfNull("source");
			resultSelector.ThrowIfNull("resultSelector");
			foreach(var sub in source){
				foreach(var item in sub){
					yield return resultSelector(item);
				}
			}
		}

		/// <summary>
		/// ネストされたシーケンスを全て平坦化する。
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static IEnumerable FlattenAll(this IEnumerable source){
			source.ThrowIfNull("source");
			return FlattenAllImpl(source);
		}

		private static IEnumerable FlattenAllImpl(IEnumerable source){
			foreach(var item in source){
				var sub = source as IEnumerable;
				if(sub != null){
					foreach(var item2 in FlattenAllImpl(sub)){
						yield return item;
					}
				}else{
					yield return item;
				}
			}
		}

		/// <summary>
		/// ネストされたシーケンスを指定された階層分平坦化する。
		/// </summary>
		/// <param name="source"></param>
		/// <param name="depth"></param>
		/// <returns></returns>
		public static IEnumerable FlattenSome(this IEnumerable source, int depth){
			source.ThrowIfNull("source");
			depth.ThrowIfOutOfRange(0, "depth");
			return FlattenSomeImpl(source, depth);
		}

		private static IEnumerable FlattenSomeImpl(IEnumerable source, int depth){
			foreach(var item in source){
				if(depth > 0){
					var sub = item as IEnumerable;
					if(sub != null){
						foreach(var item2 in FlattenSomeImpl(sub, depth - 1)){
							yield return item;
						}
					}else{
						yield return item;
					}
				}else{
					yield return item;
				}
			}
		}

		#endregion

		#region UpTo / DownTo

		public static IEnumerable<int> UpTo(this int from, int to){
			for(; from <= to; from++){
				yield return from;
			}
		}

		public static IEnumerable<long> UpTo(this long from, long to){
			for(; from <= to; from++){
				yield return from;
			}
		}


		public static IEnumerable<decimal> UpTo(this decimal from, decimal to){
			for(; from <= to; from++){
				yield return from;
			}
		}

		public static IEnumerable<int> UpToInfinity(this int from){
			while(true){
				yield return from++;
			}
		}

		public static IEnumerable<long> UpToInfinity(this long from){
			while(true){
				yield return from++;
			}
		}

		public static IEnumerable<decimal> UpToInfinity(this decimal from){
			while(true){
				yield return from++;
			}
		}

		public static IEnumerable<int> DownTo(this int from, int to){
			for(; from >= to; from--){
				yield return from;
			}
		}

		public static IEnumerable<long> DownTo(this long from, long to){
			for(; from >= to; from--){
				yield return from;
			}
		}

		public static IEnumerable<decimal> DownTo(this decimal from, decimal to){
			for(; from >= to; from--){
				yield return from;
			}
		}

		public static IEnumerable<int> UpToNegativeInfinity(this int from){
			while(true){
				yield return from--;
			}
		}

		public static IEnumerable<long> UpToNegativeInfinity(this long from){
			while(true){
				yield return from--;
			}
		}

		public static IEnumerable<decimal> UpToNegativeInfinity(this decimal from){
			while(true){
				yield return from--;
			}
		}

		#endregion

		#region RunLength

		public static IEnumerable<Tuple<T, int>> RunLength<T>(this IEnumerable<T> source){
			return source.RunLength((item, count) => Tuple.Create(item, count));
		}

		public static IEnumerable<TResult> RunLength<T, TResult>(this IEnumerable<T> source, Func<T, int, TResult> resultSelector){
			source.ThrowIfNull("source");
			resultSelector.ThrowIfNull("resultSelector");

			using(var enumerator = source.GetEnumerator()){
				if(!enumerator.MoveNext()){
					yield break;
				}
				var eq = EqualityComparer<T>.Default;
				var count = 1;
				var first = enumerator.Current;
				while(enumerator.MoveNext()){
					var cur = enumerator.Current;
					if(eq.Equals(first, cur)){
						checked{
							++count;
						}
					}else{
						yield return resultSelector(first, count);
						first = cur;
						count = 1;
					}
				}
				yield return resultSelector(first, count);
			}
		}

		public static IEnumerable<Tuple<T, long>> RunLengthLong<T>(this IEnumerable<T> source){
			return source.RunLengthLong((item, count) => Tuple.Create(item, count));
		}

		public static IEnumerable<TResult> RunLengthLong<T, TResult>(this IEnumerable<T> source, Func<T, long, TResult> resultSelector){
			source.ThrowIfNull("source");
			resultSelector.ThrowIfNull("resultSelector");

			using(var enumerator = source.GetEnumerator()){
				if(!enumerator.MoveNext()){
					yield break;
				}
				var eq = EqualityComparer<T>.Default;
				var count = 1;
				var first = enumerator.Current;
				while(enumerator.MoveNext()){
					var cur = enumerator.Current;
					if(eq.Equals(first, cur)){
						checked{
							++count;
						}
					}else{
						yield return resultSelector(first, count);
						first = cur;
						count = 1;
					}
				}
				yield return resultSelector(first, count);
			}
		}

		#endregion

		#region Scan

		public static IEnumerable<R> Scan<T, R>(this IEnumerable<T> source, Func<R, T, R> func){
			return Scan(source, default(R), func);
		}

		public static IEnumerable<R> Scan<T, R>(this IEnumerable<T> source, R initial, Func<R, T, R> func){
			source.ThrowIfNull("source");
			func.ThrowIfNull("func");
			foreach(var item in source){
				initial = func(initial, item);
				yield return initial;
			}
		}

		#endregion

		#region Shuffle

		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source){
			source.ThrowIfNull("source");
			var array = source.ToArray();
			Ext.Shuffle(array);
			return array;
		}

		#endregion

		#region Slice

		public static IEnumerable<T> Slice<T>(this IEnumerable<T> source, int start){
			source.ThrowIfNull("source");
			return source.Where((v, i) => i >= start);
		}

		public static IEnumerable<T> Slice<T>(this IEnumerable<T> source, int start, int count){
			source.ThrowIfNull("source");
			var end = start + count;
			return source.Where((v, i) => i >= start).Where((v, i) => i < end);
		}

		#endregion

		#region Enumerator

		public static IEnumerable<T> ToSequence<T>(this IEnumerator<T> input){
			while(input.MoveNext()){
				yield return input.Current;
			}
		}

		public static IEnumerable ToSequence(this IEnumerator input){
			while(input.MoveNext()){
				yield return input.Current;
			}
		}


		#endregion

		#region WithCancellation

		public static IEnumerable<T> WithCancellation<T>(this IEnumerable<T> input, CancellationToken token) {
			foreach(var item in input) {
				token.ThrowIfCancellationRequested();
				yield return item;
			}
		}

		#endregion
	}
}