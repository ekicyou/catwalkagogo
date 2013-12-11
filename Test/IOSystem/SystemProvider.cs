using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using CatWalk.IOSystem;
using CatWalk;

namespace Test.IOSystem {
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
		public abstract bool TryParsePath(ISystemEntry root, string path, out ISystemEntry entry);
		public abstract IEnumerable<ISystemEntry> GetRootEntries(ISystemEntry parent);
		public virtual BitmapSource GetEntryIcon(ISystemEntry entry, Int32Size size, CancellationToken token) {
			return null;
		}
	}
}
