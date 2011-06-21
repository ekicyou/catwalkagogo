using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CatWalk.IOSystem{
	public class PerformanceSystemCategoryDirectory : SystemDirectory{
		public string MachineName{get; private set;}

		public PerformanceSystemCategoryDirectory(ISystemDirectory parent, string name) : this(parent, name, null){
		}

		public PerformanceSystemCategoryDirectory(ISystemDirectory parent, string name, string machineName) : base(parent, name){
			this.MachineName = machineName;
		}

		public override IEnumerable<ISystemEntry> Children {
			get {
				return ((String.IsNullOrEmpty(this.MachineName)) ? PerformanceCounterCategory.GetCategories() :
					PerformanceCounterCategory.GetCategories(this.MachineName))
						.Select(cat => new PerformanceSystemCategory(this, cat.CategoryName, cat));
			}
		}
	}
}
