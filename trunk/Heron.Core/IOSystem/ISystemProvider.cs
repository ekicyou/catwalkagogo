using System;
using System.Collections.Generic;
using System.Threading;
using CatWalk.IOSystem;
using CatWalk.Heron.ViewModel.IOSystem;

namespace CatWalk.Heron.IOSystem {
	public interface ISystemProvider {
		string DisplayName { get; }
		IEnumerable<IColumnDefinition> GetColumnDefinitions(ISystemEntry entry);
		object GetEntryIcon(ISystemEntry entry, Int32Size size, System.Threading.CancellationToken token);
		IEnumerable<CatWalk.IOSystem.ISystemEntry> GetRootEntries(ISystemEntry parent);
		string Name { get; }
		bool TryParsePath(ISystemEntry root, string path, out ISystemEntry entry);
		object GetViewModel(object parent, SystemEntryViewModel entry, object previous);
		IEnumerable<EntryGroupDescription> GetGroupings(ISystemEntry entry);
	}
}
