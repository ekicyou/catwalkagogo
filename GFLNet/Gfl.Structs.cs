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
	public partial class Gfl{
		#region Struct
		
		[StructLayoutAttribute(LayoutKind.Sequential)]
		internal struct Bitmap{
			public BitmapType Type;
			public Origin Origin;
			public Int32 Width;
			public Int32 Height;
			public UInt32 BytesPerLine;
			public Int16 LinePadding;
			public UInt16 BitsPerComponent;
			public UInt16 ComponentsPerPixel;
			public UInt16 BytesPerPixel;
			public UInt16 Xdpi;
			public UInt16 Ydpi;
			public Int16 TransparentIndex;
			public Int16 Reserved;
			public Int32 ColorUsed;
			public IntPtr ColorMap;
			public IntPtr Data;
			public string Comment;
			public IntPtr MetaData;
			
			public Int32 XOffset;
			public Int32 YOffset;
			public string Name;
		}
		
		[StructLayoutAttribute(LayoutKind.Sequential)]
		internal struct Color{
			public UInt16 Red;
			public UInt16 Green;
			public UInt16 Blue;
			public UInt16 Alpha;
		}
		
		[StructLayoutAttribute(LayoutKind.Sequential)]
		internal struct ColorMap{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
			public byte[] Red;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
			public byte[] Green;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
			public byte[] Blue;
		}
		[StructLayoutAttribute(LayoutKind.Sequential)]
		internal struct FormatInformation{
			public int Index;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
			public string Name;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string Description;
			public Status Status;
			public UInt32 NumberOfExtension;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string Extension;
		}
		
		[StructLayoutAttribute(LayoutKind.Sequential)]
		internal struct LoadParams{
			public LoadOptions Options;
			public int FormatIndex;
			public int ImageWanted;
			public Origin Origin;
			public BitmapType ColorModel;
			public uint LinePadding;
			public byte DefaultAlpha;
			
			public byte PsdNoAlphaForNonLayer;
			public byte PngComposeWithAlpha;
			public byte WMFHighResolution;
			
			public int Width;
			public int Height;
			public uint Offset;
			
			public ChannelOrder ChannelOrder;
			public ChannelType ChannelType;
			
			public UInt16 PcdBase;
			
			public UInt16 EpsDpi;
			public int EpsWidth;
			public int EpsHeight;
			
			public LutType LutType;
			public UInt16 Reserved3;
			public IntPtr LutData;
			public string LutFilename;
			
			public byte CameraRawUseAutomaticBalance;
			public byte CameraRawUseCameraBalance;
			public byte CameraRawHighlight;
			public byte Reserved4;
			public float CameraRawGamma;
			public float CameraRawBrightness;
			public float CameraRawRedScaling;
			public float CameraRawBlueScaling;
			
			public LoadCallbacks Callbacks;
			
			public IntPtr UserParams;
		}
		
		[StructLayoutAttribute(LayoutKind.Sequential)]
		internal struct LoadCallbacks{
			public IntPtr Read;
			public IntPtr Tell;
			public IntPtr Seek;
			
			public IntPtr AllocateBitmap; /* Global or not???? */
			public IntPtr AllocateBitmapParams;
			
			public ProgressCallback Progress;
			public IntPtr ProgressParams;
			
			public IntPtr WantCancel;
			public IntPtr WantCancelParams;
			
			public IntPtr SetLine;
			public IntPtr SetLineParams;
		}
		
		internal delegate void ProgressCallback(int percent, IntPtr userParams);
		
		[StructLayoutAttribute(LayoutKind.Sequential)]
		internal struct FileInformation{
			public BitmapType  Type;   /* Not used */
			public Origin      Origin;
			public int         Width;
			public int         Height;
			public int         FormatIndex;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
			public string      FormatName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string      Description;
			public UInt16      Xdpi;
			public UInt16      Ydpi;
			public UInt16      BitsPerComponent;  /* 1, 8, 10, 12, 16 */
			public UInt16      ComponentsPerPixel;/* 1, 3, 4  */
			public int         NumberOfImages;
			public uint        FileSize;
			public ColorModel  ColorModel;
			public Compression Compression;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string      CompressionDescription;
			public int         XOffset;
			public int         YOffset;
			public IntPtr      ExtraInfos;
		}
		
		[StructLayoutAttribute(LayoutKind.Sequential)]
		internal struct SaveParams{
			public SaveOptions Options;
			public int FormatIndex;
			
			public Compression Compression;
			public Int16 Quality;
			public Int16 CompressionLevel; // for PNG 1 to 7
			public bool Interlaced; // for GIF
			public bool Progressive; // for JPEG
			public bool OptimizeHuffmanTable; // for JPEG
			public bool InAscii; // for PPM
			
			// for DPX/Cineon
			public LutType LutType;
			public ByteOrder DpxByteOrder;
			public byte CompressRatio; // for JPEG2000
			public UInt32 MaxFileSize; // for JPEG2000
			
			public IntPtr LutData;
			public string LutFilename;
			
			// for RAW/YUV
			public UInt32 Offset;
			public ChannelOrder ChannelOrder;
			public ChannelType ChannelType;
			
			public SaveCallbacks Callbacks;
			
			public IntPtr UserParams;
		}
		
		[StructLayoutAttribute(LayoutKind.Sequential)]
		internal struct SaveCallbacks{
			public IntPtr Read;
			public IntPtr Tell;
			public IntPtr Seek;
			
			public IntPtr GetLine;
			public IntPtr GetLineParams;
		}
		
		[StructLayoutAttribute(LayoutKind.Sequential)]
		internal struct ExifData{
			public UInt32 NumberOfItems;
			[MarshalAs(UnmanagedType.LPArray)]
			public ExifEntry[] ItemList;
		}
		
		[StructLayoutAttribute(LayoutKind.Sequential)]
		internal struct ExifEntry{
			public ExifEntryTypes Types;
			public UInt32 Tag;
			[MarshalAs(UnmanagedType.LPStr)]
			public string Name;
			[MarshalAs(UnmanagedType.LPStr)]
			public string Value;
		}
		
		#endregion
	}
}
