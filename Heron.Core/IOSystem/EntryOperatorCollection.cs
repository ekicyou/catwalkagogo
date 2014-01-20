using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CatWalk.IOSystem;

namespace CatWalk.Heron.IOSystem {
	public class EntryOperatorCollection : Collection<IEntryOperator>, IEntryOperator{
		private IEnumerable<T> Call<T>(Func<IEntryOperator, IEnumerable<T>> call) {
			return this.Call(call, CancellationToken.None);
		}
		private IEnumerable<T> Call<T>(Func<IEntryOperator, IEnumerable<T>> call, CancellationToken token) {
			var list = new List<T>();
			foreach(var op in this) {
				token.ThrowIfCancellationRequested();
				var ent = call(op);
				list.AddRange(ent);
				if(list.Count > 0) {
					break;
				}
			}
			return list;
		}

		private IEnumerable<T> Call<T>(IEnumerable<T> source, Func<IEntryOperator, IEnumerable<T>, IEnumerable<T>> call) {
			return this.Call(source, call, CancellationToken.None);
		}

		private IEnumerable<T> Call<T>(IEnumerable<T> source, Func<IEntryOperator, IEnumerable<T>, IEnumerable<T>> call, CancellationToken token) {
			var set = new HashSet<T>(source);
			var list = new List<T>();
			foreach(var op in this) {
				token.ThrowIfCancellationRequested();
				var ent = call(op, set);
				set.ExceptWith(ent);
				list.AddRange(ent);
			}
			return list;
		}

		#region IEntryOperator Members

		public IEnumerable<ISystemEntry> CanCopyTo(IEnumerable<ISystemEntry> entries, ISystemEntry dest) {
			return this.Call(entries, (op, set) => op.CanCopyTo(set, dest));
		}

		public IEnumerable<ISystemEntry> CopyTo(IEnumerable<ISystemEntry> entries, ISystemEntry dest, CancellationToken token, IJob progress) {
			return this.Call(entries, (op, set) => op.CopyTo(set, dest, token, progress), token);
		}

		public IEnumerable<ISystemEntry> CanMoveTo(IEnumerable<ISystemEntry> entries, ISystemEntry dest) {
			return this.Call(entries, (op, set) => op.CanMoveTo(set, dest));
		}

		public IEnumerable<ISystemEntry> MoveTo(IEnumerable<ISystemEntry> entries, ISystemEntry dest, System.Threading.CancellationToken token, IJob progress) {
			return this.Call(entries, (op, set) => op.MoveTo(set, dest, token, progress), token);
		}

		public IEnumerable<ISystemEntry> CanDelete(IEnumerable<ISystemEntry> entries) {
			return this.Call(entries, (op, set) => op.CanDelete(set));
		}

		public IEnumerable<ISystemEntry> Delete(IEnumerable<ISystemEntry> entries, bool canUndo, System.Threading.CancellationToken token, IJob progress) {
			return this.Call(entries, (op, set) => op.Delete(set, canUndo, token, progress), token);
		}

		public IEnumerable<ISystemEntry> CanRename(ISystemEntry entry) {
			return this.Call(op => op.CanRename(entry));
		}

		public IEnumerable<ISystemEntry> Rename(ISystemEntry entry, string newName, System.Threading.CancellationToken token, IJob progress) {
			return this.Call(op => op.Rename(entry, newName, token, progress), token);
		}


		public IEnumerable<ISystemEntry> CanCreate(ISystemEntry parent) {
			return this.Call(op => op.CanCreate(parent));
		}

		public IEnumerable<ISystemEntry> Create(ISystemEntry parent, string newName, CancellationToken token, IJob progress) {
			return this.Call(op => op.Create(parent, newName, token, progress), token);
		}

		public IEnumerable<ISystemEntry> CanOpen(IEnumerable<ISystemEntry> entries) {
			return this.Call(entries, (op, set) => op.CanOpen(set));
		}

		public IEnumerable<ISystemEntry> Open(IEnumerable<ISystemEntry> entries, CancellationToken token, IJob progress) {
			return this.Call(entries, (op, set) => op.Open(set, token, progress), token);
		}

		public IEnumerable<ISystemEntry> CanCopyToClipboard(IEnumerable<ISystemEntry> entries) {
			return this.Call(entries, (op, set) => op.CanCopyToClipboard(entries));
		}

		public IEnumerable<ISystemEntry> CopyToClipboard(IEnumerable<ISystemEntry> entries) {
			return this.Call(entries, (op, set) => op.CopyToClipboard(entries));
		}

		public IEnumerable<ISystemEntry> CanMoveToClipboard(IEnumerable<ISystemEntry> entries) {
			return this.Call(entries, (op, set) => op.CanMoveToClipboard(entries));
		}

		public IEnumerable<ISystemEntry> MoveToClipboard(IEnumerable<ISystemEntry> entries) {
			return this.Call(entries, (op, set) => op.MoveToClipboard(entries));
		}

		public IEnumerable<ISystemEntry> CanPasteTo(ISystemEntry dest) {
			return this.Call(op => this.CanPasteTo(dest));
		}

		public IEnumerable<ISystemEntry> PasteTo(ISystemEntry dest, CancellationToken token, IJob progress) {
			return this.Call(op => this.PasteTo(dest, token, progress));
		}

		#endregion
	}
}
