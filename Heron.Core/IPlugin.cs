using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.Heron {
	public interface IPlugin {
		void Load(App app);
		void Unload(App app);
		bool CanUnload(App app);
		PluginPriority Priority{get;}
	}

	public enum PluginPriority {
		Lowest = 0x01,
		Normal = 0x0a,
		Builtin = 0xff,
	}
}
