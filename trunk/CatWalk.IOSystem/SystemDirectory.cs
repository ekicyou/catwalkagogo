using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk.IOSystem {
	[ChildSystemEntryTypes(typeof(ISystemEntry))]
	public abstract class SystemDirectory : SystemEntry, ISystemDirectory{
		public const char DirectorySeperatorChar = '\\';

		public SystemDirectory(ISystemDirectory parent, string name) : base(parent, name){}

		#region ISystemDirectory Members

		public abstract IEnumerable<ISystemEntry> Children {get;}

		public virtual ISystemDirectory GetChildDirectory(string name){
			return this.Children.Cast<ISystemDirectory>().FirstOrDefault(entry => entry.Name.Equals(name));
		}

		public virtual bool Contains(string name){
			return this.Children.Any(entry => entry.Name.Equals(name));
		}

		public string ConcatPath(string name){
			return this.Path + DirectorySeperatorChar + name;
		}

		public string ConcatDisplayPath(string name) {
			return this.DisplayPath + DirectorySeperatorChar + name;
		}

		#endregion
	}
}
