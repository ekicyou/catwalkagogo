using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using CatWalk.Net.OAuth;

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

		[UserScopedSetting]
		public string BrowserPath{
			get{
				return this["BrowserPath"] as string;
			}
			set{
				this["BrowserPath"] = value;
			}
		}
	}

	[Serializable]
	public class AccountInfo{
		public string Name{get; set;}
		public AccessToken AccessToken{get; set;}

		public AccountInfo(){}
		public AccountInfo(string name, AccessToken accessToken){
			this.Name = name;
			this.AccessToken = accessToken;
		}
	}
}
