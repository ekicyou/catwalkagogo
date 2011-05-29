using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk.IOSystem{
	[ChildSystemEntryTypes(typeof(EnvironmentVariableSystemDirectory))]
	public class EnvironmentVariableTargetsDirectory : SystemDirectory{

		public EnvironmentVariableTargetsDirectory(ISystemDirectory parent) : this(parent, "Env"){}

		public EnvironmentVariableTargetsDirectory(ISystemDirectory parent, object id) : base(parent, id){
			this._Children = new Lazy<ISystemEntry[]>(() => Enum.GetValues(typeof(EnvironmentVariableTarget))
				.Cast<EnvironmentVariableTarget>()
				.Select(target => new EnvironmentVariableSystemDirectory(this, target))
				.ToArray());
		}

		#region ISystemDirectory Members

		private Lazy<ISystemEntry[]> _Children;
		public override IEnumerable<ISystemEntry> Children {
			get {
				return this._Children.Value;
			}
		}

		public override ISystemDirectory GetChildDirectory(object id) {
			return this.Children.Cast<EnvironmentVariableSystemDirectory>().FirstOrDefault(entry => entry.DisplayName.Equals(id.ToString(), StringComparison.OrdinalIgnoreCase));
		}

		public override bool Contains(object id) {
			EnvironmentVariableTarget target;
			if(Enum.TryParse<EnvironmentVariableTarget>(id as string, out target)){
				return true;
			}else{
				return false;
			}
		}

		#endregion
	}
}
