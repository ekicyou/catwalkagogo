/*
	$Id: EnvironmentVariableSystemDirectory.cs 217 2011-06-21 14:16:53Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CatWalk.IOSystem.Environment {
	[ChildSystemEntryTypes(typeof(EnvironmentVariableSystemEntry))]
	public class EnvironmentVariableSystemDirectory : SystemDirectory{
		public EnvironmentVariableTarget EnvironmentVariableTarget{get; private set;}

		public EnvironmentVariableSystemDirectory(ISystemDirectory parent, string name, EnvironmentVariableTarget target) : base(parent, name){
			this.EnvironmentVariableTarget = target;
		}

		#region ISystemDirectory Members

		public override IEnumerable<ISystemEntry> Children {
			get {
				return System.Environment.GetEnvironmentVariables(this.EnvironmentVariableTarget)
					.Cast<DictionaryEntry>()
					.Select(v => new EnvironmentVariableSystemEntry(this, (string)v.Key, this.EnvironmentVariableTarget, (string)v.Key));
			}
		}

		public override ISystemDirectory GetChildDirectory(string name) {
			return null;
		}

		#endregion
	}
}
