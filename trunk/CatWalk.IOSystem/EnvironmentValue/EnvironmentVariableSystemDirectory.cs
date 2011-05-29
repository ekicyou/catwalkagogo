using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CatWalk.IOSystem {
	[ChildSystemEntryTypes(typeof(EnvironmentVariableSystemEntry))]
	public class EnvironmentVariableSystemDirectory : SystemDirectory{
		public EnvironmentVariableTarget EnvironmentVariableTarget{
			get{
				return (EnvironmentVariableTarget)this.Id;
			}
		}

		public EnvironmentVariableSystemDirectory(ISystemDirectory parent, EnvironmentVariableTarget id) : base(parent, id){
			this._Children = new RefreshableLazy<ISystemEntry[]>(() => Environment.GetEnvironmentVariables(this.EnvironmentVariableTarget)
				.Cast<DictionaryEntry>()
				.Select(v => new EnvironmentVariableSystemEntry(this, (string)v.Key, this.EnvironmentVariableTarget))
				.ToArray());
		}

		public override void Refresh() {
			base.Refresh();
			this._Children.Refresh();
			this.OnPropertyChanged("Children");
		}

		#region ISystemDirectory Members

		private RefreshableLazy<ISystemEntry[]> _Children;
		public override IEnumerable<ISystemEntry> Children {
			get {
				return this._Children.Value;
			}
		}

		public override ISystemDirectory GetChildDirectory(object id) {
			return null;
		}

		public override bool Contains(object id) {
			return this.Children.Any(entry => entry.Id == id);
		}

		#endregion
	}
}
