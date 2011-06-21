/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk.IOSystem {
	public class RegistrySystemEntry : SystemEntry{
		public string EntryName{get; private set;}

		public RegistrySystemEntry(ISystemDirectory parent, string name, string entryName) : base(parent, name){
			this.EntryName = entryName;
		}
	}
}
