using System;
using System.Collections.Generic;
using System.Threading;
using CatWalk.IOSystem;

namespace CatWalk.Heron.IOSystem {
	public interface ISystemProvider {
		string DisplayName { get; }
		IEnumerable<ColumnDefinition> GetColumnDefinitions(ISystemEntry entry);
		System.Windows.Media.Imaging.BitmapSource GetEntryIcon(ISystemEntry entry, Int32Size size, System.Threading.CancellationToken token);
		IEnumerable<CatWalk.IOSystem.ISystemEntry> GetRootEntries(ISystemEntry parent);
		string Name { get; }
		bool TryParsePath(ISystemEntry root, string path, out ISystemEntry entry);
		object GetViewModel(object parent, ISystemEntry entry);
	}
}
