/*
	$Id$
*/
using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace GflNet {
	public static partial class Gfl{
		static Gfl(){
			Gfl.Error error = Gfl.LibraryInit();
			Gfl.EnableLZW(true);
			if(error != Gfl.Error.None){
				throw new InvalidOperationException();
			}
			
			AppDomain.CurrentDomain.ProcessExit += Finalize;
		}
		
		private static void Finalize(object sender, EventArgs e){
			Gfl.LibraryExit();
		}
		
		public static string VersionString{
			get{
				IntPtr ptr = Gfl.GetVersion();
				return Marshal.PtrToStringAnsi(ptr);
			}
		}
		
		private static bool isEnableLZW = true;
		public static bool IsEnableLZW{
			get{
				return isEnableLZW;
			}
			set{
				isEnableLZW = value;
				Gfl.EnableLZW(value);
			}
		}
		
		private static string pluginPath = null;
		public static string PluginPath{
			get{
				return pluginPath;
			}
			set{
				pluginPath = value;
				Gfl.SetPluginPathname(value);
			}
		}
		
		public static Format[] GetFormats(){
			int num = Gfl.GetNumberOfFormat();
			Format[] formats = new Format[num];
			for(int i = 0; i < num; i++){
				formats[i] = GetGflFormat(i);
			}
			return formats;
		}
		
		internal static Format GetGflFormat(int index){
			string name = Gfl.GetFormatNameByIndex(index);
			string defaultSuffix = Gfl.GetDefaultFormatSuffixByIndex(index);
			bool readable = Gfl.FormatIsReadableByIndex(index);
			bool writable = Gfl.FormatIsWritableByIndex(index);
			string description = Gfl.GetFormatDescriptionByIndex(index);
			
			return new Format(name, defaultSuffix, readable, writable, description);
		}
		
		internal static Exif GetGflExif(string path){
			return new Exif(Gfl.LoadExif(path, Gfl.GetExifOptions.None));
		}
	}
}
