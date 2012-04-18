/*
	$Id: IFileSystemDirectory.cs 217 2011-06-21 14:16:53Z cs6m7y@bma.biglobe.ne.jp $
*/
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
