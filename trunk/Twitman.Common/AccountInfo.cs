using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatWalk.Net.OAuth;
using System.Xml.Serialization;
using System.ComponentModel;
using CatWalk.Net.Twitter;

namespace Twitman {
	[Serializable]
	public class AccountInfo{
		public string Name{get; set;}
		public string ScreenName{get; set;}
		[XmlIgnore]
		public AccessToken AccessToken{get; private set;}
		public string AccessTokenString{
			get{
				return TypeDescriptor.GetConverter(typeof(AccessToken)).ConvertToString(this.AccessToken);
			}
			set{
				this.AccessToken = (AccessToken)TypeDescriptor.GetConverter(typeof(AccessToken)).ConvertFromString(value);
			}
		}

		public AccountInfo(){}
		public AccountInfo(Account account){
			if(!account.IsVerified){
				account.VerifyCredential();
			}
			this.Name = account.User.Name;
			this.ScreenName = account.User.ScreenName;
			this.AccessToken = account.AccessToken;
		}
	}
}
