using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Online;
using System.Threading.Tasks;

namespace OnlineTest5 {
	class OnlineTest5 {
		static void Main(string[] args) {
			Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
			var prm = new Parameter(Int32.Parse(args[0]), 100, 1, Int32.Parse(args[1]));
			/*
			var inputs = Enumerable.Range(0, 100).Select(i => ItemGenerator.RandomItems(prm).ToArray()).ToArray();
			Console.WriteLine("{0}", GetR(prm, inputs, inp => Algorithm.My(prm, inp)));
			*/
			var input = Enumerable.Range(0, prm.Span)
				.Select(i => new Item(1, (int)Math.Round(((double)i / (double)prm.Span) * 100d + 1))).ToArray();
			Console.WriteLine("{0}", GetR(prm, input, inp => Algorithm.My(prm, inp)));
			input = input.OrderByDescending(item => item.Value).ToArray();
			Console.WriteLine("{0}", GetR(prm, input, inp => Algorithm.My(prm, inp)));
		}

		static double GetR(Parameter prm, Item[] input, Func<Item[], IEnumerable<Item>> func){
			return (double)Algorithm.Optimum(prm, input).Sum(item => item.Value) / (double)func(input).Sum(item => item.Value);
		}

		static double GetR(Parameter prm, Item[][] inputs, Func<Item[], IEnumerable<Item>> func){
			var rs = new double[inputs.Length];
			Parallel.For(0, inputs.Length, delegate(int i){
				rs[i] = (double)Algorithm.Optimum(prm, inputs[i]).Sum(item => item.Value) / (double)func(inputs[i]).Sum(item => item.Value);
			});
			return rs.Average();
		}
	}
}
