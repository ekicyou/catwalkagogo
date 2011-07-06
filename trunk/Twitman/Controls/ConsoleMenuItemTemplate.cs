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

		public virtual ConsoleRun GetText(ConsoleRun item, int offset, int width, int line){
			return item.WidthSubstring(offset, width);
		}
	}

	public class LambdaConsoleMenuItemTemplate : ConsoleMenuItemTemplate{
		public Func<object, ConsoleMenuItem> MenuItemGetter{get; private set;}
		public Func<ConsoleRun, int, int, int, ConsoleRun> TextGetter{get; private set;}

		public LambdaConsoleMenuItemTemplate(Func<object, ConsoleMenuItem> menuItemGetter, Func<ConsoleRun, int, int, int, ConsoleRun> textGetter){
			menuItemGetter.ThrowIfNull("menuItemGetter");
			textGetter.ThrowIfNull("textGetter");
			this.MenuItemGetter = menuItemGetter;
			this.TextGetter = textGetter;
		}

		public override ConsoleMenuItem GetMenuItem(object value){
			return this.MenuItemGetter(value);
		}

		public override ConsoleRun GetText(ConsoleRun item, int offset, int width, int line){
			return this.TextGetter(item, offset, width, line);
		}
	}
}
