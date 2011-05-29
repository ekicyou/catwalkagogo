using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CatWalk.IOSystem {
	public interface ISystemEntry : INotifyPropertyChanged{
		object Id{get;}
		string DisplayName{get;}
		ISystemDirectory Parent{get;}
		string DisplayPath{get;}
		bool Exists{get;}
		void Refresh();
	}
}
