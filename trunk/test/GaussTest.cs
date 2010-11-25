using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Online;
using System.Threading.Tasks;

namespace GaussTest {
	class Program {
		static void Main(string[] args) {
			/*Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
			var CMax = 100;
			var n = 100;
			var B = 10;
			var mean = CMax / 2;
			var sd = Int32.Parse(args[0]);

			var prm = new Parameter(B, CMax, 1, n);
			var input = ItemGenerator.GaussItems(prm, mean, sd).ToArray();
			var result1 = Algorithm.GaussMy(prm, input, mean, sd);
			var result2 = Algorithm.My(prm, input);
			Console.WriteLine("GaussMy: {0}, My: {1}", result1.Sum(item => item.Value), result2.Sum(item => item.Value));
			*/
			if(args.Length < 4){
				Console.WriteLine("usage: CMax n B n2");
				return;
			}
			var CMax = Int32.Parse(args[0]);
			var n = Int32.Parse(args[1]);
			var B = Int32.Parse(args[2]);
			var n2 = Int32.Parse(args[3]);
			var mean = CMax / 2;
			
			if(args.Length > 4){
				var sd2 = Double.Parse(args[4]);
				var values = new int[CMax + 1];
				var samples = Algorithm.GaussRandom(mean, sd2)
					.Where(v => (0 <= v) && (v <= CMax))
					.Take(n2).Select(v => {
					var d = (int)Math.Ceiling(v);
					values[d]++;
					return d;
				}).ToArray();

				var values2 = new int[CMax + 1];
				var rnd = new Random();
				var samples2 = Enumerable.Range(0, n2)
					.Select(v => rnd.Next(CMax) + 1)
					.Where(v => (0 <= v) && (v <= CMax))
					.Select(v => {
					values2[v]++;
					return v;
				}).ToArray();
				for(int i = 0; i < values.Length; i++){
					Console.WriteLine("{0}, {1}, {2}", i, values[i], values2[i]);
				}
				return;
			}

			var prm = new Parameter(B, CMax, 1, n);
			var rs2 = new double[n2];
			Parallel.For(0, n2, delegate(int i){
				var input2 = ItemGenerator.RandomItems(prm).ToArray();
				rs2[i] = (double)Algorithm.Optimum(prm, input2).Sum(item => item.Value)
					/ (double)Algorithm.Random(prm, input2).Sum(item => item.Value);
			});
			var rs4 = new double[n2];
			Parallel.For(0, n2, delegate(int i){
				var input2 = ItemGenerator.RandomItems(prm).ToArray();
				rs4[i] = (double)Algorithm.Optimum(prm, input2).Sum(item => item.Value)
					/ (double)Algorithm.My(prm, input2).Sum(item => item.Value);
			});
			var rs6 = new double[n2];
			Parallel.For(0, n2, delegate(int i){
				var input2 = ItemGenerator.RandomItems(prm).ToArray();
				rs6[i] = (double)Algorithm.Optimum(prm, input2).Sum(item => item.Value)
					/ (double)Algorithm.GaussMy(prm, input2, CMax / 2, Int32.MaxValue).Sum(item => item.Value);
			});
			Console.WriteLine("{0}, {1}, {2}, {3}", rs2.Average(), rs4.Average(), rs6.Average(), n2);

			Console.WriteLine("CMax, n, B, mean, sd, R1, R2");
			for(var sdp = 1; sdp <= 100; sdp++){
				var sd = CMax * (double)sdp / 100d;
				var rs = new double[n2];
				var inputs = GetItems(prm, n2, mean, sd);
				var opts = new double[n2];
				Parallel.For(0, n2, delegate(int i){
					opts[i] = Algorithm.Optimum(prm, inputs[i]).Sum(item => item.Value);
				});
				Parallel.For(0, n2, delegate(int i){
					rs[i] = opts[i] / (double)Algorithm.Random(prm, inputs[i]).Sum(item => item.Value);
				});
				var rs3 = new double[n2];
				Parallel.For(0, n2, delegate(int i){
					rs3[i] = opts[i] / (double)Algorithm.My(prm, inputs[i]).Sum(item => item.Value);
				});
				var rs5 = new double[n2];
				Parallel.For(0, n2, delegate(int i){
					rs5[i] = opts[i] / (double)Algorithm.GaussMy(prm, inputs[i], mean, sd).Sum(item => item.Value);
				});
				Console.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", prm.ValueMax, prm.Span, prm.BoxSize, mean, sd, rs.Average(), rs3.Average(), rs5.Average());
			}
		}

		static Item[][] GetItems(Parameter prm, int n2, double mean, double sd){
			var items = new Item[n2][];
			Parallel.For(0, n2, delegate(int i){
				items[i] = ItemGenerator.GaussItems(prm, mean, sd).ToArray();
			});
			return items;
		}
	}
}
