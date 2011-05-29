using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CatWalk.IOSystem {
	public class FileSystemEntry : SystemEntry{
		public FileSystemEntry(ISystemDirectory parent, string name) : base(parent, name){
			var parentFS = parent as IFileSystemDirectory;
			if(parentFS != null){
				this.FileSystemPath = parentFS.ConcatFileSystemPath(name);
			}else{
				this.FileSystemPath = name;
			}
			this._DisplayName = new Lazy<string>(() => Path.GetFileName(this.FileSystemPath));
		}

		public override void Refresh() {
			this.RefreshInfo();
			this.OnPropertyChanged("Exists", "Size", "IsDirectory", "CreationTime", "LastAccessTime", "LastWriteTime", "Attributes");
			base.Refresh();
		}

		private void RefreshInfo(){
			try{
				var info = new FileInfo(this.FileSystemPath);
				this._Exists = info.Exists;
				if(this._Exists.Value){
					try{
						this._FileAttibutes = info.Attributes;
					}catch{
						this._FileAttibutes = FileAttributes.Normal;
					}
					if((this.FileAttibutes & System.IO.FileAttributes.Directory) == 0){
						try{
							this._Size = info.Length;
						}catch(IOException){
							this._Size = 0;
						}
					}
					try{
						this._CreationTime = info.CreationTime;
					}catch{
						this._CreationTime = DateTime.MinValue;
					}
					try{
						this._LastAccessTime = info.LastAccessTime;
					}catch{
						this._LastAccessTime = DateTime.MinValue;
					}
					try{
						this._LastWriteTime = info.LastWriteTime;
					}catch{
						this._LastWriteTime = DateTime.MinValue;
					}
				}
			}catch{
				this._Exists = false;
				this._Size = 0;
				this._CreationTime = this._LastAccessTime = this._LastWriteTime = DateTime.MinValue;
			}
		}

		private Lazy<string> _DisplayName;
		public override string DisplayName {
			get {
				return this._DisplayName.Value;
			}
		}

		public string FileSystemPath{get; private set;}

		private bool? _Exists;
		public override bool Exists {
			get {
				if(this._Exists == null){
					this.RefreshInfo();
				}
				return this._Exists.Value;
			}
		}

		private FileAttributes? _FileAttibutes;
		public FileAttributes FileAttibutes{
			get{
				if(this._FileAttibutes == null){
					this.RefreshInfo();
				}
				return this._FileAttibutes.Value;
			}
		}

		private long? _Size;
		public long Size{
			get{
				if(this._Size == null){
					this.RefreshInfo();
				}
				return this._Size.Value;
			}
		}

		private DateTime? _CreationTime;
		public DateTime CreationTime{
			get{
				if(this._CreationTime == null){
					this.RefreshInfo();
				}
				return this._CreationTime.Value;
			}
		}

		private DateTime? _LastWriteTime;
		public DateTime LastWriteTime{
			get{
				if(this._LastWriteTime == null){
					this.RefreshInfo();
				}
				return this._LastWriteTime.Value;
			}
		}

		private DateTime? _LastAccessTime;
		public DateTime LastAccessTime{
			get{
				if(this._LastAccessTime == null){
					this.RefreshInfo();
				}
				return this._LastAccessTime.Value;
			}
		}
	}
}
