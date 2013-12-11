using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.ViewModel {
	public interface IJobManagerSite {
		IJobManager JobManager { get; }
	}
}
