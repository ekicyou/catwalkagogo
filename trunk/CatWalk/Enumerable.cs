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
		
		public static R Let<T, R>(this T t, Func<T, R> f){
			f.ThrowIfNull("f");
			return f(t);
		}
		
		public static IEnumerable<T> Cycle<T>(params T[] source){
			source.ThrowIfNull("source");
			while(true){
				foreach(var item in source) {
					yield return item;
				}
			}
		}
	}
}