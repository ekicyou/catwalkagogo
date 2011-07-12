using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatWalk.IOSystem;
using CatWalk.Net.Twitter;

namespace Twitman.IOSystem {
	public class AccountSystemDirectory : SystemDirectory{
		public Account Account{get; private set;}
		public AccountInfo AccountInfo{get; private set;}

		public AccountSystemDirectory(ISystemDirectory parent, AccountInfo account) : base(parent, account.ScreenName){
			this.Account = new Account(Program.TwitterApi, account.AccessToken);
			this._DisplayName = account.Name;
			this.AccountInfo = account;
		}

		public void VerifyCredential(){
			this.Account.VerifyCredential();
			this.AccountInfo.Name = this.Account.User.Name;
		}

		private string _DisplayName;
		public override string DisplayName{
			get{
				return this._DisplayName;
			}
		}

		public override IEnumerable<ISystemEntry> Children {
			get {
				return new[]{new TimelineSystemDirectory(this, "Home Timeline", this.Account, token => this.Account.GetHomeTimeline(20, 0, 0, 0, false, false, token))};
			}
		}
	}
}
