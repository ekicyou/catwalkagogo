/*
	$Id: FileSystemFile.cs 217 2011-06-21 14:16:53Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CatWalk.IOSystem.FileSystem{
	public class FileSystemFileEntry : FileSystemEntry{
		public FileSystemFileEntry(ISystemDirectory parent, string name, string path) : base(parent, name, path){
		}

		public override bool Exists {
			get {
				return File.Exists(this.FileSystemPath);
			}
		}

		public long Size{
			get{
				var info = new FileInformation(this.FileSystemPath);
				return info.Length;
			}
		}

		public int LinkCount{
			get{
				var info = new FileInformation(this.FileSystemPath);
				return info.LinkCount;
			}
		}
	}
}
