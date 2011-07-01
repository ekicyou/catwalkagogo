using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatWalk.IOSystem;

namespace Twitman.IOSystem {
	public class HomeSystemDirectory : SystemDirectory{
		public override IEnumerable<ISystemEntry> Children {
			get {
				throw new NotImplementedException();
			}
		}
	}
}
