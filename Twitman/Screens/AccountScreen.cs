using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Twitman.Controls;
using CatWalk;
using Twitman.IOSystem;
using System.Net;

namespace Twitman.Screens {
	public class AccountScreen : DirectoryScreen{
		public AccountSystemDirectory AccountDirectory{
			get{
				return (AccountSystemDirectory)this.Directory;
			}
		}

		public AccountScreen(AccountSystemDirectory dir) : base(dir){
			this.MessageLabel.Text = new ConsoleRun(new []{
				new ConsoleText(dir.DisplayPath, ConsoleColor.Cyan)
			});
		}

		protected override void OpenDirectory(CatWalk.IOSystem.ISystemDirectory dir) {
			var tlDir = dir as TimelineSystemDirectory;
			if(tlDir != null){
				ConsoleApplication.SetScreen(new TimelineScreen(tlDir), true);
			}else{
				base.OpenDirectory(dir);
			}
		}

		protected override void OnAttach(EventArgs e) {
			base.OnAttach(e);

			if(!this.AccountDirectory.Account.IsVerified){
				try{
					this.PromptBox.Text = new ConsoleRun("Verifying account credential...");
					this.AccountDirectory.Account.VerifyCredential();
				}catch(WebException ex){
					this.PromptBox.Text = new ConsoleRun(ex.Message, ConsoleColor.Red);
					ConsoleApplication.RestoreScreen();
				}
				this.PromptBox.Text = new ConsoleRun("Ready");
			}
		}
	}
}
