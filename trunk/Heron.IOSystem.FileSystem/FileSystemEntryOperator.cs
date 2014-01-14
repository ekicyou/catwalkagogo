using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CatWalk.Heron.IOSystem;
using CatWalk.IOSystem;
using CatWalk.IOSystem.FileSystem;
using CatWalk.Win32.Shell;

namespace CatWalk.Heron.FileSystem {
	public class FileSystemEntryOperator : IEntryOperator{

		#region IEntryOperator Members

		private static FileOperation GetFileOperation(CancellationToken token, IJob progress) {
			var op = new FileOperation();
			op.ProgressSink.ProgressChanged += (s, e) => {
				progress.Report(e.Progress);
			};
			return op;
		}

		private static IEnumerable<ISystemEntry> Can(IEnumerable<ISystemEntry> entries) {
			return entries.OfType<IFileSystemEntry>();
		}

		private static IEnumerable<ISystemEntry> Can(IEnumerable<ISystemEntry> entries, ISystemEntry dest) {
			return (dest is IFileSystemEntry) ? new IFileSystemEntry[0] : entries.OfType<IFileSystemEntry>();
		}

		public IEnumerable<ISystemEntry> CanCopy(IEnumerable<ISystemEntry> entries, ISystemEntry dest) {
			return Can(entries, dest);
		}

		public IEnumerable<ISystemEntry> Copy(IEnumerable<ISystemEntry> entries, ISystemEntry dest, System.Threading.CancellationToken token, IJob progress) {
			token.ThrowIfCancellationRequested();
			var files = entries.OfType<IFileSystemEntry>().ToArray();
			var d = (IFileSystemEntry)dest;
			using(var op = GetFileOperation(token, progress)) {
				op.ProgressSink.Copying += (s, e) => {
					e.Cancel = token.IsCancellationRequested;
				};
				op.ProgressSink.Copied += (s, e) => {
					e.Cancel = token.IsCancellationRequested;
				};
				op.Copy(files.Select(ent => ent.FileSystemPath.FullPath).ToArray(), d.FileSystemPath.FullPath);
				if(op.IsOperationAborted) {
					progress.ReportCancelled();
					token.ThrowIfCancellationRequested();
				}
			}
			return files;
		}

		public IEnumerable<ISystemEntry> CanMove(IEnumerable<ISystemEntry> entries, ISystemEntry dest) {
			return Can(entries, dest);
		}

		public IEnumerable<ISystemEntry> Move(IEnumerable<ISystemEntry> entries, ISystemEntry dest, System.Threading.CancellationToken token, IJob progress) {
			token.ThrowIfCancellationRequested();
			var files = entries.OfType<IFileSystemEntry>().ToArray();
			var d = (IFileSystemEntry)dest;
			using(var op = GetFileOperation(token, progress)) {
				op.ProgressSink.Moving += (s, e) => {
					e.Cancel = token.IsCancellationRequested;
				};
				op.ProgressSink.Moved += (s, e) => {
					e.Cancel = token.IsCancellationRequested;
				};
				op.Move(files.Select(ent => ent.FileSystemPath.FullPath).ToArray(), d.FileSystemPath.FullPath);
				if(op.IsOperationAborted) {
					progress.ReportCancelled();
					token.ThrowIfCancellationRequested();
				}
			}
			return files;
		}

		public IEnumerable<ISystemEntry> CanDelete(IEnumerable<ISystemEntry> entries) {
			return Can(entries);
		}

		public IEnumerable<ISystemEntry> Delete(IEnumerable<ISystemEntry> entries, CancellationToken token, IJob progress) {
			token.ThrowIfCancellationRequested();
			var files = entries.OfType<IFileSystemEntry>().ToArray();
			using(var op = GetFileOperation(token, progress)) {
				op.ProgressSink.Deleting += (s, e) => {
					e.Cancel = token.IsCancellationRequested;
				};
				op.ProgressSink.Deleted += (s, e) => {
					e.Cancel = token.IsCancellationRequested;
				};
				op.Delete(files.Select(f => f.FileSystemPath.FullPath).ToArray());
				if(op.IsOperationAborted) {
					progress.ReportCancelled();
					token.ThrowIfCancellationRequested();
				}
			}
			return files;
		}

		public IEnumerable<ISystemEntry> CanRename(ISystemEntry entry) {
			return (entry is IFileSystemEntry) ? Seq.Make(entry) : new ISystemEntry[0];
		}

		public IEnumerable<ISystemEntry> Rename(ISystemEntry entry, string newName, CancellationToken token, IJob progress) {
			token.ThrowIfCancellationRequested();
			var file = entry as IFileSystemEntry;
			if(file != null) {
				using(var op = GetFileOperation(token, progress)) {
					op.ProgressSink.Renaming += (s, e) => {
						e.Cancel = token.IsCancellationRequested;
					};
					op.ProgressSink.Renamed += (s, e) => {
						e.Cancel = token.IsCancellationRequested;
					};
					op.Rename(file.FileSystemPath.FullPath, newName);
					if(op.IsOperationAborted) {
						progress.ReportCancelled();
						token.ThrowIfCancellationRequested();
					}
				}
				return Seq.Make(entry);
			} else {
				return new ISystemEntry[0];
			}
		}

		public IEnumerable<ISystemEntry> CanCreate(ISystemEntry parent) {
			return (parent is IFileSystemEntry) ? Seq.Make(parent) : new ISystemEntry[0];
		}

		public IEnumerable<ISystemEntry> Create(ISystemEntry parent, string newName, CancellationToken token, IJob progress) {
			token.ThrowIfCancellationRequested();
			var file = parent as IFileSystemEntry;
			if(file != null) {
				using(var op = GetFileOperation(token, progress)) {
					if(op.IsOperationAborted) {
						progress.ReportCancelled();
						token.ThrowIfCancellationRequested();
					}
				}
				return Seq.Make(parent);
			} else {
				return new ISystemEntry[0];
			}
		}

		public IEnumerable<ISystemEntry> CanOpen(IEnumerable<ISystemEntry> entries) {
			return Can(entries);
		}

		public IEnumerable<ISystemEntry> Open(IEnumerable<ISystemEntry> entries, CancellationToken token, IJob progress) {
			token.ThrowIfCancellationRequested();
			var files = entries.OfType<IFileSystemEntry>().ToArray();
			FileOperations.ExecuteDefaultAction(IntPtr.Zero, files.Select(file => file.FileSystemPath.FullPath).ToArray());
			return files;
		}


		#endregion
	}
}
