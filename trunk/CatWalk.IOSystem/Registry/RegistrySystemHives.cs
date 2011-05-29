using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace CatWalk.IOSystem{
	[ChildSystemEntryTypes(typeof(RegistrySystemHive))]
	public class RegistrySystemHives : SystemDirectory{
		public RegistrySystemHives(ISystemDirectory parent) : this(parent, "Registry"){
		}

		public RegistrySystemHives(ISystemDirectory parent, object id) : base(parent, id){
		}

		public override IEnumerable<ISystemEntry> Children {
			get {
				return new ISystemEntry[]{
					new RegistrySystemHive(this, RegistryHive.ClassesRoot),
					new RegistrySystemHive(this, RegistryHive.CurrentConfig),
					new RegistrySystemHive(this, RegistryHive.CurrentUser),
					new RegistrySystemHive(this, RegistryHive.LocalMachine),
					new RegistrySystemHive(this, RegistryHive.PerformanceData),
					new RegistrySystemHive(this, RegistryHive.Users),
				};
			}
		}

		public override ISystemDirectory GetChildDirectory(object id) {
			return this.Children.OfType<RegistrySystemHive>().FirstOrDefault(key => key.Name.Equals(id.ToString(), StringComparison.OrdinalIgnoreCase));
		}

		public override bool Contains(object id) {
			throw new NotImplementedException();
		}
	}
}
