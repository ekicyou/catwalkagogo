using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatWalk;
using CatWalk.IOSystem;
using CatWalk.Net.Twitter;

namespace Twitman.IOSystem {
	public class HomeSystemDirectory : SystemDirectory{
		public HomeSystemDirectory(ISystemDirectory parent, string name) : base(parent, name){}

		public override IEnumerable<ISystemEntry> Children {
			get {
				return Program.Settings.Accounts.EmptyIfNull().Select(account => new AccountSystemDirectory(this, account));
			}
		}
	}
}
