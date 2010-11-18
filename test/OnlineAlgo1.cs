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
			int CMax = 100;
			int N = Int32.Parse(args[0]);
			Console.WriteLine("     C,     N,     B,    A1,    A2,    O1,    O2,    R1,    R2");
			for(int B = 1; B <= N; B++){
				var prm = new Parameter(B, CMax, 1, N);
				var inputA = Algorithm.GetWorstInputForMy1(prm);
				var inputB = Algorithm.GetWorstInputForMy2(prm);
				double myA = Algorithm.My(prm, inputA).Sum(item => item.Value);
				double myB = Algorithm.My(prm, inputB).Sum(item => item.Value);
				double optA = Algorithm.Optimum(prm, inputA).Sum(item => item.Value);
				double optB = Algorithm.Optimum(prm, inputB).Sum(item => item.Value);
				Console.WriteLine("{0,6},{1,6},{2,6},{3,6},{4,6},{5,6},{6,6},{7:f3},{8:f3}", prm.ValueMax, prm.Span, prm.BoxSize,
					myA, myB, optA, optB, optA / myA, optB / myB);
			}
		}
	}
}
