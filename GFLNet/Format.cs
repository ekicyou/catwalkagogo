/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GflNet {
	public struct Format{
		public string Name{get; private set;}
		public string DefaultSuffix{get; private set;}
		public bool Readable{get; private set;}
		public bool Writable{get; private set;}
		public string Description{get; private set;}
		
		internal Format(string name, string defaultSuffix, bool readable, bool writable, string description) : this(){
			this.Name = name;
			this.DefaultSuffix = defaultSuffix;
			this.Readable = readable;
			this.Writable = writable;
			this.Description = description;
		}
	}
}
