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
	public partial class Gfl{
		[DllImport("kernel32.dll")]
		private static extern IntPtr LoadLibrary(String lpFileName);
		[DllImport("kernel32.dll")]
		private static extern IntPtr GetProcAddress(IntPtr hModule, String lpProcName);
		[DllImport("kernel32.dll")]
		private static extern Boolean FreeLibrary(IntPtr hLibModule);

		private T LoadMethod<T>(string name) where T : class{
			return LoadMethod<T>(name, this.GflHandle);
		}

		private static T LoadMethod<T>(string name, IntPtr hModule) where T : class{
			return Marshal.GetDelegateForFunctionPointer(GetProcAddress(hModule, name), typeof(T)) as T;
		}

		#region Initialization
		
		private delegate Error LibraryInitDelegate();
		private LibraryInitDelegate _LibraryInitDelegate;
		internal Error LibraryInit(){
			this.ThrowIfDisposed();
			if(this._LibraryInitDelegate == null){
				this._LibraryInitDelegate = this.LoadMethod<LibraryInitDelegate>("gflLibraryInit");
			}
			return this._LibraryInitDelegate();
		}

		private delegate Error LibraryExitDelegate();
		private LibraryExitDelegate _LibraryExitDelegate;
		internal Error LibraryExit(){
			this.ThrowIfDisposed();
			if(this._LibraryExitDelegate == null){
				this._LibraryExitDelegate = this.LoadMethod<LibraryExitDelegate>("gflLibraryExit");
			}
			return this._LibraryExitDelegate();
		}

		private delegate void EnableLZWDelegate(bool enable);
		private EnableLZWDelegate _EnableLZWDelegate;
		internal void EnableLZW(bool enable){
			this.ThrowIfDisposed();
			if(this._EnableLZWDelegate == null){
				this._EnableLZWDelegate = this.LoadMethod<EnableLZWDelegate>("gflEnableLZW");
			}
			this._EnableLZWDelegate(enable);
		}

		
		private delegate IntPtr GetVersionDelegate();
		private GetVersionDelegate _GetVersionDelegate;
		internal IntPtr GetVersion(){
			this.ThrowIfDisposed();
			if(this._GetVersionDelegate == null){
				this._GetVersionDelegate = this.LoadMethod<GetVersionDelegate>("gflGetVersion");
			}
			return this._GetVersionDelegate();
		}

		private delegate void SetPluginsPathnameDelegate(string path);
		private SetPluginsPathnameDelegate _SetPluginPathnameDelegate;
		internal void SetPluginPathname(string path){
			this.ThrowIfDisposed();
			if(this._SetPluginPathnameDelegate == null){
				this._SetPluginPathnameDelegate = this.LoadMethod<SetPluginsPathnameDelegate>("gflSetPluginsPathname");
			}
			this._SetPluginPathnameDelegate(path);
		}
		
		#endregion
		
		#region Allocation
		
		private delegate IntPtr AllockBitmapDelegate(BitmapType type, int width, int height, uint linePadding, ref Color backgroundColor);
		private AllockBitmapDelegate _AllockBitmapDelegate;
		internal IntPtr AllockBitmap(BitmapType type, int width, int height, uint linePadding, ref Color backgroundColor){
			this.ThrowIfDisposed();
			if(this._AllockBitmapDelegate == null){
				this._AllockBitmapDelegate = this.LoadMethod<AllockBitmapDelegate>("gflAllockBitmap");
			}
			return this._AllockBitmapDelegate(type, width, height, linePadding, ref backgroundColor);
		}

		private delegate void FreeBitmapDelegate(ref Gfl.Bitmap bitmap);
		private FreeBitmapDelegate _FreeBitmapDelegate;
		internal void FreeBitmap(ref Gfl.Bitmap bitmap){
			this.ThrowIfDisposed();
			if(this._FreeBitmapDelegate == null){
				this._FreeBitmapDelegate = this.LoadMethod<FreeBitmapDelegate>("gflFreeBitmap");
			}
			this._FreeBitmapDelegate(ref bitmap);
		}
		
		private delegate IntPtr CloneBitmapDelegate(ref Gfl.Bitmap bitmap);
		private CloneBitmapDelegate _CloneBitmapDelegate;
		internal IntPtr CloneBitmap(ref Gfl.Bitmap bitmap){
			this.ThrowIfDisposed();
			if(this._CloneBitmapDelegate == null){
				this._CloneBitmapDelegate = this.LoadMethod<CloneBitmapDelegate>("gflCloneBitmap");
			}
			return this._CloneBitmapDelegate(ref bitmap);
		}

		private delegate void FreeBitmapDataDelegate(ref Gfl.Bitmap bitmap);
		private FreeBitmapDataDelegate _FreeBitmapDataDelegate;
		internal void FreeBitmapData(ref Gfl.Bitmap bitmap){
			this.ThrowIfDisposed();
			if(this._FreeBitmapDataDelegate == null){
				this._FreeBitmapDataDelegate = this.LoadMethod<FreeBitmapDataDelegate>("gflFreeBitmapData");
			}
			this._FreeBitmapDataDelegate(ref bitmap);
		}

		private delegate void FreeFileInformationDelegate(ref Gfl.FileInformation info);
		private FreeFileInformationDelegate _FreeFileInformationDelegate;
		internal void FreeFileInformation(ref Gfl.FileInformation info){
			this.ThrowIfDisposed();
			if(this._FreeFileInformationDelegate == null){
				this._FreeFileInformationDelegate = this.LoadMethod<FreeFileInformationDelegate>("gflFreeFileInformation");
			}
			this._FreeFileInformationDelegate(ref info);
		}
		
		#endregion
		
		#region Format
		
		private delegate int GetNumberOfFormatDelegate();
		private GetNumberOfFormatDelegate _GetNumberOfFormatDelegate;
		internal int GetNumberOfFormat(){
			this.ThrowIfDisposed();
			if(this._GetNumberOfFormatDelegate == null){
				this._GetNumberOfFormatDelegate = this.LoadMethod<GetNumberOfFormatDelegate>("gflGetNumberOfFormat");
			}
			return this._GetNumberOfFormatDelegate();
		}
		
		private delegate bool FormatIsSupportedDelegate(string name);
		private FormatIsSupportedDelegate _FormatIsSupportedDelegate;
		internal bool FormatIsSupported(string name){
			this.ThrowIfDisposed();
			if(this._FormatIsSupportedDelegate == null){
				this._FormatIsSupportedDelegate = this.LoadMethod<FormatIsSupportedDelegate>("gflFormatIsSupported");
			}
			return this._FormatIsSupportedDelegate(name);
		}

		private delegate string GetFormatNameByIndexDelegate(int index);
		private GetFormatNameByIndexDelegate _GetFormatNameByIndexDelegate;
		internal string GetFormatNameByIndex(int index){
			this.ThrowIfDisposed();
			if(this._GetFormatNameByIndexDelegate == null){
				this._GetFormatNameByIndexDelegate = this.LoadMethod<GetFormatNameByIndexDelegate>("gflGetFormatNameByIndex");
			}
			return this._GetFormatNameByIndexDelegate(index);
		}

		private delegate string GetFormatIndexByNameDelegate(string name);
		private GetFormatIndexByNameDelegate _GetFormatIndexByNameDelegate;
		internal string GetFormatIndexByName(string name){
			this.ThrowIfDisposed();
			if(this._GetFormatIndexByNameDelegate == null){
				this._GetFormatIndexByNameDelegate = this.LoadMethod<GetFormatIndexByNameDelegate>("gflGetFormatIndexByName");
			}
			return this._GetFormatIndexByNameDelegate(name);
		}

		private delegate bool FormatIsReadableByIndexDelegate(int index);
		private FormatIsReadableByIndexDelegate _FormatIsReadableByIndexDelegate;
		internal bool FormatIsReadableByIndex(int index){
			this.ThrowIfDisposed();
			if(this._FormatIsReadableByIndexDelegate == null){
				this._FormatIsReadableByIndexDelegate = this.LoadMethod<FormatIsReadableByIndexDelegate>("gflFormatIsReadableByIndex");
			}
			return this._FormatIsReadableByIndexDelegate(index);
		}

		private delegate bool FormatIsReadableByNameDelegate(string name);
		private FormatIsReadableByNameDelegate _FormatIsReadableByNameDelegate;
		internal bool FormatIsReadableByName(string name){
			this.ThrowIfDisposed();
			if(this._FormatIsReadableByNameDelegate == null){
				this._FormatIsReadableByNameDelegate = this.LoadMethod<FormatIsReadableByNameDelegate>("gflFormatIsReadableByName");
			}
			return this._FormatIsReadableByNameDelegate(name);
		}

		private delegate bool FormatIsWritableByIndexDelegate(int index);
		private FormatIsWritableByIndexDelegate _FormatIsWritableByIndexDelegate;
		internal bool FormatIsWritableByIndex(int index){
			this.ThrowIfDisposed();
			if(this._FormatIsWritableByIndexDelegate == null){
				this._FormatIsWritableByIndexDelegate = this.LoadMethod<FormatIsWritableByIndexDelegate>("gflFormatIsWritableByIndex");
			}
			return this._FormatIsWritableByIndexDelegate(index);
		}

		private delegate bool FormatIsWritableByNameDelegate(string name);
		private FormatIsWritableByNameDelegate _FormatIsWritableByNameDelegate;
		internal bool FormatIsWritableByName(string name){
			this.ThrowIfDisposed();
			if(this._FormatIsWritableByNameDelegate == null){
				this._FormatIsWritableByNameDelegate = this.LoadMethod<FormatIsWritableByNameDelegate>("gflFormatIsWritableByName");
			}
			return this._FormatIsWritableByNameDelegate(name);
		}

		private delegate string GetFormatDescriptionByNameDelegate(string name);
		private GetFormatDescriptionByNameDelegate _GetFormatDescriptionByNameDelegate;
		internal string GetFormatDescriptionByName(string name){
			this.ThrowIfDisposed();
			if(this._GetFormatDescriptionByNameDelegate == null){
				this._GetFormatDescriptionByNameDelegate = this.LoadMethod<GetFormatDescriptionByNameDelegate>("gflGetFormatDescriptionByName");
			}
			return this._GetFormatDescriptionByNameDelegate(name);
		}

		private delegate string GetFormatDescriptionByIndexDelegate(int index);
		private GetFormatDescriptionByIndexDelegate _GetFormatDescriptionByIndexDelegate;
		internal string GetFormatDescriptionByIndex(int index){
			this.ThrowIfDisposed();
			if(this._GetFormatDescriptionByIndexDelegate == null){
				this._GetFormatDescriptionByIndexDelegate = this.LoadMethod<GetFormatDescriptionByIndexDelegate>("gflGetFormatDescriptionByIndex");
			}
			return this._GetFormatDescriptionByIndexDelegate(index);
		}

		private delegate string GetDefaultFormatSuffixByIndexByNameDelegate(string name);
		private GetDefaultFormatSuffixByIndexByNameDelegate _GetDefaultFormatSuffixByIndexByNameDelegate;
		internal string GetDefaultFormatSuffixByIndexByName(string name){
			this.ThrowIfDisposed();
			if(this._GetDefaultFormatSuffixByIndexByNameDelegate == null){
				this._GetDefaultFormatSuffixByIndexByNameDelegate = this.LoadMethod<GetDefaultFormatSuffixByIndexByNameDelegate>("gflGetDefaultFormatSuffixByIndexByName");
			}
			return this._GetDefaultFormatSuffixByIndexByNameDelegate(name);
		}

		private delegate string GetDefaultFormatSuffixByIndexDelegate(int index);
		private GetDefaultFormatSuffixByIndexDelegate _GetDefaultFormatSuffixByIndexDelegate;
		internal string GetDefaultFormatSuffixByIndex(int index){
			this.ThrowIfDisposed();
			if(this._GetDefaultFormatSuffixByIndexDelegate == null){
				this._GetDefaultFormatSuffixByIndexDelegate = this.LoadMethod<GetDefaultFormatSuffixByIndexDelegate>("gflGetDefaultFormatSuffixByIndex");
			}
			return this._GetDefaultFormatSuffixByIndexDelegate(index);
		}

		private delegate Error GetFormatInformationByNameDelegate(string name, ref FormatInformation info);
		private GetFormatInformationByNameDelegate _GetFormatInformationByNameDelegate;
		internal Error GetFormatInformationByName(string name, ref FormatInformation info){
			this.ThrowIfDisposed();
			if(this._GetFormatInformationByNameDelegate == null){
				this._GetFormatInformationByNameDelegate = this.LoadMethod<GetFormatInformationByNameDelegate>("gflGetFormatInformationByName");
			}
			return this._GetFormatInformationByNameDelegate(name, ref info);
		}

		private delegate Error GetFormatInformationByIndexDelegate(int index, ref FormatInformation info);
		private GetFormatInformationByIndexDelegate _GetFormatInformationByIndexDelegate;
		internal Error GetFormatInformationByIndex(int index, ref FormatInformation info){
			this.ThrowIfDisposed();
			if(this._GetFormatInformationByIndexDelegate == null){
				this._GetFormatInformationByIndexDelegate = this.LoadMethod<GetFormatInformationByIndexDelegate>("gflGetFormatInformationByIndex");
			}
			return this._GetFormatInformationByIndexDelegate(index, ref info);
		}
		
		#endregion
		
		#region Read
		
		private delegate void GetDefaultLoadParamsDelegate(ref LoadParams prms);
		private GetDefaultLoadParamsDelegate _GetDefaultLoadParamsDelegate;
		internal void GetDefaultLoadParams(ref LoadParams prms){
			this.ThrowIfDisposed();
			if(this._GetDefaultLoadParamsDelegate == null){
				this._GetDefaultLoadParamsDelegate = this.LoadMethod<GetDefaultLoadParamsDelegate>("gflGetDefaultLoadParams");
			}
			this._GetDefaultLoadParamsDelegate(ref prms);
		}

		private delegate void GetDefaultThumbailParamsDelegate(ref LoadParams prms);
		private GetDefaultThumbailParamsDelegate _GetDefaultThumbailParamsDelegate;
		internal void GetDefaultThumbailParams(ref LoadParams prms){
			this.ThrowIfDisposed();
			if(this._GetDefaultThumbailParamsDelegate == null){
				this._GetDefaultThumbailParamsDelegate = this.LoadMethod<GetDefaultThumbailParamsDelegate>("gflGetDefaultThumbailParams");
			}
			this._GetDefaultThumbailParamsDelegate(ref prms);
		}

		private delegate Error LoadBitmapDelegate(string filename, ref IntPtr bitmap, ref LoadParams prms, ref FileInformation info);
		private LoadBitmapDelegate _LoadBitmapDelegate;
		internal Error LoadBitmap(string filename, ref IntPtr bitmap, ref LoadParams prms, ref FileInformation info){
			this.ThrowIfDisposed();
			if(this._LoadBitmapDelegate == null){
				this._LoadBitmapDelegate = this.LoadMethod<LoadBitmapDelegate>("gflLoadBitmap");
			}
			return this._LoadBitmapDelegate(filename, ref bitmap, ref prms, ref info);
		}

		private delegate Error LoadBitmapFromMemoryDelegate(IntPtr data, uint data_length, ref IntPtr bitmap, ref LoadParams prms, ref FileInformation info);
		private LoadBitmapFromMemoryDelegate _LoadBitmapFromMemoryDelegate;
		internal Error LoadBitmapFromMemory(IntPtr data, uint data_length, ref IntPtr bitmap, ref LoadParams prms, ref FileInformation info){
			this.ThrowIfDisposed();
			if(this._LoadBitmapFromMemoryDelegate == null){
				this._LoadBitmapFromMemoryDelegate = this.LoadMethod<LoadBitmapFromMemoryDelegate>("gflLoadBitmapFromMemory");
			}
			return this._LoadBitmapFromMemoryDelegate(data, data_length, ref bitmap, ref prms, ref info);
		}

		private delegate Error LoadBitmapFromHandleDelegate(IntPtr handle, ref IntPtr bitmap, ref LoadParams prms, ref FileInformation info);
		private LoadBitmapFromHandleDelegate _LoadBitmapFromHandleDelegate;
		internal Error LoadBitmapFromHandle(IntPtr handle, ref IntPtr bitmap, ref LoadParams prms, ref FileInformation info){
			this.ThrowIfDisposed();
			if(this._LoadBitmapFromHandleDelegate == null){
				this._LoadBitmapFromHandleDelegate = this.LoadMethod<LoadBitmapFromHandleDelegate>("gflLoadBitmapFromHandle");
			}
			return this._LoadBitmapFromHandleDelegate(handle, ref bitmap, ref prms, ref info);
		}
		
		private delegate Error GetFileInformationDelegate(string filename, int index, ref FileInformation info);
		private GetFileInformationDelegate _GetFileInformationDelegate;
		internal Error GetFileInformation(string filename, int index, ref FileInformation info){
			this.ThrowIfDisposed();
			if(this._GetFileInformationDelegate == null){
				this._GetFileInformationDelegate = this.LoadMethod<GetFileInformationDelegate>("gflGetFileInformation");
			}
			return this._GetFileInformationDelegate(filename, index, ref info);
		}

		private delegate Error LoadThumbnailDelegate(string filename, int width, int height, ref IntPtr bitmap, ref LoadParams prms, ref FileInformation info);
		private LoadThumbnailDelegate _LoadThumbnailDelegate;
		internal Error LoadThumbnail(string filename, int width, int height, ref IntPtr bitmap, ref LoadParams prms, ref FileInformation info){
			this.ThrowIfDisposed();
			if(this._LoadThumbnailDelegate == null){
				this._LoadThumbnailDelegate = this.LoadMethod<LoadThumbnailDelegate>("gflLoadThumbnail");
			}
			return this._LoadThumbnailDelegate(filename, width, height, ref bitmap, ref prms, ref info);
		}

		private delegate Error LoadThumbnailFromMemoryDelegate(IntPtr data, uint data_length, ref IntPtr bitmap, ref LoadParams prms, ref FileInformation info);
		private LoadThumbnailFromMemoryDelegate _LoadThumbnailFromMemoryDelegate;
		internal Error LoadThumbnailFromMemory(IntPtr data, uint data_length, ref IntPtr bitmap, ref LoadParams prms, ref FileInformation info){
			this.ThrowIfDisposed();
			if(this._LoadThumbnailFromMemoryDelegate == null){
				this._LoadThumbnailFromMemoryDelegate = this.LoadMethod<LoadThumbnailFromMemoryDelegate>("gflLoadThumbnailFromMemory");
			}
			return this._LoadThumbnailFromMemoryDelegate(data, data_length, ref bitmap, ref prms, ref info);
		}

		private delegate Error LoadThumbnailFromHandleDelegate(IntPtr handle, ref IntPtr bitmap, ref LoadParams prms, ref FileInformation info);
		private LoadThumbnailFromHandleDelegate _LoadThumbnailFromHandleDelegate;
		internal Error LoadThumbnailFromHandle(IntPtr handle, ref IntPtr bitmap, ref LoadParams prms, ref FileInformation info){
			this.ThrowIfDisposed();
			if(this._LoadThumbnailFromHandleDelegate == null){
				this._LoadThumbnailFromHandleDelegate = this.LoadMethod<LoadThumbnailFromHandleDelegate>("gflLoadThumbnailFromHandle");
			}
			return this._LoadThumbnailFromHandleDelegate(handle, ref bitmap, ref prms, ref info);
		}

		#endregion
		
		#region Write
		
		private delegate void GetDefaultSaveParamsDelegate(ref SaveParams prms);
		private GetDefaultSaveParamsDelegate _GetDefaultSaveParamsDelegate;
		internal void GetDefaultSaveParams(ref SaveParams prms){
			this.ThrowIfDisposed();
			if(this._GetDefaultSaveParamsDelegate == null){
				this._GetDefaultSaveParamsDelegate = this.LoadMethod<GetDefaultSaveParamsDelegate>("gflGetDefaultSaveParams");
			}
			this._GetDefaultSaveParamsDelegate(ref prms);
		}

		private delegate Error SaveBitmapDelegate(string filename, ref Gfl.Bitmap bitmap, ref SaveParams prms);
		private SaveBitmapDelegate _SaveBitmapDelegate;
		internal Error SaveBitmap(string filename, ref Gfl.Bitmap bitmap, ref SaveParams prms){
			this.ThrowIfDisposed();
			if(this._SaveBitmapDelegate == null){
				this._SaveBitmapDelegate = this.LoadMethod<SaveBitmapDelegate>("gflSaveBitmap");
			}
			return this._SaveBitmapDelegate(filename, ref bitmap, ref prms);
		}
		
		#endregion
		
		#region Metadata

		private delegate bool BitmapHasEXIFDelegate(ref Gfl.Bitmap bitmap);
		private BitmapHasEXIFDelegate _BitmapHasEXIFDelegate;
		internal bool BitmapHasEXIF(ref Gfl.Bitmap bitmap){
			this.ThrowIfDisposed();
			if(this._BitmapHasEXIFDelegate == null){
				this._BitmapHasEXIFDelegate = this.LoadMethod<BitmapHasEXIFDelegate>("gflBitmapHasEXIF");
			}
			return this._BitmapHasEXIFDelegate(ref bitmap);
		}

		private delegate bool BitmapHasIPTCDelegate(ref Gfl.Bitmap bitmap);
		private BitmapHasIPTCDelegate _BitmapHasIPTCDelegate;
		internal bool BitmapHasIPTC(ref Gfl.Bitmap bitmap){
			this.ThrowIfDisposed();
			if(this._BitmapHasIPTCDelegate == null){
				this._BitmapHasIPTCDelegate = this.LoadMethod<BitmapHasIPTCDelegate>("gflBitmapHasIPTC");
			}
			return this._BitmapHasIPTCDelegate(ref bitmap);
		}

		private delegate bool BitmapHasICCProfileDelegate(ref Gfl.Bitmap bitmap);
		private BitmapHasICCProfileDelegate _BitmapHasICCProfileDelegate;
		internal bool BitmapHasICCProfile(ref Gfl.Bitmap bitmap){
			this.ThrowIfDisposed();
			if(this._BitmapHasICCProfileDelegate == null){
				this._BitmapHasICCProfileDelegate = this.LoadMethod<BitmapHasICCProfileDelegate>("gflBitmapHasICCProfile");
			}
			return this._BitmapHasICCProfileDelegate(ref bitmap);
		}

		private delegate IntPtr BitmapGetEXIFDelegate(ref Gfl.Bitmap bitmap, GetExifOptions options);
		private BitmapGetEXIFDelegate _BitmapGetEXIFDelegate;
		internal IntPtr BitmapGetEXIF(ref Gfl.Bitmap bitmap, GetExifOptions options){
			this.ThrowIfDisposed();
			if(this._BitmapGetEXIFDelegate == null){
				this._BitmapGetEXIFDelegate = this.LoadMethod<BitmapGetEXIFDelegate>("gflBitmapGetEXIF");
			}
			return this._BitmapGetEXIFDelegate(ref bitmap, options);
		}

		private delegate IntPtr FreeEXIFDelegate(ref Gfl.ExifData exifData);
		private FreeEXIFDelegate _FreeEXIFDelegate;
		internal IntPtr FreeEXIF(ref Gfl.ExifData exifData){
			this.ThrowIfDisposed();
			if(this._FreeEXIFDelegate == null){
				this._FreeEXIFDelegate = this.LoadMethod<FreeEXIFDelegate>("gflFreeEXIF");
			}
			return this._FreeEXIFDelegate(ref exifData);
		}
		
		private delegate Error BitmapRemoveEXIFThumbnailDelegate(ref Gfl.Bitmap bitmap);
		private BitmapRemoveEXIFThumbnailDelegate _BitmapRemoveEXIFThumbnailDelegate;
		internal Error BitmapRemoveEXIFThumbnail(ref Gfl.Bitmap bitmap){
			this.ThrowIfDisposed();
			if(this._BitmapRemoveEXIFThumbnailDelegate == null){
				this._BitmapRemoveEXIFThumbnailDelegate = this.LoadMethod<BitmapRemoveEXIFThumbnailDelegate>("gflBitmapRemoveEXIFThumbnail");
			}
			return this._BitmapRemoveEXIFThumbnailDelegate(ref bitmap);
		}
		
		private delegate void BitmapRemoveMetaDataDelegate(ref Gfl.Bitmap bitmap);
		private BitmapRemoveMetaDataDelegate _BitmapRemoveMetaDataDelegate;
		internal void BitmapRemoveMetaData(ref Gfl.Bitmap bitmap){
			this.ThrowIfDisposed();
			if(this._BitmapRemoveMetaDataDelegate == null){
				this._BitmapRemoveMetaDataDelegate = this.LoadMethod<BitmapRemoveMetaDataDelegate>("gflBitmapRemoveMetaData");
			}
			this._BitmapRemoveMetaDataDelegate(ref bitmap);
		}

		private delegate void BitmapSetEXIFThumbnailDelegate(ref Gfl.Bitmap bitmap, ref Gfl.Bitmap thumbnail);
		private BitmapSetEXIFThumbnailDelegate _BitmapSetEXIFThumbnailDelegate;
		internal void BitmapSetEXIFThumbnail(ref Gfl.Bitmap bitmap, ref Gfl.Bitmap thumbnail){
			this.ThrowIfDisposed();
			if(this._BitmapSetEXIFThumbnailDelegate == null){
				this._BitmapSetEXIFThumbnailDelegate = this.LoadMethod<BitmapSetEXIFThumbnailDelegate>("gflBitmapSetEXIFThumbnail");
			}
			this._BitmapSetEXIFThumbnailDelegate(ref bitmap, ref thumbnail);
		}

		#endregion
		
		#region Error
		
		[return: MarshalAs(UnmanagedType.LPStr)]
		private delegate string GetErrorStringDelegate(Error error);
		private GetErrorStringDelegate _GetErrorStringDelegate;
		internal string GetErrorString(Error error){
			this.ThrowIfDisposed();
			if(this._GetErrorStringDelegate == null){
				this._GetErrorStringDelegate = this.LoadMethod<GetErrorStringDelegate>("gflGetErrorString");
			}
			return this._GetErrorStringDelegate(error);
		}

		#endregion
	}
}
