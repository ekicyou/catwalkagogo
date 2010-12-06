/*
	$Id$
*/
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using CatWalk.Windows;

namespace Nekome{
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
		
		[NoSettingsVersionUpgrade]
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
		
		public void UpgradeOnce(){
			if(!this.IsUpgradedSettings){
				this.Upgrade();
				this.IsUpgradedSettings = true;
			}
		}
	}
	
	public class ApplicationSettings : UpgradeOnceApplicationSettingsBase{
		public ApplicationSettings(){}
		public ApplicationSettings(string key) : base(key){}
		
		[UserScopedSetting]
		public ExternalTool[] FindTools{
			get{
				return (ExternalTool[])this["FindTools"];
			}
			set{
				this["FindTools"] = value;
			}
		}
		
		[UserScopedSetting]
		public ExternalTool[] GrepTools{
			get{
				return (ExternalTool[])this["GrepTools"];
			}
			set{
				this["GrepTools"] = value;
			}
		}
		
		[UserScopedSetting]
		public SearchOption SearchOption{
			get{
				return (this["SearchOption"] != null) ? (SearchOption)this["SearchOption"] : SearchOption.TopDirectoryOnly;
			}
			set{
				this["SearchOption"] = value;
			}
		}
		
		[UserScopedSetting]
		[DefaultSettingValue("false")]
		public bool IsIgnoreCase{
			get{
				return (bool)this["IsIgnoreCase"];
			}
			set{
				this["IsIgnoreCase"] = value;
			}
		}
		
		[UserScopedSetting]
		[DefaultSettingValue("false")]
		public bool IsUseRegex{
			get{
				return (bool)this["IsUseRegex"];
			}
			set{
				this["IsUseRegex"] = value;
			}
		}

		[UserScopedSetting]
		public double GrepResultFileColumnWidth{
			get{
				return (double)this["GetResultFileColumnWidth"];
			}
			set{
				this["GetResultFileColumnWidth"] = (double)value;
			}
		}

		[UserScopedSetting]
		public string[] SearchWordHistory{
			get{
				return (string[])this["SearchWordHistory"];
			}
			set{
				this["SearchWordHistory"] = value;
			}
		}

		[UserScopedSetting]
		public string[] DirectoryHistory{
			get{
				return (string[])this["DirectoryHistory"];
			}
			set{
				this["DirectoryHistory"] = value;
			}
		}

		[UserScopedSetting]
		public string[] FileMaskHistory{
			get{
				return (string[])this["FileMaskHistory"];
			}
			set{
				this["FileMaskHistory"] = value;
			}
		}

		[UserScopedSetting]
		[DefaultSettingValue("true")]
		public bool IsCheckUpdatesOnStartUp{
			get{
				return (bool)this["IsCheckUpdatesOnStartUp"];
			}
			set{
				this["IsCheckUpdatesOnStartUp"] = value;
			}
		}

		[UserScopedSetting]
		[DefaultSettingValue("*.exe;*.dll;*.bmp;*.jpg;*.png;*.gif;*.avi;*.wmv;*.mpg;*.mp3;*.wav;*.ogg")]
		public string ExcludingMask{
			get{
				return (string)this["ExcludingMask"];
			}
			set{
				this["ExcludingMask"] = value;
			}
		}

		[UserScopedSetting]
		public string[] ExcludingMaskHistory{
			get{
				return (string[])this["ExcludingMaskHistory"];
			}
			set{
				this["ExcludingMaskHistory"] = value;
			}
		}

		[UserScopedSetting]
		[DefaultSettingValue("Grep")]
		public ExcludingTargets ExcludingTargets{
			get{
				return (ExcludingTargets)this["ExcludingTargets"];
			}
			set{
				this["ExcludingTargets"] = value;
			}
		}

		[UserScopedSetting]
		public Font GrepPreviewFont{
			get{
				return (Font)this["GrepPreviewFont"];
			}
			set{
				this["GrepPreviewFont"] = value;
			}
		}
	}
	
	public class WindowSettings : UpgradeOnceApplicationSettingsBase{
		public WindowSettings(){}
		public WindowSettings(string key) : base(key){}
		
		[UserScopedSetting]
		[DefaultSettingValue("NaN, NaN, 640, 480")]
		public Rect RestoreBounds{
			get{
				return (Rect)this["RestoreBounds"];
			}
			set{
				this["RestoreBounds"] = value;
			}
		}
		
		private Rect VerifyRect(Rect rect){
			Double width = rect.Width;
			Double height = rect.Height;
			Double x = rect.X;
			Double y = rect.Y;
			if(!ValidSizeValue(rect.Width)){
				width = 640;
			}
			if(!ValidSizeValue(rect.Height)){
				height = 480;
			}
			if(!ValidPositionValue(rect.X)){
				x = Double.NaN;
			}
			if(!ValidPositionValue(rect.Y)){
				y = Double.NaN;
			}
			return new Rect(x, y, width, height);
		}
		
		[UserScopedSetting]
		[DefaultSettingValue("Normal")]
		public WindowState WindowState{
			get{
				return (WindowState)this["WindowState"];
			}
			set{
				this["WindowState"] = value;
			}
		}
		
		private static bool ValidSizeValue(double v){
			return ValidPositionValue(v);
		}
		
		private static bool ValidPositionValue(double v){
			return ((Double.NegativeInfinity < v) && (v < Double.PositiveInfinity));
		}
		
		public virtual void RestoreWindow(Window window){
			Rect safeRect = VerifyRect(this.RestoreBounds);
			window.Left = safeRect.Left;
			window.Top = safeRect.Top;
			window.Width = safeRect.Width;
			window.Height = safeRect.Height;
			window.WindowState = this.WindowState;
		}
		
		public virtual void SaveWindow(Window window){
			this.RestoreBounds = window.RestoreBounds;
			if(this.WindowState != WindowState.Minimized){
				this.WindowState = window.WindowState;
			}
		}
	}
}