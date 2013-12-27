using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.Mvvm;
using System.Reactive;
using System.Reactive.Linq;
using Codeplex.Reactive;
using Codeplex.Reactive.Extensions;

namespace CatWalk.Heron.ViewModel.Windows {
	public class ViewViewModel : ControlViewModel {
		private ResetLazy<Messenger> _Messenger;
		private ResetLazy<MainWindowViewModel> _MainWindow;
		private ResetLazy<ApplicationViewModel> _Application;

		public ViewViewModel() {
			this._Messenger = new ResetLazy<Messenger>(this.MessengerFactory);
			this._MainWindow = new ResetLazy<MainWindowViewModel>(() => this.Ancestors.OfType<MainWindowViewModel>().FirstOrDefault());
			this._Application = new ResetLazy<ApplicationViewModel>(() => this.Ancestors.OfType<ApplicationViewModel>().FirstOrDefault());
		}

		protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e) {
			if(e.PropertyName == "Parent") {
				this._Messenger.Reset();
				this._MainWindow.Reset();
				this._Application.Reset();
				this.OnPropertyChanged("Messenger", "MainWindow", "Application");
			}
			base.OnPropertyChanged(e);
		}

		public MainWindowViewModel MainWindow {
			get {
				return this._MainWindow.Value;
			}
		}

		public ApplicationViewModel Application {
			get {
				return this._Application.Value;
			}
		}

		public Messenger Messenger {
			get {
				return this._Messenger.Value;
			}
		}
		private Messenger MessengerFactory() {
			var app = this.Ancestors.OfType<ApplicationViewModel>().FirstOrDefault();
			if(app != null) {
				return app.Messenger;
			} else {
				return null;
			}
		}
	}
}
