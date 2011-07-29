using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Microsoft.Win32;

namespace CatWalk.Win32 {
	public static class ApplicationAssociation {
		private static WeakReference _Instance;
		private static IApplicationAssociationRegistration Instance{
			get{
				object instance;
				if(_Instance == null || (instance = _Instance.Target) == null){
					instance = (IApplicationAssociationRegistration)new ApplicationAssociationRegistration();
					_Instance = new WeakReference(instance);
				}
				return (IApplicationAssociationRegistration)instance;
			}
		}
		
		private static bool IsVistaOrHigher{
			get{
				return (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6);
			}
		}

		/// <summary>
		/// Supports Vista or higher.
		/// </summary>
		/// <param name="extOrProto"></param>
		/// <param name="assocType"></param>
		/// <param name="assocLevel"></param>
		/// <returns></returns>
		public static string GetCurrentDefault(string extOrProto, AssociationType assocType, AssociationLevel assocLevel){
			if(IsVistaOrHigher){
				string name;
				Marshal.ThrowExceptionForHR(Instance.QueryCurrentDefault(extOrProto, assocType, assocLevel, out name));
				return name;
			}else{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// Supports Vista or higher.
		/// </summary>
		/// <param name="appRegisterName"></param>
		/// <param name="extOrProto"></param>
		/// <param name="assocType"></param>
		/// <param name="assocLevel"></param>
		/// <returns></returns>
		public static bool IsAppDefault(string appRegisterName, string extOrProto, AssociationType assocType, AssociationLevel assocLevel){
			if(IsVistaOrHigher){
				bool b;
				Marshal.ThrowExceptionForHR(Instance.QueryAppIsDefault(extOrProto, assocType, assocLevel, appRegisterName, out b));
				return b;
			}else{
				// IQueryAssociationsを仕様
				throw new NotSupportedException();
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// Supports Vista or higher.
		/// </remarks>
		/// <param name="appRegisterName"></param>
		/// <param name="assocLevel"></param>
		/// <returns></returns>
		public static bool IsAppDefaultAll(string appRegisterName, AssociationLevel assocLevel){
			if(IsVistaOrHigher){
				bool b;
				Marshal.ThrowExceptionForHR(Instance.QueryAppIsDefaultAll(assocLevel, appRegisterName, out b));
				return b;
			}else{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// Set app as default
		/// </summary>
		/// <remarks>
		/// Supports Windows XP or higher (might be 95 or higher)
		/// </remarks>
		/// <param name="appRegisterName"></param>
		/// <param name="extOrProto">extension starts with dot or protocol name</param>
		/// <param name="assocType"></param>
		public static void SetAppAsDefault(string appRegisterName, string extOrProto, AssociationType assocType){
			if(IsVistaOrHigher){
				Marshal.ThrowExceptionForHR(Instance.SetAppAsDefault(appRegisterName, extOrProto, assocType));
			}else{
				using(var regExtKey = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\" + extOrProto)){
					regExtKey.SetValue("ProgID", appRegisterName);
				}
				//using(var regExtKey = Registry.CurrentUser.CreateSubKey(@"Software\Classes\" + extOrProto)){
				//	regExtKey.SetValue(null, appRegisterName);
				//}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// Supports Vista or higher.
		/// </remarks>
		/// <param name="appRegisterName"></param>
		public static void SetAppAsDefaultAll(string appRegisterName){
			if(IsVistaOrHigher){
				Marshal.ThrowExceptionForHR(Instance.SetAppAsDefaultAll(appRegisterName));
			}else{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// Supports Vista or higher.
		/// </remarks>
		public static void ClearUserAssociations(){
			if(IsVistaOrHigher){
				Marshal.ThrowExceptionForHR(Instance.ClearUserAssociations());
			}else{
				throw new NotSupportedException();
			}
		}

		public static void ShowAssociationRegistrationUI(string appRegisterName){
			if(IsVistaOrHigher){
				IApplicationAssociationRegistrationUI ui = null;
				try{
					ui = (IApplicationAssociationRegistrationUI)new ApplicationAssociationRegistrationUI();
					Marshal.ThrowExceptionForHR(ui.LaunchAdvancedAssociationUI(appRegisterName));
				}finally{
					if(ui != null){
						Marshal.ReleaseComObject(ui);
					}
				}
			}else{
				throw new NotSupportedException();
			}
		}

		private const string RegisteredApplicationsPath = @"Software\RegisteredApplications";
		public static void RegisterApplication(string appName, string company, string description){
			var companyAppName = (String.IsNullOrWhiteSpace(company)) ? appName : company + "\\" + appName;
			var softwareRegPath = @"Software\" + companyAppName;
			var capabilitiesPath = softwareRegPath + @"\Capabilities";

			// Capabilities
			using(var regCapKey = Registry.LocalMachine.CreateSubKey(capabilitiesPath)){
				regCapKey.SetValue("ApplicationName", appName);
				regCapKey.SetValue("ApplicationDescription", description);
			}

			// RegisteredApplications
			using(var regAppsKey = Registry.LocalMachine.CreateSubKey(RegisteredApplicationsPath)){
				regAppsKey.SetValue(appName, capabilitiesPath);
			}
		}

		public static void RegisterFileAssociation(string appName, string company, string appRegisterName, string extension, string description, string verb){
			var companyAppName = (String.IsNullOrWhiteSpace(company)) ? appName : company + "\\" + appName;
			var softwareRegPath = @"Software\" + companyAppName;
			var capabilitiesPath = softwareRegPath + @"\Capabilities";
			using(var regAsscKey = Registry.ClassesRoot.CreateSubKey(capabilitiesPath + @"\FileAssociations")){
				regAsscKey.SetValue(extension, appRegisterName);
			}
		}

		public static void RegisterUrlAssociation(string appName, string company, string appRegisterName, string protocol, string description, string verb){
			var companyAppName = (String.IsNullOrWhiteSpace(company)) ? appName : company + "\\" + appName;
			var softwareRegPath = @"Software\" + companyAppName;
			var capabilitiesPath = softwareRegPath + @"\Capabilities";
			using(var regAsscKey = Registry.LocalMachine.CreateSubKey(capabilitiesPath + @"\URLAssociations")){
				regAsscKey.SetValue(protocol, appRegisterName);
			}
		}

		public static void RegisterAppRegisterName(string appName, string appRegisterName, string description, string verb){
			using(var regAppKey = Registry.ClassesRoot.CreateSubKey(appRegisterName)){
				regAppKey.SetValue(null, description);
				using(var regOpenComKey = regAppKey.OpenSubKey(@"shell\open\command")){
					regOpenComKey.SetValue(null, verb);
				}
				using(var regAppComKey = regAppKey.OpenSubKey(@"shell\" + appName + @"\command")){
					regAppComKey.SetValue(null, verb);
				}
			}
		}

		public static void RegisterAppRegisterNameToCurrentUser(string appName, string appRegisterName, string description, string verb){
			using(var regAppKey = Registry.CurrentUser.CreateSubKey(@"Software\Classes\" + appRegisterName)){
				regAppKey.SetValue(null, description);
				using(var regOpenComKey = regAppKey.OpenSubKey(@"shell\open\command")){
					regOpenComKey.SetValue(null, verb);
				}
				using(var regAppComKey = regAppKey.OpenSubKey(@"shell\" + appName + @"\command")){
					regAppComKey.SetValue(null, verb);
				}
			}
		}

		public static bool IsEnableUac{
			get{
				if(!IsVistaOrHigher){
					return false;
				}
				using(var regSysKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\\Windows\CurrentVersion\Policies\System", false)){
					return (int)regSysKey.GetValue("EnableLUA", 0) > 0;
				}
			}
		}

		public static bool IsAdministrator{
			get{
				WindowsPrincipal windowsPricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
				return windowsPricipal.IsInRole(WindowsBuiltInRole.Administrator);
			}
		}
	}

	[ComImport]
	[Guid("4e530b0a-e611-4c77-a3ac-9031d022281b")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IApplicationAssociationRegistration{
		int QueryCurrentDefault(string pszQuery, AssociationType atQueryType, AssociationLevel alQueryLevel, out string ppszAssociation);
		int QueryAppIsDefault(string pszQuery, AssociationType atQueryType, AssociationLevel alQueryLevel, string pszAppRegistryName, out bool pfDefault);
		int QueryAppIsDefaultAll(AssociationLevel alQueryLevel, string pszAppRegistryName, out bool pfDefault);
		int SetAppAsDefault(string pszAppRegistryName, string pszSet, AssociationType atSetType);
		int SetAppAsDefaultAll(string pszAppRegistryName);
		int ClearUserAssociations();
	}

	// TODO : Wrapper
	[ComImport]
	[Guid("591209c7-767b-42b2-9fba-44ee4615f2c7")]
	public class ApplicationAssociationRegistration{
	}

	public enum AssociationLevel : int{
		MAchine = 0,
		Effective = 1,
		User = 2,
	};

	public enum AssociationType : int{
		FileExtension = 0,
		UrlProtocol = 1,
		StartMenuClient = 2,
		MimeType = 3,
	};

	[ComImport]
	[Guid("1f76a169-f994-40ac-8fc8-0959e8874710")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IApplicationAssociationRegistrationUI{
		int LaunchAdvancedAssociationUI(string appRegisterName);
	}

	[ComImport]
	[Guid("1968106d-f3b5-44cf-890e-116fcb9ecef1")]
	public class ApplicationAssociationRegistrationUI{
	}
}
