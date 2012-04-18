/*
	$Id: PerformanceSystemCategory.cs 217 2011-06-21 14:16:53Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CatWalk.IOSystem.Environment{
	public class PerformanceSystemCategory : SystemDirectory{
		public PerformanceCounterCategory CounterCategory{get; private set;}

		public PerformanceSystemCategory(ISystemDirectory parent, string name, PerformanceCounterCategory category) : base(parent, name){
			if(category == null){
				throw new ArgumentNullException("category");
			}
			this.CounterCategory = category;
		}

		public override IEnumerable<ISystemEntry> Children {
			get {
				var instanceNames = this.CounterCategory.GetInstanceNames();
				return ((instanceNames.Length > 0) ? this.CounterCategory.GetCounters(instanceNames[0]) : this.CounterCategory.GetCounters())
					.Select(counter => new PerformanceSystemCounter(this, counter.CounterName, counter));
			}
		}
	}
}
