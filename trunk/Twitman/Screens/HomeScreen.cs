using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatWalk;
using CatWalk.Net.Twitter;
using Twitman.Controls;
using Twitman.IOSystem;

namespace Twitman.Screens {
	public class HomeScreen : DirectoryScreen{

		public HomeScreen() : base(new HomeSystemDirectory(null, "")){
			this.MessageLabel.Text = new ConsoleRun("[n]create new account", ConsoleColor.Cyan);
			this.PromptBox.Text = new ConsoleRun("Ready!", ConsoleColor.Green);
		}

		protected override void OnAttach(EventArgs e) {
			base.OnAttach(e);
		}

		protected override void OnKeyPress(ConsoleKeyEventArgs e) {
			if(!e.IsHandled){
				if(e.Modifiers == 0){
					switch(e.Key){
						case ConsoleKey.N: this.CreateNewAccount(); e.IsHandled = true; break;
						case ConsoleKey.Q: ConsoleApplication.Exit(); e.IsHandled = true; break;
					}
				}
			}
			base.OnKeyPress(e);
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

		protected override void OpenDirectory(CatWalk.IOSystem.ISystemDirectory dir) {
			var accDir = dir as AccountSystemDirectory;
			if(accDir != null){
				ConsoleApplication.SetScreen(new AccountScreen(accDir), true);
			}else{
				base.OpenDirectory(dir);
			}
		}
	}
}
