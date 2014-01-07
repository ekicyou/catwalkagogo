using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CatWalk.Heron.Configuration;
using CatWalk.Heron.Scripting;
using CatWalk.Heron.ViewModel;
using CatWalk.Heron.ViewModel.Windows;
using CatWalk.Windows.Threading;

namespace CatWalk.Heron {
	public partial class Program : System.Windows.Application {
		protected override void OnStartup(StartupEventArgs e) {
			var app = new WindowsApplication(this);
			app.Run(e.Args);
		}

		private class WindowsApplication : Application {
			private System.Windows.Application _App;

			public WindowsApplication(System.Windows.Application app) {
				app.ThrowIfNull("app");
				this._App = app;

				this._App.Exit += _App_Exit;
			}

			private void _App_Exit(object sender, System.Windows.ExitEventArgs e) {
				this.OnExit(new ApplicationExitEventArgs(e.ApplicationExitCode));
			}

			protected override void OnFirstStartUp(ApplicationStartUpEventArgs e) {
				this.SynchronizeInvoke = new DispatcherSynchronizeInvoke(this._App.Dispatcher);
				base.OnFirstStartUp(e);
			}
		}
	}
}
