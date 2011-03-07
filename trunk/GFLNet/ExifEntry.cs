/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GflNet {
	public struct ExifEntry{
		public ExifEntryTypes Types{get; private set;}
		public int Tag{get; private set;}
		public string Name{get; private set;}
		public string Value{get; private set;}
		
		internal ExifEntry(Gfl.ExifEntry entry) : this(){
			this.Types = entry.Types;
			this.Tag = (int)entry.Tag;
			this.Name = entry.Name;
			this.Value = entry.Value;
		}
	}
}
