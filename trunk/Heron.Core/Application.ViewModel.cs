using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using CatWalk;
using CatWalk.Windows.Input;
using CatWalk.IOSystem;
using CatWalk.Heron.IOSystem;
using CatWalk.Heron.ViewModel.IOSystem;
using CatWalk.Heron.ViewModel.Windows;
using CatWalk.Mvvm;
using CatWalk.Heron.ViewModel;

namespace CatWalk.Heron {
	public abstract partial class Application : ControlViewModel, IJobManagerSite {
		private IJobManager _JobManager;

		private void InitializeViewModel() {
			this._JobManager = new JobManager();
		}

		public MainWindowViewModel CreateMainWindow() {
			var vm = new MainWindowViewModel();
			var v = this._ViewFactory.Create(vm);
			this.Children.Add(vm);
			return vm;
		}

		#region IJobManagerSite Members

		public IJobManager JobManager {
			get {
				return this._JobManager;
			}
		}

		#endregion

		#region ArrangeWindows

		private DelegateCommand<ArrangeMode> _ArrangeWindowsCommand;

		public DelegateCommand<ArrangeMode> ArrangeWindowsCommand {
			get {
				return this._ArrangeWindowsCommand ?? (this._ArrangeWindowsCommand = new DelegateCommand<ArrangeMode>(this.ArrangeWindows));
			}
		}

		public void ArrangeWindows(ArrangeMode mode) {
			this.Messenger.Send(new WindowMessages.ArrangeWindowsMessage(this, mode));
		}

		#endregion
	}
}
