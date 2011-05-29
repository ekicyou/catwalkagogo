using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace CatWalk.IOSystem {
	public interface IRegistrySystemKey : ISystemDirectory{
		RegistryKey RegistryKey{get;}
		string RegistryPath{get;}
		string ConcatRegistryPath(string name);
		bool Contains(string name);
	}
}
