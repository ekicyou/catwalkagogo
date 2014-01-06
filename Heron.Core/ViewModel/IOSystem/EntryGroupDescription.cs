using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CatWalk.Heron.IOSystem;

namespace CatWalk.Heron.ViewModel.IOSystem {
	public class EntryGroupDescription : GroupDescription{
		public sealed override object GroupNameFromItem(object item, int level, System.Globalization.CultureInfo culture) {
			return this.GroupNameFromItem((SystemEntryViewModel)item, level, culture);
		}

		public virtual string ColumnName{
			get {
				return null;
			}
		}

		protected abstract IEntryGroup GroupNameFromItem(SystemEntryViewModel entry, int level, System.Globalization.CultureInfo culture);
	}

	public class EntryGroup<TID> : IEntryGroup, IComparable<EntryGroup<TID>>, IComparable where TID : IComparable {
		public TID Id { get; private set; }
		public string Name { get; private set; }

		public EntryGroup(TID id, string name) {
			id.ThrowIfNull("id");
			name.ThrowIfNull("name");
			this.Id = id;
			this.Name = name;
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public override bool Equals(object obj) {
			if(obj != null) {
				var grp = obj as EntryGroup<TID>;
				if(grp != null) {
					return this.Id.Equals(grp.Id);
				}
			}
			return base.Equals(obj);
		}



		#region IComparable<EntryGroup<TID>> Members

		public int CompareTo(EntryGroup<TID> other) {
			return this.Id.CompareTo(other.Id);
		}

		#endregion

		#region IComparable Members

		public int CompareTo(object obj) {
			throw new NotImplementedException();
		}

		#endregion
	}

	public interface IEntryGroup {
		string Name { get; }
	}

	public class DelegateEntryGroup<T> : EntryGroup<T> {
		private Predicate<SystemEntryViewModel> _Predicate;
		public DelegateEntryGroup(T id, string name, Predicate<SystemEntryViewModel> pred)
			: base(id, name) {
			pred.ThrowIfNull("pred");
			this._Predicate = pred;
		}

		public bool IsMatch(SystemEntryViewModel entry) {
			return this._Predicate(entry);
		}
	}
}
