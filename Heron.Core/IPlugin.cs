using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.Heron {
	public interface IPlugin {
		void Load(App app);
		void Unload(App app);
	}
}
