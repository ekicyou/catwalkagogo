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

	public static class ItemGenerator{
		public static IEnumerable<Item> RandomItems(Parameter prm){
			var rnd = new Random();
			return Enumerable.Range(0, prm.Span).Select(n => new Item(1, (int)rnd.Next(0, (int)(prm.ValueMax * 100)) / 100));
		}

		public static IEnumerable<Item> GaussItems(Parameter prm, double mean, double standardDeviation){
			return Algorithm.GaussRandom(mean, standardDeviation)
				.Select(n => Math.Min(prm.ValueMax, Math.Max(0, n)))
				.Select(v => new Item(1, (int)v))
				.Take(prm.Span);
		}
	}
}