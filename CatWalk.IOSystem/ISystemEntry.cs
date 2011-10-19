﻿/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;

namespace CatWalk.IOSystem {
	public interface ISystemEntry{
		string Name{get;}
		string Path{get;}
		string DisplayName{get;}
		ISystemDirectory Parent{get;}
		string DisplayPath{get;}
		bool Exists{get;}
		bool IsExists(CancellationToken token);
		//void Refresh();
	}
}
