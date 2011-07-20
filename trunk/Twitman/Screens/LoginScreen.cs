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
		private CancellationTokenSource _CancellationTokenSource = new CancellationTokenSource();
		private ConsoleTextBox _MessageBox = new ConsoleTextBox(new Int32Point(0, 0), new Int32Size(Screen.Size.Width, Screen.Size.Height));
		public AuthorizedTwitterApi TwitterApi{get; private set;}

		public LoginScreen(AuthorizedTwitterApi twitterApi){
			if(twitterApi == null){
				throw new ArgumentNullException();
			}
			this.Controls.Add(this._MessageBox);
			this.TwitterApi = twitterApi;
		}

		protected override void OnCancelKeyPress(ConsoleCancelEventArgs e) {
			this._CancellationTokenSource.Cancel();
			e.Cancel = true;
		}

		protected override void OnDetach(EventArgs e) {
			if(!this._CancellationTokenSource.IsCancellationRequested){
				this._CancellationTokenSource.Cancel();
			}
			base.OnDetach(e);
		}

		public RequestToken GetRequestToken(){
			try{
				this._MessageBox.Text = new ConsoleText("Receiving a request token...", ConsoleColor.Green);
				return this.TwitterApi.ObtainUnauthorizedRequestToken(this._CancellationTokenSource.Token);
			}catch(WebException ex){
				this._MessageBox.Text += new ConsoleText("\n" + ex.Message, ConsoleColor.Red);
				Console.Read();
			}
			return null;
		}

		public AccessToken GetAccessToken(RequestToken token){
			var url = this.TwitterApi.BuildUserAuthorizationURL(token);
			this._MessageBox.Text += new ConsoleText("\nAccess bellow url:", ConsoleColor.Green);
			this._MessageBox.Text += new ConsoleText("\n" + url);

			var choose = this._MessageBox.Prompt(new ConsoleText("\nDo you want to open the verifying page in browser?(Y/N): ", ConsoleColor.Cyan));
			// Launch browser
			if(choose.Equals("Y", StringComparison.OrdinalIgnoreCase)){
				ThreadPool.QueueUserWorkItem(new WaitCallback(delegate{
					try{
						var info = new ProcessStartInfo(url){Verb = "open"};
						Process.Start(info);
					}catch{
					}
				}));
			}

			var y = this._MessageBox.DisplayText.Length;
			var veri = this._MessageBox.Prompt(new ConsoleText("\nInput verify number:\n", ConsoleColor.Green));
			try{
				this._MessageBox.Text += new ConsoleText("\nGetting an access token...", ConsoleColor.Green);
				var accessToken = this.TwitterApi.GetAccessToken(token, veri, this._CancellationTokenSource.Token);
				this._MessageBox.Text += new ConsoleText("\nOK");
				return accessToken;
			}catch(WebException ex){
				this._MessageBox.Text += new ConsoleText("\n" + ex.Message, ConsoleColor.Red);
				Console.Read();
			}
			return null;
		}

		public Account GetAccount(AccessToken token){
			var account = new Account(Program.TwitterApi, token);
			try{
				this._MessageBox.Text += new ConsoleText("\nVerifing your credential...", ConsoleColor.Green);
				account.VerifyCredential();
				this._MessageBox.Text += new ConsoleText("\nHello " + account.User.Name + " !");
				Console.Read();
				return account;
			}catch(WebException ex){
				this._MessageBox.Text += new ConsoleText("\n" + ex.Message, ConsoleColor.Red);
				Console.Read();
			}
			return null;
		}
	}
}
