/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk {
	public class WeakMulticastDelegate{
		LinkedList<WeakDelegate> _Handlers;
		private LinkedList<WeakDelegate> Handlers{
			get{
				return this._Handlers ?? (this._Handlers = new LinkedList<WeakDelegate>());
			}
		}

		public void Add(Delegate handler){
			this.Handlers.AddLast(new WeakDelegate(handler));
		}

		public void Remove(Delegate handler){
			this.RemoveHandler(wd => !wd.IsAlive || wd.Equals(handler));
		}
		public void Invoke(){this.Invoke(null);}
		public void Invoke(params object[] args){
			for(var node = this.Handlers.First; node != null;){
				var next = node.Next;
				var wd = node.Value;
				if(wd.IsAlive){
					wd.Delegate.DynamicInvoke(args);
				}else{
					this.Handlers.Remove(node);
				}
				node = next;
			}
		}

		private void RemoveHandler(Predicate<WeakDelegate> pred){
#if DEBUG
			if(pred == null){
				throw new ArgumentNullException("pred");
			}
#endif
			for(var node = this.Handlers.First; node != null;){
				var next = node.Next;
				if(pred(node.Value)){
					this.Handlers.Remove(node);
				}
				node = next;
			}
		}
	}
}
