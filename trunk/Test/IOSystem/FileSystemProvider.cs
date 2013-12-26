using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CatWalk;
using CatWalk.IOSystem;
using CatWalk.IOSystem.FileSystem;
using CatWalk.Win32;
using CatWalk.IO;

namespace CatWalk.Heron.IOSystem {
	using Drawing = System.Drawing;
	public class FileSystemProvider : SystemProvider, IDisposable{
		private static readonly ColumnDefinition _ExtensionColumn = new ExtensionColumn();
		private static readonly ColumnDefinition _BaseNameColumn = new BaseNameColumn();
		private Dictionary<ImageListSize, ImageList> _ImageLists = new Dictionary<ImageListSize,ImageList>();

		protected override IEnumerable<ColumnDefinition> GetAdditionalColumnProviders(ISystemEntry entry) {
			entry.ThrowIfNull("entry");
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

		public override bool TryParsePath(ISystemEntry root, string path, out ISystemEntry entry) {
			root.ThrowIfNull("root");
			path.ThrowIfNull("path");
			var filePath = new FilePath(path, FilePathKind.Absolute);
			if(filePath.IsValid && filePath.PathKind == FilePathKind.Absolute) {
				var drives = new FileSystemDriveDirectory(root, "Drives");
				var drive = drives.GetChildDirectory(filePath.VolumeName);
				entry = drive;
				foreach(var name in filePath.Fragments) {
					entry = entry.GetChildDirectory(name);
				}
				return true;
			} else {
				entry = null;
				return false;
			}
		}

		public override IEnumerable<ISystemEntry> GetRootEntries(ISystemEntry parent) {
			parent.ThrowIfNull("parent");
			return Seq.Make(new FileSystemDriveDirectory(parent, "Drives"));
		}

		public override System.Windows.Media.Imaging.BitmapSource GetEntryIcon(ISystemEntry entry, Int32Size size, CancellationToken token) {
			entry.ThrowIfNull("entry");
			var ife = entry as IFileSystemEntry;
			if(ife != null) {
				var bmp = new WriteableBitmap(size.Width, size.Height, 96, 96, PixelFormats.Pbgra32, null);
				Task.Factory.StartNew(new Action(delegate {
					var list = this.GetImageList(size);
					int overlay;
					var index = list.GetIconIndexWithOverlay(ife.FileSystemPath.FullPath, out overlay);
					Drawing::Bitmap bitmap = null;
					Drawing::Imaging.BitmapData bitmapData = null;
					try {
						bitmap = list.Draw(index, overlay, ImageListDrawOptions.PreserveAlpha);
						bitmapData = bitmap.LockBits(new Drawing::Rectangle(0, 0, bitmap.Width, bitmap.Height), Drawing::Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);
						bmp.Dispatcher.BeginInvoke(new Action(delegate {
							try {
								bmp.WritePixels(
									new System.Windows.Int32Rect(((int)bmp.Width - bitmap.Width) / 2, ((int)bmp.Height - bitmap.Height) / 2, bitmap.Width, bitmap.Height),
									bitmapData.Scan0,
									bitmapData.Stride * bitmapData.Height,
									bitmapData.Stride);
							} finally {
								bitmap.UnlockBits(bitmapData);
								bitmap.Dispose();
							}
						}));
					} catch {
						if(bitmapData != null) {
							bitmap.UnlockBits(bitmapData);
						}
						if(bitmap != null) {
							bitmap.Dispose();
						}
					}
				}), token);
				return bmp;
			} else {
				return base.GetEntryIcon(entry, size, token);
			}
		}

		private ImageList GetImageList(Int32Size size) {
			lock(this._ImageLists){
				var ilsize = GetImageListSize(size);
				ImageList list;
				if(this._ImageLists.TryGetValue(ilsize, out list)) {
					return list;
				} else {
					list = new ImageList(ilsize);
					this._ImageLists.Add(ilsize, list);
					return list;
				}
			}
		}

		private static ImageListSize GetImageListSize(Int32Size size) {
			if(size.Width <= 16 && size.Height <= 16) {
				return ImageListSize.Small;
			} else if(size.Width <= 32 && size.Height <= 32) {
				return ImageListSize.Large;
			} else if(size.Width <= 48 && size.Height <= 48) {
				return ImageListSize.ExtraLarge;
			} else {
				return ImageListSize.Jumbo;
			}
		}

		#region IDisposable

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private bool _Disposed = false;

		protected virtual void Dispose(bool disposing) {
			if(!this._Disposed) {
				foreach(var list in this._ImageLists.Values) {
					list.Dispose();
				}
				this._Disposed = true;
			}
		}

		~FileSystemProvider() {
			this.Dispose(false);
		}

		#endregion

		#region Entry Columns
		private class OwnerColumn : CacheColumnDefinition<NTAccount> {
			public OwnerColumn(IColumnValueSource<NTAccount> source) : base(source) { }
		}

		private class AccessControlColumn : CacheColumnDefinition<FileSecurity> {
			public AccessControlColumn(IColumnValueSource<FileSecurity> source) : base(source) { }
		}

				private abstract class FileInfoColumn : CacheColumnDefinition<IFileInformation> {
			public FileInfoColumn(FileInfoSource source) : base(source) { }
		}

		private class FileInfoSource : ResetLazyColumnValueSource<IFileInformation> {
			public FileInfoSource(FileSystemEntry entry)
				: base(() => entry.FileInformation) {
			}
		}

		private class CreationTimeColumn : FileInfoColumn {
			public CreationTimeColumn(FileInfoSource source) : base(source) { }

			protected override object SelectValue(IFileInformation value) {
				return value.CreationTime;
			}
		}

		private class LastWriteTimeColumn : FileInfoColumn {
			public LastWriteTimeColumn(FileInfoSource source) : base(source) { }

			protected override object SelectValue(IFileInformation value) {
				return value.LastWriteTime;
			}
		}

		private class LastAccessTimeColumn : FileInfoColumn {
			public LastAccessTimeColumn(FileInfoSource source) : base(source) { }

			protected override object SelectValue(IFileInformation value) {
				return value.LastAccessTime;
			}
		}

		private class AttributesColumn : FileInfoColumn {
			public AttributesColumn(FileInfoSource source) : base(source) { }

			protected override object SelectValue(IFileInformation value) {
				return value.Attributes;
			}
		}

		private class FileSizeColumn : FileInfoColumn {
			public FileSizeColumn(FileInfoSource source) : base(source) { }

			protected override object SelectValue(IFileInformation value) {
				return value.Length;
			}
		}


		#endregion

		#region File Columns

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

		#endregion

		#region Drive Columns

		private class DriveInfoSource : ResetLazyColumnValueSource<DriveInfo> {
			public DriveInfoSource(FileSystemDrive drive)
				: base(() => drive.DriveInfo) {

			}
		}

		private class AvailableFreeSpaceColumn : CacheColumnDefinition<DriveInfo> {
			public AvailableFreeSpaceColumn(DriveInfoSource source) : base(source) { }

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
		#endregion

	}
}
