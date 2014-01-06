using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using CatWalk.IOSystem;

namespace CatWalk.Heron.IOSystem {
	public abstract class ColumnDefinition{
		public string Name {
			get {
				return this.GetType().FullName;
			}
		}
		public virtual string DisplayName {
			get {
				return this.GetType().Name;
			}
		}
		public object GetValue(ISystemEntry entry) {
			return this.GetValue(entry, false, CancellationToken.None);
		}

		public object GetValue(ISystemEntry entry, bool noCache) {
			return this.GetValue(entry, noCache, CancellationToken.None);
		}

		public abstract object GetValue(ISystemEntry entry, bool noCache, CancellationToken token);

		/*
		#region Equals

		public override bool Equals(object obj) {
			var def = obj as ColumnDefinition;
			if(def != null) {
				return this.Equals(def);
			} else {
				return this.Equals(obj);
			}
		}

		public override int GetHashCode() {
			return this.Name.GetHashCode();
		}

		public bool Equals(ColumnDefinition other) {
			if(other == null) {
				return false;
			} else {
				return this.Name.Equals(other);
			}
		}

		#endregion
		*/

		#region Builtins
		private static ColumnDefinition _NameColumn = new NameColumnDefinition();
		public static ColumnDefinition NameColumn {
			get {
				return _NameColumn;
			}
		}

		private static ColumnDefinition _DisplayNameColumn = new DisplayNameColumnDefinition();
		public static ColumnDefinition DisplayNameColumn {
			get {
				return _DisplayNameColumn;
			}
		}

		private class NameColumnDefinition : ColumnDefinition {

			public override string DisplayName {
				get {
					return "Name";
				}
			}

			public override object GetValue(ISystemEntry entry, bool noCache, CancellationToken token){
				return entry.Name;
			}
		}

		private class DisplayNameColumnDefinition : ColumnDefinition {

			public override string DisplayName {
				get {
					return "DisplayName";
				}
			}

			public override object GetValue(ISystemEntry entry, bool noCache, CancellationToken token){
				return entry.DisplayName;
			}
		}

	}
	#endregion
}
