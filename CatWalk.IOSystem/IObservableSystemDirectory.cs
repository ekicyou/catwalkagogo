/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading;

namespace CatWalk.IOSystem {
	public interface IObservableSystemDirectory : ISystemDirectory{
		bool IsObserving{get; set;}
		void Refresh();
		void Refresh(CancellationToken token);
	}
}
