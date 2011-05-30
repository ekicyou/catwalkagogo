using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CatWalk.IOSystem {
	using IO = System.IO;

	[ChildSystemEntryTypes(typeof(FileSystemDirectory), typeof(FileSystemEntry))]
	public class FileSystemDirectory : FileSystemEntry, ISystemDirectory{
		public FileSystemDirectory(ISystemDirectory parent, string name, string path) : base(parent, name, path){
		}

		public override bool Exists {
			get {
				return Directory.Exists(this.FileSystemPath);
			}
		}

		public override long Size {
			get {
				return 0;
			}
		}

		#region ISystemDirectory Members

		/// <summary>
		/// 
		/// </summary>
		/// <exception cref="ArgumentException">Ivalid path char</exception>
		/// <exception cref="DirectoryNotFoundException"></exception>
		/// <exception cref="IOException">Not directory</exception>
		/// <exception cref="PathTooLongException"></exception>
		/// <exception cref="SecurityException"></exception>
		/// <exception cref="UnauthorizedAccessException"></exception>
		/// <exception cref="NotSupportedException"></exception>
		public IEnumerable<ISystemEntry> Children {
			get{
				return Directory.EnumerateDirectories(this.FileSystemPath)
					.Select(file => new FileSystemDirectory(this, IO::Path.GetFileName(file), file))
					.Cast<ISystemEntry>()
					.Concat(
						Directory.EnumerateFiles(this.FileSystemPath)
						.Select(file => new FileSystemFileEntry(this, IO::Path.GetFileName(file), file)));
			}
		}

		public ISystemDirectory GetChildDirectory(string name) {
			var path = this.ConcatFileSystemPath(name);
			if(Directory.Exists(path)){
				return new FileSystemDirectory(this, name, path);
			}else{
				return null;
			}
		}

		public bool Contains(string name){
			var path = this.ConcatFileSystemPath(name);
			return Directory.Exists(path) || File.Exists(path);
		}

		public string ConcatDisplayPath(string name){
			return this.DisplayPath + IO::Path.DirectorySeparatorChar + name;
		}

		public string ConcatPath(string name){
			return this.Path + SystemDirectory.DirectorySeperatorChar + name;
		}

		#endregion

		#region IFileSystemDirectory Members

		public string ConcatFileSystemPath(string name){
			return this.FileSystemPath + IO::Path.DirectorySeparatorChar + name;
		}

		#endregion
	}
}
