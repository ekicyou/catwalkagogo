/*
	$Id$
*/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.IO;
using System.Windows.Media;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using Microsoft.Win32;

namespace CatWalk.Shell{
	public static class ShellIcon{
		#region アイコン
		
		public static Icon GetIcon(string path, IconSize size){
			IntPtr hIcon;
			GetIcon(path, size, out hIcon);
			if(hIcon != IntPtr.Zero){
				using(Icon icon = Icon.FromHandle(hIcon)){
					Icon copiedIcon = (Icon)icon.Clone();
					Win32.DestroyIcon(icon.Handle);
					return copiedIcon;
				}
			}else{
				return null;
			}
		}
		
		public static Image GetIconImage(string path, IconSize size){
			IntPtr hIcon;
			GetIcon(path, size, out hIcon);
			if(hIcon != IntPtr.Zero){
				Icon icon = Icon.FromHandle(hIcon);
				if(icon != null){
					using(icon){
						Image image = icon.ToBitmap();
						Win32.DestroyIcon(icon.Handle);
						return image;
					}
				}else{
					return null;
				}
			}
			return null;
		}

		public static Bitmap GetIconBitmap(string path, IconSize size){
			IntPtr hIcon;
			GetIcon(path, size, out hIcon);
			if(hIcon != IntPtr.Zero){
				Icon icon = Icon.FromHandle(hIcon);
				if(icon != null){
					using(icon){
						Bitmap bmp = icon.ToBitmap();
						Win32.DestroyIcon(icon.Handle);
						return bmp;
					}
				}else{
					return null;
				}
			}
			return null;
		}


		public static ImageSource GetIconImageSource(string path, IconSize size){
			IntPtr hIcon;
			GetIcon(path, size, out hIcon);
			if(hIcon != IntPtr.Zero){
				int s = (size == IconSize.Large) ? 32 : 16;
				var image = Imaging.CreateBitmapSourceFromHIcon(hIcon, new Int32Rect(0, 0, s, s), BitmapSizeOptions.FromWidthAndHeight(s, s));
				return image;
			}
			return null;
		}
		
