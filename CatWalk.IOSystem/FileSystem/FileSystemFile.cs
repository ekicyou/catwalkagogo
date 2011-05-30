using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CatWalk.IOSystem{
	public class FileSystemFileEntry : FileSystemEntry{
		public FileSystemFileEntry(ISystemDirectory parent, string name, string path) : base(parent, name, path){
		}


		public override bool Exists {
			get {
				return File.Exists(this.FileSystemPath);
			}
		}
	}
}
