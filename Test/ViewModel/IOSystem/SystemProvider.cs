using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.IOSystem;

namespace Test.ViewModel.IOSystem {
	public abstract class SystemProvider {
		public virtual string Name {
			get {
				return this.GetType().Name;
			}
		}
		public virtual string DisplayName {
			get {
				return this.Name;
			}
		}

		public IEnumerable<ColumnDefinition> GetColumnDefinitions(ISystemEntry entry) {
			return (new ColumnDefinition[]{ColumnDefinition.NameColumn, ColumnDefinition.DisplayNameColumn}).Concat(this.GetAdditionalColumnProviders(entry));
		}
		protected virtual IEnumerable<ColumnDefinition> GetAdditionalColumnProviders(ISystemEntry entry) {
			return new ColumnDefinition[0];
		}
		public virtual SystemEntryViewModel GetEntryViewModel(SystemEntryViewModel parent, ISystemEntry entry) {
			return new SystemEntryViewModel(parent, this, entry);
		}
	}
}
