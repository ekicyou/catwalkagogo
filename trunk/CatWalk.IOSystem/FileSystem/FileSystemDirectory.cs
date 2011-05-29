using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CatWalk.IOSystem {
	[ChildSystemEntryTypes(typeof(FileSystemDirectory), typeof(FileSystemEntry))]
	public class FileSystemDirectory : FileSystemEntry, IFileSystemDirectory{
		public FileSystemDirectory(ISystemDirectory parent, string name) : base(parent, name){
			this._Exists = new RefreshableLazy<bool>(() => Directory.Exists(this.FileSystemPath));
			this._Children = new RefreshableLazy<ISystemEntry[]>(() =>
				Directory.EnumerateDirectories(this.FileSystemPath)
					.Select(file => new FileSystemDirectory(this, Path.GetFileName(file)))
					.Concat(
						Directory.EnumerateFiles(this.FileSystemPath)
							.Select(file => new FileSystemEntry(this, Path.GetFileName(file)))).ToArray());
		}

		public override void Refresh() {
			this._Exists.Refresh();
			this._Children.Refresh();
			this.OnPropertyChanged("Children");
			base.Refresh();
		}

		private RefreshableLazy<bool> _Exists;
		public override bool Exists {
			get {
				return this._Exists.Value;
			}
		}

		#region ISystemDirectory Members

		private RefreshableLazy<ISystemEntry[]> _Children;
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
				return this._Children.Value;
			}
		}

		public ISystemDirectory GetChildDirectory(object id) {
			return this.GetChildDirectory(id as string);
		}

		public bool Contains(object id){
			return this.Contains(id.ToString());
		}

		public string ConcatDisplayPath(string name){
			return this.DisplayPath + Path.DirectorySeparatorChar + name;
		}

		#endregion

		#region IFileSystemDirectory Members

		public string ConcatFileSystemPath(string name){
			return this.FileSystemPath + Path.DirectorySeparatorChar + name;
		}

		public bool Contains(string name){
			return Directory.Exists(this.ConcatFileSystemPath(name));
		}

		public IFileSystemDirectory GetChildDirectory(string name) {
			if(name == null){
				throw new ArgumentNullException("name");
			}
			if(String.IsNullOrWhiteSpace(name)){
				throw new ArgumentException("name");
			}
			return new FileSystemDirectory(this, name);
		}

		#endregion
	}
}
