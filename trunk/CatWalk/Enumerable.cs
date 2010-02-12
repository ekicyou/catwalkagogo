/*
	$Id$
*/
using System;
using System.Collections.Generic;

namespace CatWalk{
	public static class Enumerable{
		public static IEnumerable<Tuple<T, T>> Pairwise<T>(this IEnumerable<T> source){
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
	}
}