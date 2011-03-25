/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CatWalk.Collections {
	[Serializable]
	internal class SimpleMonitor : IDisposable{
		private int busyCount = 0;

		public bool IsBusy{
			get{
				return this.busyCount > 0;
			}
		}
		
		public void Enter(){
			Interlocked.Increment(ref this.busyCount);
		}
		
		public void Dispose(){
			Interlocked.Decrement(ref this.busyCount);
		}
	}
}
