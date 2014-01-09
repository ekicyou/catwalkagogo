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
		private ResetLazy<Application> _Application;

		public ViewViewModel() {
			this._Messenger = new ResetLazy<Messenger>(this.MessengerFactory);
			this._MainWindow = new ResetLazy<MainWindowViewModel>(() => 
				this.Ancestors
					.OfType<ViewViewModel>()
					.Select(vvm => vvm.MainWindow)
					.Concat(
						this.Ancestors
							.OfType<MainWindowViewModel>())
					.FirstOrDefault());
			this._Application = new ResetLazy<Application>(() => 
				this.Ancestors
					.OfType<ViewViewModel>()
					.Select(vvm => vvm.Application)
					.Concat(
						this.Ancestors
							.OfType<Application>())
					.FirstOrDefault());
		}

		protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e) {
			if(e.PropertyName == "Ancestors") {
				this._Application.Reset();
				this._Messenger.Reset();
				this._MainWindow.Reset();
				this.OnPropertyChanged("Messenger", "MainWindow", "Application");
			}
			base.OnPropertyChanged(e);
		}

		public MainWindowViewModel MainWindow {
			get {
				return this._MainWindow.Value;
			}
		}

		public Application Application {
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
			var app = this.Application;
			if(app != null) {
				return app.Messenger;
			} else {
				return null;
			}
		}
	}
}
