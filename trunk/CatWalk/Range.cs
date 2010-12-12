/*
 $Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk{
	[Serializable]
	public struct Range<T> where T : IComparable<T>{
		private T min;
		private T max;
		private bool isIncludingLowerBound;
		private bool isIncludingUpperBound;

		public Range(T min, T max) : this(min, max, true, true){
		}

		public Range(T min, T max, bool includeLower, bool includeUpper){
			this.min = min;
			this.max = max;
			this.isIncludingLowerBound = includeLower;
			this.isIncludingUpperBound = includeUpper;
		}

		public bool Contains(T value){
			bool lower = (this.isIncludingLowerBound) ? this.min.CompareTo(value) <= 0 : this.min.CompareTo(value) < 0;
			bool upper = (this.isIncludingUpperBound) ? this.max.CompareTo(value) >= 0 : this.max.CompareTo(value) > 0;
			return lower && upper;
		}

		public T Min{
			get{
				return this.min;
			}
		}

		public T Max{
			get{
				return this.max;
			}
		}

		public bool IsIncludingLowerBound{
			get{
				return this.isIncludingLowerBound;
			}
		}

		public bool IsIncludingUpperBound{
			get{
				return this.isIncludingUpperBound;
			}
		}

	}
}
