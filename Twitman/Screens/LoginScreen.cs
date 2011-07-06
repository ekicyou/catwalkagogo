using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using CatWalk;
using CatWalk.Net.OAuth;
using CatWalk.Net.Twitter;
using Twitman.Controls;
using System.Threading;
using System.Diagnostics;

namespace Twitman.Screens {
	public class LoginScreen : Screen{
		private ConsoleLabel _MessageLabel = new ConsoleLabel(new Int32Point(0, 0), new Int32Size(Screen.Size.Width, Screen.Size.Height));
		public AuthorizedTwitterApi TwitterApi{get; private set;}

		public LoginScreen(AuthorizedTwitterApi twitterApi){
			if(twitterApi == null){
				throw new ArgumentNullException();
			}
			this.Controls.Add(this._MessageLabel);
			this.TwitterApi = twitterApi;
		}

		public RequestToken GetRequestToken(){
			try{
				this._MessageLabel.Text = new ConsoleText("Recieving a request token...", ConsoleColor.Green);
				return this.TwitterApi.ObtainUnauthorizedRequestToken();
			}catch(WebException ex){
				this._MessageLabel.Text += new ConsoleText("\n" + ex.Message, ConsoleColor.Red);
				Console.Read();
			}
			return null;
		}

		public AccessToken GetAccessToken(RequestToken token){
			var url = this.TwitterApi.BuildUserAuthorizationURL(token);
			this._MessageLabel.Text += new ConsoleText("\nAccess bellow url:", ConsoleColor.Green);
			this._MessageLabel.Text += new ConsoleText("\n" + url);
			try{
				var info = new ProcessStartInfo(url){Verb = "open"};
				Process.Start(info);
			}catch{
			}

			var y = this._MessageLabel.DisplayText.Length;
			var textBox = new ConsoleTextBox(new Int32Point(0, y), new Int32Size(Screen.Size.Width, Screen.Size.Height - y));
			textBox.Text = new ConsoleText("Input verify number:", ConsoleColor.Green);
			this.Controls.Add(textBox);
			var veri = textBox.Prompt();
			this.Controls.Remove(textBox);
			try{
				this._MessageLabel.Text += new ConsoleText("\nGetting an access token...", ConsoleColor.Green);
				var accessToken = this.TwitterApi.GetAccessToken(token, veri);
				this._MessageLabel.Text += new ConsoleText("\nOK");
				return accessToken;
			}catch(WebException ex){
				this._MessageLabel.Text += new ConsoleText("\n" + ex.Message, ConsoleColor.Red);
				Console.Read();
			}
			return null;
		}

		public Account GetAccount(AccessToken token){
			var account = new Account(Program.TwitterApi, token);
			try{
				this._MessageLabel.Text += new ConsoleText("\nVerifing your credential...", ConsoleColor.Green);
				account.VerifyCredential();
				this._MessageLabel.Text += new ConsoleText("\nHello " + account.User.Name + " !");
				Console.Read();
				return account;
			}catch(WebException ex){
				this._MessageLabel.Text += new ConsoleText("\n" + ex.Message, ConsoleColor.Red);
				Console.Read();
			}
			return null;
		}
	}
}
