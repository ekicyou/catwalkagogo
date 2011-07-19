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
			var text = item.DisplayText[line].WidthSubstring(offset, width - 1);
			if(line == 0){
				if(item.IsFocused){
					return new ConsoleRun(Seq.Make(new ConsoleText(" ", null, ConsoleColor.White)).Concat(text.Texts), text.ForegroundColor, text.BackgroundColor);
				}else if(item.IsSelected){
					return new ConsoleRun(Seq.Make(new ConsoleText("+", null, ConsoleColor.Yellow)).Concat(text.Texts), text.ForegroundColor, text.BackgroundColor);
				}
			}
			return new ConsoleRun(Seq.Make(new ConsoleText(" ")).Concat(text.Texts), text.ForegroundColor, text.BackgroundColor);
		}
	}
}
