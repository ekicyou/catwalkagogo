using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using CatWalk.IOSystem;

namespace CatWalk.Heron.IOSystem {
	public interface IEntryOperator {
		IEnumerable<ISystemEntry> CanCopy(IEnumerable<ISystemEntry> entries, ISystemEntry dest);
		IEnumerable<ISystemEntry> Copy(IEnumerable<ISystemEntry> entries, ISystemEntry dest, CancellationToken token, IJob progress);
		IEnumerable<ISystemEntry> CanMove(IEnumerable<ISystemEntry> entries, ISystemEntry dest);
		IEnumerable<ISystemEntry> Move(IEnumerable<ISystemEntry> entries, ISystemEntry dest, CancellationToken token, IJob progress);
		IEnumerable<ISystemEntry> CanDelete(IEnumerable<ISystemEntry> entries);
		IEnumerable<ISystemEntry> Delete(IEnumerable<ISystemEntry> entries, CancellationToken token, IJob progress);
		IEnumerable<ISystemEntry> CanRename(ISystemEntry entry);
		IEnumerable<ISystemEntry> Rename(ISystemEntry entry, string newName, CancellationToken token, IJob progress);
		IEnumerable<ISystemEntry> CanCreate(ISystemEntry parent);
		IEnumerable<ISystemEntry> Create(ISystemEntry parent, string newName, CancellationToken token, IJob progress);
	}
}
