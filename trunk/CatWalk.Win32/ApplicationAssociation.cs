using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Principal;

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

		public static string GetCurrentDefault(string extOrProto, AssociationType assocType, AssociationLevel assocLevel){
			if(IsVistaOrHigher){
				string name;
				Marshal.ThrowExceptionForHR(Instance.QueryCurrentDefault(extOrProto, assocType, assocLevel, out name));
				return name;
			}else{
				throw new NotSupportedException();
			}
		}

		public static bool IsAppDefault(string appRegisterName, string extOrProto, AssociationType assocType, AssociationLevel assocLevel){
			if(IsVistaOrHigher){
				bool b;
				Marshal.ThrowExceptionForHR(Instance.QueryAppIsDefault(extOrProto, assocType, assocLevel, appRegisterName, out b));
				return b;
			}else{
				throw new NotSupportedException();
			}
		}

		public static bool IsAppDefaultAll(string appRegisterName, AssociationLevel assocLevel){
			if(IsVistaOrHigher){
				bool b;
				Marshal.ThrowExceptionForHR(Instance.QueryAppIsDefaultAll(assocLevel, appRegisterName, out b));
				return b;
			}else{
				throw new NotSupportedException();
			}
		}

		public static void SetAppAsDefault(string appRegisterName, string extOrProto, AssociationType assocType){
			if(IsVistaOrHigher){
				Marshal.ThrowExceptionForHR(Instance.SetAppAsDefault(appRegisterName, extOrProto, assocType));
			}else{
				throw new NotSupportedException();
			}
		}

		public static void SetAppAsDefaultAll(string appRegisterName){
			if(IsVistaOrHigher){
				Marshal.ThrowExceptionForHR(Instance.SetAppAsDefaultAll(appRegisterName));
			}else{
				throw new NotSupportedException();
			}
		}

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

		public static void RegisterApplication(string appName, string company, string appRegisterName, string verb){
			if(IsVistaOrHigher){
				// HKLM\Software\RegisteredApplications、HKLM\Software\({company}\){appName}、HKCR\appRegisterNameに登録
			}else{
				// HKCU\Software\Classes\{appRegisterName}に登録
			}
		}

		public static bool IsEnableUac{
			get{
				if(!IsVistaOrHigher){
					return false;
				}
				// Software\Microsoft\\Windows\CurrentVersion\Policies\System\EnableLUA > 0かどうか
				return true;
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
