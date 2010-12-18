/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk{
	public static class Platform{
		static Platform(){
			if(Environment.OSVersion.Platform == PlatformID.Win32NT){
				Version ver = Environment.OSVersion.Version;
				isWindows7OrHigher = (ver.Major >= 6) && (ver.Minor >= 1);
				isWindowsVistaOrHigher = (ver.Major >= 6);
				isWindowsXPOrHigher = (ver.Major > 5) || (ver.Major >= 5 && ver.Minor >= 1);
			}
		}

		private static bool isWindows7OrHigher = false;
		public static bool IsWindows7OrHigher{
			get{
				return isWindows7OrHigher;
			}
		}

		private static bool isWindowsXPOrHigher = false;
		public static bool IsWindowsXPOrHigher{
			get{
				return isWindowsXPOrHigher;
			}
		}

		private static bool isWindowsVistaOrHigher = false;
		public static bool IsWindowsVistaOrHigher{
			get{
				return isWindowsVistaOrHigher;
			}
		}
	}
}
