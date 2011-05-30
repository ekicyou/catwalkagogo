using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace CatWalk.IOSystem{
	[ChildSystemEntryTypes(typeof(RegistrySystemKey))]
	public class RegistrySystemHives : SystemDirectory{
		public RegistrySystemHives() : this(null, "Registry"){
		}

		public RegistrySystemHives(ISystemDirectory parent, string name) : base(parent, name){
		}

		public override IEnumerable<ISystemEntry> Children {
			get {
				return new RegistryHive[]{
					RegistryHive.ClassesRoot,
					RegistryHive.CurrentConfig,
					RegistryHive.CurrentUser,
					RegistryHive.LocalMachine,
					RegistryHive.PerformanceData,
					RegistryHive.Users,
				}.Select(hive => new RegistrySystemKey(this, RegistryUtility.GetHiveName(hive), hive));
			}
		}

		public override ISystemDirectory GetChildDirectory(string name){
			return this.Children.OfType<ISystemDirectory>().FirstOrDefault(key => key.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
		}
	}
}
