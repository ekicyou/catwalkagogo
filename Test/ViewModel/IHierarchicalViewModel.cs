using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.ViewModel {
	public interface IHierarchicalViewModel<T> {
		T Parent { get; }
		IEnumerable<T> Children { get; }
	}
}
