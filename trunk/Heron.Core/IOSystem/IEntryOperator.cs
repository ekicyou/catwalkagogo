using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using CatWalk.IOSystem;

namespace CatWalk.Heron.IOSystem {
	public interface IEntryOperator {
		IEnumerable<ISystemEntry> CanCopyTo(IEnumerable<ISystemEntry> entries, ISystemEntry dest);
		IEnumerable<ISystemEntry> CopyTo(IEnumerable<ISystemEntry> entries, ISystemEntry dest, CancellationToken token, IJob progress);
		
		IEnumerable<ISystemEntry> CanMoveTo(IEnumerable<ISystemEntry> entries, ISystemEntry dest);
		IEnumerable<ISystemEntry> MoveTo(IEnumerable<ISystemEntry> entries, ISystemEntry dest, CancellationToken token, IJob progress);
		
		IEnumerable<ISystemEntry> CanDelete(IEnumerable<ISystemEntry> entries);
		IEnumerable<ISystemEntry> Delete(IEnumerable<ISystemEntry> entries, bool canUndo, CancellationToken token, IJob progress);
		
		IEnumerable<ISystemEntry> CanRename(ISystemEntry entry);
		IEnumerable<ISystemEntry> Rename(ISystemEntry entry, string newName, CancellationToken token, IJob progress);
		
		IEnumerable<ISystemEntry> CanCreate(ISystemEntry parent);
		IEnumerable<ISystemEntry> Create(ISystemEntry parent, string newName, CancellationToken token, IJob progress);

		IEnumerable<ISystemEntry> CanOpen(IEnumerable<ISystemEntry> entries);
		IEnumerable<ISystemEntry> Open(IEnumerable<ISystemEntry> entries, CancellationToken token, IJob progress);

		IEnumerable<ISystemEntry> CanCopyToClipboard(IEnumerable<ISystemEntry> entries);
		IEnumerable<ISystemEntry> CopyToClipboard(IEnumerable<ISystemEntry> entries);

		IEnumerable<ISystemEntry> CanMoveToClipboard(IEnumerable<ISystemEntry> entries);
		IEnumerable<ISystemEntry> MoveToClipboard(IEnumerable<ISystemEntry> entries);

		IEnumerable<ISystemEntry> CanPasteTo(ISystemEntry dest);
		IEnumerable<ISystemEntry> PasteTo(ISystemEntry dest, CancellationToken token, IJob progress);
	}
}
