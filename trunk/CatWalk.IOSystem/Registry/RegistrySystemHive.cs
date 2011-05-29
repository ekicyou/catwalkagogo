using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace CatWalk.IOSystem {
	[ChildSystemEntryTypes(typeof(RegistrySystemKey), typeof(RegistrySystemEntry))]
	public class RegistrySystemHive : SystemDirectory, IRegistrySystemKey{
		public RegistryHive RegistryHive{get; private set;}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="hive"></param>
		/// <exception cref="ArgumentOutOfRangeException">hive parameter is not valid value.</exception>
		public RegistrySystemHive(ISystemDirectory parent, RegistryHive hive) : base(parent, GetHiveName(hive)){
			this.RegistryHive = hive;
			this.Initialize();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="name"></param>
		/// <exception cref="ArgumentException">name is not valid value</exception>
		public RegistrySystemHive(ISystemDirectory parent, string name) : base(parent, name){
			this.RegistryHive = GetHive(name);
			this.Initialize();
		}

		private void Initialize(){
			this._RegistryKey = new RefreshableLazy<RegistryKey>(() => GetRegistryKey(this.RegistryHive));
			this._Children = new RefreshableLazy<ISystemEntry[]>(() => this.RegistryKey.GetSubKeyNames()
				.Select(name => new RegistrySystemKey(this, name))
				.Cast<SystemEntry>()
				.Concat(
					this.RegistryKey.GetValueNames()
					.Select(name => new RegistrySystemEntry(this, name)))
				.ToArray());
		}

		public string Name{
			get{
				return (string)this.Id;
			}
		}

		private static string GetHiveName(RegistryHive hive){
			switch(hive){
				case RegistryHive.ClassesRoot: return "HKEY_CLASSES_ROOT";
				case RegistryHive.CurrentConfig: return "HKEY_CURRENT_CONFIG";
				case RegistryHive.CurrentUser: return "HKEY_CURRENT_USER";
				//case RegistryHive.DynData: return "HKEY_DYNAMIC_DATA";
				case RegistryHive.LocalMachine: return "HKEY_LOCAL_MACHINE";
				case RegistryHive.PerformanceData: return "HKEY_PERFORMANCE_DATA";
				case RegistryHive.Users: return "HKEY_USERS";
				default: throw new ArgumentOutOfRangeException("hive");
			}
		}

		private static RegistryHive GetHive(string name){
			switch(name.ToUpper()){
				case "HKEY_CLASSES_ROOT": return RegistryHive.ClassesRoot;
				case "HKEY_CURRENT_CONFIG": return RegistryHive.CurrentConfig;
				case "HKEY_CURRENT_USER": return RegistryHive.CurrentUser;
				//case "HKEY_DYNAMIC_DATA": return RegistryHive.DynData;
				case "HKEY_LOCAL_MACHINE": return RegistryHive.LocalMachine;
				case "HKEY_PERFORMANCE_DATA": return RegistryHive.PerformanceData;
				case "HKEY_USERS": return RegistryHive.Users;
				default: throw new ArgumentException("name");
			}
		}

		private RegistryKey GetRegistryKey(RegistryHive hive){
			switch(hive){
				case RegistryHive.ClassesRoot: return Registry.ClassesRoot;
				case RegistryHive.CurrentConfig: return Registry.CurrentConfig;
				case RegistryHive.CurrentUser: return Registry.CurrentUser;
				//case RegistryHive.DynData: return Registry.DynData;
				case RegistryHive.LocalMachine: return Registry.LocalMachine;
				case RegistryHive.PerformanceData: return Registry.PerformanceData;
				case RegistryHive.Users: return Registry.Users;
				default: throw new ArgumentOutOfRangeException("hive");
			}
		}

		public override void Refresh() {
			this._Children.Refresh();
			this._RegistryKey.Refresh();
			this.OnPropertyChanged("Children", "RegistryKey");
			base.Refresh();
		}

		#region IRegistrySystemKey Members

		private RefreshableLazy<RegistryKey> _RegistryKey;
		public RegistryKey RegistryKey {
			get{
				return this._RegistryKey.Value;
			}
		}

		public string RegistryPath {get; private set;}

		public string ConcatRegistryPath(string name){
			return this.RegistryPath + "\\" + name;
		}

		public bool Contains(string name){
			return this.Contains((object)name);
		}

		#endregion

		#region ISystemDirectory Members

		private RefreshableLazy<ISystemEntry[]> _Children;
		/// <summary>
		/// 
		/// </summary>
		/// <exception cref="System.SecurityException"></exception>
		public override IEnumerable<ISystemEntry> Children {
			get {
				return this._Children.Value;
			}
		}

		public override ISystemDirectory GetChildDirectory(object id) {
			this._Children.Refresh();
			this.OnPropertyChanged("Children");
			return this.Children.OfType<RegistrySystemKey>().FirstOrDefault(key => key.Name.Equals(id.ToString(), StringComparison.OrdinalIgnoreCase));
		}

		public override bool Contains(object id) {
			return this.Children.Any(entry => entry.Id == id);
		}

		public override string ConcatDisplayPath(string name) {
			return this.DisplayPath + "\\" + name;
		}

		#endregion
	}
}
