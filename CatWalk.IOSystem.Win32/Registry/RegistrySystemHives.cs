/*
	$Id: RegistrySystemHives.cs 217 2011-06-21 14:16:53Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace CatWalk.IOSystem.Win32{
	[ChildSystemEntryTypes(typeof(RegistrySystemKey))]
	public class RegistrySystemHiveDirectory : SystemDirectory{
		public RegistrySystemHiveDirectory() : this(null, "Registry"){
		}

		public RegistrySystemHiveDirectory(ISystemDirectory parent, string name) : base(parent, name){
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
