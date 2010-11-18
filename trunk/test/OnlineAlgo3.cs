using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Online;

namespace AlgoTest3 {
	class OnlineAlgo3 {
		static void Main(string[] args) {
			int CMax = Int32.Parse(args[0]);
			int n = Int32.Parse(args[1]);

			const int n2 = 100;
			var prm = new Parameter(1, CMax, 1, n);

			var inputs1 = new Item[n2][];
			Parallel.For(0, inputs1.GetLength(0), delegate(int i){
				inputs1[i] = ItemGenerator.RandomItems(prm).ToArray();
			});
			var inputs2 = new Item[n2][];
			var mean = CMax / 2;
			var sd = Math.Sqrt(CMax / 4);
			Parallel.For(0, inputs2.GetLength(0), delegate(int i){
				inputs2[i] = ItemGenerator.GaussItems(prm, mean, sd).ToArray();
			});

			Console.WriteLine("Mean, {0}, SD, {1}", mean, sd);

			Console.WriteLine("C, n, B, R1, R2, d, AR1, AR2, GR1, GR2");
			for(int B = 1; B <= n; B++){
				prm = new Parameter(B, CMax, 1, n);

				var optRs1 = new double[n2];
				Parallel.For(0, n2, delegate(int i){
					optRs1[i] = Algorithm.Optimum(prm, inputs1[i]).Sum(item => item.Value);
				});
				var optRs2 = new double[n2];
				Parallel.For(0, n2, delegate(int i){
					optRs2[i] = Algorithm.Optimum(prm, inputs2[i]).Sum(item => item.Value);
				});

				var myInput1 = Algorithm.GetWorstInputForMy1(prm);
				var myInput2 = Algorithm.GetWorstInputForMy2(prm);
				var my1 = Algorithm.My(prm, myInput1).Sum(item => item.Value);
				var my2 = Algorithm.My(prm, myInput2).Sum(item => item.Value);
				var myOpt1 = Algorithm.Optimum(prm, myInput1).Sum(item => item.Value);
				var myOpt2 = Algorithm.Optimum(prm, myInput2).Sum(item => item.Value);
				var myR1 = Algorithm.GetR(myOpt1, my1);
				var myR2 = Algorithm.GetR(myOpt2, my2);
				var myR = Math.Max(myR1, myR2);
				var myRs1 = new double[n2];
				Parallel.For(0, n2, delegate(int i){
					myRs1[i] = optRs1[i] / Algorithm.My(prm, inputs1[i]).Sum(item => item.Value);
				});
				var myRs2 = new double[n2];
				Parallel.For(0, n2, delegate(int i){
					myRs2[i] = optRs2[i] / Algorithm.My(prm, inputs2[i]).Sum(item => item.Value);
				});

				var d = Algorithm.GetOptimumD(prm);
				var divInput1 = Algorithm.GetWorstInputForDiv1(prm, d);
				var divInput2 = Algorithm.GetWorstInputForDiv2(prm, d);
				var div1 = Algorithm.Div(prm, divInput1, d).Sum(item => item.Value);
				var div2 = Algorithm.Div(prm, divInput2, d).Sum(item => item.Value);
				var divOpt1 = Algorithm.Optimum(prm, divInput1).Sum(item => item.Value);
				var divOpt2 = Algorithm.Optimum(prm, divInput2).Sum(item => item.Value);
				var divR1 = Algorithm.GetR(divOpt1, div1);
				var divR2 = Algorithm.GetR(divOpt2, div2);
				var divR = Math.Max(divR1, divR2);
				var divRs1 = new double[n2];
				Parallel.For(0, n2, delegate(int i){
					divRs1[i] = optRs1[i] / Algorithm.Div(prm, inputs1[i], d).Sum(item => item.Value);
				});
				var divRs2 = new double[n2];
				Parallel.For(0, n2, delegate(int i){
					divRs2[i] = optRs2[i] / Algorithm.Div(prm, inputs2[i], d).Sum(item => item.Value);
				});

				Console.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}",
					CMax, n, B, myR, d, divR, myRs1.Average(), divRs1.Average(), myRs2.Average(), divRs2.Average());
			}
		}
	}
}
