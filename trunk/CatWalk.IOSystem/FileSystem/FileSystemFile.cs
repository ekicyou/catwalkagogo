using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CatWalk.IOSystem.FileSystem {
	public class FileSystemFileEntry : FileSystemEntry{
		public FileSystemFileEntry(ISystemDirectory parent, string name) : base(parent, name){
			this._Exists = new RefreshableLazy<bool>(() => File.Exists(this.FileSystemPath));
		}

		public override void Refresh() {
			this._Exists.Refresh();
			this.OnPropertyChanged("Exists");
			base.Refresh();
		}

		private RefreshableLazy<bool> _Exists;
		public override bool Exists {
			get {
				return this._Exists.Value;
			}
		}
	}
}
