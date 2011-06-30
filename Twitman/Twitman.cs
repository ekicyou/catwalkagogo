using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatWalk;
using Twitman.Controls;
using CatWalk.Net.Twitter;

namespace Twitman {
	public static class Program{
		private static ConsoleMenu _Menu = new ConsoleMenu(new Int32Point(0, 1), new Int32Size(Screen.Size.Width, Screen.Size.Height - 2));

		static void Main(string[] arguments){
			Console.CancelKeyPress += delegate{
				ConsoleApplication.Exit();
			};
			var screen = new Screen();
			screen.KeyPress += (s, e) => {
				ConsoleApplication.Exit();
			};
			screen.Controls.Add(_Menu);
			_Menu.ItemTemplate = new LambdaConsoleMenuItemTemplate(
				status => new ConsoleMenuItem(status.ToString(), status),
				(item, offset, width) => {
					var status = (Status)item.Value;
					return new ConsoleRun(new ConsoleText[]{
						new ConsoleText(status.User.Name, ConsoleColor.Green),
						new ConsoleText(status.Text)
					});
				}
			);
			_Menu.ItemsSource = User.GetTimeline("twitterapi", 20, 0, 0, 0, false, false);
			ConsoleApplication.Start(screen);
		}
	}
}
