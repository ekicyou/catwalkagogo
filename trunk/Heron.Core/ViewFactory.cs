using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.Heron {
	public class ViewFactory {
		private IDictionary<Type, Delegate> _Creaters = new Dictionary<Type, Delegate>();

		public void Register<TViewModel>(Delegate d) {
			d.ThrowIfNull("d");
			this._Creaters.Add(typeof(TViewModel), d);
		}

		public object Create(object vm, params object[] args) {
			vm.ThrowIfNull("vm");
			Delegate d;
			if(this._Creaters.TryGetValue(vm.GetType(), out d)) {
				return d.DynamicInvoke(Seq.Make(vm).Concat(args.EmptyIfNull()).ToArray());
			} else {
				return null;
			}
		}
	}
}
