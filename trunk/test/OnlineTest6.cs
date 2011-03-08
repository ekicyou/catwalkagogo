using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Online;

namespace OnlineTest6 {
	class OnlineTest6 {
		static void Main(string[] args){
			var prm = new Parameter(Int32.Parse(args[1]), 100, 1, Int32.Parse(args[0]));
			var ctLog = new double[prm.Span];
			/*
			double[] maxCtLog = null;
			Item[] maxInput = null;
			double maxR = 0;
			for(var i = 0; i < 100000; i++){
				var input = ItemGenerator.RandomItems(prm).ToArray();
				var r = Algorithm.GetR(Algorithm.Optimum(prm, input).Sum(item => item.Value), Algorithm.My(prm, input, out ctLog).Sum());
				if(maxR < r){
					maxR = r;
					maxCtLog = ctLog;
					maxInput = input;
				}
			}
			Console.WriteLine("i,ci,ct");
			for(var i = 0; i < prm.Span; i++){
				Console.WriteLine("{0}, {1}, {2}", i, maxInput[i].Value, maxCtLog[i]);
			}
			Console.WriteLine(maxR.ToString());
			*/
			var worstInput = GetWorstInput(prm, ItemGenerator.RandomItems(prm).ToArray()).ToArray();
			var my = Algorithm.My(prm, worstInput, out ctLog).ToArray().Sum();
			var opt = Algorithm.Optimum(prm, worstInput).Sum(item => item.Value);
			Console.WriteLine("i,ci,ct");
			for(var i = 0; i < prm.Span; i++){
				Console.WriteLine("{0}, {1}, {2}", i, worstInput[i].Value, ctLog[i]);
			}
			Console.WriteLine("opt={0}, my={1}, r={2}", opt, my, (double)opt / (double)my);
			Console.ReadLine();
		}

		static IEnumerable<Item> GetWorstInput(Parameter prm, Item[] input){
			var comparer = new ItemComparer();
			Array.Sort(input, comparer);
			var list = new List<Item>(input);
			var B = prm.BoxSize;
			var k = prm.Span;

			while(list.Count > 0){
				var ct = (int)((1 - (double)B / (double)k) * (double)prm.ValueMax);
				var idx = list.BinarySearch(new Item(1, ct), comparer);
				if(idx < 0){
					idx = ~idx;
				}
				idx--;
				if(idx < 0){
					idx++;
				}
				var item = list[idx];
				if(item.Value > ct){
					B--;
				}
				list.RemoveAt(idx);
				k--;
				yield return item;
			}
		}

		static IEnumerable<Item> GetWorstInput2(Parameter prm, Item[] input){
			var comparer = new ItemComparer();
			Array.Sort(input, comparer);
			var listTake = new List<Item>(input.Take(prm.BoxSize));
			var listThru = new List<Item>(input.Skip(prm.BoxSize).Reverse());
			var B = prm.BoxSize;
			var k = prm.Span;

			while(true){
				var ct = (int)((1 - (double)B / (double)k) * (double)prm.ValueMax);
				var take = listTake.Where(item => item.Value > ct).FirstOrDefault();
				if(take != null){
					B--;
					yield return take;
					listTake.Remove(take);
				}else{
					var thru = listThru.Where(item => item.Value <= ct).FirstOrDefault();
					if(thru != null){
						yield return thru;
						listThru.Remove(thru);
					}else{
						take = listThru.Where(item => item.Value > ct).OrderBy(item => item.Value).FirstOrDefault();
						if(take != null){
							B--;
							yield return take;
							listThru.Remove(take);
						}else{
							break;
						}
					}
				}
				k--;
			}
			foreach(var item in listTake){
				yield return item;
			}
			foreach(var item in listThru){
				yield return item;
			}
		}

		private class ItemComparer : IComparer<Item>{
			public int Compare(Item x, Item y) {
				return x.Value.CompareTo(y.Value);
			}
		}
	}
}
