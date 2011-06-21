/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CatWalk.IOSystem {
	using IO = System.IO;
	[ChildSystemEntryTypes(typeof(FileSystemDirectory), typeof(FileSystemFileEntry))]
	public class FileSystemDrive : SystemDirectory{
		public char DriveLetter{get; private set;}

		public FileSystemDrive(ISystemDirectory parent, string name, char driveLetter) : base(parent, name){
			this.DriveLetter = ValidateDriveLetter(driveLetter);
			this.FileSystemPath = this.DriveLetter + ":" + IO::Path.DirectorySeparatorChar;
		}

		private static char ValidateDriveLetter(char driveLetter){
			driveLetter = driveLetter.ToString().ToUpper()[0];
			if(driveLetter < 'A' || 'Z' < driveLetter){
				throw new ArgumentException("driveLetter");
			}
			return driveLetter;
		}

		#region Properties

		public override string DisplayName{
			get{
				var info = new DriveInfo(this.FileSystemPath);
				return (info.IsReady) ? info.VolumeLabel + " (" + this.DriveLetter + ":)" : info.Name;
			}
		}

		public override bool Exists {
			get {
				var info = new DriveInfo(this.FileSystemPath);
				return info.IsReady;
			}
		}

		public bool IsReady{
			get{
				return this.Exists;
			}
		}

		public DriveType DriveType{
			get{
				var info = new DriveInfo(this.FileSystemPath);
				return (info.IsReady) ? info.DriveType : System.IO.DriveType.Unknown;
			}
		}

		public string DriveFormat{
			get{
				var info = new DriveInfo(this.FileSystemPath);
				return (info.IsReady) ? info.DriveFormat : "";
			}
		}

		public long AvailableFreeSpace{
			get{
				var info = new DriveInfo(this.FileSystemPath);
				return (info.IsReady) ? info.AvailableFreeSpace : 0;
			}
		}

		public long TotalSize{
			get{
				var info = new DriveInfo(this.FileSystemPath);
				return (info.IsReady) ? info.TotalSize : 0;
			}
		}

		public long TotalFreeSpace{
			get{
				var info = new DriveInfo(this.FileSystemPath);
				return (info.IsReady) ? info.TotalFreeSpace : 0;
			}
		}

		#endregion

		#region IFileSystemDirectory Members

		public string ConcatFileSystemPath(string name) {
			return this.FileSystemPath + name;
		}

		public string FileSystemPath {get; private set;}

		#endregion

		#region ISystemDirectory Members

		public override IEnumerable<ISystemEntry> Children {
			get {
				return Directory.EnumerateDirectories(this.FileSystemPath)
					.Select(file => new FileSystemDirectory(this, IO::Path.GetFileName(file), file))
					.Cast<ISystemEntry>()
					.Concat(
						Directory.EnumerateFiles(this.FileSystemPath)
						.Select(file => new FileSystemFileEntry(this, IO::Path.GetFileName(file), file)));
			}
		}

		public override ISystemDirectory GetChildDirectory(string name) {
			var path = this.ConcatFileSystemPath(name);
			if(Directory.Exists(path)){
				return new FileSystemDirectory(this, name, path);
			}else{
				return null;
			}
		}

		public override bool Contains(string name) {
			var path = this.ConcatFileSystemPath(name);
			return Directory.Exists(path) || File.Exists(path);
		}

		#endregion
	}
}
