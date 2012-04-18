/*
	$Id: RegistrySystemEntry.cs 217 2011-06-21 14:16:53Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk.IOSystem.Win32 {
	public class RegistrySystemEntry : SystemEntry{
		public string EntryName{get; private set;}

		public RegistrySystemEntry(ISystemDirectory parent, string name, string entryName) : base(parent, name){
			this.EntryName = entryName;
		}
	}
}
