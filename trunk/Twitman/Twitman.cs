using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatWalk;
using Twitman.Controls;
using CatWalk.Net.Twitter;
using Twitman.Screens;
using CatWalk.Net.OAuth;

namespace Twitman {
	public static partial class Program{
		public static AuthorizedTwitterApi TwitterApi{get; private set;}
		public static ApplicationSettings Settings{get; private set;}

		static Program(){
			TwitterApi = new AuthorizedTwitterApi(new Consumer(ConsumerKey, ConsumerSecretKey));
		}

		static void Main(string[] arguments){
			Console.CancelKeyPress += delegate{
				ConsoleApplication.Exit();
			};

			LoadSettings();
			ConsoleApplication.Exited += OnExited;
			var screen = new HomeScreen();
			ConsoleApplication.Start(screen);
		}

		public static void LoadSettings(){
			Settings = new ApplicationSettings();
			Settings.UpgradeOnce();
		}

		private static void OnExited(object sender, EventArgs e){
			Settings.Save();
		}
	}
}
