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
using System.Windows.Forms;

namespace CatWalk.Shell{
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
			public string From{get; set;}
			public string To{get; set;}
			public string ProgressTitle{get; set;}
		}
		
		private static FileOperationResult SHFileOperation(SHFileOperationArgs args){
			SHFileOperationStruct sh = new SHFileOperationStruct();
			sh.Handle = args.Handle;
			sh.Func = args.Func;
			sh.From = args.From + '\0';
			sh.To = args.To + '\0';
			sh.Options = args.Options;
			sh.AnyOperationsAborted = false;
			sh.NameMappings = IntPtr.Zero;
			sh.ProgressTitle = args.ProgressTitle;
			int errorCode = SHFileOperation(ref sh);
			return new FileOperationResult(args.From, args.To, errorCode, sh.AnyOperationsAborted, sh.Func);
		}
		
		#endregion
		
		#region 同期処理
		
		public static FileOperationResult Delete(string[] files){
			return Delete(files, FileOperationOptions.None, IntPtr.Zero, null);
		}
		
		public static FileOperationResult Delete(string[] files, FileOperationOptions options){
			return Delete(files, options, IntPtr.Zero, null);
		}
		
		public static FileOperationResult Delete(string[] files, FileOperationOptions options, IntPtr hwnd){
			return Delete(files, options, hwnd, null);
		}
		
		public static FileOperationResult Delete(string[] files, FileOperationOptions options, IntPtr hwnd, string progressTitle){
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
			args.From = String.Join("\0", files);
			args.To = null;
			args.ProgressTitle = progressTitle;
			return SHFileOperation(args);
		}
		
		public static FileOperationResult Move(string to, params string[] files){
			return Move(files, to, FileOperationOptions.None, IntPtr.Zero, null);
		}
		
		public static FileOperationResult Move(string[] files, string to, FileOperationOptions options){
			return Move(files, to, options, IntPtr.Zero, null);
		}
		
		public static FileOperationResult Move(string[] files, string to, FileOperationOptions options, IntPtr hwnd){
			return Move(files, to, options, hwnd, null);
		}
		
		public static FileOperationResult Move(string[] files, string to, FileOperationOptions options, IntPtr hwnd, string progressTitle){
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
			args.From = String.Join("\0", files);
			args.To = to;
			args.ProgressTitle = progressTitle;
			return SHFileOperation(args);
		}
		
		public static FileOperationResult Copy(string to, params string[] files){
			return Copy(files, to, FileOperationOptions.None, IntPtr.Zero, null);
		}
		
		public static FileOperationResult Copy(string[] files, string to, FileOperationOptions options){
			return Copy(files, to, options, IntPtr.Zero, null);
		}
		
		public static FileOperationResult Copy(string[] files, string to, FileOperationOptions options, IntPtr hwnd){
			return Copy(files, to, options, hwnd, null);
		}
		
		public static FileOperationResult Copy(string[] files, string to, FileOperationOptions options, IntPtr hwnd, string progressTitle){
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
			args.From = String.Join("\0", files);
			args.To = to;
			args.ProgressTitle = progressTitle;
			return SHFileOperation(args);
		}
		
		public static FileOperationResult Rename(string from, string to){
			return Rename(from, to, FileOperationOptions.None, IntPtr.Zero, null);
		}
		
		public static FileOperationResult Rename(string from, string to, FileOperationOptions options){
			return Rename(from, to, options, IntPtr.Zero, null);
		}
		
		public static FileOperationResult Rename(string from, string to, FileOperationOptions options, IntPtr hwnd){
			return Rename(from, to, options, hwnd, null);
		}
		
		public static FileOperationResult Rename(string from, string to, FileOperationOptions options, IntPtr hwnd, string progressTitle){
			if((from == null) || (to == null)){
				throw new ArgumentNullException();
			}
			SHFileOperationArgs args = new SHFileOperationArgs();
			args.Handle = hwnd;
			args.Func = FileOperationFunc.Rename;
			args.Options = options;
			args.From = from;
			args.To = to;
			args.ProgressTitle = progressTitle;
			return SHFileOperation(args);
		}
		
		#endregion
	}
	
	public class FileOperationResult{
		public FileOperationResult(string from, string to, int errorCode, bool isAborted, FileOperationFunc func){
			this.FromFiles = from.Split('\0');
			this.To = to;
			this.IsAborted = isAborted;
			this.ErrorCode = errorCode;
			this.Operation = func;
		}
		
		public string[] FromFiles{get; private set;}
		
		public string To{get; private set;}
		
		/// <summary>
		/// 中止されたかどうか。
		/// </summary>
		public bool IsAborted{get; private set;}
		
		/// <summary>
		/// エラー時は0以外の数字
		/// </summary>
		public int ErrorCode{get; private set;}
		
		public bool IsErrored{
			get{
				return (this.ErrorCode != 0);
			}
		}
		
		public FileOperationFunc Operation{get; private set;}
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
}