		private static void GetIcon(string path, IconSize size, out IntPtr hIcon){
			object value;
			string ext = Path.GetExtension(path);
			hIcon = IntPtr.Zero;
			if(String.IsNullOrEmpty(ext)){
				if((
					((path.Length == 2) && (Char.IsLetter(path, 0)) && (path[1] == ':')) ||
					((path.Length == 3) && (Char.IsLetter(path, 0)) && (path[1] == ':') && (path[2] == System.IO.Path.DirectorySeparatorChar)))){      // ドライブアイコン
					GetIconShell(path, size, out hIcon);
				}else{                      // 不明のアイコン
					IntPtr hLargeIcon;
					IntPtr hSmallIcon;
					GetUnknownIconHandle(out hLargeIcon, out hSmallIcon);
					if(size == IconSize.Large){
						hIcon = hLargeIcon;
						Win32.DestroyIcon(hSmallIcon);
					}else{
						hIcon = hSmallIcon;
						Win32.DestroyIcon(hLargeIcon);
					}
				}
			}else{
				RegistryKey root = Registry.ClassesRoot;
				try{
					// typeName取得
					string typeName = GetTypeName(root, ext);
					
					string clsid = null;
					if(typeName != null){
						RegistryKey typeKey = root.OpenSubKey(typeName, false);
						if(typeKey != null){	// ファイルタイプキー
							try{
								RegistryKey defaultIconKey = typeKey.OpenSubKey("DefaultIcon", false);
								if(defaultIconKey != null){	// ファイルタイプ\DefaultIconキー
									try{
										value = defaultIconKey.GetValue("");
										if(value != null){
											string spec = value.ToString();
											IntPtr hLargeIcon;
											IntPtr hSmallIcon;
											GetIconFromSpec(spec, path, out hLargeIcon, out hSmallIcon);
											if(size == IconSize.Large){
												hIcon = hLargeIcon;
												Win32.DestroyIcon(hSmallIcon);
											}else{
												hIcon = hSmallIcon;
												Win32.DestroyIcon(hLargeIcon);
											}
										}else{
											//Debug.WriteLine(ext + " -> " + typeName + " value is null.");
										}
									}finally{
										defaultIconKey.Close();
									}
								}else{
									//Debug.WriteLine(ext + " -> " + typeName + " -> DefaultIcon is not found.");
								}
								
								// typeName -> DefaultIconで取得できなかったとき、clsidを取得。
								if(hIcon == IntPtr.Zero){
									// 実行ファイルの時
									if(typeName.Equals("exefile", StringComparison.OrdinalIgnoreCase)){
										IntPtr smallIconHandle;
										IntPtr largeIconHandle;
										GetExecutableIconHandle(out largeIconHandle, out smallIconHandle);
										if(size == IconSize.Large){
											hIcon = largeIconHandle;
											Win32.DestroyIcon(smallIconHandle);
										}else{
											hIcon = smallIconHandle;
											Win32.DestroyIcon(largeIconHandle);
										}
									}else{	// CLSID
										RegistryKey typeClsidKey = typeKey.OpenSubKey("CLSID");
										if(typeClsidKey != null){
											try{
												value = typeClsidKey.GetValue("");
												if(value != null){
													clsid = value.ToString();
												}else{
													//Debug.WriteLine(ext + " -> " + typeName + " -> CLSID value is null.");
												}
											}finally{
												typeClsidKey.Close();
											}
										}else{
											//Debug.WriteLine(ext + " -> " + typeName + " CLSID is not found.");
										}
									}
								}
							}finally{
								typeKey.Close();
							}
						}else{
							//Debug.WriteLine(ext + " -> " + typeName + " is not found.");
						}
					}
					
					// CLSIDから取得。
					if(clsid != null){
						RegistryKey clsidKey = root.OpenSubKey("CLSID");
						if(clsidKey != null){
							try{
								RegistryKey typeClsidKey2 = clsidKey.OpenSubKey(clsid);
								if(typeClsidKey2 != null){
									try{
										RegistryKey defaultIconKey2 = typeClsidKey2.OpenSubKey("DefaultIcon");
										if(defaultIconKey2 != null){
											try{
												value = defaultIconKey2.GetValue("");
												if(value != null){
													string spec = value.ToString();
													IntPtr hLargeIcon;
													IntPtr hSmallIcon;
													GetIconFromSpec(spec, path, out hLargeIcon, out hSmallIcon);
													if(size == IconSize.Large){
														hIcon = hLargeIcon;
														Win32.DestroyIcon(hSmallIcon);
													}else{
														hIcon = hSmallIcon;
														Win32.DestroyIcon(hLargeIcon);
													}
												}else{
													//Debug.WriteLine(ext + " -> " + typeName + " CLSID -> " + clsid + " -> DefaultIcon value is null.");
												}
											}finally{
												defaultIconKey2.Close();
											}
										}else{
											//Debug.WriteLine(ext + " -> " + typeName + " CLSID -> " + clsid + " -> DefaultIcon is not found.");
										}
									}finally{
										typeClsidKey2.Close();
									}
								}else{
									//Debug.WriteLine(ext + " -> " + typeName + " CLSID -> " + clsid + " is not found.");
								}
							}finally{
								clsidKey.Close();
							}
						}else{
							//Debug.WriteLine("CLSID is not found.");
						}
					}
				}finally{
					root.Close();
				}
			}
			
			// 結局取得できなかったとき。
			if(hIcon == IntPtr.Zero){
				IntPtr hLargeIcon;
				IntPtr hSmallIcon;
				GetUnknownIconHandle(out hLargeIcon, out hSmallIcon);
				if(size == IconSize.Large){
					hIcon = hLargeIcon;
					Win32.DestroyIcon(hSmallIcon);
				}else{
					hIcon = hSmallIcon;
					Win32.DestroyIcon(hLargeIcon);
				}
			}
		}
		
		private static string GetTypeName(RegistryKey root, string ext){
			RegistryKey extKey = root.OpenSubKey(ext, false);
			if(extKey != null){	// .拡張子キー
				try{
					object value = extKey.GetValue("");
					if(value != null){
						return value.ToString();
					}else{
						//Debug.WriteLine(ext + " value is null.");
					}
				}finally{
					extKey.Close();
				}
			}else{
				//Debug.WriteLine(ext + " is not found.");
			}
			return null;
		}
		
		private static void GetIconFromSpec(string spec, string path, out IntPtr hLargeIcon, out IntPtr hSmallIcon){
			int sepIdx = spec.LastIndexOf(',');
			string res;
			int idx = 0;
			if(sepIdx == -1){
				res = spec;
			}else{
				if(Int32.TryParse(spec.Substring(sepIdx + 1), out idx)){
					res = spec.Substring(0, sepIdx);
				}else{
					res = spec;
					idx = 0;
				}
			}
			res = Environment.ExpandEnvironmentVariables(res.Replace("%1", path));
			
			ExtractIconEx(res, idx, out hLargeIcon, out hSmallIcon, 1);
		}
		
