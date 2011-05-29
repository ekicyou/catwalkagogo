using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk.IOSystem {
	public abstract class SystemDirectory : SystemEntry, ISystemDirectory{
		public const char DirectorySeperatorChar = '\\';

		public SystemDirectory(ISystemDirectory parent, object id) : base(parent, id){}

		#region ISystemDirectory Members

		public abstract IEnumerable<ISystemEntry> Children {get;}

		public virtual ISystemDirectory GetChildDirectory(object id){
			return this.Children.Cast<ISystemDirectory>().FirstOrDefault(entry => entry.Id == id);
		}

		public abstract bool Contains(object id);

		public virtual string ConcatDisplayPath(string name) {
			return this.DisplayPath + DirectorySeperatorChar + name;
		}

		#endregion
	}
}
