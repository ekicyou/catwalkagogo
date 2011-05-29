using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk.IOSystem {
	public class RegistrySystemEntry : SystemEntry{
		public RegistrySystemEntry(ISystemDirectory parent, string name) : base(parent, name){
			if(name == null){
				throw new ArgumentNullException("name");
			}
		}

		public string Name{
			get{
				return (string)this.Id;
			}
		}
	}
}