		public static void GetUnknownIconImage(out Image largeIcon, out Image smallIcon){
			IntPtr hLargeIcon;
			IntPtr hSmallIcon;
			GetUnknownIconHandle(out hLargeIcon, out hSmallIcon);
			if(hLargeIcon != IntPtr.Zero){
				using(Icon icon = (Icon)Icon.FromHandle(hLargeIcon)){
					largeIcon = icon.ToBitmap();
					Win32.DestroyIcon(hLargeIcon);
				}
			}else{
				largeIcon = null;
			}
			if(hSmallIcon != IntPtr.Zero){
				using(Icon icon = (Icon)Icon.FromHandle(hSmallIcon)){
					smallIcon = icon.ToBitmap();
					Win32.DestroyIcon(hSmallIcon);
				}
			}else{
				smallIcon = null;
			}
		}
		
		private static void GetUnknownIconHandle(out IntPtr hLargeIcon, out IntPtr hSmallIcon){
			if(Environment.OSVersion.Version.Major >= 6){
				if(Environment.OSVersion.Version.Minor >= 1){
					ExtractIconEx("imageres.dll", 2, out hLargeIcon, out hSmallIcon, 1);
				}else{
					ExtractIconEx("imageres.dll", 1, out hLargeIcon, out hSmallIcon, 1);
				}
			}else{
				ExtractIconEx("Shell32.dll", 0, out hLargeIcon, out hSmallIcon, 1);
			}
		}
		
		public static void GetExecutableIconImage(out Image largeIcon, out Image smallIcon){
			IntPtr hLargeIcon;
			IntPtr hSmallIcon;
			GetExecutableIconHandle(out hLargeIcon, out hSmallIcon);
			if(hLargeIcon != IntPtr.Zero){
				using(Icon icon = (Icon)Icon.FromHandle(hLargeIcon)){
					largeIcon = icon.ToBitmap();
					Win32.DestroyIcon(hLargeIcon);
				}
			}else{
				largeIcon = null;
			}
			if(hSmallIcon != IntPtr.Zero){
				using(Icon icon = (Icon)Icon.FromHandle(hSmallIcon)){
					smallIcon = icon.ToBitmap();
					Win32.DestroyIcon(hSmallIcon);
				}
			}else{
				smallIcon = null;
			}
		}
		
		private static void GetExecutableIconHandle(out IntPtr hLargeIcon, out IntPtr hSmallIcon){
			if(Environment.OSVersion.Version.Major >= 6){
				if(Environment.OSVersion.Version.Minor >= 1){
					ExtractIconEx("imageres.dll", 11, out hLargeIcon, out hSmallIcon, 1);
				}else{
					ExtractIconEx("imageres.dll", 10, out hLargeIcon, out hSmallIcon, 1);
				}
			}else{
				ExtractIconEx("Shell32.dll", 2, out hLargeIcon, out hSmallIcon, 1);
			}
		}
		
		private static IntPtr GetIconShell(string path, IconSize size, out IntPtr hIcon){
			SHFileInfo shinfo = new SHFileInfo();
			SHGFIFlags flags = SHGFIFlags.Icon;
			if(size == IconSize.Small){
				flags |= SHGFIFlags.SmallIcon;
			}else{
				flags |= SHGFIFlags.LargeIcon;
			}
			IntPtr r = SHGetFileInfo(path, FileAttributes.None, ref shinfo, (uint)Marshal.SizeOf(shinfo), flags);
			hIcon = shinfo.hIcon;
			return r;
		}
		
		public static Icon[] ExtractIcon(string path, int index){
			Icon[] icons = new Icon[2];
			IntPtr largeIconHandle;
			IntPtr smallIconHandle;
			ExtractIconEx(path, index, out largeIconHandle, out smallIconHandle, 1);
			if(largeIconHandle != IntPtr.Zero){
				Icon icon = Icon.FromHandle(largeIconHandle);
				using(icon){
					icons[0] = (Icon)icon.Clone();
					Win32.DestroyIcon(largeIconHandle);
				}
			}
			if(largeIconHandle != IntPtr.Zero){
				Icon icon = Icon.FromHandle(smallIconHandle);
				using(icon){
					icons[1] = (Icon)icon.Clone();
					Win32.DestroyIcon(smallIconHandle);
				}
			}
			return icons;
		}
		
		public static Image[] ExtractIconImage(string path, int index){
			Image[] icons = new Image[2];
			IntPtr largeIconHandle;
			IntPtr smallIconHandle;
			ExtractIconEx(path, index, out largeIconHandle, out smallIconHandle, 1);
			if(largeIconHandle != IntPtr.Zero){
				using(Icon icon = Icon.FromHandle(largeIconHandle)){
					icons[0] = icon.ToBitmap();
					Win32.DestroyIcon(largeIconHandle);
				}
			}
			if(largeIconHandle != IntPtr.Zero){
				using(Icon icon = Icon.FromHandle(smallIconHandle)){
					icons[1] = icon.ToBitmap();
					Win32.DestroyIcon(smallIconHandle);
				}
			}
			return icons;
		}
		
