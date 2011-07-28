using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace CatWalk.Win32 {
	[StructLayout(LayoutKind.Sequential)]
	public struct WindowPlacement{
		public long Length;
		public long Flags;
		public ShowWindowCommand Command;
		public Point MinPosition;
		public Point MaxPosition;
		public Rectangle NormalPosition;
	}
		
	/// <summary>
	/// Win32.WindowPlacement構造体のメンバで使用する構造体。
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Point{
		public long X;
		public long Y;
	}
		
	/// <summary>
	/// Win32.WindowPlacement構造体のメンバで使用する構造体。
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Rectangle{
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct GDIPoint{
		public long X;
		public long Y;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct NMListView{
		public IntPtr Hdr;
		public int IDForm;
		public int Code;
		public int Item;
		public int SubItem;
		public uint NewState;
		public uint OldState;
		public uint Changed;
		public GDIPoint Action;
		public IntPtr LParam;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct NMHdr{
		public IntPtr Hdr;
		public int IDForm;
		public int Code;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct CopyDataStruct{
		public IntPtr dwData;
		public int cbData;
		public IntPtr lpData;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct HeaderItem{
		public HeaderItemMask Mask;
		public int CXY;
		public string Text;
		public IntPtr Bitmap;
		public int TextMax;
		public ListViewColumnFormat Format;
		public int LParam;
		public int ImageIndex;
		public int OrderIndex;
		public uint Type;
		public IntPtr Filter;
		public uint State;
	}
}
