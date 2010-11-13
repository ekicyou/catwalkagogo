using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Online;

namespace OnlineAlgo1 {
	class OnlineAlgo1 {
		static void Main(string[] args) {
			//Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
			int CMax = Int32.Parse(args[0]);
			int N = 128;
			int B = 32;
			Console.WriteLine("     C,     N,     B,     d,    A1,    A2,    O1,    O2,    R1,    R2");
			for(double d = 1; d <= CMax; d++){
				var prm = new Parameter(B, CMax, 1, N);
				var inputA = CreateWorstInputA(prm, d);
				var inputB = CreateWorstInputB(prm, d);
				double myA = Algorithm.Div(prm, inputA, d).Sum(item => item.Value);
				double myB = Algorithm.Div(prm, inputB, d).Sum(item => item.Value);
				double optA = Algorithm.Optimum(prm, inputA).Sum(item => item.Value);
				double optB = Algorithm.Optimum(prm, inputB).Sum(item => item.Value);
				Console.WriteLine("{0,6},{1,6},{2,6},{3,6},{4,6},{5,6},{6,6},{7,6},{8:f3},{9:f3}", prm.ValueMax, prm.Span, prm.BoxSize, d,
					myA, myB, optA, optB, optA / myA, optB / myB);
			}
		}

		static Item[] CreateWorstInputA(Parameter prm, double d){
			var input = new int[prm.Span];
			for(int i = 0; i < (prm.Span - prm.BoxSize); i++){
				input[i] = Math.Min((int)(prm.ValueMax / d), prm.ValueMax);
			}
			for(int i = (prm.Span - prm.BoxSize); i < prm.Span; i++){
				input[i] = 1;
			}
			return input.Select(c => new Item(1, c)).ToArray();
		}

		static Item[] CreateWorstInputB(Parameter prm, double d){
			var input = new int[prm.Span];
			for(int i = 0; i < prm.BoxSize; i++){
				input[i] = Math.Min((int)(prm.ValueMax / d + 1), prm.ValueMax);
			}
			for(int i = prm.BoxSize; i < prm.Span; i++){
				input[i] = prm.ValueMax;
			}
			return input.Select(c => new Item(1, c)).ToArray();
		}
	}
}
