using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace CatWalk.Text{
	public static class StringUtil{
		public static string[] ExpandSequentialNumbers(this string src){
			List<string> retStrs = new List<string>();
			string[] tmpStrArray;
			Regex rexBracket = new Regex(@"(?<!\\)\[.*?(?<!\\)\]", RegexOptions.Compiled);
			Regex rexNumbers = new Regex(@"^(\d+)-(\d+)", RegexOptions.Compiled);
			Regex rexHexNumbers = new Regex(@"^0x([0-9a-f]+)-0x([0-9a-f]+)", RegexOptions.Compiled & RegexOptions.IgnoreCase);
			Regex rexHexNumbersUpper = new Regex(@"^0x([0-9A-F]+)-0x([0-9A-F]+)", RegexOptions.Compiled);
			Regex rexChars = new Regex(@"^(.)-(.)", RegexOptions.Compiled);
			Match match;
			retStrs.Add(src);
			while(rexBracket.IsMatch(retStrs[0])){
				tmpStrArray = retStrs.ToArray();
				retStrs.Clear();
				foreach(string srci in tmpStrArray){
					match = rexBracket.Match(srci);
					if(match != Match.Empty){
						string left = srci.Substring(0, match.Index);
						string middle = srci.Substring(match.Index + 1, match.Length - 2);
						string right = srci.Substring(match.Index + match.Length);
						string source = middle;
						while(!(source.IsNullOrEmpty())){
							// dec-dec
							match = rexNumbers.Match(source);
							if(match != Match.Empty){
								source = source.Substring(match.Index + match.Length);
								string strStart = match.Groups[1].Value;
								string strEnd = match.Groups[2].Value;
								int numLength = strStart.Length;
								int start = Int32.Parse(strStart);
								int end = Int32.Parse(strEnd);
								if(start > end){
									retStrs.Add(left + source + right);
								}else{
									while(start <= end){
										retStrs.Add(left + start.ToString().PadLeft(numLength, '0') + right);
										start++;
									}
								}
								continue;
							}
							
							// hex-hex
							bool isUpper = false;
							match = rexHexNumbersUpper.Match(source);
							if(match != Match.Empty){
								isUpper = true;
							}else{
								match = rexHexNumbers.Match(source);
							}
							if(match != Match.Empty){
								source = source.Substring(match.Index + match.Length);
								string strStart = match.Groups[1].Value;
								string strEnd = match.Groups[2].Value;
								int numLength = strStart.Length;
								int start = Convert.ToInt32(strStart, 16);
								int end = Convert.ToInt32(strEnd, 16);
								if(start > end){
									retStrs.Add(left + source + right);
								}else{
									while(start <= end){
										string hexNumber;
										if(isUpper){
											hexNumber = Convert.ToString(start, 16).ToUpper();
										}else{
											hexNumber = Convert.ToString(start, 16).ToLower();
										}
										retStrs.Add(left + hexNumber.PadLeft(numLength, '0') + right);
										start++;
									}
								}
								continue;
							}
							
							// char-char
							match = rexChars.Match(source);
							if(match != Match.Empty){
								source = source.Substring(match.Index + match.Length);
								char charStart = match.Groups[1].Value[0];
								char charEnd = match.Groups[2].Value[0];
								if(charStart > charEnd){
									retStrs.Add(left + source + right);
								}else{
									while(charStart <= charEnd){
										retStrs.Add(left + charStart + right);
										charStart++;
									}
								}
								continue;
							}
							
							foreach(string strj in source.Split(new char[]{','})){
								source = null;
								retStrs.Add(left + strj + right);
							}
						} // while : sourceがnullか空でない
					} // if : 括弧が含まれる
				} // foreach : 置換文字列分
			} // while : 括弧がある
			for(int i = 0; i < retStrs.Count; i++){
				retStrs[i] = retStrs[i].Replace("\\[", "[").Replace("\\]", "]");
			}
			return retStrs.ToArray();
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
