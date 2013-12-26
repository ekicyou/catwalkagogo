using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.Mvvm;

namespace CatWalk.Heron.ViewModel.Windows {
	public class ViewViewModel : ControlViewModel {
		public ViewViewModel(ControlViewModel parent)
			: base(parent) {
			this._Messenger = new Lazy<Messenger>(this.MessengerFactory);
		}

		public WindowViewModel Window {
			get {
				return Seq.Make(this).Concat(this.Ancestors).OfType<WindowViewModel>().FirstOrDefault();
			}
		}

		public MainWindowViewModel MainWindow {
			get {
				return Seq.Make(this).Concat(this.Ancestors).OfType<MainWindowViewModel>().FirstOrDefault();
			}
		}

		public ApplicationViewModel Application {
			get {
				return this.Ancestors.OfType<ApplicationViewModel>().FirstOrDefault();
			}
		}

		private Lazy<Messenger> _Messenger;
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
