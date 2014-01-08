using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.Heron.ViewModel.Windows;

namespace CatWalk.Heron.Windows {
	public class Plugin : IPlugin{
		#region IPlugin Members

		public void Load(Application app) {
			app.Messenger.Register<WindowMessages.ArrangeWindowsMessage>(OnArrangeWindowsMessage, app);

			app.ViewFactory.Register<MainWindowViewModel>(new Func<MainWindowViewModel, MainWindow>(vm => {
				var window = new MainWindow();
				window.DataContext = vm;
				return window;
			}));
		}

		public void Unload(Application app) {
			throw new NotImplementedException();
		}

		public bool CanUnload(Application app) {
			return false;
		}

		private static void OnArrangeWindowsMessage(WindowMessages.ArrangeWindowsMessage m) {
			WindowUtility.ArrangeMainWindows(m.Mode);
		}

		public PluginPriority Priority {
			get {
				return PluginPriority.Builtin;
			}
		}

		#endregion
	}
}
