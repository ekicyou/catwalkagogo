/*
	$Id: FileSystemEntry.cs 217 2011-06-21 14:16:53Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace CatWalk.IOSystem.FileSystem {
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
		/*
		public FileSecurity AccessControl{
			get{
				return File.GetAccessControl(this.FileSystemPath);
			}
		}

		public NTAccount Owner{
			get{
				return this.AccessControl.GetOwner(typeof(NTAccount)) as NTAccount;
			}
		}

		public IEnumerable<FileSystemAccessRule> AccessRules{
			get{
				var current = WindowsIdentity.GetCurrent();
				var fs = this.AccessControl;
				var rules = fs.GetAccessRules(true, true, typeof(SecurityIdentifier));
				foreach(FileSystemAccessRule rule in rules){
					if(rule.IdentityReference == current.User){
						yield return rule;
					}
					foreach(IdentityReference group in current.Groups){
						if(rule.IdentityReference == group){
							yield return rule;
							break;
						}
					}
				}
			}
		}
		
		public string FileName{
			get{
				return IO.Path.GetFileName(this.FileSystemPath);
			}
		}

		public string FileNameWithoutExtension{
			get{
				return IO.Path.GetFileNameWithoutExtension(this.FileSystemPath);
			}
		}

		public string BaseName{
			get{
				return this.FileName.Split('.').First();
			}
		}

		public string DirectoryName{
			get{
				return IO.Path.GetDirectoryName(this.FileSystemPath);
			}
		}

		public string Extension{
			get{
				return IO.Path.GetExtension(this.FileSystemPath);
			}
		}

		public IEnumerable<string> Extensions{
			get{
				return this.FileName.Split('.').Skip(1);
			}
		}
		*/
	}
}
