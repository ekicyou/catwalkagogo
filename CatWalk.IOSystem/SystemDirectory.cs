/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
/*
namespace CatWalk.IOSystem {
	[ChildSystemEntryTypes(typeof(ISystemEntry))]
	public abstract class SystemDirectory : SystemEntry{
		public const char DirectorySeperatorChar = '/';

		public SystemDirectory(ISystemDirectory parent, string name) : base(parent, name){}

		#region ISystemDirectory Members

		public virtual IEnumerable<ISystemEntry> GetChildren() {
			return this.GetChildren(CancellationToken.None);
		}
		public abstract IEnumerable<ISystemEntry> GetChildren(CancellationToken token);

		public virtual ISystemDirectory GetChildDirectory(string name){
			return this.GetChildren().OfType<ISystemDirectory>().FirstOrDefault(entry => entry.Name.Equals(name));
		}
		public virtual ISystemDirectory GetChildDirectory(string name, CancellationToken token){
			return this.GetChildren(token).OfType<ISystemDirectory>().FirstOrDefault(entry => entry.Name.Equals(name));
		}

		public virtual bool Contains(string name){
			return this.GetChildren().Any(entry => entry.Name.Equals(name));
		}
		public virtual bool Contains(string name, CancellationToken token){
			return this.GetChildren(token).Any(entry => entry.Name.Equals(name));
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
*/