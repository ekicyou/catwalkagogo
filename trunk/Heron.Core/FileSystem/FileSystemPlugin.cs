using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.Heron.FileSystem {
	public class FileSystemPlugin : IPlugin{
		#region IPlugin Members

		public void Load(App app) {
			app.RegisterProvider(typeof(FileSystemProvider));
			app.RegisterEntryOperator(typeof(FileSystemEntryOperator));
		}

		public void Unload(App app) {
			app.UnregisterProvider(typeof(FileSystemProvider));
			app.UnregisterEntryOperator(typeof(FileSystemEntryOperator));
		}

		#endregion
	}
}
