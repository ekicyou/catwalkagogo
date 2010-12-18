/*
	$Id$
*/
using System;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CatWalk {
	public static class Extensions{
		#region Exception
		public static void ThrowIfNull(this object obj) {
			if(obj == null) {
				throw new ArgumentNullException();
			}
		}

		public static void ThrowIfNull(this object obj, string message) {
			if(obj == null) {
				throw new ArgumentNullException(message);
			}
		}

		public static void ThrowIfOutOfRange(this int n, int min, int max) {
			if((n < min) || (max < n)) {
				throw new ArgumentOutOfRangeException();
			}
		}

		public static void ThrowIfOutOfRange(this int n, int min, int max, string message) {
			if((n < min) || (max < n)) {
				throw new ArgumentOutOfRangeException(message);
			}
		}

		public static void ThrowIfOutOfRange(this int n, int min) {
			if(n < min) {
				throw new ArgumentOutOfRangeException();
			}
		}

		public static void ThrowIfOutOfRange(this int n, int min, string message) {
			if(n < min) {
				throw new ArgumentOutOfRangeException(message);
			}
		}

		#endregion

		#region object

		public static bool IsNull(this object obj) {
			return (obj == null);
		}

		#endregion

		#region Assembly

		public static string GetInformationalVersion(this Assembly asm) {
			var ver = asm.GetCustomAttributes(true).OfType<AssemblyInformationalVersionAttribute>().First();
			return (ver != null) ? ver.InformationalVersion : null;
		}

		public static Version GetVersion(this Assembly asm) {
			return asm.GetName().Version;
		}

		public static string GetCopyright(this Assembly asm) {
			var copy = asm.GetCustomAttributes(true).OfType<AssemblyCopyrightAttribute>().First();
			return (copy != null) ? copy.Copyright : null;
		}

		#endregion

		#region string

		public static bool IsNullOrEmpty(this string str) {
			return String.IsNullOrEmpty(str);
		}

		public static bool IsMatchWildCard(this string str, string mask) {
			return PathMatchSpec(str, mask);
		}

		[DllImport("shlwapi.dll", EntryPoint = "PathMatchSpec", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool PathMatchSpec([MarshalAs(UnmanagedType.LPTStr)] string path, [MarshalAs(UnmanagedType.LPTStr)] string spec);

		public static int IndexOfRegex(this string str, string pattern) {
			return IndexOfRegex(str, pattern, 0, RegexOptions.None);
		}

		public static int IndexOfRegex(this string str, string pattern, int start) {
			return IndexOfRegex(str, pattern, start, RegexOptions.None);
		}

		public static int IndexOfRegex(this string str, string pattern, int start, RegexOptions options) {
			var rex = new Regex(pattern);
			var match = rex.Match(str, start);
			return (match.Success) ? match.Index : match.Index;
		}
		public static string ReplaceRegex(this string str, string pattern, string replacement) {
			return Regex.Replace(str, pattern, replacement);
		}

		public static string ReplaceRegex(this string str, string pattern, string replacement, RegexOptions option) {
			return Regex.Replace(str, pattern, replacement, option);
		}

		public static string ReplaceRegex(this string str, string pattern, MatchEvaluator eval) {
			return Regex.Replace(str, pattern, eval);
		}

		public static string ReplaceRegex(this string str, string pattern, MatchEvaluator eval, RegexOptions option) {
			return Regex.Replace(str, pattern, eval, option);
		}

		public static string Replace(this string str, string[] oldValues, string newValue) {
			oldValues.ForEach(value => str = str.Replace(value, newValue));
			return str;
		}

		public static string Join(this IEnumerable<string> source, string separator) {
			return String.Join(separator, source.ToArray());
		}

		[DllImport("shell32.dll", EntryPoint = "CommandLineToArgvW", CharSet = CharSet.Unicode)]
		internal static extern IntPtr CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string commmandLine, out int argCount);

		public static string[] ParseCommandLineString(this string commmandLineString){
			IntPtr lparray = IntPtr.Zero;
			try{
				int count;
				lparray = CommandLineToArgvW(commmandLineString, out count);
				if(lparray != IntPtr.Zero){
					string[] args = new string[count];
					int offset = 0;
					for(int i = 0; i < count; i++){
						IntPtr lpstr = Marshal.ReadIntPtr(lparray, offset);
						args[i] = Marshal.PtrToStringUni(lpstr);
						offset += Marshal.SizeOf(lpstr);
					}
					return args;
				}
			}finally{
				Marshal.FreeHGlobal(lparray);
			}
			return new string[0];
		}

		private const long    KBThreathold = 1000;
		private const long    MBThreathold = 1000 * 1000;
		private const long    GBThreathold = 1000 * 1000 * 1000;
		private const long    TBThreathold = Int64.MaxValue;
		private const double  KiBDouble    = 1024d;
		private const double  MiBDouble    = 1024d * 1024;
		private const double  GiBDouble    = 1024d * 1024 * 1024;
		private const double  KBDouble     = 1000d;
		private const double  MBDouble     = 1000d * 1000;
		private const double  GBDouble     = 1000d * 1000 * 1000;
		private const decimal KiBDecimal   = 1024m;
		private const decimal MiBDecimal   = 1024m * 1024;
		private const decimal GiBDecimal   = 1024m * 1024 * 1024;
		private const decimal TiBDecimal   = 1024m * 1024 * 1024 * 1024;
		private const decimal PiBDecimal   = 1024m * 1024 * 1024 * 1024 * 1024;
		private const decimal EiBDecimal   = 1024m * 1024 * 1024 * 1024 * 1024 * 1024;
		private const decimal ZiBDecimal   = 1024m * 1024 * 1024 * 1024 * 1024 * 1024 * 1024;
		private const decimal YiBDecimal   = 1024m * 1024 * 1024 * 1024 * 1024 * 1024 * 1024 * 1024;
		private const decimal KBDecimal    = 1000m;
		private const decimal MBDecimal    = 1000m * 1000;
		private const decimal GBDecimal    = 1000m * 1000 * 1000;
		private const decimal TBDecimal    = 1000m * 1000 * 1000 * 1000;
		private const decimal PBDecimal    = 1000m * 1000 * 1000 * 1000 * 1000;
		private const decimal EBDecimal    = 1000m * 1000 * 1000 * 1000 * 1000 * 1000;
		private const decimal ZBDecimal    = 1000m * 1000 * 1000 * 1000 * 1000 * 1000 * 1000;
		private const decimal YBDecimal    = 1000m * 1000 * 1000 * 1000 * 1000 * 1000 * 1000 * 1000;

		public static string GetFileSizeString(decimal size){
			return GetFileSizeString(size, FileSizeScale.Auto);
		}
		
		public static string GetFileSizeString(decimal size, FileSizeScale scale){
			if(scale == FileSizeScale.Auto){
				if(size < KBDecimal){
					scale = FileSizeScale.Bytes;
				}else if(size < MBDecimal){
					scale = FileSizeScale.KB;
				}else if(size < GBDecimal){
					scale = FileSizeScale.MB;
				}else if(size < TBDecimal){
					scale = FileSizeScale.GB;
				}else if(size < PBDecimal){
					scale = FileSizeScale.TB;
				}else if(size < EBDecimal){
					scale = FileSizeScale.PB;
				}else if(size < ZBDecimal){
					scale = FileSizeScale.EB;
				}else if(size < YBDecimal){
					scale = FileSizeScale.ZB;
				}else{
					scale = FileSizeScale.YB;
				}
			}else if(scale == FileSizeScale.AutoBinary){
				if(scale == FileSizeScale.AutoBinary){
					if(size < KBDecimal){
						scale = FileSizeScale.Bytes;
					}else if(size < MBDecimal){
						scale = FileSizeScale.KiB;
					}else if(size < GBDecimal){
						scale = FileSizeScale.MiB;
					}else if(size < TBDecimal){
						scale = FileSizeScale.GiB;
					}else if(size < PBDecimal){
						scale = FileSizeScale.TiB;
					}else if(size < EBDecimal){
						scale = FileSizeScale.PiB;
					}else if(size < ZBDecimal){
						scale = FileSizeScale.EiB;
					}else if(size < YBDecimal){
						scale = FileSizeScale.ZiB;
					}else{
						scale = FileSizeScale.YiB;
					}
				}
			}
			switch(scale){
				case FileSizeScale.Bytes:
					return size.ToString("N") + " B";
				case FileSizeScale.KB:
					return (size / KBDecimal).ToString("N") + " KB";
				case FileSizeScale.MB:
					return (size / MBDecimal).ToString("N") + " MB";
				case FileSizeScale.GB:
					return (size / GBDecimal).ToString("N") + " GB";
				case FileSizeScale.TB:
					return (size / TBDecimal).ToString("N") + " TB";
				case FileSizeScale.PB:
					return (size / PBDecimal).ToString("N") + " TB";
				case FileSizeScale.EB:
					return (size / EBDecimal).ToString("N") + " EB";
				case FileSizeScale.ZB:
					return (size / ZBDecimal).ToString("N") + " ZB";
				case FileSizeScale.YB:
					return (size / YBDecimal).ToString("N") + " YB";
				case FileSizeScale.KiB:
					return (size / KiBDecimal).ToString("N") + " KB";
				case FileSizeScale.MiB:
					return (size / MiBDecimal).ToString("N") + " MB";
				case FileSizeScale.GiB:
					return (size / GiBDecimal).ToString("N") + " GB";
				case FileSizeScale.TiB:
					return (size / TiBDecimal).ToString("N") + " TB";
				case FileSizeScale.PiB:
					return (size / PiBDecimal).ToString("N") + " TB";
				case FileSizeScale.EiB:
					return (size / EiBDecimal).ToString("N") + " EB";
				case FileSizeScale.ZiB:
					return (size / ZiBDecimal).ToString("N") + " ZB";
				case FileSizeScale.YiB:
					return (size / YiBDecimal).ToString("N") + " YB";
				default:
					throw new ArgumentException();
			}
		}

		public static bool IsDecimalNumber(this char c){
			return (('0' <= c) && (c <= '9'));
		}

		public static bool IsSmallAlphabet(this char c){
			int n = (int)c;
			return (('a' <= n) && (n <= 'z'));
		}

		public static bool IsLargeAlphabet(this char c){
			int n = (int)c;
			return (('A' <= n) && (n <= 'Z'));
		}

		#endregion

		#region EventHandler

		public static void Invoke(this EventHandler handler, object sender, EventArgs e) {
			if(handler != null) {
				handler(sender, e);
			}
		}

		#endregion
	}

	public enum FileSizeScale{
		Auto,
		AutoBinary,
		Bytes,
		KB,
		KiB,
		MB,
		MiB,
		GB,
		GiB,
		TB,
		TiB,
		PB,
		PiB,
		EB,
		EiB,
		ZB,
		ZiB,
		YB,
		YiB
	}
}