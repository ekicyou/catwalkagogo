/*
	$Id$
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace CatWalk.Collections{
	public struct ArrayDifference<T>{
		//public T[] Source{get; private set;}
		//public T[] Destination{get; private set;}
		public T[] RemovedItems{get; private set;}
		public T[] AddedItems{get; private set;}
		public T[] RemainItems{get; private set;}
		public IEqualityComparer<T> Comparer{get; private set;}

		public ArrayDifference(IEnumerable<T> source, IEnumerable<T> destination) : this(source, destination, EqualityComparer<T>.Default){}
		public ArrayDifference(IEnumerable<T> source, IEnumerable<T> destination, IEqualityComparer<T> comparer) : this(){
			source.ThrowIfNull("source");
			destination.ThrowIfNull("destination");
			comparer.ThrowIfNull("comparer");
			this.Comparer = comparer;
			
			var dstSet = new HashSet<T>(destination, this.Comparer);
			var removedItems = new List<T>();
			var remainItems = new List<T>();
			foreach(var srcItem in source){
				if(!dstSet.Remove(srcItem)){
					remainItems.Add(srcItem);
				}else{
					removedItems.Add(srcItem);
				}
			}
			this.AddedItems = dstSet.ToArray();
			this.RemovedItems = removedItems.ToArray();
			this.RemainItems = remainItems.ToArray();
		}
	}
}