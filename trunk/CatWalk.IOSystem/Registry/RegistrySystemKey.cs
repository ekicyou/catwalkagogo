/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Security.AccessControl;

namespace CatWalk.IOSystem {
	[ChildSystemEntryTypes(typeof(RegistrySystemKey), typeof(RegistrySystemEntry))]
	public class RegistrySystemKey : SystemDirectory{
		public RegistrySystemKey ParentRegistry{get; private set;}
		public string KeyName{get; private set;}
		public string RegistryPath {get; private set;}

		public RegistrySystemKey(RegistrySystemKey parent, string name, string keyName) : base(parent, name){
			this.ParentRegistry = parent;
			this.KeyName = keyName;
			this._RegistryKey = new Lazy<RegistryKey>(this.GetRegistryKey);
			this.RegistryPath = this.ParentRegistry.ConcatRegistryPath(keyName);
		}

		public RegistrySystemKey(ISystemDirectory parent, string name, RegistryHive hive) : base(parent, RegistryUtility.GetHiveName(hive)){
			this.ParentRegistry = null;
			this._RegistryKey = new Lazy<RegistryKey>(() => RegistryUtility.GetRegistryKey(hive));
			this.KeyName = this.RegistryPath = RegistryUtility.GetHiveName(hive);
		}

		private RegistryKey GetRegistryKey(){
			if(this.ParentRegistry == null){
				return null;
			}else{
				if(this.ParentRegistry.RegistryKey ==  null){
					return null;
				}else{
					return this.ParentRegistry.RegistryKey.OpenSubKey(
						this.Name,
						RegistryKeyPermissionCheck.ReadSubTree,
						RegistryRights.EnumerateSubKeys | RegistryRights.QueryValues | RegistryRights.ReadKey);
				}
			}
		}

		#region ISystemDirectory Members

		private IEnumerable<ISystemEntry> GetChildren(){
			if(this.RegistryKey == null){
				return new RegistrySystemEntry[0];
			}else{
				return this.RegistryKey.GetSubKeyNames()
					.Select(name => new RegistrySystemKey(this, name, name))
					.Cast<SystemEntry>()
					.Concat(
						this.RegistryKey.GetValueNames()
						.Select(name => new RegistrySystemEntry(this, name, name)));
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <exception cref="System.SecurityException"></exception>
		public override IEnumerable<ISystemEntry> Children {
			get {
				return this.GetChildren();
			}
		}

		public override bool Contains(string name) {
			return this.Children.Any(entry => entry.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
		}

		public override ISystemDirectory GetChildDirectory(string name){
			return this.Children.OfType<ISystemDirectory>().FirstOrDefault(entry => entry.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
		}

		#endregion

		#region IDisposable Members

		~RegistrySystemKey(){
			if(this._RegistryKey.IsValueCreated){
				this._RegistryKey.Value.Close();
			}
		}

		#endregion

		#region IRegistrySystemKey Members

		private readonly Lazy<RegistryKey> _RegistryKey;
		public RegistryKey RegistryKey{
			get{
				return this._RegistryKey.Value;
			}
		}

		public string ConcatRegistryPath(string name){
			return this.RegistryPath + "\\" + name;
		}

		#endregion
	}
}
