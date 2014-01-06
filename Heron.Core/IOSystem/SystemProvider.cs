using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using CatWalk.IOSystem;
using CatWalk;
using CatWalk.Heron.ViewModel.IOSystem;

namespace CatWalk.Heron.IOSystem {
	public abstract class SystemProvider : ISystemProvider {
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
		public abstract object GetViewModel(object parent, SystemEntryViewModel entry);

		private static readonly NameEntryGroupDescription _NameEntryGroupDescription = new NameEntryGroupDescription();

		public IEnumerable<EntryGroupDescription> GetGroupings(ISystemEntry entry) {
			return Seq.Make(_NameEntryGroupDescription).Concat(this.GetAdditionalGroupings(entry));
		}

		protected virtual IEnumerable<EntryGroupDescription> GetAdditionalGroupings(ISystemEntry entry) {
			return new EntryGroupDescription[0];
		}

		#region NameGroup

		private class NameEntryGroupDescription : EntryGroupDescription {
			private static readonly NameGroup NumericGroup = new NameGroup(0x0001, "0 - 9", name => name[0].IsDecimalNumber());
			/*private static readonly NameGroup AHGroup = new NameGroup(0, "A - H", name => {
				var c = Char.ToUpper(name[0]);
				return 'A' <= c && c <= 'H';
			});
			private static readonly NameGroup IPGroup = new NameGroup(0, "I - P", name => {
				var c = Char.ToUpper(name[0]);
				return 'I' <= c && c <= 'P';
			});
			private static readonly NameGroup QZGroup = new NameGroup(0, "Q - Z", name => {
				var c = Char.ToUpper(name[0]);
				return 'Q' <= c && c <= 'Z';
			});*/
			private static readonly NameGroup[] AlphabetGroups;
			private static readonly NameGroup HiraganaGroup = new NameGroup(0x0100, "ひらがな", name => {
				var c = name[0];
				return c.IsHiragana();
			});
			private static readonly NameGroup KatakanaGroup = new NameGroup(0x0101, "カタカナ", name => {
				var c = name[0];
				return c.IsKatakana();
			});
			private static readonly NameGroup KanjiGroup = new NameGroup(0x0102, "漢字", name => {
				var c = name[0];
				return c.IsKanji();
			});
			private static readonly NameGroup ETCGroup = new NameGroup(0, "etc.", name => true);
			private static readonly NameGroup[] Candidates;

			static NameEntryGroupDescription() {
				AlphabetGroups = Enumerable.Range('A', 'Z').Select(c => new NameGroup(0x0010 + c, "" + (char)c, name => name[0] == c)).ToArray();
				Candidates =
					new[] { 
						NumericGroup
					}
					.Concat(AlphabetGroups)
					.Concat(new[]{
						HiraganaGroup,
						KatakanaGroup,
						KanjiGroup,
						ETCGroup
					})
					.ToArray();
			}

			public override string ColumnName {
				get {
					return ColumnDefinition.DisplayNameColumn.Name;
				}
			}

			protected override IEntryGroup GroupNameFromItem(SystemEntryViewModel entry, int level, System.Globalization.CultureInfo culture) {
				return Candidates.FirstOrDefault(grp => grp.IsMatch(entry));
			}

			private class NameGroup : EntryGroup<int>{
				private Predicate<string> _Predicate;
				public NameGroup(int id, string name, Predicate<string> pred) : base(id, name){
					this._Predicate = pred;
				}

				public bool IsMatch(SystemEntryViewModel entry) {
					return this._Predicate(entry.DisplayName);
				}
			}
		}

		#endregion
	}
}