		public static void ExtractIconImage(string path, int index, out Image largeIconImage, out Image smallIconImage){
			largeIconImage = null;
			smallIconImage = null;
			IntPtr largeIconHandle;
			IntPtr smallIconHandle;
			ExtractIconEx(path, index, out largeIconHandle, out smallIconHandle, 1);
			if(largeIconHandle != IntPtr.Zero){
				using(Icon icon = Icon.FromHandle(largeIconHandle)){
					largeIconImage = icon.ToBitmap();
					Win32.DestroyIcon(largeIconHandle);
				}
			}
			if(largeIconHandle != IntPtr.Zero){
				using(Icon icon = Icon.FromHandle(smallIconHandle)){
					smallIconImage = icon.ToBitmap();
					Win32.DestroyIcon(smallIconHandle);
				}
			}
		}
		
		[DllImport("shell32.dll", EntryPoint = "ExtractIconEx", CharSet = CharSet.Auto)]
		private static extern int ExtractIconEx([MarshalAs(UnmanagedType.LPTStr)] string file, int index, out IntPtr largeIconHandle, out IntPtr smallIconHandle, int icons);
		
		[DllImport("shell32.dll", EntryPoint = "SHGetFileInfo", CharSet = CharSet.Auto)]
		private static extern IntPtr SHGetFileInfo(string pszPath, FileAttributes attr, ref SHFileInfo psfi, uint cbSizeFileInfo, SHGFIFlags uFlags);
		
		[Flags]
		private enum SHGFIFlags : uint{
			Icon                = 0x000000100,    // get icon
			DisplayName         = 0x000000200,    // get display name
			TypeName            = 0x000000400,    // get type name
			Attributes          = 0x000000800,    // get attributes
			IconLocation        = 0x000001000,    // get icon location
			ExeType             = 0x000002000,    // return exe type
			SysIconIndex        = 0x000004000,    // get system icon index
			LinkOverlay         = 0x000008000,    // put a link overlay on icon
			Selected            = 0x000010000,    // show icon in selected state
			                                      // (NTDDI_VERSION >= NTDDI_WIN2K)
			SpecifiedAttributes = 0x000020000,    // get only specified attributes
			LargeIcon           = 0x000000000,    // get large icon
			SmallIcon           = 0x000000001,    // get small icon
			OpenIcon            = 0x000000002,    // get open icon
			ShellIconSize       = 0x000000004,    // get shell size icon
			Pidl                = 0x000000008,    // pszPath is a pidl
			UseFileAttributes   = 0x000000010,    // use passed dwFileAttribute
			                                      // (_WIN32_IE >= 0x0500)
			AddOverlays         = 0x000000020,    // apply the appropriate overlays
			OverlayIndex        = 0x000000040,    // Get the index of the overlay
		}
		
		#endregion
		
		[Flags]
		private enum FileAttributes : uint{
			None      = 0x00000000,
			ReadOnly  = 0x00000001,
			Hidden    = 0x00000002,
			System    = 0x00000004,
			Directory = 0x00000010,
			Archive   = 0x00000020,
			Normal    = 0x00000080,
			Temporary = 0x00000100,
		}
	}
	
	public enum IconSize{
		Large,
		Small,
	}
	
	[StructLayoutAttribute(LayoutKind.Sequential)]
	public struct SHFileInfo{
		public IntPtr hIcon;
		public IntPtr iIcon;
		public uint dwAttributes;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string szDisplayName;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
		public string szTypeName;
	}

	public class ShellIconConverter : IValueConverter{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			var file = (string)value;
			var bmp = new WriteableBitmap(16, 16, 96, 96, PixelFormats.Bgra32, null);
			ThreadPool.QueueUserWorkItem(new WaitCallback(delegate{
				var icon = ShellIcon.GetIconBitmap(file, IconSize.Small);
				var bmpData = icon.LockBits(new Rectangle(0, 0, icon.Width, icon.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, icon.PixelFormat);
				bmp.Dispatcher.BeginInvoke(new Action(delegate{
					bmp.WritePixels(new Int32Rect(0, 0, icon.Width, icon.Height), bmpData.Scan0, bmpData.Stride * bmpData.Height, bmpData.Stride);
					icon.UnlockBits(bmpData);
					icon.Dispose();
				}), null);
			}));
			return bmp;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			throw new NotImplementedException();
		}
	}
}