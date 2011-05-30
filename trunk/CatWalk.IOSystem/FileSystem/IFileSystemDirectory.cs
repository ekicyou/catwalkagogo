using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk.IOSystem {
	public interface IFileSystemDirectory : ISystemDirectory{
		string ConcatFileSystemPath(string name);
		string FileSystemPath{get;}
	}
}
