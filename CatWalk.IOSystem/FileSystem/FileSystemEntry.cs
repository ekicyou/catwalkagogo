using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CatWalk.IOSystem {
	using IO = System.IO;
	public class FileSystemEntry : SystemEntry{
		public FileSystemEntry(ISystemDirectory parent, string name, string path) : base(parent, name){
			this.FileSystemPath = IO::Path.GetFullPath(path);
			this._DisplayName = new Lazy<string>(() => IO::Path.GetFileName(this.FileSystemPath));
		}

		private Lazy<string> _DisplayName;
		public override string DisplayName {
			get {
				return this._DisplayName.Value;
			}
		}

		public string FileSystemPath{get; private set;}

		public override bool Exists {
			get {
				try{
					var info = new FileInfo(this.FileSystemPath);
					return info.Exists;
				}catch{
					return false;
				}
			}
		}

		public FileAttributes FileAttibutes{
			get{
				var info = new FileInfo(this.FileSystemPath);
				return info.Attributes;
			}
		}

		public virtual long Size{
			get{
				var info = new FileInfo(this.FileSystemPath);
				return info.Length;
			}
		}

		public DateTime CreationTime{
			get{
				var info = new FileInfo(this.FileSystemPath);
				return info.CreationTime;
			}
		}

		public DateTime LastWriteTime{
			get{
				var info = new FileInfo(this.FileSystemPath);
				return info.LastWriteTime;
			}
		}

		public DateTime LastAccessTime{
			get{
				var info = new FileInfo(this.FileSystemPath);
				return info.LastAccessTime;
			}
		}
	}
}
