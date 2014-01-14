using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive;
using Codeplex.Reactive;
using Codeplex.Reactive.Extensions;
using CatWalk.Collections;
using CatWalk.Heron.ViewModel.IOSystem;

namespace CatWalk.Heron.ViewModel.Windows {
	public class PanelCollectionViewModel : ViewViewModel{
		private WrappedObservableList<PanelViewModel> _Panels;

		public PanelCollectionViewModel() {
			this._Panels = new WrappedObservableList<PanelViewModel>(() => new SkipList<PanelViewModel>());

			this._Panels.NotifyToCollection(this.Children);

			this.AddPanelCommand = new ReactiveCommand<string>();
			this.AddPanelCommand.Subscribe(this.AddPanel);
		}

		public IReadOnlyObservableList<PanelViewModel> Panels {
			get {
				return this._Panels;
			}
		}

		#region AddPanel

		public ReactiveCommand<string> AddPanelCommand {
			get;
			private set;
		}

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
			this._Panels.Add(panel);
		}

		#endregion

	}
}
