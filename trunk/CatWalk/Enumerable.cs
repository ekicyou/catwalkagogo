/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;

namespace CatWalk{
	public static class Enumerable{
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
		
		public static R Let<T, R>(this T var, Func<T, R> func){
			func.ThrowIfNull("func");
			return func(var);
		}
		
		public static IEnumerable<T> Cycle<T>(params T[] source){
			source.ThrowIfNull("source");
			while(true){
				foreach(var item in source) {
					yield return item;
				}
			}
		}

		public static IEnumerable<T[]> CombinationAll<T>(this IEnumerable<T> source){
			source.ThrowIfNull("source");
			return CombinationAll(source.ToArray(), new T[0], 0);
		}

		private static IEnumerable<T[]> CombinationAll<T>(T[] source, T[] part, int n){
			if(source.Length > n){
				var part2 = new T[part.Length + 1];
				part.CopyTo(part2, 0);
				part2[part.Length] = source[n];
				foreach(var comb in CombinationAll(source, part2, n + 1)){
					yield return comb;
				}
				foreach(var comb in CombinationAll(source, part, n + 1)){
					yield return comb;
				}
			}else if(source.Length == n){
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
	}
}