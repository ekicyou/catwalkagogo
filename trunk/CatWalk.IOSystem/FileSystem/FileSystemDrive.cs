using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CatWalk.IOSystem {
	[ChildSystemEntryTypes(typeof(FileSystemDirectory), typeof(FileSystemEntry))]
	public class FileSystemDrive : SystemDirectory, IFileSystemDirectory{
		public char DriveLetter{
			get{
				return (char)this.Id;
			}
		}

		public FileSystemDrive(ISystemDirectory parent, char driveLetter) : base(parent, ValidateDriveLetter(driveLetter)){
			this.FileSystemPath = this.DriveLetter + ":" + Path.DirectorySeparatorChar;
			this._Children = new RefreshableLazy<ISystemEntry[]>(() =>
				Directory.EnumerateDirectories(this.FileSystemPath)
					.Select(file => new FileSystemDirectory(this, Path.GetFileName(file)))
					.Concat(
						Directory.EnumerateFiles(this.FileSystemPath)
							.Select(file => new FileSystemEntry(this, Path.GetFileName(file))))
								.ToArray());
		}

		private static char ValidateDriveLetter(char driveLetter){
			driveLetter = driveLetter.ToString().ToUpper()[0];
			if(driveLetter < 'A' || 'Z' < driveLetter){
				throw new ArgumentException("driveLetter");
			}
			return driveLetter;
		}

		public override void Refresh() {
			this._Children.Refresh();
			this.RefreshDriveInfo();
			this.OnPropertyChanged("Children", "DisplayName","DriveType", "DriveFormat", "AvailableFreeSpace", "TotalSize", "TotalFreeSpace");
			base.Refresh();
		}

		private void RefreshDriveInfo(){
			var info = new DriveInfo(this.FileSystemPath);
			this._Exists = info.IsReady;
			if(info.IsReady){
				this._DisplayName = info.VolumeLabel + " (" + this.DriveLetter + ":)";
				this._DriveType = info.DriveType;
				this._DriveFormat = info.DriveFormat;
				this._AvailableFreeSpace = info.AvailableFreeSpace;
				this._TotalSize = info.TotalSize;
				this._TotalFreeSpace = info.TotalFreeSpace;
			}else{
				this._DisplayName = info.Name;
				this._DriveType = System.IO.DriveType.Unknown;
				this._DriveFormat = "";
				this._AvailableFreeSpace = this._TotalSize = this._TotalFreeSpace = 0;
			}
		}

		#region Properties

		private string _DisplayName;
		public override string DisplayName{
			get{
				if(this._DisplayName == null){
					this.RefreshDriveInfo();
				}
				return this._DisplayName;
			}
		}

		private bool? _Exists;
		public override bool Exists {
			get {
				if(this._Exists == null){
					this.RefreshDriveInfo();
				}
				return this._Exists.Value;
			}
		}

		public bool IsReady{
			get{
				return this.Exists;
			}
		}

		private DriveType? _DriveType;
		public DriveType DriveType{
			get{
				if(this._DriveType == null){
					this.RefreshDriveInfo();
				}
				return this._DriveType.Value;
			}
		}

		private string _DriveFormat;
		public string DriveFormat{
			get{
				if(this._DriveFormat == null){
					this.RefreshDriveInfo();
				}
				return this._DriveFormat;
			}
		}

		private long? _AvailableFreeSpace;
		public long AvailableFreeSpace{
			get{
				if(this._AvailableFreeSpace == null){
					this.RefreshDriveInfo();
				}
				return this._AvailableFreeSpace.Value;
			}
		}

		private long? _TotalSize;
		public long TotalSize{
			get{
				if(this._TotalSize == null){
					this.RefreshDriveInfo();
				}
				return this._TotalSize.Value;
			}
		}

		private long? _TotalFreeSpace;
		public long TotalFreeSpace{
			get{
				if(this._TotalFreeSpace == null){
					this.RefreshDriveInfo();
				}
				return this._TotalFreeSpace.Value;
			}
		}

		#endregion

		#region IFileSystemDirectory Members

		public string ConcatFileSystemPath(string name) {
			return this.FileSystemPath + name;
		}

		public string FileSystemPath {get; private set;}

		public IFileSystemDirectory GetChildDirectory(string name) {
			if(name == null){
				throw new ArgumentNullException("name");
			}
			if(String.IsNullOrWhiteSpace(name)){
				throw new ArgumentException("name");
			}
			return new FileSystemDirectory(this, name);
		}

		public bool Contains(string name) {
			return Directory.Exists(name);
		}

		#endregion

		#region ISystemDirectory Members

		private RefreshableLazy<ISystemEntry[]> _Children;
		public override IEnumerable<ISystemEntry> Children {
			get {
				return this._Children.Value;
			}
		}

		public override ISystemDirectory GetChildDirectory(object id) {
			return this.GetChildDirectory(id as string);
		}

		public override bool Contains(object id) {
			return this.Contains(id.ToString());
		}

		#endregion
	}
}
