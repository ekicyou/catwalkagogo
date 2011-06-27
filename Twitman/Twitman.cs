using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CatWalk;
using Twitman.Controls;
using CatWalk.Net.Twitter;
using System.Threading;

namespace Twitman {
	public static class Twitman {
		private class CommandLineOption{
			[CommandLineParemeterOrder(0)]
			public string Command{get; set;}
			[CommandLineOptionName("user")]
			public string User{get; set;}
		}

		static ObservableCollection<Status> Items;
		static ConsoleLabel Label;
		
		static void Main(string[] arguments){
			var options = new CommandLineOption();
			CommandLineParser.Parse(options, arguments, StringComparer.Ordinal);
			Console.Title = "Twitman";

			var screen = new Screen();
			Items = new ObservableCollection<Status>();
			var menu = new ConsoleMenu(new Int32Point(1, 1), new Int32Size(Screen.Size.Width - 1, Screen.Size.Height - 2));
			menu.ItemsSource = Items;
			menu.ItemToTextConverter = status => ((Status)status).User.ScreenName + ":" + ((Status)status).Text;
			/*menu.ItemTextTrimmer = (item, pos, length) => {
				var status = (Status)item.Value;
				var header = status.User.Name + ":";
				if(header.Length < length){
					return header + status.Text.Substring(pos, length - header.Length);
				}else{
					return header;
				}
			};*/
			screen.Controls.Add(menu);

			Label = new ConsoleLabel(new Int32Point(0, 0), new Int32Size(Screen.Size.Width, 1), "Ready!");
			screen.Controls.Add(Label);

			ThreadPool.QueueUserWorkItem(new WaitCallback(delegate{
				Label.Text = "Retrieving...";
				foreach(var status in User.GetTimeline("ufcpp", 32, 0, 0, 0, false, false)){
					Items.Add(status);
				}
				Label.Text = "Done!";
			}));

			ConsoleApplication.Start(screen);
		}
	}
}
