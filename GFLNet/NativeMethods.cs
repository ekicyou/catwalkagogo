using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace GflNet {
	internal static class NativeMethods{
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr LoadLibrary(String lpFileName);
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr GetProcAddress(IntPtr hModule, String lpProcName);
		[DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = false)]
		public static extern Boolean FreeLibrary(IntPtr hLibModule);
		[DllImport("kernel32")]
		public static extern bool ReadFile(IntPtr handle, IntPtr buffer, uint nbytestoread, out uint lpnbytestoread, IntPtr overlap);
		[DllImport("kernel32")]
		public static extern uint SetFilePointer(IntPtr handle, int offset, IntPtr offsetHigh, SeekOrigin origin);
	}
}
