/*
	$Id: EnvironmentVariableTargetsDirectory.cs 217 2011-06-21 14:16:53Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk.IOSystem.Environment{
	[ChildSystemEntryTypes(typeof(EnvironmentVariableSystemDirectory))]
	public class EnvironmentVariableTargetDirectory : SystemDirectory{
		public EnvironmentVariableTargetDirectory(ISystemDirectory parent, string name) : base(parent, name){
			this._Children = new Lazy<ISystemEntry[]>(() => Enum.GetValues(typeof(EnvironmentVariableTarget))
				.Cast<EnvironmentVariableTarget>()
				.Select(target => new EnvironmentVariableSystemDirectory(this, target.ToString(), target))
				.ToArray());
		}

		#region ISystemDirectory Members

		private Lazy<ISystemEntry[]> _Children;
		public override IEnumerable<ISystemEntry> Children {
			get {
				return this._Children.Value;
			}
		}

		public override ISystemDirectory GetChildDirectory(string name) {
			return this.Children.Cast<EnvironmentVariableSystemDirectory>().FirstOrDefault(entry => entry.DisplayName.Equals(name, StringComparison.OrdinalIgnoreCase));
		}

		public override bool Contains(string name) {
			EnvironmentVariableTarget target;
			if(Enum.TryParse<EnvironmentVariableTarget>(name, out target)){
				return true;
			}else{
				return false;
			}
		}

		#endregion
	}
}
