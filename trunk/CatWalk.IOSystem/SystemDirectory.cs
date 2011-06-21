/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CatWalk.IOSystem {
	[ChildSystemEntryTypes(typeof(ISystemEntry))]
	public abstract class SystemDirectory : SystemEntry, ISystemDirectory{
		public const char DirectorySeperatorChar = '\\';

		public SystemDirectory(ISystemDirectory parent, string name) : base(parent, name){}

		#region ISystemDirectory Members

		public abstract IEnumerable<ISystemEntry> Children {get;}
		public virtual IEnumerable<ISystemEntry> GetChildren(CancellationToken token){
			return this.Children.TakeWhile(item => !token.IsCancellationRequested);
		}

		public virtual ISystemDirectory GetChildDirectory(string name){
			return this.Children.OfType<ISystemDirectory>().FirstOrDefault(entry => entry.Name.Equals(name));
		}
		public virtual ISystemDirectory GetChildDirectory(string name, CancellationToken token){
			return this.GetChildren(token).OfType<ISystemDirectory>().FirstOrDefault(entry => entry.Name.Equals(name));
		}

		public virtual bool Contains(string name){
			return this.Children.Any(entry => entry.Name.Equals(name));
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
