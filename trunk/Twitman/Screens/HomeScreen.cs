using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatWalk;
using CatWalk.Net.Twitter;
using Twitman.Controls;
using Twitman.IOSystem;

namespace Twitman.Screens {
	public class HomeScreen : TwitmanScreen{
		private ConsoleLabel _MessageLabel;
		private ConsoleTextBox _PromptLabel;
		private ConsoleMenu _MainMenu;
		private HomeSystemDirectory _Directory;

		public HomeScreen(){
			this._MessageLabel = new ConsoleLabel(new Int32Point(0, 0), new Int32Size(Screen.Size.Width, 1));
			this._MainMenu = new ConsoleMenu(new Int32Point(0, 1), new Int32Size(Screen.Size.Width, Screen.Size.Height - 2));
			this._PromptLabel = new ConsoleTextBox(new Int32Point(0, Screen.Size.Height - 1), new Int32Size(Screen.Size.Width, 1));
			this._MessageLabel.Text = new ConsoleRun("[n]create new account", ConsoleColor.Cyan);
			this._PromptLabel.Text = "Ready";
			this._Directory = new HomeSystemDirectory(null, "");

			this.Controls.Add(this._MessageLabel);
			this.Controls.Add(this._MainMenu);
			this.Controls.Add(this._PromptLabel);
			this._MainMenu.Focus();
		}

		protected override void OnAttach(EventArgs e) {
			base.OnAttach(e);
			this._MainMenu.ItemsSource = this._Directory.Children;
		}

		protected override void OnKeyPress(ConsoleKeyEventArgs e) {
			if(e.Modifiers == 0){
				switch(e.Key){
					case ConsoleKey.N: this.CreateNewAccount(); break;
					case ConsoleKey.Q: ConsoleApplication.Exit(); break;
				}
			}
		}

		public void CreateNewAccount(){
			var loginScreen = new LoginScreen(Program.TwitterApi);
			ConsoleApplication.SetScreen(loginScreen, true);
			try{
				var reqToken = loginScreen.GetRequestToken();
				if(reqToken != null){
					var accToken = loginScreen.GetAccessToken(reqToken);
					if(accToken != null){
						var account = loginScreen.GetAccount(accToken);
						if(account != null){
							Program.Settings.AddAccount(account);
							Program.Settings.Save();
						}
					}
				}
			}finally{
				ConsoleApplication.RestoreScreen();
			}
		}
	}
}
