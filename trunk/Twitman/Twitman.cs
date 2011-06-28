using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatWalk;
using Twitman.Controls;

namespace Twitman {
	public static class Program{
		private static ConsoleMenu _Menu = new ConsoleMenu(new Int32Point(0, 1), new Int32Size(Screen.Size.Width, Screen.Size.Height - 2));

		static void Main(string[] arguments){
			Console.CancelKeyPress += delegate{
				ConsoleApplication.Exit();
			};
			var screen = new Screen();
			screen.Controls.Add(_Menu);
			_Menu.Items.Add(new ConsoleMenuItem("Test"));
			_Menu.Items.Add(new ConsoleMenuItem("Test2"));
			_Menu.Items.Add(new ConsoleMenuItem("Test3"));
			ConsoleApplication.Start(screen);
		}
	}
}
