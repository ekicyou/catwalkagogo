using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive;
using Codeplex.Reactive;
using Codeplex.Reactive.Extensions;
using CatWalk.IOSystem;
using CatWalk.Heron.ViewModel.IOSystem;

namespace CatWalk.Heron.ViewModel.Windows {
	public class MainWindowViewModel : WindowViewModel, IJobManagerSite{
		private JobManager _JobManager = new JobManager();

		public MainWindowViewModel(){
		}

		#region IJobManagerSite Members

		public IJobManager JobManager {
			get {
				return this._JobManager;
			}
		}

		#endregion
	}
}
