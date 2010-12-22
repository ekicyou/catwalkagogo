using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Online;

namespace OnlineTest6 {
	class OnlineTest6 {
		static void Main(string[] args) {
			var prm = new Parameter(Int32.Parse(args[1]), 100, 1, Int32.Parse(args[0]));
			var input = ItemGenerator.RandomItems(prm).ToArray();

		}

		static IEnumerable<Item> GetWorstInput(Parameter prm, Item[] input){
			var list = new List<Item>(input);
			var comparer = new ItemComparer();
			list.Sort(comparer);
			var B = prm.BoxSize;
			var k = prm.Span;

			var ct = (1 - B / k) * prm.ValueMax;
			var idx = list.BinarySearch(new Item(1, ct), comparer);
			if(idx < 0){
				idx = ~idx + 1;
			}
		}

		private class ItemComparer : IComparer<Item>{
			public int Compare(Item x, Item y) {
				return x.Value.CompareTo(y.Value);
			}
		}
	}
}
