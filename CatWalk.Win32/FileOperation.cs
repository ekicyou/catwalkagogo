/*
	$Id$
*/

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Text;

namespace CatWalk.Win32{
	using IO = System.IO;
	using Path = System.IO.Path;
	using MemoryStream = System.IO.MemoryStream;
	
	public static class FileOperation{
		[DllImport("Shell32.dll", EntryPoint = "SHFileOperation", CharSet = CharSet.Auto)]
		private static extern int SHFileOperation(ref SHFileOperationStruct lpFileOp);
		
		[StructLayout(LayoutKind.Sequential)]
		internal struct SHFileOperationStruct{
			public IntPtr Handle;
			public FileOperationFunc Func;
			[MarshalAs(UnmanagedType.LPTStr)]
			public string From;
			[MarshalAs(UnmanagedType.LPTStr)]
			public string To;
			public FileOperationOptions Options;
			public bool AnyOperationsAborted;
			public IntPtr NameMappings;
			[MarshalAs(UnmanagedType.LPTStr)]
			public string ProgressTitle;
		}
		
		#region SHFileOperation
		
		private struct SHFileOperationArgs{
			public IntPtr Handle{get; set;}
			public FileOperationFunc Func{get; set;}
			public FileOperationOptions Options{get; set;}
			public string[] From{get; set;}
			public string[] To{get; set;}
			public string ProgressTitle{get; set;}
		}
		
		private static void SHFileOperation(SHFileOperationArgs args){
			SHFileOperationStruct sh = new SHFileOperationStruct();
			sh.Handle = args.Handle;
			sh.Func = args.Func;
			sh.From = String.Join("\0", args.From) + '\0';
			sh.To = String.Join("\0", args.To) + '\0';;
			sh.Options = args.Options;
			sh.AnyOperationsAborted = false;
			sh.NameMappings = IntPtr.Zero;
			sh.ProgressTitle = args.ProgressTitle;
			int errorCode = SHFileOperation(ref sh);
			if(errorCode != 0){
				throw new Win32Exception(errorCode);
			}
			if(sh.AnyOperationsAborted){
				throw new OperationCanceledException();
			}
		}
		
		#endregion
		
		#region 同期処理
		
		public static void Delete(string[] files){
			Delete(files, FileOperationOptions.None, IntPtr.Zero, null);
		}
		
		public static void Delete(string[] files, FileOperationOptions options){
			Delete(files, options, IntPtr.Zero, null);
		}
		
		public static void Delete(string[] files, FileOperationOptions options, IntPtr hwnd){
			Delete(files, options, hwnd, null);
		}
		
		public static void Delete(string[] files, FileOperationOptions options, IntPtr hwnd, string progressTitle){
			if(files == null){
				throw new ArgumentNullException();
			}
			if(files.Length == 0){
				throw new ArgumentException();
			}
			SHFileOperationArgs args = new SHFileOperationArgs();
			args.Handle = hwnd;
			args.Func = FileOperationFunc.Delete;
			args.Options = options;
			args.From = files;
			args.To = null;
			args.ProgressTitle = progressTitle;
			SHFileOperation(args);
		}
		
		public static void Move(string to, params string[] files){
			Move(files, to, FileOperationOptions.None, IntPtr.Zero, null);
		}
		
		public static void Move(string[] files, string to, FileOperationOptions options){
			Move(files, to, options, IntPtr.Zero, null);
		}
		
		public static void Move(string[] files, string to, FileOperationOptions options, IntPtr hwnd){
			Move(files, to, options, hwnd, null);
		}
		
		public static void Move(string[] files, string to, FileOperationOptions options, IntPtr hwnd, string progressTitle){
			if(files == null){
				throw new ArgumentNullException();
			}
			if(files.Length == 0){
				throw new ArgumentException();
			}
			SHFileOperationArgs args = new SHFileOperationArgs();
			args.Handle = hwnd;
			args.Func = FileOperationFunc.Move;
			args.Options = options;
			args.From = files;
			args.To = new []{to};
			args.ProgressTitle = progressTitle;
			SHFileOperation(args);
		}
		
