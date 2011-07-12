using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatWalk;

namespace Twitman.Controls {
	public class ConsoleMenuItemTemplate {
		public virtual ConsoleMenuItem GetMenuItem(object value){
			return new ConsoleMenuItem(value.ToString());
		}

		public virtual ConsoleRun[] GetDisplayText(ConsoleMenuItem item, Int32Size size){
			return item.Text.Split("\n").ToArray();
		}

		public virtual ConsoleRun GetText(ConsoleMenuItem item, int line, int offset, int width){
			return item.DisplayText[line].WidthSubstring(offset, width);
		}
	}
}
