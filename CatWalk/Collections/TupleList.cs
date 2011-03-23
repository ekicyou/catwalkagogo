/*
	$Id$
	see http://d.hatena.ne.jp/NyaRuRu/20080115/p1
*/
using System;
using System.Linq;
using System.Collections.Generic;

namespace CatWalk.Collections{
	[Serializable]
	public sealed partial class TupleList<T1, T2> : List<Tuple<T1, T2>>{
		public void Add(T1 item1, T2 item2){
			this.Add(Tuple.Create(item1, item2));
		}
	}

	[Serializable]
	public sealed partial class TupleList<T1, T2, T3> : List<Tuple<T1, T2, T3>>{
		public void Add(T1 item1, T2 item2, T3 item3){
			this.Add(Tuple.Create(item1, item2, item3));
		}
	}

	[Serializable]
	public sealed partial class TupleList<T1, T2, T3, T4> : List<Tuple<T1, T2, T3, T4>>{
		public void Add(T1 item1, T2 item2, T3 item3, T4 item4){
			this.Add(Tuple.Create(item1, item2, item3, item4));
		}
	}

	[Serializable]
	public sealed partial class TupleList<T1, T2, T3, T4, T5> : List<Tuple<T1, T2, T3, T4, T5>>{
		public void Add(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5){
			this.Add(Tuple.Create(item1, item2, item3, item4, item5));
		}
	}

	[Serializable]
	public sealed partial class TupleList<T1, T2, T3, T4, T5, T6> : List<Tuple<T1, T2, T3, T4, T5, T6>>{
		public void Add(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6){
			this.Add(Tuple.Create(item1, item2, item3, item4, item5, item6));
		}
	}

	[Serializable]
	public sealed partial class TupleList<T1, T2, T3, T4, T5, T6, T7> : List<Tuple<T1, T2, T3, T4, T5, T6, T7>>{
		public void Add(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7){
			this.Add(Tuple.Create(item1, item2, item3, item4, item5, item6, item7));
		}
	}
}