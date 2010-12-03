/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk{
	[Serializable]
	public class Range<T> where T : IComparable{
		public T Min{get; set;}
		public T Max{get; set;}
		public bool IsIncludingLowerBound{get; set;}
		public bool IsIncludingUpperBound{get; set;}

		public Range(){
			this.IsIncludingLowerBound = true;
			this.IsIncludingUpperBound = true;
		}
		
		public Range(T min, T max) : this(min, max, true, true){}

		public Range(T min, T max, bool includeLower, bool includeUpper){
			this.Min = min;
			this.Max = max;
			this.IsIncludingLowerBound = includeLower;
			this.IsIncludingUpperBound = includeUpper;
		}
		
		public bool Contains(T value){
			bool lower = (this.IsIncludingLowerBound) ? this.Min.CompareTo(value) <= 0 : this.Min.CompareTo(value) < 0;
			bool upper = (this.IsIncludingUpperBound) ? this.Max.CompareTo(value) >= 0 : this.Min.CompareTo(value) > 0;
			return lower && upper;
		}
	}
}