/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CatWalk.IOSystem {
	public class FileSystemDriveDirectory : SystemDirectory{
		public FileSystemDriveDirectory(ISystemDirectory parent, string name) : base(parent, name){
		}

		private IEnumerable<ISystemEntry> GetChildren(){
			return DriveInfo.GetDrives().Select(drive => new FileSystemDrive(this, drive.Name, drive.Name[0]));
		}

		public override ISystemDirectory GetChildDirectory(string name) {
			if(String.IsNullOrEmpty(name)){
				throw new ArgumentException("name");
			}
			return new FileSystemDrive(this, name, name[0]);
		}

		public override IEnumerable<ISystemEntry> Children {
			get {
				return this.GetChildren();
			}
		}
	}
}
