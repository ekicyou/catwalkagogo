/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk.IOSystem {
	[AttributeUsage(AttributeTargets.Class, Inherited=true, AllowMultiple=false)]
	public class ChildSystemEntryTypesAttribute : Attribute{
		public IEnumerable<Type> ChildSystemEntryTypes{get; set;}

		public ChildSystemEntryTypesAttribute(params Type[] types){
			this.ChildSystemEntryTypes = types;
		}
	}
}
