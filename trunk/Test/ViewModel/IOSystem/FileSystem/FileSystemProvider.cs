using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using CatWalk;
using CatWalk.IOSystem;
using CatWalk.IOSystem.FileSystem;

namespace Test.ViewModel.IOSystem.FileSystem {
	public class FileSystemProvider : SystemProvider{
		private static readonly ColumnDefinition _ExtensionColumn = new ExtensionColumn();
		private static readonly ColumnDefinition _BaseNameColumn = new BaseNameColumn();

		protected override IEnumerable<ColumnDefinition> GetAdditionalColumnProviders(ISystemEntry entry) {
			IEnumerable<ColumnDefinition> columns = new ColumnDefinition[]{
			};
			var fsentry = entry as FileSystemEntry;
			if(fsentry != null){
				var source = new FileInfoSource(fsentry);
				columns = columns.Concat(new ColumnDefinition[]{
					new CreationTimeColumn(source),
					new LastWriteTimeColumn(source),
					new LastAccessTimeColumn(source),
					new AttributesColumn(source),
					_ExtensionColumn,
					_BaseNameColumn,
					new OwnerColumn(new ResetLazyColumnValueSource<NTAccount>(() => fsentry.Owner)),
					new AccessControlColumn(new ResetLazyColumnValueSource<FileSecurity>(() => fsentry.AccessControl)),
				});
				if(entry.IsDirectory) {
				} else {
					columns = columns.Concat(new ColumnDefinition[]{
						new FileSizeColumn(source),
					});
				}
			}

			var drive = entry as FileSystemDrive;
			if(drive != null) {
				var source = new DriveInfoSource(drive);
				columns = columns.Concat(new ColumnDefinition[]{
					new VolumeLabelColumn(source),
					new AvailableFreeSpaceColumn(source),
					new TotalFreeSpaceColumn(source),
					new UsedSpaceForAvailableFreeSpaceColumn(source),
					new UsedSpaceForTotalFreeSpaceColumn(source),
					new DriveFormatColumn(source),
				});
			}

			return columns;
		}

		private class OwnerColumn : CacheColumnDefinition<NTAccount> {
			public OwnerColumn(IColumnValueSource<NTAccount> source) : base(source){}
		}

		private class AccessControlColumn : CacheColumnDefinition<FileSecurity> {
			public AccessControlColumn(IColumnValueSource<FileSecurity> source) : base(source){}
		}

		private class ExtensionColumn : ColumnDefinition {
			public override object GetValue(ISystemEntry entry, bool noCache, CancellationToken token) {
				return ((FileSystemEntry)entry).Extension;
			}
		}

		private class BaseNameColumn : ColumnDefinition {
			public override object GetValue(ISystemEntry entry, bool noCache, CancellationToken token) {
				return ((FileSystemEntry)entry).BaseName;
			}
		}

		private abstract class FileInfoColumn : CacheColumnDefinition<FileInfo> {
			public FileInfoColumn(FileInfoSource source) : base(source){}
		}

		private class FileInfoSource : ResetLazyColumnValueSource<FileInfo> {
			public FileInfoSource(FileSystemEntry entry) : base(() => entry.FileInfo){
			}
		}

		private class CreationTimeColumn : FileInfoColumn {
			public CreationTimeColumn(FileInfoSource source) : base(source){}

			protected override object SelectValue(FileInfo value) {
				return value.CreationTime;
			}
		}

		private class LastWriteTimeColumn : FileInfoColumn {
			public LastWriteTimeColumn(FileInfoSource source) : base(source) { }

			protected override object SelectValue(FileInfo value) {
				return value.LastWriteTime;
			}
		}

		private class LastAccessTimeColumn : FileInfoColumn {
			public LastAccessTimeColumn(FileInfoSource source) : base(source) { }

			protected override object SelectValue(FileInfo value) {
				return value.LastAccessTime;
			}
		}

		private class AttributesColumn : FileInfoColumn {
			public AttributesColumn(FileInfoSource source) : base(source) { }

			protected override object SelectValue(FileInfo value) {
				return value.Attributes;
			}
		}

		private class FileSizeColumn : FileInfoColumn {
			public FileSizeColumn(FileInfoSource source) : base(source){}

			protected override object SelectValue(FileInfo value) {
				return value.Length;
			}
		}

		private class DriveInfoSource : ResetLazyColumnValueSource<DriveInfo> {
			public DriveInfoSource(FileSystemDrive drive)
				: base(() => drive.DriveInfo) {

			}
		}

		private class AvailableFreeSpaceColumn : CacheColumnDefinition<DriveInfo> {
			public AvailableFreeSpaceColumn(DriveInfoSource source) : base(source) {}

			protected override object SelectValue(DriveInfo value) {
				return value.AvailableFreeSpace;
			}
		}

		private class TotalFreeSpaceColumn : CacheColumnDefinition<DriveInfo> {
			public TotalFreeSpaceColumn(DriveInfoSource source) : base(source) { }

			protected override object SelectValue(DriveInfo value) {
				return value.TotalFreeSpace;
			}
		}

		private class TotalSizeColumn : CacheColumnDefinition<DriveInfo> {
			public TotalSizeColumn(DriveInfoSource source) : base(source) { }

			protected override object SelectValue(DriveInfo value) {
				return value.TotalSize;
			}
		}

		private class UsedSpaceForAvailableFreeSpaceColumn : CacheColumnDefinition<DriveInfo> {
			public UsedSpaceForAvailableFreeSpaceColumn(DriveInfoSource source) : base(source) { }

			protected override object SelectValue(DriveInfo value) {
				return value.TotalSize - value.AvailableFreeSpace;
			}
		}

		private class UsedSpaceForTotalFreeSpaceColumn : CacheColumnDefinition<DriveInfo> {
			public UsedSpaceForTotalFreeSpaceColumn(DriveInfoSource source) : base(source) { }

			protected override object SelectValue(DriveInfo value) {
				return value.TotalSize - value.TotalFreeSpace;
			}
		}

		private class VolumeLabelColumn : CacheColumnDefinition<DriveInfo> {
			public VolumeLabelColumn(DriveInfoSource source) : base(source) { }

			protected override object SelectValue(DriveInfo value) {
				return value.VolumeLabel;
			}
		}

		private class DriveFormatColumn : CacheColumnDefinition<DriveInfo> {
			public DriveFormatColumn(DriveInfoSource source) : base(source) { }

			protected override object SelectValue(DriveInfo value) {
				return value.DriveFormat;
			}
		}
	}
}
