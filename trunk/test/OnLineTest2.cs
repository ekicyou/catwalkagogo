// todo “ü—Í•ª•z
//      Åˆ«‚Ìê‡
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Online;

namespace OnlineTest2{
	class Program{
		static void Main(String[] args){
			if(args.Length < 2){
				Console.WriteLine("usage: .exe [span] [boxsize]");
			}else{
				Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
				var prm = new Parameter(Int32.Parse(args[1]), 100, 1, Int32.Parse(args[0]));
				if(prm.BoxSize > prm.Span){
					Console.WriteLine("Box size is too big.");
					return;
				}
				//var input = RandomItems(prm).Take(prm.Span).ToArray();
				var inputC = new int[prm.Span];
				for(int i = 0; i < prm.BoxSize; i++){
					inputC[i] = (int)((1 - ((double)(prm.BoxSize - i) / (double)(prm.Span - i))) * prm.ValueMax + 1);
				}
				for(int i = prm.BoxSize; i < prm.Span; i++){
					inputC[i] = prm.ValueMax;
				}
				var input = inputC.Select(c => new Item(1, c)).ToArray();
				Console.WriteLine("Est R:{0}", (double)(prm.BoxSize * prm.ValueMax) / (double)inputC.Take(prm.BoxSize).Sum());

				var myItems = Algorithm.My(prm, input).ToArray();
				Debug.Assert(myItems.Length == (int)prm.BoxSize);

				var optItems = Algorithm.Optimum(prm, input).ToArray();
				Debug.Assert(optItems.Length == (int)prm.BoxSize);
				/*
				var rndItems = Algorithm.Random(prm, input).ToArray();
				Debug.Assert(rndItems.Length == (int)prm.BoxSize);

				var halfItems = Algorithm.Half(prm, input).ToArray();
				Debug.Assert(halfItems.Length == (int)prm.BoxSize);
				*/
				var my = myItems.Sum(item => item.Value);
				var opt = optItems.Sum(item => item.Value);
				//var rnd = rndItems.Sum(item => item.Value);
				//var half = halfItems.Sum(item => item.Value);
				var r = (double)opt / (double)my;
				//var r2 = opt / rnd;
				//var r3 = opt / half;
				//Console.WriteLine("MY: {0} OPT: {1} R: {2} RND: {3} R2: {4} HALF: {5} R3: {6}", my, opt, r, rnd, r2, half, r3);
				Console.WriteLine("MY: {0} OPT: {1} R: {2}", my, opt, r);
			}
		}
	}
}