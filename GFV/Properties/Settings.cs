﻿/*
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
using GFV.ViewModel;
using GflNet;

namespace GFV.Properties{
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

	public class Settings : UpgradeOnceApplicationSettingsBase{
		public Settings(){}
		public Settings(string key) : base(key){}

		private static Settings _Default;
		public static Settings Default{
			get{
				if(_Default == null){
					_Default = new Settings();
				}
				return _Default;
			}
		}

		[UserScopedSetting]
		[DefaultSettingValue("None")]
		public ImageFittingMode ImageFittingMode{
			get{
				return (ImageFittingMode)this["ImageFittingMode"];
			}
			set{
				this["ImageFittingMode"] = value;
			}
		}

		[UserScopedSetting]
		[DefaultSettingValue("Fant")]
		public BitmapScalingMode BitmapScalingMode{
			get{
				return (BitmapScalingMode)this["BitmapScalingMode"];
			}
			set{
				this["BitmapScalingMode"] = value;
			}
		}

		[UserScopedSetting]
		public bool? IsShowMenubar{
			get{
				return (bool?)this["IsShowMenubar"];
			}
			set{
				this["IsShowMenubar"] = value;
			}
		}

		[UserScopedSetting]
		[SettingsSerializeAs(SettingsSerializeAs.Xml)]
		public InputBindingInfo[] ViewerWindowInputBindingInfos{
			get{
				return (InputBindingInfo[])this["ViewerWindowInputBindingInfos"];
			}
			set{
				this["ViewerWindowInputBindingInfos"] = value;
			}
		}

		[UserScopedSetting]
		[SettingsSerializeAs(SettingsSerializeAs.Xml)]
		public InputBindingInfo[] ViewerInputBindingInfos{
			get{
				return (InputBindingInfo[])this["ViewerInputBindingInfos"];
			}
			set{
				this["ViewerInputBindingInfos"] = value;
			}
		}

		[UserScopedSetting]
		public string[] RecentFiles{
			get{
				var files = (string[])this["RecentFiles"];
				return files;
			}
			set{
				this["RecentFiles"] = value;
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
		public bool IsCheckUpdatesOnStartup{
			get{
				return (bool)this["IsCheckUpdatesOnStartup"];
			}
			set{
				this["IsCheckUpdatesOnStartup"] = value;
			}
		}

		[UserScopedSetting]
		public string[] AdditionalFormatExtensions{
			get{
				return (string[])this["AdditionalFormatExtensions"];
			}
			set{
				this["AdditionalFormatExtensions"] = value;
			}
		}

		[UserScopedSetting]
		[DefaultSettingValue("Extension")]
		public FileInfoSortKey PrimarySortKey {
			get {
				return (FileInfoSortKey)this["PrimarySortKey"];
			}
			set {
				this["PrimarySortKey"] = value;
			}
		}
		
		[UserScopedSetting]
		[DefaultSettingValue("Ascending")]
		public SortOrder PrimarySortOrder {
			get {
				return (SortOrder)this["PrimarySortOrder"];
			}
			set {
				this["PrimarySortOrder"] = value;
			}
		}
		
		[UserScopedSetting]
		[DefaultSettingValue("FileName")]
		public FileInfoSortKey SecondarySortKey {
			get {
				return (FileInfoSortKey)this["SecondarySortKey"];
			}
			set {
				this["SecondarySortKey"] = value;
			}
		}
		
		[UserScopedSetting]
		[DefaultSettingValue("Ascending")]
		public SortOrder SecondarySortOrder {
			get {
				return (SortOrder)this["SecondarySortOrder"];
			}
			set {
				this["SecondarySortOrder"] = value;
			}
		}
		
		[UserScopedSetting]
		[DefaultSettingValue("false")]
		public bool IsHideFromTaskbar {
			get {
				return (bool)this["IsHideFromTaskbar"];
			}
			set {
				this["IsHideFromTaskbar"] = value;
			}
		}
		
		[UserScopedSetting]
		[DefaultSettingValue("false")]
		public bool IsHideFromAltTab {
			get {
				return (bool)this["IsHideFromAltTab"];
			}
			set {
				this["IsHideFromAltTab"] = value;
			}
		}
		
		[UserScopedSetting]
		[DefaultSettingValue("false")]
		public bool IsGlassBackground {
			get {
				return (bool)this["IsGlassBackground"];
			}
			set {
				this["IsGlassBackground"] = value;
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
			var state = this.WindowState;
			window.Loaded += delegate{
				window.WindowState = state;
			};
		}
		
		public virtual void StoreWindow(Window window){
			this.RestoreBounds = window.RestoreBounds;
			if(this.WindowState != WindowState.Minimized){
				this.WindowState = window.WindowState;
			}
		}
	}
}
