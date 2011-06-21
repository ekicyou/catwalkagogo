using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CatWalk.IOSystem{
	public class PerformanceSystemCounter : SystemEntry{
		public PerformanceCounter Counter{get; private set;}

		public PerformanceSystemCounter(ISystemDirectory parent, string name, PerformanceCounter counter) : base(parent, name){
			this.Counter = counter;
		}

		public long RawValue{
			get{
				return this.Counter.RawValue;
			}
		}

		public string Description{
			get{
				return this.Counter.CounterHelp;
			}
		}
	}
}
