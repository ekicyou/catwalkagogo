using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Online;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;

namespace OnlineTest5 {
	// ソートされたデータ入力時の競合比
	class OnlineTest5 {
		static void Main(string[] args) {
			const int tryN = 10;
			var n = Int32.Parse(args[0]);
			var cts = new double[3][];
			var rs = new double[3];
			var prm1 = new Parameter(0, 100, 1, n);
			/*
			var input = ItemGenerator.RandomItems(prm1).OrderBy(item => item.Value).ToArray();
			var inputs = Enumerable.Range(1, tryN).Select(j => ItemGenerator.RandomItems(prm1).OrderBy(item => item.Value));
			for(var i = 1; i < 4; i++){
				var B = n * i / 4;
				var prm = new Parameter(B, 100, 1, n);
				Algorithm.My(prm, input, out cts[i - 1]);
				rs[i - 1] = Test.GetR(prm, inputs, inp => Algorithm.My(prm, inp));
			}

			Console.WriteLine("i,ci,ct1/4,ct2/4,ct3/4");
			for(var i = 0; i < n; i++){
				Console.WriteLine("{0},{1},{2},{3},{4}", i, input[i].Value, cts[0][i], cts[1][i], cts[2][i]);
			}
			Console.WriteLine(",r=,{0},{1},{2}", rs[0], rs[1], rs[2]);

			input = ItemGenerator.RandomItems(prm1).OrderByDescending(item => item.Value).ToArray();
			inputs = Enumerable.Range(1, tryN).Select(j => ItemGenerator.RandomItems(prm1).OrderByDescending(item => item.Value));
			for(var i = 1; i < 4; i++){
				var B = n * i / 4;
				var prm = new Parameter(B, 100, 1, n);
				Algorithm.My(prm, input, out cts[i - 1]);
				rs[i - 1] = Test.GetR(prm, inputs, inp => Algorithm.My(prm, inp));
			}
			Console.WriteLine("i,ci,ct1/4,ct2/4,ct3/4");
			for(var i = 0; i < n; i++){
				Console.WriteLine("{0},{1},{2},{3},{4}", i, input[i].Value, cts[0][i], cts[1][i], cts[2][i]);
			}
			Console.WriteLine(",r=,{0},{1},{2}", rs[0], rs[1], rs[2]);
			*/
			var normal = Normal.WithMeanStdDev(100 / 2, Double.Parse(args[1]));
			var input = ItemGenerator.GaussItems(prm1, normal.Mean, normal.StdDev).OrderBy(item => item.Value).ToArray();
			var inputs = Enumerable.Range(1, tryN).Select(j => ItemGenerator.GaussItems(prm1, normal.Mean, normal.StdDev).OrderBy(item => item.Value));
			for(var i = 1; i < 4; i++){
				var B = n * i / 4;
				var prm = new Parameter(B, 100, 1, n);
				Algorithm.GaussMy(prm, input, normal.Mean, normal.StdDev, out cts[i - 1]);
				rs[i - 1] = Test.GetR(prm, inputs, inp => Algorithm.GaussMy(prm, inp, normal.Mean, normal.StdDev));
			}
			Console.WriteLine("i,ci,ct1/4,ct2/4,ct3/4");
			for(var i = 0; i < n; i++){
				Console.WriteLine("{0},{1},{2},{3},{4}", i, input[i].Value, cts[0][i], cts[1][i], cts[2][i]);
			}
			Console.WriteLine(",r=,{0},{1},{2}", rs[0], rs[1], rs[2]);
			
			input = ItemGenerator.GaussItems(prm1, normal.Mean, normal.StdDev).OrderByDescending(item => item.Value).ToArray();
			inputs = Enumerable.Range(1, tryN).Select(j => ItemGenerator.GaussItems(prm1, normal.Mean, normal.StdDev).OrderByDescending(item => item.Value));
			for(var i = 1; i < 4; i++){
				var B = n * i / 4;
				var prm = new Parameter(B, 100, 1, n);
				Algorithm.GaussMy(prm, input, normal.Mean, normal.StdDev, out cts[i - 1]);
				rs[i - 1] = Test.GetR(prm, inputs, inp => Algorithm.GaussMy(prm, inp, normal.Mean, normal.StdDev));
			}
			Console.WriteLine("i,ci,ct1/4,ct2/4,ct3/4");
			for(var i = 0; i < n; i++){
				Console.WriteLine("{0},{1},{2},{3},{4}", i, input[i].Value, cts[0][i], cts[1][i], cts[2][i]);
			}
			Console.WriteLine(",r=,{0},{1},{2}", rs[0], rs[1], rs[2]);
			/*
			var normal = Normal.WithMeanStdDev(100 / 2, Double.Parse(args[1]));
			var inputs2 = Enumerable.Range(1, tryN).Select(j => ItemGenerator.RandomItems(prm1));
			for(var i = 1; i < 4; i++){
				var B = n * i / 4;
				var prm = new Parameter(B, 100, 1, n);
				rs[i - 1] = Test.GetR(prm, inputs2, inp => Algorithm.My(prm, inp));
			}
			Console.WriteLine(",r=,{0},{1},{2}", rs[0], rs[1], rs[2]);

			inputs2 = Enumerable.Range(1, tryN).Select(j => ItemGenerator.GaussItems(prm1, normal.Mean, normal.StdDev));
			for(var i = 1; i < 4; i++){
				var B = n * i / 4;
				var prm = new Parameter(B, 100, 1, n);
				rs[i - 1] = Test.GetR(prm, inputs2, inp => Algorithm.GaussMy(prm, inp, normal.Mean, normal.StdDev));
			}
			Console.WriteLine(",r=,{0},{1},{2}", rs[0], rs[1], rs[2]);
			/*
			
			input = ItemGenerator.GaussItems(prm1, normal.Mean, normal.StdDev).ToArray();
			for(var i = 1; i < 4; i++){
				var B = n * i / 4;
				var prm = new Parameter(B, 100, 1, n);
				Algorithm.GaussMy(prm, input, normal.Mean, normal.StdDev, out cts[i - 1]);
			}
			//Console.WriteLine("i,ci,ct1/4,ct2/4,ct3/4");
			//for(var i = 0; i < n; i++){
			//	Console.WriteLine("{0},{1},{2},{3},{4}", i, input[i].Value, cts[0][i], cts[1][i], cts[2][i]);
			//}
			Console.WriteLine(",r=,{0},{1},{2}", rs[0], rs[1], rs[2]);

			input = ItemGenerator.RandomItems(prm1).ToArray();
			for(var i = 1; i < 4; i++){
				var B = n * i / 4;
				var prm = new Parameter(B, 100, 1, n);
				Algorithm.My(prm, input, out cts[i - 1]);
			}
			//Console.WriteLine("i,ci,ct1/4,ct2/4,ct3/4");
			//for(var i = 0; i < n; i++){
			//	Console.WriteLine("{0},{1},{2},{3},{4}", i, input[i].Value, cts[0][i], cts[1][i], cts[2][i]);
			//}
			Console.WriteLine(",r=,{0},{1},{2}", rs[0], rs[1], rs[2]);
			/*
			Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
			var prm = new Parameter(Int32.Parse(args[0]), 100, 1, Int32.Parse(args[1]));
			
			var inputs = Enumerable.Range(0, 100).Select(i => ItemGenerator.RandomItems(prm).ToArray()).ToArray();
			Console.WriteLine("{0}", GetR(prm, inputs, inp => Algorithm.My(prm, inp)));
			
			var input = Enumerable.Range(0, prm.Span)
				.Select(i => new Item(1, (int)Math.Round(((double)i / (double)prm.Span) * 100d + 1))).ToArray();
			Console.WriteLine("{0}", GetR(prm, input, inp => Algorithm.My(prm, inp)));
			input = input.OrderByDescending(item => item.Value).ToArray();
			Console.WriteLine("{0}", GetR(prm, input, inp => Algorithm.My(prm, inp)));
			var normal = Normal.WithMeanStdDev(prm.ValueMax / 2, Double.Parse(args[2]));
			input = Enumerable.Range(0, prm.Span)
				.Select(i => new Item(1, (int)Math.Round(GetInverseCumulativeTruncatedNormalDistribution((double)i / prm.Span, normal, 0, prm.ValueMax) + 1))).ToArray();
			Console.WriteLine("{0}", GetR(prm, input, inp => Algorithm.My(prm, inp)));
			input = input.OrderByDescending(item => item.Value).ToArray();
			Console.WriteLine("{0}", GetR(prm, input, inp => Algorithm.My(prm, inp)));
			*/
		}

		public static double GetInverseCumulativeTruncatedNormalDistribution(double p, Normal normal, double left, double right){
			var clft = normal.CumulativeDistribution(left);
			var crgt = normal.CumulativeDistribution(right);
			return normal.InverseCumulativeDistribution(p * (crgt - clft) + clft);
		}
	}
}
