using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.ComponentModel;
using System.Xml.Serialization;
using CatWalk;
using CatWalk.Net.OAuth;
using CatWalk.Net.Twitter;

namespace Twitman {
	public static class ApplicationSettingsBaseExtension{
		public static void CopyTo(this ApplicationSettingsBase source, ApplicationSettingsBase dest){
			foreach(SettingsProperty prop in source.Properties){
				if(dest[prop.Name] != source[prop.Name]){
					dest[prop.Name] = source[prop.Name];
				}
			}
		}
	}
	
	public abstract class UpgradeOnceApplicationSettingsBase : ApplicationSettingsBase{
		public UpgradeOnceApplicationSettingsBase(){}
		public UpgradeOnceApplicationSettingsBase(string key) : base(key){}
		
		[UserScopedSetting]
		[DefaultSettingValue("false")]
		public virtual bool IsUpgradedSettings{
			get{
				return (bool)this["IsUpgradedSettings"];
			}
			set{
				this["IsUpgradedSettings"] = value;
			}
		}
		
		protected override void OnSettingsLoaded(object sender, SettingsLoadedEventArgs e){
			base.OnSettingsLoaded(sender, e);
		}
		
		public virtual void UpgradeOnce(){
			if(!this.IsUpgradedSettings){
				this.Upgrade();
				this.IsUpgradedSettings = true;
			}
		}
		
	}

	public class ApplicationSettings : UpgradeOnceApplicationSettingsBase{
		[UserScopedSetting]
		public AccountInfo[] Accounts{
			get{
				return this["Accounts"] as AccountInfo[];
			}
			set{
				this["Accounts"] = value;
			}
		}

		[UserScopedSetting]
		public string EditorPath{
			get{
				return this["EditorPath"] as string;
			}
			set{
				this["EditorPath"] = value;
			}
		}

		public void AddAccount(Account account){
			var info = new AccountInfo(account);
			this.Accounts = Seq.Make(info).Concat(this.Accounts.EmptyIfNull()).Distinct(info2 => info2.ScreenName).ToArray();
		}
	}

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

	[Serializable]
	public class AccountInfo2{
		public string Name{get; set;}
		public string ScreenName{get; set;}

		public AccountInfo2(){}
		public AccountInfo2(Account account){
			if(!account.IsVerified){
				account.VerifyCredential();
			}
			this.Name = account.User.Name;
			this.ScreenName = account.User.ScreenName;
		}
	}
}
