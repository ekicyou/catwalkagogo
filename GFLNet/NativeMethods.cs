using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace GflNet {
	internal static class NativeMethods{
		[DllImport("kernel32.dll")]
		public static extern IntPtr LoadLibrary(String lpFileName);
		[DllImport("kernel32.dll")]
		public static extern IntPtr GetProcAddress(IntPtr hModule, String lpProcName);
		[DllImport("kernel32.dll")]
		public static extern Boolean FreeLibrary(IntPtr hLibModule);
	}
}
