/*
 $Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk{
	[Serializable]
	public struct Range<T> : IEquatable<Range<T>> where T : IComparable<T>{
		private T min;
		private T max;
		private bool isExcludingLowerBound;
		private bool isExcludingUpperBound;

		public Range(T min, T max){
			this.min = min;
			this.max = max;
			this.isExcludingLowerBound = false;
			this.isExcludingUpperBound = false;
		}

		public Range(T min, T max, bool excludeLower, bool excludeUpper){
			this.min = min;
			this.max = max;
			this.isExcludingLowerBound = excludeLower;
			this.isExcludingUpperBound = excludeUpper;
		}

		public bool Contains(T value){
			bool lower = 
				(this.min == null) ? true :
				(this.isExcludingLowerBound) ? this.min.CompareTo(value) < 0 : this.min.CompareTo(value) <= 0;
			bool upper =
				(this.max == null) ? true :
				(this.isExcludingUpperBound) ? this.max.CompareTo(value) > 0 : this.max.CompareTo(value) >= 0;
			return lower && upper;
		}

		public T Min{
			get{
				return this.min;
			}
			set{
				this.min = value;
			}
		}

		public T Max{
			get{
				return this.max;
			}
			set{
				this.max = value;
			}
		}

		public bool IsExcludingLowerBound{
			get{
				return this.isExcludingLowerBound;
			}
			set{
				this.isExcludingLowerBound = value;
			}
		}

		public bool IsExcludingUpperBound{
			get{
				return this.isExcludingUpperBound;
			}
			set{
				this.isExcludingUpperBound = value;
			}
		}

		#region IEquatable

		public bool Equals(Range<T> other){
			return this.max.Equals(other.max) && this.min.Equals(other.min);
		}

		public override bool Equals(object obj){
			if(!(obj is Range<T>)) {
				return false;
			}
			return this.Equals((Range<T>)obj);
		}

		public override int GetHashCode(){
			return this.max.GetHashCode() ^ this.min.GetHashCode();
		}

		public static bool operator ==(Range<T> a, Range<T> b){
			return a.Equals(b);
		}

		public static bool operator !=(Range<T> a, Range<T> b){
			return !a.Equals(b);
		}

		#endregion
	}
}
