using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.Heron {
	public class Factory<TKey, TValue> {
		private IDictionary<TKey, Delegate> _Creaters = new Dictionary<TKey, Delegate>();

		public void Register(TKey key, Delegate d) {
			key.ThrowIfNull("key");
			d.ThrowIfNull("d");
			this._Creaters.Add(key, d);
		}

		public TValue Create(TKey key, params object[] args) {
			key.ThrowIfNull("key");
			Delegate d;
			if(this._Creaters.TryGetValue(key, out d)) {
				return (TValue)d.DynamicInvoke(Seq.Make((object)key).Concat(args.EmptyIfNull()).ToArray());
			} else {
				return default(TValue);
			}
		}
	}
}
