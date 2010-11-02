// todo “ü—Í•ª•z
//      Åˆ«‚Ìê‡
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Online;

namespace OnlineTest{
	class Program{
		static void Main(String[] args){
			if(args.Length < 2){
				Console.WriteLine("usage: .exe [span] [boxsize]");
			}else{
				Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
				var prm = new Parameter(Int32.Parse(args[1]), 100, 1, Int32.Parse(args[0]));
				//var input = RandomItems(prm).Take(prm.Span).ToArray();
				var input = ItemGenerator.GaussItems(prm, 50, 10).Take(prm.Span).ToArray();

				var myItems = Algorithm.My(prm, input).ToArray();
				Debug.Assert(myItems.Length == (int)prm.BoxSize);

				var optItems = Algorithm.Optimum(prm, input).ToArray();
				Debug.Assert(optItems.Length == (int)prm.BoxSize);

				var rndItems = Algorithm.Random(prm, input).ToArray();
				Debug.Assert(rndItems.Length == (int)prm.BoxSize);

				var halfItems = Algorithm.Half(prm, input).ToArray();
				Debug.Assert(halfItems.Length == (int)prm.BoxSize);

				var my = myItems.Sum(item => item.Value);
				var opt = optItems.Sum(item => item.Value);
				var rnd = rndItems.Sum(item => item.Value);
				var half = halfItems.Sum(item => item.Value);
				var r = opt / my;
				var r2 = opt / rnd;
				var r3 = opt / half;
				Console.WriteLine("MY: {0} OPT: {1} R: {2} RND: {3} R2: {4} HALF: {5} R3: {6}", my, opt, r, rnd, r2, half, r3);
			}
		}
	}
}