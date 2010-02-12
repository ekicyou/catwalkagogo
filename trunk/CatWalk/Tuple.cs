/*
	$Id$
*/
using System;
using System.Collections.Generic;

namespace CatWalk{
	[Serializable]
	public struct Tuple<T1, T2> : IEquatable<Tuple<T1, T2>>{
		public T1 Item1{get; private set;}
		public T2 Item2{get; private set;}

		public Tuple(T1 item1, T2 item2) : this(){
			this.Item1 = item1;
			this.Item2 = item2;
		}

		#region 演算子オーバーロード
		public static bool operator !=(Tuple<T1, T2> tuple1, Tuple<T1, T2> tuple2){
			return !tuple1.Equals(tuple2);
		}

		public static bool operator ==(Tuple<T1, T2> tuple1, Tuple<T1, T2> tuple2){
			return tuple1.Equals(tuple2);
		}
		#endregion

		public override bool Equals(object obj){
			if (!(obj is Tuple<T1, T2>)) return false;
			return Equals((Tuple<T1, T2>)obj);
		}

		public override int GetHashCode(){
			int hash = (Item1 == null) ? 0 : Item1.GetHashCode();
			hash ^= ((Item2 == null) ? 0 : Item2.GetHashCode());
			return hash;
		}

		public static Tuple<T1, T2> Default{
			get{
				return new Tuple<T1, T2>(default(T1), default(T2));
			}
		}

		#region IEquatable<Tuple<T1,T2>> メンバ

		public bool Equals(Tuple<T1, T2> tuple){
			return Equals(Item1, tuple.Item1) && Equals(Item2, tuple.Item2);
		}

		#endregion
	}
}