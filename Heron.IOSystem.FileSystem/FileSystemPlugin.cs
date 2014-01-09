using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.Heron.Configuration;
using Codeplex.Reactive.Extensions;
using Codeplex.Reactive;

namespace CatWalk.Heron.FileSystem {
	public class FileSystemPlugin : Plugin{
		#region IPlugin Members

		protected override void OnLoaded(PluginEventArgs e) {
			var app = e.Application;
			app.RegisterSystemProvider(typeof(FileSystemProvider));
			app.RegisterEntryOperator(typeof(FileSystemEntryOperator));
			base.OnLoaded(e);
		}

		protected override void OnUnloaded(PluginEventArgs e) {
			var app = e.Application;
			app.UnregisterSystemProvider(typeof(FileSystemProvider));
			app.UnregisterEntryOperator(typeof(FileSystemEntryOperator));
		}

		public bool CanUnload(Application app) {
			return true;
		}

		public PluginPriority Priority {
			get {
				return PluginPriority.Normal;
			}
		}

		#endregion
	}
}
