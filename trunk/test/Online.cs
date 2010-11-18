// todo “ü—Í•ª•z
//      Åˆ«‚Ìê‡
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Online{
	public class Item : IComparable<Item>{
		public int Size{get; private set;}
		public int Value{get; private set;}

		public Item(int size, int value){
			this.Size = size;
			this.Value = value;
		}

		public int CompareTo(Item item){
			return this.Value.CompareTo(item.Value);
		}
	}

	public struct Parameter{
		public int BoxSize{private set; get;}
		public int ValueMax{private set; get;}
		public int SizeMax{private set; get;}
		public int Span{private set; get;}

		public Parameter(int boxSize, int valueMax, int sizeMax, int span) : this(){
			this.BoxSize = boxSize;
			this.ValueMax = valueMax;
			this.SizeMax = sizeMax;
			this.Span = span;
		}
	}

	public static class Algorithm{
		public static double GetR(double opt, double my){
			return opt / my;
		}

		public static IEnumerable<Item> My(Parameter prm, IEnumerable<Item> input){
			int rspan = prm.Span;
			var space = prm.BoxSize;
			return input
				.Take(prm.Span)
				.Select(item => {
					var ct = (int)((1 - (double)space / (double)rspan) * prm.ValueMax);
					if(ct < item.Value){
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
			var rnd = new Random();
			var rspan = prm.Span;
			var space = prm.BoxSize;
			return input.Take(prm.Span)
				.Where(item => rnd.NextDouble() <= space / (rspan--))
				.Select(item => {space -= item.Size; return item;});
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

	}

	public static class ItemGenerator{
		public static IEnumerable<Item> RandomItems(Parameter prm){
			var rnd = new Random();
			return Enumerable.Range(0, prm.Span).Select(n => new Item(1, (int)rnd.Next(0, (int)(prm.ValueMax * 100)) / 100));
		}

		/// <summary>
		///  Box-Mullar–@‚É‚æ‚é³‹K•ª•z‚É]‚¤—”‚ğ¶¬B
		/// </summary>
		/// <param name="mean">•½‹Ï</param>
		/// <param name="standardDeviation">•ªU</param>
		/// <returns></returns>
		private static IEnumerable<double> GaussRandom(double mean, double standardDeviation){
			var rnd = new Random();
			double a, b;
			while(true){
				a = Math.Sqrt(-2d * Math.Log(rnd.NextDouble()));
				b = 2d * Math.PI * rnd.NextDouble();
				yield return mean + standardDeviation * a * Math.Sin(b);
				yield return mean + standardDeviation * a * Math.Cos(b);
			}
		}

		public static IEnumerable<Item> GaussItems(Parameter prm, double mean, double standardDeviation){
			return GaussRandom(mean, standardDeviation)
				.Select(n => Math.Min(prm.ValueMax, Math.Max(0, n)))
				.Select(v => new Item(1, (int)v))
				.Take(prm.Span);
		}
	}
}