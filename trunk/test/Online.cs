// todo “ü—Í•ª•z
//      Åˆ«‚Ìê‡
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Online{
	public class Item : IComparable<Item>{
		public double Size{get; private set;}
		public double Value{get; private set;}

		public Item(double size, double value){
			this.Size = size;
			this.Value = value;
		}

		public int CompareTo(Item item){
			return this.Value.CompareTo(item.Value);
		}
	}

	public struct Parameter{
		public double BoxSize{private set; get;}
		public double ValueMax{private set; get;}
		public double SizeMax{private set; get;}
		public int Span{private set; get;}

		public Parameter(int boxSize, double valueMax, double sizeMax, int span) : this(){
			this.BoxSize = boxSize;
			this.ValueMax = valueMax;
			this.SizeMax = sizeMax;
			this.Span = span;
		}
	}

	public static class Algorithm{
		public static IEnumerable<Item> My(Parameter prm, IEnumerable<Item> input){
			int rspan = prm.Span;
			var space = prm.BoxSize;
			return input
				.Take(prm.Span)
				.Select(item => {
					if(rspan < space){
						Debug.WriteLine("Take: {0,6} space: {1,4} rspan: {2,4} vt: 0", item.Value, space, rspan);
						return item;
					}else if(space > 0){
						if(((1 - space / (double)rspan) * prm.ValueMax) < item.Value){
							Debug.WriteLine("Take: {0,6} space: {1,4} rspan: {2,4} vt: {3}",
								item.Value,
								space,
								rspan,
								(1 - space / (double)rspan) * prm.ValueMax);
							return item;
						}else{
							Debug.WriteLine("Thru: {0,6} space: {1,4} rspan: {2,4} vt: {3}",
								item.Value,
								space,
								rspan,
								(1 - space / (double)rspan) * prm.ValueMax);
							return null;
						}
					}else{
						Debug.WriteLine("Thru: {0,6} space: {1,4} rspan: {2,4} vt: 100", item.Value, space, rspan);
						return null;
					}
				})
				.Select(item => {rspan--; return item;})
				.Where(item => item != null)
				.Select(item => {space -= item.Size; return item;});
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
			while(true){
				yield return new Item(1, (double)rnd.Next(0, (int)(prm.ValueMax * 100)) / 100);
			}
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
				.Select(v => new Item(1, v));
		}
	}
}