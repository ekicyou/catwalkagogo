using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Online;
using System.Threading.Tasks;

namespace GaussTest {
	class Program {
		static void Main(string[] args) {
			var CMax = Int32.Parse(args[0]);
			var n = Int32.Parse(args[1]);
			var mean = CMax / 2;
			var sd = Double.Parse(args[2]);
			var values = new int[CMax + 1];
			var samples = Algorithm.GaussRandom(mean, sd).Take(n).Select(v => {
				var d = (int)Math.Max(Math.Min(Math.Round(v), CMax), 0);
				values[d]++;
				return d;
			}).ToArray();
			values.Select((v, i) => {
				Console.WriteLine("{0}, {1}", i, v);
				return v;
			}).ToArray();
		}
	}
}
