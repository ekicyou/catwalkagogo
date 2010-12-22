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
using System.Linq;
using CatWalk;
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
		
		public void AddHistory(SearchCondition cond){
			cond.ThrowIfNull("cond");

			if(!cond.Path.IsNullOrEmpty()){
				this.DirectoryHistory = Enumerable.Concat(Seq.Make(cond.Path), this.DirectoryHistory.EmptyIfNull()).Distinct().ToArray();
			}
			if(cond.Pattern != null){
				this.SearchWordHistory = Enumerable.Concat(Seq.Make(cond.Pattern), this.SearchWordHistory.EmptyIfNull()).Distinct().ToArray();
			}
			if(cond.Mask != null){
				this.FileMaskHistory = Enumerable.Concat(Seq.Make(cond.Mask), this.FileMaskHistory.EmptyIfNull()).Distinct().ToArray();
			}
			if(cond.AdvancedGrepCondition.ExcludingMask != null){
				this.GrepExcludingMaskHistory = Enumerable.Concat(
					Seq.Make(cond.AdvancedGrepCondition.ExcludingMask),
						this.GrepExcludingMaskHistory.EmptyIfNull()).Distinct().ToArray();
			}
			if(cond.AdvancedFindCondition.ExcludingMask != null){
				this.FindExcludingMaskHistory = Enumerable.Concat(
					Seq.Make(cond.AdvancedFindCondition.ExcludingMask),
						this.FindExcludingMaskHistory.EmptyIfNull()).Distinct().ToArray();
			}

			this.IsIgnoreCase = cond.IsIgnoreCase;
			this.IsUseRegex = cond.IsUseRegex;
			this.FileSearchOption = cond.FileSearchOption;
			this.IsEnableAdvancedFindCondition = cond.IsEnableAdvancedFindCondition;
			this.IsEnableAdvancedGrepCondition = cond.IsEnableAdvancedGrepCondition;
			this.FindFileSizeRange = cond.AdvancedFindCondition.FileSizeRange;
			this.GrepFileSizeRange = cond.AdvancedGrepCondition.FileSizeRange;
		}

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
		[DefaultSettingValue("TopDirectoryOnly")]
		public SearchOption FileSearchOption{
			get{
				return (SearchOption)this["FileSearchOption"];
			}
			set{
				this["FileSearchOption"] = value;
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

		[ApplicationScopedSetting]
		[DefaultSettingValue("")]
		public DateTime LastCheckUpdatesDateTime{
			get{
				return (DateTime)this["LastCheckUpdatesDateTime"];
			}
			set{
				this["LastCheckUpdatesDateTime"] = value;
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
		[DefaultSettingValue("false")]
		public bool IsEnableAdvancedGrepCondition{
			get{
				return (bool)this["IsEnableAdvancedGrepCondition"];
			}
			set{
				this["IsEnableAdvancedGrepCondition"] = value;
			}
		}

		[UserScopedSetting]
		[DefaultSettingValue("false")]
		public bool IsEnableAdvancedFindCondition{
			get{
				return (bool)this["IsEnableAdvancedFindCondition"];
			}
			set{
				this["IsEnableAdvancedFindCondition"] = value;
			}
		}

		[UserScopedSetting]
		public string[] GrepExcludingMaskHistory{
			get{
				return (string[])this["GrepExcludingMaskHistory"];
			}
			set{
				this["GrepExcludingMaskHistory"] = value;
			}
		}

		[UserScopedSetting]
		public string[] FindExcludingMaskHistory{
			get{
				return (string[])this["FindExcludingMaskHistory"];
			}
			set{
				this["FindExcludingMaskHistory"] = value;
			}
		}

		[UserScopedSetting]
		public Range<long> GrepFileSizeRange{
			get{
				if(this["GrepFileSizeRange"] == null){
					this["GrepFileSizeRange"] = new Range<long>(0, 1024L * 1024L * 1024L * 1024L * 1024L);
				}
				return (Range<long>)this["GrepFileSizeRange"];
			}
			set{
				this["GrepFileSizeRange"] = value;
			}
		}

		[UserScopedSetting]
		public Range<long> FindFileSizeRange{
			get{
				if(this["FindFileSizeRange"] == null){
					this["FindFileSizeRange"] = new Range<long>(0, 1024L * 1024L * 1024L * 1024L * 1024L);
				}
				return (Range<long>)this["FindFileSizeRange"];
			}
			set{
				this["FindFileSizeRange"] = value;
			}
		}

		[UserScopedSetting]
		[DefaultSettingValue("")]
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