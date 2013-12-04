using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk.IOSystem.FileSystem{
	public interface IFileSystemEntry : ISystemEntry, IEquatable<IFileSystemEntry> {
		FilePath FileSystemPath { get; }
	}
}
