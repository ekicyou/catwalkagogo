using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Security.AccessControl;

namespace CatWalk.IOSystem {
	[ChildSystemEntryTypes(typeof(RegistrySystemKey), typeof(RegistrySystemEntry))]
	public class RegistrySystemKey : SystemDirectory, IRegistrySystemKey{
		private readonly IRegistrySystemKey ParentRegistry;

		public RegistrySystemKey(IRegistrySystemKey parent, string name) : base(parent, name){
			this.ParentRegistry = parent;
			this._RegistryKey = new RefreshableLazy<RegistryKey>(this.GetRegistryKey);
			this.RegistryPath = this.ParentRegistry.ConcatRegistryPath(name);
			this._Children = new RefreshableLazy<ISystemEntry[]>(this.GetChildren);
		}

		public string Name{
			get{
				return (string)this.Id;
			}
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

		public override void Refresh() {
			this._RegistryKey.Refresh();
			this._Children.Refresh();
			this.OnPropertyChanged("RegistryKey", "Children");
			base.Refresh();
		}

		#region ISystemDirectory Members

		private ISystemEntry[] GetChildren(){
			if(this.RegistryKey == null){
				return new RegistrySystemEntry[0];
			}else{
				return this.RegistryKey.GetSubKeyNames()
					.Select(name => new RegistrySystemKey(this, name))
					.Cast<SystemEntry>()
					.Concat(
						this.RegistryKey.GetValueNames()
						.Select(name => new RegistrySystemEntry(this, name)))
					.ToArray();
			}
		}

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

		#region IDisposable Members

		~RegistrySystemKey(){
			if(this._RegistryKey.IsValueCreated){
				this._RegistryKey.Value.Close();
			}
		}

		#endregion

		#region IRegistrySystemKey Members

		private readonly RefreshableLazy<RegistryKey> _RegistryKey;
		public RegistryKey RegistryKey{
			get{
				return this._RegistryKey.Value;
			}
		}

		public bool Contains(string name) {
			throw new NotImplementedException();
		}

		public string RegistryPath {get; private set;}

		public string ConcatRegistryPath(string name){
			return this.RegistryPath + "\\" + name;
		}

		#endregion
	}
}
