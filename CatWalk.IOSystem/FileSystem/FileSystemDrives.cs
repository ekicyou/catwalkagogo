using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CatWalk.IOSystem {
	public class FileSystemDrives : SystemDirectory{
		public FileSystemDrives(ISystemDirectory parent) : this(parent, "Drives"){
		}
		public FileSystemDrives(ISystemDirectory parent, object id) : base(parent, id){
			this._Children = new RefreshableLazy<ISystemEntry[]>(this.GetChildren);
		}

		public override void Refresh() {
			this._Children.Refresh();
			this.OnPropertyChanged("Children");
			base.Refresh();
		}

		private ISystemEntry[] GetChildren(){
			return DriveInfo.GetDrives().Select(drive => new FileSystemDrive(this, drive.Name[0])).ToArray();
		}

		private RefreshableLazy<ISystemEntry[]> _Children;
		public override IEnumerable<ISystemEntry> Children {
			get {
				return this._Children.Value;
			}
		}

		public override ISystemDirectory GetChildDirectory(object id) {
			throw new NotImplementedException();
		}

		public override bool Contains(object id) {
			throw new NotImplementedException();
		}
	}
}
