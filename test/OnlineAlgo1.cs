using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Online;

namespace OnlineAlgo1 {
	class Program {
		static void Main(string[] args) {
			//Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
			int CMax = 20;
			int N = Int32.Parse(args[0]);
			var prm = new Parameter(Int32.Parse(args[1]), CMax, 1, N);
			var inputA = Algorithm.GetWorstInputForMy1(prm);
			var inputB = Algorithm.GetWorstInputForMy2(prm);
			var ct1 = new double[N];
			var ct2 = new double[N];
			Algorithm.My(prm, inputA, out ct1);
			Algorithm.My(prm, inputB, out ct2);
			Console.WriteLine("i,ct,ci");
			for(var i = 0; i < N; i++){
				Console.WriteLine("{0},{1},{2}", i, (int)Math.Floor(ct1[i]), inputA[i].Value);
			}
			Console.WriteLine("i,ct,ci");
			for(var i = 0; i < N; i++){
				Console.WriteLine("{0},{1},{2}", i, (int)Math.Floor(ct2[i]), inputB[i].Value);
			}
		}
	}
}
