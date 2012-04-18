/*
	$Id: PerformanceSystemCategoryDirectory.cs 217 2011-06-21 14:16:53Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CatWalk.IOSystem.Environment{
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
