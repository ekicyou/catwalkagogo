using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using MathNet.Numerics.Distributions;

namespace Online {
	public static class Algorithm{
		public static double GetR(double opt, double my){
			return opt / my;
		}

		public static IEnumerable<Item> GaussMy(Parameter prm, IEnumerable<Item> input, double mean, double sd){
			var normal = Normal.WithMeanStdDev(mean, sd);
			var rspan = prm.Span;
			var space = prm.BoxSize;
			foreach(var item in input){
				var p = 1 - (double)space / (double)rspan;
				var ct = GetInverseCumulativeTruncatedNormalDistribution(p, normal, 0, prm.ValueMax);
				if(space > 0 && ct < item.Value){
					Debug.WriteLine("Take: {0,6} space: {1,4} rspan: {2,4} vt: {3}, p:{4,4}",
						item.Value,
						space,
						rspan,
						ct,
						p);
					space--;
					yield return item;
				}else{
					Debug.WriteLine("Thru: {0,6} space: {1,4} rspan: {2,4} vt: {3}, p:{4,4}",
						item.Value,
						space,
						rspan,
						ct,
						p);
				}
				rspan--;
			}
		}

		public static double GetInverseCumulativeTruncatedNormalDistribution(double p, Normal normal, double left, double right){
			var clft = normal.CumulativeDistribution(left);
			var crgt = normal.CumulativeDistribution(right);
			return normal.InverseCumulativeDistribution(p * (crgt - clft) + clft);
		}

		public static IEnumerable<Item> My(Parameter prm, IEnumerable<Item> input){
			int rspan = prm.Span;
			var space = prm.BoxSize;
			return input
				.Take(prm.Span)
				.Select(item => {
					var ct = (int)((1 - (double)space / (double)rspan) * prm.ValueMax);
					if(space > 0 && ct < item.Value){
						Debug.WriteLine("Take: {0,6} space: {1,4} rspan: {2,4} vt: {3}",
							item.Value,
							space,
							rspan,
							ct);
						return item;
					}else{
						Debug.WriteLine("Thru: {0,6} space: {1,4} rspan: {2,4} vt: {3}",
							item.Value,
							space,
							rspan,
							ct);
						return null;
					}
				})
				.Select(item => {rspan--; return item;})
				.Where(item => item != null)
				.Select(item => {space -= item.Size; return item;});
		}

		public static Item[] GetWorstInputForMy1(Parameter prm){
			var input = new int[prm.Span];
			for(int i = 0; i < prm.BoxSize; i++){
				input[i] = Math.Max(1, (int)((1 - ((double)prm.BoxSize / (double)(prm.Span - i))) * prm.ValueMax));
			}
			for(int i = prm.BoxSize; i < prm.Span; i++){
				input[i] = 1;
			}
			return input.Select(c => new Item(1, c)).ToArray();
		}

		public static Item[] GetWorstInputForMy2(Parameter prm){
			var input = new int[prm.Span];
			for(int i = 0; i < prm.BoxSize; i++){
				input[i] = (int)((1 - ((double)(prm.BoxSize - i) / (double)(prm.Span - i))) * prm.ValueMax + 1);
			}
			for(int i = prm.BoxSize; i < prm.Span; i++){
				input[i] = prm.ValueMax;
			}
			return input.Select(c => new Item(1, c)).ToArray();
		}

		public static IEnumerable<Item> Div(Parameter prm, IEnumerable<Item> input, double d){
			var space = prm.BoxSize;
			var rspan = prm.Span;
			var ct = prm.ValueMax / d;
			return input
				.Take(prm.Span)
				.Where(item => {
					if(rspan <= space || (ct < item.Value && space > 0)){
						Debug.WriteLine("DivTake: {0,6} space: {1,4} vt: {2}",
							item.Value,
							space,
							ct);
						rspan--;
						space--;
						return true;
					}else{
						Debug.WriteLine("DivThru: {0,6} space: {1,4} vt: {2}",
							item.Value,
							space,
							ct);
						rspan--;
						return false;
					}
				});
		}

		public static double GetOptimumD(Parameter prm){
			return 0.5d + Math.Sqrt(1 + 4 * prm.ValueMax) / 2.0d; 
		}

		public static Item[] GetWorstInputForDiv1(Parameter prm, double d){
			var input = new int[prm.Span];
			for(int i = 0; i < (prm.Span - prm.BoxSize); i++){
				input[i] = Math.Min((int)(prm.ValueMax / d), prm.ValueMax);
			}
			for(int i = (prm.Span - prm.BoxSize); i < prm.Span; i++){
				input[i] = 1;
			}
			return input.Select(c => new Item(1, c)).ToArray();
		}

		public static Item[] GetWorstInputForDiv2(Parameter prm, double d){
			var input = new int[prm.Span];
			for(int i = 0; i < prm.BoxSize; i++){
				input[i] = Math.Min((int)(prm.ValueMax / d + 1), prm.ValueMax);
			}
			for(int i = prm.BoxSize; i < prm.Span; i++){
				input[i] = prm.ValueMax;
			}
			return input.Select(c => new Item(1, c)).ToArray();
		}

		public static IEnumerable<Item> Optimum(Parameter prm, IEnumerable<Item> input){
			return input.Take(prm.Span).OrderByDescending(item => item.Value).Take((int)prm.BoxSize);
		}

		public static IEnumerable<Item> Random(Parameter prm, IEnumerable<Item> input){
			var rnd = new Random((int)(DateTime.Now.Ticks % input.GetHashCode()));
			var rspan = prm.Span;
			var space = prm.BoxSize;
			foreach(var item in input.Take(prm.Span)){
				if(space >= rspan || rnd.Next(rspan) < space){
					space--;
					yield return item;
				}
				rspan--;
			}
		}

		public static IEnumerable<Item> Head(Parameter prm, IEnumerable<Item> input){
			var rspan = prm.Span;
			var space = prm.BoxSize;
			foreach(var item in input.Take(prm.Span)){
				if(space >= rspan){
					space--;
					yield return item;
				}
				rspan--;
			}
		}


		public static IEnumerable<Item> Half(Parameter prm, IEnumerable<Item> input){
			var rspan = prm.Span;
			var space = prm.BoxSize;
			return input.Take(prm.Span)
				.Select(item => {
					if((space > 0) && ((rspan <= space) || (item.Value > (prm.ValueMax / 2d)))){
						Debug.WriteLine("HalfTake: {0,6} space: {1,4} rspan: {2,4}", item.Value, space, rspan);
						return item;
					}else{
						Debug.WriteLine("HalfThru: {0,6} space: {1,4} rspan: {2,4}", item.Value, space, rspan);
						return null;
					}
				})
				.Select(item => {rspan--; return item;})
				.Where(item => item != null)
				.Select(item => {space -= item.Size; return item;});
		}

		/// <summary>
		///  Box-Mullar法による正規分布に従う乱数を生成。
		/// </summary>
		/// <param name="mean">平均</param>
		/// <param name="standardDeviation">分散</param>
		/// <returns></returns>
		public static IEnumerable<double> GaussRandom(double mean, double standardDeviation){
			var obj = new object();
			var rnd = new Random((int)(DateTime.Now.Ticks % obj.GetHashCode()));
			double a, b;
			while(true){
				a = Math.Sqrt(-2d * Math.Log(rnd.NextDouble()));
				b = 2d * Math.PI * rnd.NextDouble();
				yield return mean + standardDeviation * a * Math.Sin(b);
				yield return mean + standardDeviation * a * Math.Cos(b);
			}
		}
	}
}
