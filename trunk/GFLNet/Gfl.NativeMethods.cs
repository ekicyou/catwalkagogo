/*
	$Id$
*/
using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace GflNet{
	public static partial class Gfl{
		const string GflDllName = "libgfl340.dll";
		
		#region Initialization
		
		[DllImport(GflDllName, EntryPoint = "gflLibraryInit", CharSet = CharSet.Auto)]
		internal static extern Error LibraryInit();
		
		[DllImport(GflDllName, EntryPoint = "gflLibraryExit", CharSet = CharSet.Auto)]
		internal static extern Error LibraryExit();
		
		[DllImport(GflDllName, EntryPoint = "gflEnableLZW", CharSet = CharSet.Auto)]
		internal static extern void EnableLZW(bool enable);
		
		[DllImport(GflDllName, EntryPoint = "gflGetVersion", CharSet = CharSet.Auto)]
		internal static extern IntPtr GetVersion();
		
		[DllImport(GflDllName, EntryPoint = "gflSetPluginsPathname", CharSet = CharSet.Auto)]
		internal static extern void SetPluginPathname(string path);
		
		#endregion
		
		#region Allocation
		
		[DllImport(GflDllName, EntryPoint = "gflAllocBitmap", CharSet = CharSet.Auto)]
		internal static extern IntPtr AllocBitmap(BitmapType type, int width, int height, uint linePadding, ref Color backgroundColor);
		
		[DllImport(GflDllName, EntryPoint = "gflFreeBitmap", CharSet = CharSet.Auto)]
		internal static extern void FreeBitmap(ref Gfl.Bitmap bitmap);
		
		[DllImport(GflDllName, EntryPoint = "gflCloneBitmap", CharSet = CharSet.Auto)]
		internal static extern IntPtr CloneBitmap(ref Gfl.Bitmap bitmap);
		
		[DllImport(GflDllName, EntryPoint = "gflFreeBitmapData", CharSet = CharSet.Auto)]
		internal static extern IntPtr FreeBitmapData(ref Gfl.Bitmap bitmap);
		
		[DllImport(GflDllName, EntryPoint = "gflFreeFileInformation", CharSet = CharSet.Auto)]
		internal static extern IntPtr FreeFileInformation(ref FileInformation info);
		
		#endregion
		
		#region Format
		
		[DllImport(GflDllName, EntryPoint = "gflGetNumberOfFormat", CharSet = CharSet.Auto)]
		internal static extern int GetNumberOfFormat();
		
		[DllImport(GflDllName, EntryPoint = "gflFormatIsSupported", CharSet = CharSet.Auto)]
		internal static extern bool FormatIsSupported(string name);
		
		[DllImport(GflDllName, EntryPoint = "gflGetFormatNameByIndex", CharSet = CharSet.Auto)]
		internal static extern string GetFormatNameByIndex(int index);
		
		[DllImport(GflDllName, EntryPoint = "gflGetFormatIndexByName", CharSet = CharSet.Auto)]
		internal static extern int GetFormatIndexByName(string name);
		
		[DllImport(GflDllName, EntryPoint = "gflFormatIsReadableByIndex", CharSet = CharSet.Auto)]
		internal static extern bool FormatIsReadableByIndex(int index);
		
		[DllImport(GflDllName, EntryPoint = "gflFormatIsReadableByName", CharSet = CharSet.Auto)]
		internal static extern bool FormatIsReadableByName(string name);
		
		[DllImport(GflDllName, EntryPoint = "gflFormatIsWritableByIndex", CharSet = CharSet.Auto)]
		internal static extern bool FormatIsWritableByIndex(int index);
		
		[DllImport(GflDllName, EntryPoint = "gflFormatIsReadableByName", CharSet = CharSet.Auto)]
		internal static extern bool FormatIsWritableByName(string name);
		
		[DllImport(GflDllName, EntryPoint = "gflGetFormatDescriptionByIndex", CharSet = CharSet.Auto)]
		internal static extern string GetFormatDescriptionByIndex(int index);
		
		[DllImport(GflDllName, EntryPoint = "gflGetFormatDescriptionByName", CharSet = CharSet.Auto)]
		internal static extern string GetFormatDescriptionByName(string name);
		
		[DllImport(GflDllName, EntryPoint = "gflGetDefaultFormatSuffixByIndex", CharSet = CharSet.Auto)]
		internal static extern string GetDefaultFormatSuffixByIndex(int index);
		
		[DllImport(GflDllName, EntryPoint = "gflGetDefaultFormatSuffixByName", CharSet = CharSet.Auto)]
		internal static extern string GetDefaultFormatSuffixByName(string name);
		
		[DllImport(GflDllName, EntryPoint = "gflGetFormatInformationByIndex", CharSet = CharSet.Auto)]
		internal static extern Error GetFormatInformationByIndex(int index, ref FormatInformation info);
		
		[DllImport(GflDllName, EntryPoint = "gflGetFormatInformationByName", CharSet = CharSet.Auto)]
		internal static extern Error GetFormatInformationByName(string name, ref FormatInformation info);
		
		#endregion
		
		#region Read
		
		[DllImport(GflDllName, EntryPoint = "gflGetDefaultLoadParams", CharSet = CharSet.Auto)]
		internal static extern void GetDefaultLoadParams(ref LoadParams prms);
		
		[DllImport(GflDllName, EntryPoint = "gflGetDefaultThumbnailParams", CharSet = CharSet.Auto)]
		internal static extern void GetDefaultThumbnailParams(ref LoadParams prms);
		
		[DllImport(GflDllName, EntryPoint = "gflLoadBitmap", CharSet = CharSet.Auto)]
		internal static extern Error LoadBitmap(string filename, ref IntPtr bitmap, ref LoadParams prms, ref FileInformation info);
		
		[DllImport(GflDllName, EntryPoint = "gflLoadBitmapFromMemory", CharSet = CharSet.Auto)]
		internal static extern Error LoadBitmapFromMemory(IntPtr data, uint data_length, ref IntPtr bitmap, ref LoadParams prms, ref FileInformation informations);
		
		[DllImport(GflDllName, EntryPoint = "gflLoadBitmapFromHandle", CharSet = CharSet.Auto)]
		internal static extern Error LoadBitmapFromHandle(IntPtr handle, ref IntPtr bitmap, ref LoadParams prms, ref FileInformation informations);
		
		[DllImport(GflDllName, EntryPoint = "gflGetFileInformation", CharSet = CharSet.Auto)]
		internal static extern void GetFileInformation(string filename, int index, ref FileInformation info);
		
		[DllImport(GflDllName, EntryPoint = "gflLoadThumbnail", CharSet = CharSet.Auto)]
		internal static extern Error LoadThumbnail(string filename, int width, int height, ref IntPtr bitmap, ref LoadParams prms, ref FileInformation info);
		
		[DllImport(GflDllName, EntryPoint = "gflLoadThumbnailFromMemory", CharSet = CharSet.Auto)]
		internal static extern Error LoadThumbnailFromMemory(IntPtr data, uint data_length, int width, int height, ref IntPtr bitmap, ref LoadParams prms, ref FileInformation informations);
		
		[DllImport(GflDllName, EntryPoint = "gflLoadThumbnailFromHandle", CharSet = CharSet.Auto)]
		internal static extern Error LoadThumbnailFromHandle(IntPtr handle, int width, int height, ref IntPtr bitmap, ref LoadParams prms, ref FileInformation informations);
		
		#endregion
		
		#region Write
		
		[DllImport(GflDllName, EntryPoint = "gflGetDefaultSaveParams", CharSet = CharSet.Auto)]
		internal static extern void GetDefaultSaveParams(ref SaveParams prms);
		
		[DllImport(GflDllName, EntryPoint = "gflSaveBitmap", CharSet = CharSet.Auto)]
		internal static extern Error SaveBitmap(string filename, ref Gfl.Bitmap bitmap, ref SaveParams prms);
		
		#endregion
		
		#region Metadata
		
		[DllImport(GflDllName, EntryPoint = "gflBitmapHasEXIF", CharSet = CharSet.Auto)]
		internal static extern bool HasExif(ref Gfl.Bitmap bitmap);
		
		[DllImport(GflDllName, EntryPoint = "gflBitmapHasIPTC", CharSet = CharSet.Auto)]
		internal static extern bool HasIptc(ref Gfl.Bitmap bitmap);
		
		[DllImport(GflDllName, EntryPoint = "gflBitmapHasICCProfile", CharSet = CharSet.Auto)]
		internal static extern bool HasIccProfile(ref Gfl.Bitmap bitmap);
		
		[DllImport(GflDllName, EntryPoint = "gflBitmapGetEXIF", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.LPStruct)]
		internal static extern ExifData GetExif(ref Gfl.Bitmap bitmap, GetExifOptions options);
		
		[DllImport(GflDllName, EntryPoint = "gflLoadEXIF", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.LPStruct)]
		internal static extern ExifData LoadExif(string path, GetExifOptions options);
		
		[DllImport(GflDllName, EntryPoint = "gflFreeEXIF", CharSet = CharSet.Auto)]
		internal static extern void FreeExif(ref ExifData exif);
		
		[DllImport(GflDllName, EntryPoint = "gflBitmapRemoveEXIFThumbnail", CharSet = CharSet.Auto)]
		internal static extern void RemoveExifThumbnail(ref Gfl.Bitmap bitmap);
		
		[DllImport(GflDllName, EntryPoint = "gflBitmapRemoveICCProfile", CharSet = CharSet.Auto)]
		internal static extern void RemoveIccProfile(ref Gfl.Bitmap bitmap);
		
		[DllImport(GflDllName, EntryPoint = "gflBitmapGetICCProfile", CharSet = CharSet.Auto)]
		internal static extern void GetIccProfile(ref Gfl.Bitmap bitmap, ref IntPtr pData, ref UInt32 length);
		
		[DllImport(GflDllName, EntryPoint = "gflBitmapCopyICCProfile", CharSet = CharSet.Auto)]
		internal static extern void CopyIccProfile(ref Gfl.Bitmap src, ref Gfl.Bitmap dst);
		
		[DllImport(GflDllName, EntryPoint = "gflBitmapRemoveMetaData", CharSet = CharSet.Auto)]
		internal static extern void RemoveMetaData(ref Gfl.Bitmap bitmap);
		
		[DllImport(GflDllName, EntryPoint = "gflBitmapGetXMP", CharSet = CharSet.Auto)]
		internal static extern bool GetXmp(ref Gfl.Bitmap bitmap, ref IntPtr pData, ref UInt32 length);
		
		[DllImport(GflDllName, EntryPoint = "gflBitmapSetEXIFThumbnail", CharSet = CharSet.Auto)]
		internal static extern void SetExifThumbnail(ref Gfl.Bitmap bitmap, ref Gfl.Bitmap thumbnail);
		
		#endregion
		
		#region Extended
		
		const string GfleDllName = "libgfle340.dll";
		
		[DllImport(GfleDllName, EntryPoint = "gflNegative", CharSet = CharSet.Auto)]
		internal static extern Error Negative(ref Gfl.Bitmap src, ref IntPtr dst);
		
		[DllImport(GfleDllName, EntryPoint = "gflReduceNoise", CharSet = CharSet.Auto)]
		internal static extern Error ReduceNoise(ref Gfl.Bitmap src, ref IntPtr dst);
		
		[DllImport(GfleDllName, EntryPoint = "gflLogCorrection", CharSet = CharSet.Auto)]
		internal static extern Error LogCorrection(ref Gfl.Bitmap src, ref IntPtr dst);
		
		[DllImport(GfleDllName, EntryPoint = "gflNormalize", CharSet = CharSet.Auto)]
		internal static extern Error Normalize(ref Gfl.Bitmap src, ref IntPtr dst);
		
		[DllImport(GfleDllName, EntryPoint = "gflEqualize", CharSet = CharSet.Auto)]
		internal static extern Error Equalize(ref Gfl.Bitmap src, ref IntPtr dst);
		
		[DllImport(GfleDllName, EntryPoint = "gflEqualizeOnLuminance", CharSet = CharSet.Auto)]
		internal static extern Error EqualizeOnLuminance(ref Gfl.Bitmap src, ref IntPtr dst);
		
		[DllImport(GfleDllName, EntryPoint = "gflAutomaticContrast", CharSet = CharSet.Auto)]
		internal static extern Error AutomaticContrast(ref Gfl.Bitmap src, ref IntPtr dst);
		
		[DllImport(GfleDllName, EntryPoint = "gflAutomaticLevels", CharSet = CharSet.Auto)]
		internal static extern Error AutomaticLevels(ref Gfl.Bitmap src, ref IntPtr dst);
		
		[DllImport(GfleDllName, EntryPoint = "gflEnhanceDetail", CharSet = CharSet.Auto)]
		internal static extern Error EnhanceDetail(ref Gfl.Bitmap src, ref IntPtr dst);
		
		[DllImport(GfleDllName, EntryPoint = "gflEnhanceFocus", CharSet = CharSet.Auto)]
		internal static extern Error EnhanceFocus(ref Gfl.Bitmap src, ref IntPtr dst);
		
		[DllImport(GfleDllName, EntryPoint = "gflFocusRestoration", CharSet = CharSet.Auto)]
		internal static extern Error FocusRestoration(ref Gfl.Bitmap src, ref IntPtr dst);
		
		[DllImport(GfleDllName, EntryPoint = "gflEdgeDetectLight", CharSet = CharSet.Auto)]
		internal static extern Error EdgeDetectLight(ref Gfl.Bitmap src, ref IntPtr dst);
		
		[DllImport(GfleDllName, EntryPoint = "gflEdgeDetectMedium", CharSet = CharSet.Auto)]
		internal static extern Error EdgeDetectMedium(ref Gfl.Bitmap src, ref IntPtr dst);
		
		[DllImport(GfleDllName, EntryPoint = "gflEdgeDetectHeavy", CharSet = CharSet.Auto)]
		internal static extern Error EdgeDetectHeavy(ref Gfl.Bitmap src, ref IntPtr dst);
		
		[DllImport(GfleDllName, EntryPoint = "gflEmboss", CharSet = CharSet.Auto)]
		internal static extern Error Emboss(ref Gfl.Bitmap src, ref IntPtr dst);
		
		[DllImport(GfleDllName, EntryPoint = "gflEmbossMore", CharSet = CharSet.Auto)]
		internal static extern Error EmbossMore(ref Gfl.Bitmap src, ref IntPtr dst);
		
		#endregion
		
		#region Error
		
		[DllImport(GflDllName, EntryPoint = "gflGetErrorString", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.LPStr)]
		internal static extern string GetErrorString(Error error);
		
		#endregion
	}
}