		public static void Copy(string to, params string[] files){
			Copy(files, to, FileOperationOptions.None, IntPtr.Zero, null);
		}
		
		public static void Copy(string[] files, string to, FileOperationOptions options){
			Copy(files, to, options, IntPtr.Zero, null);
		}
		
		public static void Copy(string[] files, string to, FileOperationOptions options, IntPtr hwnd){
			Copy(files, to, options, hwnd, null);
		}
		
		public static void Copy(string[] files, string to, FileOperationOptions options, IntPtr hwnd, string progressTitle){
			if(files == null){
				throw new ArgumentNullException();
			}
			if(files.Length == 0){
				throw new ArgumentException();
			}
			SHFileOperationArgs args = new SHFileOperationArgs();
			args.Handle = hwnd;
			args.Func = FileOperationFunc.Copy;
			args.Options = options;
			args.From = files;
			args.To = new[]{to};
			args.ProgressTitle = progressTitle;
			SHFileOperation(args);
		}
		
		public static void Rename(string from, string to){
			Rename(from, to, FileOperationOptions.None, IntPtr.Zero, null);
		}
		
		public static void Rename(string from, string to, FileOperationOptions options){
			Rename(from, to, options, IntPtr.Zero, null);
		}
		
		public static void Rename(string from, string to, FileOperationOptions options, IntPtr hwnd){
			Rename(from, to, options, hwnd, null);
		}
		
		public static void Rename(string from, string to, FileOperationOptions options, IntPtr hwnd, string progressTitle){
			if((from == null) || (to == null)){
				throw new ArgumentNullException();
			}
			SHFileOperationArgs args = new SHFileOperationArgs();
			args.Handle = hwnd;
			args.Func = FileOperationFunc.Rename;
			args.Options = options;
			args.From = new[]{from};
			args.To = new[]{to};
			args.ProgressTitle = progressTitle;
			SHFileOperation(args);
		}
		
		#endregion

		#region Symbolic Link

		public static void CreateSymbolicLink(string linkToCreate, string target, SymbolicLinkKind kind){
			if(!Win32Api.CreateSymbolicLink(linkToCreate, target, kind)){
				throw new Win32Exception();
			}
		}

		#endregion

		#region Long Path / Short Path

		public static string GetShortPathName(string path){
			for(int count = path.Length; count < 32767; count *= 2){
				StringBuilder sb = new StringBuilder(count);
				if(Win32Api.GetShortPathName(path, sb, count) != 0){
					return sb.ToString();
				}
			}
			return null;
		}

		public static string GetLongPathName(string path){
			for(int count = path.Length + 256; count < 32767; count *= 2){
				StringBuilder sb = new StringBuilder(count);
				if(Win32Api.GetLongPathName(path, sb, count) != 0){
					return sb.ToString();
				}
			}
			return null;
		}

		#endregion
	}
	
	/// <summary>
	/// SHFileOperationで行う処理のオプション。
	/// </summary>
	[Flags]
	public enum FileOperationOptions : ushort{
		None = 0x0000,
		MultiDestFiles = 0x0001,
		ConfirmMouse = 0x0002,
		Silent = 0x0004,
		RenameOnCollision = 0x0008,
		NoConfirmation = 0x0010,
		WantMappingHandle = 0x0020,
		AllowUndo = 0x0040,
		FilesOnly = 0x0080,
		SimpleProgress = 0x0100,
		NoConfirmMakeDirectory = 0x0200,
		NoErrorUI = 0x0400,
		NoCopySecurityAttributes = 0x0800,
		NoRecursion = 0x1000,
		NoConectedElements = 0x2000,
		WantNukeWarning = 0x4000,
		NoRecurseParsing = 0x8000,
	}

	public enum FileOperationFunc : uint{
		Move = 0x0001,
		Copy = 0x0002,
		Delete = 0x0003,
		Rename = 0x0004,
	}

	public enum SymbolicLinkKind : int{
		File = 0,
		Directory = 1,
	}
}