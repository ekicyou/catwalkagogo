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
		private ReadOnlyObservableCollection<PanelViewModel> _Panels;

		public MainWindowViewModel(){
			this._Panels = new ReadOnlyObservableCollection<PanelViewModel>(
				this.Children
					.CollectionChangedAsObservable()
					.OfType<PanelViewModel>()
					.ToReactiveCollection());

			this.AddPanelCommand = new ReactiveCommand<string>();
			this.AddPanelCommand.Subscribe(this.AddPanel);
		}

		public ReadOnlyObservableCollection<PanelViewModel> Panels {
			get {
				return this._Panels;
			}
		}

		#region AddPanel

		public ReactiveCommand<string> AddPanelCommand { get; private set; }

		public void AddPanel(string path) {
			SystemEntryViewModel vm;
			if(path.IsNullOrEmpty()) {
				vm = this.Application.Entry;
			} else {
				if(!this.Application.TryParseEntryPath(path, out vm)) {
					vm = this.Application.Entry;
				}
			}

			var panel = new PanelViewModel();
			panel.Content = new ListViewModel(vm);
			this.Children.Add(panel);
		}

		#endregion

		#region IJobManagerSite Members

		public IJobManager JobManager {
			get {
				return this._JobManager;
			}
		}

		#endregion
	}
}
