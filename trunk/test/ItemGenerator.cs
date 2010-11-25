using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Online {
	public static class ItemGenerator{
		public static IEnumerable<Item> RandomItems(Parameter prm){
			var rnd = new Random();
			return Enumerable.Range(0, prm.Span).Select(n => new Item(1, (int)rnd.Next(prm.ValueMax) + 1));
		}

		public static IEnumerable<Item> GaussItems(Parameter prm, double mean, double standardDeviation){
			return Algorithm.GaussRandom(mean, standardDeviation)
				.Where(n => (0 < n) && (n < prm.ValueMax))
				.Select(v => new Item(1, (int)Math.Ceiling(v)))
				.Take(prm.Span);
		}
	}
}
