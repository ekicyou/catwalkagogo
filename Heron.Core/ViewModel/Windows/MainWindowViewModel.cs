using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.Heron.ViewModel.Windows {
	public class MainWindowViewModel : WindowViewModel, IJobManagerSite{
		public MainWindowViewModel(ApplicationViewModel app) : base(app) {

		}

		private JobManager _JobManager = new JobManager();

		#region IJobManagerSite Members

		public IJobManager JobManager {
			get {
				return this._JobManager;
			}
		}

		#endregion
	}
}
