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
			const int tryN = 10000;
			var n = Int32.Parse(args[0]);
			var prm1 = new Parameter(0, 100, 1, n);
			
			//var input = ItemGenerator.RandomItems(prm1).OrderBy(item => item.Value).ToArray();
			var inputs = Enumerable.Range(1, tryN).Select(j => ItemGenerator.RandomItems(prm1));
			var inputs2 = Enumerable.Range(1, tryN).Select(j => ItemGenerator.RandomItems(prm1).OrderBy(item => item.Value));
			var inputs3 = Enumerable.Range(1, tryN).Select(j => ItemGenerator.RandomItems(prm1).OrderByDescending(item => item.Value));
			for(var B = 1; B <= (n / 2); B += 10 - B % 10){
				var prm = new Parameter(B, 100, 1, n);
				Console.WriteLine("{0},{1},{2},{3}", B,
					Test.GetR(prm, inputs, input => Algorithm.CTA(prm, input, (int)Math.Floor(prm.ValueMax * (1d - (double)B / (double)n)))),
					Test.GetR(prm, inputs2, input => Algorithm.CTA(prm, input, (int)Math.Floor(prm.ValueMax * (1d - (double)B / (double)n)))),
					Test.GetR(prm, inputs3, input => Algorithm.CTA(prm, input, (int)Math.Floor(prm.ValueMax * (1d - (double)B / (double)n)))));
			}
		}

		public static double GetInverseCumulativeTruncatedNormalDistribution(double p, Normal normal, double left, double right){
			var clft = normal.CumulativeDistribution(left);
			var crgt = normal.CumulativeDistribution(right);
			return normal.InverseCumulativeDistribution(p * (crgt - clft) + clft);
		}
	}
}
