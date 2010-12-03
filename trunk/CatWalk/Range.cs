/*
 $Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk{
	[Serializable]
	public class Range<T> where T : IComparable<T>{
		private T min;
		private T max;
		public bool IsIncludingLowerBound{get; set;}
		public bool IsIncludingUpperBound{get; set;}

		public Range(T min, T max) : this(min, max, true, true){
		}

		public Range(T min, T max, bool includeLower, bool includeUpper){
			this.min = min;
			this.max = max;
			this.IsIncludingLowerBound = includeLower;
			this.IsIncludingUpperBound = includeUpper;
		}

		public bool IsInRange(T value){
			bool lower = (this.IsIncludingLowerBound) ? this.min.CompareTo(value) <= 0 : this.min.CompareTo(value) < 0;
			bool upper = (this.IsIncludingUpperBound) ? this.max.CompareTo(value) >= 0 : this.max.CompareTo(value) > 0;
			return lower && upper;
		}
	}
}
