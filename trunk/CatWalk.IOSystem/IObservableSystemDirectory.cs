using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace CatWalk.IOSystem {
	public interface IObservableSystemDirectory : ISystemDirectory{
		bool IsObserving{get; set;}
		ReadOnlyObservableCollection<SystemEntry> ObservableChildren{get;}
	}
}
