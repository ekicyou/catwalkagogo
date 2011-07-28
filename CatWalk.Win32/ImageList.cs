﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Drawing;

namespace CatWalk.Win32 {
	/// <summary>
	/// A wrapper of IImageList.
	/// </summary>
	/// <remarks>
	/// This works on XP or higher.
	/// </remarks>
	public class ImageList : IDisposable {
		private ImageListSize _Size;
		private IImageList _ImageList;
		private static Guid IID_ImageList = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");

		public ImageList(ImageListSize size){
			this._Size = size;
			this._SizePixels = new Lazy<Int32Size>(this.GetSizePixels);
			var hresult = Win32Api.SHGetImageList(size, ref IID_ImageList, out this._ImageList);
			Marshal.ThrowExceptionForHR(hresult);
		}

		#region Method

		public int GetIconIndex(string path){
			var options = SHGetFileInfoOptions.SysIconIndex;
			var shfi = new SHFileInfo();
			var shfiSize = Marshal.SizeOf(shfi.GetType());
			IntPtr retVal = Win32Api.SHGetFileInfo(path, FileAttributes.None, ref shfi, shfiSize, options);
			if(shfi.hIcon != IntPtr.Zero){
				Win32Api.DestroyIcon(shfi.hIcon);
			}

			if (retVal.Equals(IntPtr.Zero)){
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}else{
				return shfi.iIcon;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <param name="overlayIndex">Index of the overlay icon that use for Draw or GetIndexOfOverlay method.</param>
		/// <returns></returns>
		public int GetIconIndexWithOverlay(string path, out int overlayIndex){
			var options = SHGetFileInfoOptions.SysIconIndex | SHGetFileInfoOptions.OverlayIndex | SHGetFileInfoOptions.Icon;
			var shfi = new SHFileInfo();
			var shfiSize = Marshal.SizeOf(shfi.GetType());
			IntPtr retVal = Win32Api.SHGetFileInfo(path, FileAttributes.None, ref shfi, shfiSize, options);
			if(shfi.hIcon != IntPtr.Zero){
				Win32Api.DestroyIcon(shfi.hIcon);
			}

			if (retVal.Equals(IntPtr.Zero)){
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}else{
				int idx = shfi.iIcon & 0xFFFFFF;
				int iOverlay = shfi.iIcon >> 24;
				overlayIndex = iOverlay;
				return idx;
			}
		}

		public Icon GetIcon(int index){
			return this.GetIcon(index, ImageListDrawOptions.Normal);
		}
		public Icon GetIcon(int index, ImageListDrawOptions options){
			IntPtr hIcon;
			var hresult = this._ImageList.GetIcon(index, options, out hIcon);
			Marshal.ThrowExceptionForHR(hresult);
			if(hIcon != IntPtr.Zero){
				return Icon.FromHandle(hIcon);
			}else{
				throw new Win32Exception();
			}
		}

		public Icon GetIcon(string path){
			return this.GetIcon(path, ImageListDrawOptions.Normal);
		}
		public Icon GetIcon(string path, ImageListDrawOptions options){
			return this.GetIcon(this.GetIconIndex(path), options);
		}

		private Int32Size GetSizePixels(){
			int x;
			int y;
			var hresult = this._ImageList.GetIconSize(out x, out y);
			Marshal.ThrowExceptionForHR(hresult);
			return new Int32Size(x, y);
		}

		public Bitmap Draw(int index, int overlayIndex, ImageListDrawOptions options){
			var bitmap = new Bitmap(this.Width, this.Height);
			using(var g = Graphics.FromImage(bitmap)){
				var param = new IMAGELISTDRAWPARAMS();
				param.cbSize = Marshal.SizeOf(param);
				param.himl = this.Handle;
				param.hdcDst = g.GetHdc();
				param.i = index;
				param.cx = param.cy = 0;
				param.fStyle = (int)options | (overlayIndex << 8);

				var hresult = this._ImageList.Draw(ref param);
				Marshal.ThrowExceptionForHR(hresult);
			}
			return bitmap;
		}

		public int GetIndexOfOverlay(int overlayIndex){
			int idx;
			var hresult = this._ImageList.GetOverlayImage(overlayIndex, out idx);
			Marshal.ThrowExceptionForHR(hresult);
			return idx;
		}

		#endregion

		#region Property

		public ImageListSize Size{
			get{
				return this._Size;
			}
		}

		private Lazy<Int32Size> _SizePixels;
		public int Width{
			get{
				return this._SizePixels.Value.Width;
			}
		}

		public int Height{
			get{
				return this._SizePixels.Value.Height;
			}
		}

		public IntPtr Handle{
			get{
				return Marshal.GetIUnknownForObject(this._ImageList);
			}
		}

		public static ImageListSize MaxSize{
			get{
				if(Environment.OSVersion.Platform == PlatformID.Win32NT){
					if(Environment.OSVersion.Version.Major >= 6){
						return ImageListSize.Jumbo;
					}else{
						return ImageListSize.ExtraLarge;
					}
				}
				return ImageListSize.Jumbo;
			}
		}

		#endregion


		#region IDisposable

		private bool _Disposed = false;

		public void Dispose(){
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		public virtual void Dispose(bool disposing){
			if (!this._Disposed){
				if (this._ImageList != null){
					Marshal.ReleaseComObject(this._ImageList);
				}
				this._ImageList = null;
			}
			this._Disposed = true;
		}

		~ImageList(){
			this.Dispose(false);
		}
	
		#endregion
	}

	#region enum

	public enum ImageListSize : int{
		Large = 0,
		Small = 1,
		ExtraLarge = 2,
		SystemSmall = 3,
		/// <summary>
		/// Vista or higher
		/// </summary>
		Jumbo = 4,
	}

	[Flags]
	public enum ImageListDrawOptions : uint{
		/// <summary>
		/// Draw item normally.
		/// </summary>
		Normal = 0x0,
		/// <summary>
		/// Draw item transparently.
		/// </summary>
		Transparent = 0x1,
		/// <summary>
		/// Draw item blended with 25% of the specified foreground colour
		/// or the Highlight colour if no foreground colour specified.
		/// </summary>
		Blend25 = 0x2,
		/// <summary>
		/// Draw item blended with 50% of the specified foreground colour
		/// or the Highlight colour if no foreground colour specified.
		/// </summary>
		Selected = 0x4,
		/// <summary>
		/// Draw the icon's mask
		/// </summary>
		Mask = 0x10,
		/// <summary>
		/// Draw the icon image without using the mask
		/// </summary>
		Image = 0x20,
		/// <summary>
		/// Draw the icon using the ROP specified.
		/// </summary>
		Rop = 0x40,
		/// <summary>
		/// Preserves the alpha channel in dest. XP only.
		/// </summary>
		PreserveAlpha = 0x1000,
		/// <summary>
		/// Scale the image to cx, cy instead of clipping it.  XP only.
		/// </summary>
		Scale = 0x2000,
		/// <summary>
		/// Scale the image to the current DPI of the display. XP only.
		/// </summary>
		DpiScale = 0x4000
	}

	#endregion

	#region IImageList

	[ComImportAttribute()]
	[GuidAttribute("46EB5926-582E-4017-9FDF-E8998DAA0950")]
	[InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IImageList{
		[PreserveSig]
		int Add(
			IntPtr hbmImage, 
			IntPtr hbmMask, 
			ref int pi);

		[PreserveSig]
		int ReplaceIcon(
			int i, 
			IntPtr hicon, 
			ref int pi);

		[PreserveSig]
		int SetOverlayImage(
			int iImage, 
			int iOverlay);

		[PreserveSig]
		int Replace(
			int i,
			IntPtr hbmImage, 
			IntPtr hbmMask);

		[PreserveSig]
		int AddMasked(
			IntPtr hbmImage, 
			int crMask, 
			ref int pi);

		[PreserveSig]
		int Draw(
			ref IMAGELISTDRAWPARAMS pimldp);

		[PreserveSig]
		int Remove(
			int i);

		[PreserveSig]
		int GetIcon(
			int i, 
			ImageListDrawOptions flags, 
			out IntPtr picon);

		[PreserveSig]
		int GetImageInfo(
			int i, 
			ref IMAGEINFO pImageInfo);

		[PreserveSig]
		int Copy(
			int iDst, 
			IImageList punkSrc, 
			int iSrc, 
			int uFlags);

		[PreserveSig]
		int Merge(
			int i1, 
			IImageList punk2, 
			int i2, 
			int dx, 
			int dy, 
			ref Guid riid, 
			ref IntPtr ppv);

		[PreserveSig]
		int Clone(
			ref Guid riid, 
			ref IntPtr ppv);

		[PreserveSig]
		int GetImageRect(
			int i, 
			ref Rectangle prc);

		[PreserveSig]
		int GetIconSize(
			out int cx, 
			out int cy);

		[PreserveSig]
		int SetIconSize(
			int cx, 
			int cy);

		[PreserveSig]
		int GetImageCount(
			ref int pi);

		[PreserveSig]
		int SetImageCount(
			int uNewCount);

		[PreserveSig]
		int SetBkColor(
			int clrBk, 
			ref int pclr);

		[PreserveSig]
		int GetBkColor(
			ref int pclr);

		[PreserveSig]
		int BeginDrag(
			int iTrack, 
			int dxHotspot, 
			int dyHotspot);

		[PreserveSig]
		int EndDrag();

		[PreserveSig]
		int DragEnter(
			IntPtr hwndLock, 
			int x, 
			int y);

		[PreserveSig]
		int DragLeave(
			IntPtr hwndLock);

		[PreserveSig]
		int DragMove(
			int x, 
			int y);

		[PreserveSig]
		int SetDragCursorImage(
			ref IImageList punk, 
			int iDrag, 
			int dxHotspot, 
			int dyHotspot);

		[PreserveSig]
		int DragShowNolock(
			int fShow);

		[PreserveSig]
		int GetDragImage(
			ref Point ppt, 
			ref Point pptHotspot, 
			ref Guid riid, 
			ref IntPtr ppv);
			
		[PreserveSig]
		int GetItemFlags(
			int i, 
			ref int dwFlags);

		[PreserveSig]
		int GetOverlayImage(
			int iOverlay, 
			out int piIndex);
	};

	[StructLayout(LayoutKind.Sequential)]
	public struct IMAGELISTDRAWPARAMS{
		public int cbSize;
		public IntPtr himl;
		public int i;
		public IntPtr hdcDst;
		public int x;
		public int y;
		public int cx;
		public int cy;
		public int xBitmap;        // x offest from the upperleft of bitmap
		public int yBitmap;        // y offset from the upperleft of bitmap
		public int rgbBk;
		public int rgbFg;
		public int fStyle;
		public int dwRop;
		public int fState;
		public int Frame;
		public int crEffect;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct IMAGEINFO{
		public IntPtr hbmImage;
		public IntPtr hbmMask;
		public int Unused1;
		public int Unused2;
		public Rectangle rcImage;
	}

	#endregion
}