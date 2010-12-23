using System;
using System.IO;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using CatWalk;

namespace Nekome.Search{
	static class Program{
		static void Main(){
			var worker = new FileListWorker(@"V:\text\CSharp", "*.*", SearchOption.AllDirectories, false);
			worker.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e){
				var files = e.UserState as string[];
				if(files != null){
					foreach(var file in files){
						try{
							foreach(var match in Grep.Match(new Regex("using"), file)){
								/*
								var block = match.Map.Pairwise()
								                     .Concat(new[]{
								                     	new Tuple<IndexLinePair, IndexLinePair>(
								                     		match.Map[match.Map.Count - 1],
								                     		new IndexLinePair(Int64.MaxValue, Int64.MaxValue))})
								                     .Where(tuple => (tuple.Item1.Line <= match.Line) && (match.Line < tuple.Item2.Line))
								                     .Select(tuple => tuple.Item1)
								                     .First();
								*/
								int idx = ArrayList.Adapter((IList)match.Map).BinarySearch(new IndexLinePair(0, match.Line), new CustomComparer<IndexLinePair>(
									delegate(IndexLinePair x, IndexLinePair y){
										return x.Line.CompareTo(y.Line);
									}
								));
								if(idx < 0){
									idx = (~idx) - 1;
								}
								var block = match.Map[idx];
								
								Console.WriteLine("{0}:{1}:{2}:{3}:{4}:{5}", match.Path, block.Index, block.Line, match.Line, match.Map.Count, match.LineText);
							}
						}catch(IOException ex){
							Console.WriteLine(ex.ToString());
						}catch(UnauthorizedAccessException ex){
							Console.WriteLine(ex.ToString());
						}
					}
				}else{
					Console.WriteLine(e.UserState.ToString());
				}
			};
			worker.RunWorkerCompleted += delegate{
				Console.WriteLine("CMPL");
			};
			worker.Start();
			Console.ReadLine();
		}
	}
	
	public static class IListExtension{
		public static int BinarySearch<T>(this IList<T> list, int index, int count, T item, IComparer<T> comparer){
			return list.BinarySearchImpl(index, index + count - 1, item, comparer);
		}
		
		public static int BinarySearchImpl<T>(this IList<T> list, int min, int max, T item, IComparer<T> comparer){
			while(min <= max){
				int center = (min + max) / 2;
				//Console.WriteLine("Min: {0}, Center: {1}, Max: {2}", min, center, max);
				int d = comparer.Compare(item, list[center]);
				if(d < 0){
					max = center - 1;
				}else if(d > 0){
					min = center + 1;
				}else{
					//Console.WriteLine("Match: {0}", center);
					return center;
				}
			}
			max++;
			//Console.WriteLine("Not match: {0} {1}", max, ~max);
			return ~max;
		}
	}
}