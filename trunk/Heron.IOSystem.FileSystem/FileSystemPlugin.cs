using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.Heron.FileSystem {
	public class FileSystemPlugin : IPlugin{
		#region IPlugin Members

		public void Load(Application app) {
			app.RegisterSystemProvider(typeof(FileSystemProvider));
			app.RegisterEntryOperator(typeof(FileSystemEntryOperator));
		}

		public void Unload(Application app) {
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
