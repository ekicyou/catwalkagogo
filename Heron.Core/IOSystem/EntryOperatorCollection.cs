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

		#region IEntryOperator Members

		public IEnumerable<ISystemEntry> CanCopy(IEnumerable<ISystemEntry> entries, ISystemEntry dest) {
			var set = new HashSet<ISystemEntry>(entries);
			var list = new List<ISystemEntry>();
			foreach(var op in this) {
				var ent = op.CanCopy(set, dest);
				set.ExceptWith(ent);
				list.AddRange(ent);
			}
			return list;
		}

		public IEnumerable<ISystemEntry> Copy(IEnumerable<ISystemEntry> entries, ISystemEntry dest, CancellationToken token, IJob progress) {
			var set = new HashSet<ISystemEntry>(entries);
			var list = new List<ISystemEntry>();
			foreach(var op in this) {
				token.ThrowIfCancellationRequested();
				var ent = op.Copy(entries, dest, token, progress);
				set.ExceptWith(ent);
				list.AddRange(ent);
			}
			return list;
		}

		public IEnumerable<ISystemEntry> CanMove(IEnumerable<ISystemEntry> entries, ISystemEntry dest) {
			var set = new HashSet<ISystemEntry>(entries);
			var list = new List<ISystemEntry>();
			foreach(var op in this) {
				var ent = op.CanMove(set, dest);
				set.ExceptWith(ent);
				list.AddRange(ent);
			}
			return list;
		}

		public IEnumerable<ISystemEntry> Move(IEnumerable<ISystemEntry> entries, ISystemEntry dest, System.Threading.CancellationToken token, IJob progress) {
			var set = new HashSet<ISystemEntry>(entries);
			var list = new List<ISystemEntry>();
			foreach(var op in this) {
				token.ThrowIfCancellationRequested();
				var ent = op.Move(entries, dest, token, progress);
				set.ExceptWith(ent);
				list.AddRange(ent);
			}
			return list;
		}

		public IEnumerable<ISystemEntry> CanDelete(IEnumerable<ISystemEntry> entries) {
			var set = new HashSet<ISystemEntry>(entries);
			var list = new List<ISystemEntry>();
			foreach(var op in this) {
				var ent = op.CanDelete(entries);
				set.ExceptWith(ent);
				list.AddRange(ent);
			}
			return list;
		}

		public IEnumerable<ISystemEntry> Delete(IEnumerable<ISystemEntry> entries, System.Threading.CancellationToken token, IJob progress) {
			var set = new HashSet<ISystemEntry>(entries);
			var list = new List<ISystemEntry>();
			foreach(var op in this) {
				token.ThrowIfCancellationRequested();
				var ent = op.Delete(entries, token, progress);
				set.ExceptWith(ent);
				list.AddRange(ent);
			}
			return list;
		}

		public IEnumerable<ISystemEntry> CanRename(ISystemEntry entry) {
			var list = new List<ISystemEntry>();
			foreach(var op in this) {
				var ent = op.CanRename(entry);
				list.AddRange(ent);
				if(list.Count > 0) {
					break;
				}
			}
			return list;
		}

		public IEnumerable<ISystemEntry> Rename(ISystemEntry entry, string newName, System.Threading.CancellationToken token, IJob progress) {
			var list = new List<ISystemEntry>();
			foreach(var op in this) {
				token.ThrowIfCancellationRequested();
				var ent = op.Rename(entry, newName, token, progress);
				list.AddRange(ent);
				if(list.Count > 0) {
					break;
				}
			}
			return list;
		}


		public IEnumerable<ISystemEntry> CanCreate(ISystemEntry parent) {
			var list = new List<ISystemEntry>();
			foreach(var op in this) {
				var ent = op.CanCreate(parent);
				list.AddRange(ent);
				if(list.Count > 0) {
					break;
				}
			}
			return list;
		}

		public IEnumerable<ISystemEntry> Create(ISystemEntry parent, string newName, CancellationToken token, IJob progress) {
			var list = new List<ISystemEntry>();
			foreach(var op in this) {
				token.ThrowIfCancellationRequested();
				var ent = op.Create(parent, newName, token, progress);
				list.AddRange(ent);
				if(list.Count > 0) {
					break;
				}
			}
			return list;
		}

		#endregion
	}
}
