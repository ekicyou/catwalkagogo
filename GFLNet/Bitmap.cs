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
	using Drawing = System.Drawing;
	using Imaging = System.Drawing.Imaging;

	public class Bitmap : IDisposable{
		private Gfl.Bitmap bitmap;
		private ImageInfo info;
		
		internal Bitmap(Gfl.Bitmap bitmap, Gfl.FileInformation info) : this(bitmap, new ImageInfo(info)){
		}
		
		internal Bitmap(Gfl.Bitmap bitmap, ImageInfo info){
			this.bitmap = bitmap;
			this.info = info;
		}

		#region Load

		public static Bitmap FromFile(string path){
			return FromFile(path, null);
		}
		
		public static Bitmap FromFile(string path, GflProgressCallback progressCallback){
			Gfl.LoadParams prms = new Gfl.LoadParams();
			Gfl.GetDefaultLoadParams(ref prms);
			prms.Options = Gfl.LoadOptions.ForceColorModel | Gfl.LoadOptions.IgnoreReadError | Gfl.LoadOptions.OnlyFirstFrame;
			prms.ColorModel = Gfl.BitmapType.Bgra;
			if(progressCallback != null){
				prms.Callbacks.Progress = new Gfl.ProgressCallback(delegate(int percent, IntPtr userParams){
					progressCallback(null, new GflProgressEventArgs(percent));
				});
			}
			
			IntPtr ptr = IntPtr.Zero;
			Gfl.FileInformation info = new Gfl.FileInformation();
			try{
				Gfl.Error error = Gfl.LoadBitmap(path, ref ptr, ref prms, ref info);

				switch(error){
					case Gfl.Error.None:
						Gfl.Bitmap bitmap = (Gfl.Bitmap)Marshal.PtrToStructure(ptr, typeof(Gfl.Bitmap));
						return new Bitmap(bitmap, info);
					case Gfl.Error.FileOpen:
					case Gfl.Error.FileRead:
					case Gfl.Error.FileCreate:
					case Gfl.Error.FileWrite:
						throw new IOException(Gfl.GetErrorString(error));
					case Gfl.Error.NoMemory:
						throw new OutOfMemoryException(Gfl.GetErrorString(error));
					case Gfl.Error.BadBitmap:
						throw new FormatException(Gfl.GetErrorString(error));
					default:
						throw new ApplicationException(Gfl.GetErrorString(error));
				}
			}finally{
				Gfl.FreeFileInformation(ref info);
			}
		}

		#endregion

		#region Effects

		public Bitmap ReduceNoise(){
			IntPtr ptr = IntPtr.Zero;
			if(Gfl.ReduceNoise(ref this.bitmap, ref ptr) == Gfl.Error.None){
				return new Bitmap((Gfl.Bitmap)Marshal.PtrToStructure(ptr, typeof(Gfl.Bitmap)), this.info);
			}else{
				throw new InvalidOperationException();
			}
		}

		public Bitmap Negative(){
			IntPtr ptr = IntPtr.Zero;
			if(Gfl.Negative(ref this.bitmap, ref ptr) == Gfl.Error.None){
				return new Bitmap((Gfl.Bitmap)Marshal.PtrToStructure(ptr, typeof(Gfl.Bitmap)), this.info);
			}else{
				throw new InvalidOperationException();
			}
		}

		public Bitmap LogCorrection(){
			IntPtr ptr = IntPtr.Zero;
			if(Gfl.LogCorrection(ref this.bitmap, ref ptr) == Gfl.Error.None){
				return new Bitmap((Gfl.Bitmap)Marshal.PtrToStructure(ptr, typeof(Gfl.Bitmap)), this.info);
			}else{
				throw new InvalidOperationException();
			}
		}

		public Bitmap Normalize(){
			IntPtr ptr = IntPtr.Zero;
			if(Gfl.Normalize(ref this.bitmap, ref ptr) == Gfl.Error.None){
				return new Bitmap((Gfl.Bitmap)Marshal.PtrToStructure(ptr, typeof(Gfl.Bitmap)), this.info);
			}else{
				throw new InvalidOperationException();
			}
		}
		
		public Bitmap Equalize(){
			IntPtr ptr = IntPtr.Zero;
			if(Gfl.Equalize(ref this.bitmap, ref ptr) == Gfl.Error.None){
				return new Bitmap((Gfl.Bitmap)Marshal.PtrToStructure(ptr, typeof(Gfl.Bitmap)), this.info);
			}else{
				throw new InvalidOperationException();
			}
		}

		public Bitmap EqualizeOnLuminance(){
			IntPtr ptr = IntPtr.Zero;
			if(Gfl.EqualizeOnLuminance(ref this.bitmap, ref ptr) == Gfl.Error.None){
				return new Bitmap((Gfl.Bitmap)Marshal.PtrToStructure(ptr, typeof(Gfl.Bitmap)), this.info);
			}else{
				throw new InvalidOperationException();
			}
		}

		public Bitmap AutomaticContrast(){
			IntPtr ptr = IntPtr.Zero;
			if(Gfl.AutomaticContrast(ref this.bitmap, ref ptr) == Gfl.Error.None){
				return new Bitmap((Gfl.Bitmap)Marshal.PtrToStructure(ptr, typeof(Gfl.Bitmap)), this.info);
			}else{
				throw new InvalidOperationException();
			}
		}

		public Bitmap AutomaticLevels(){
			IntPtr ptr = IntPtr.Zero;
			if(Gfl.AutomaticLevels(ref this.bitmap, ref ptr) == Gfl.Error.None){
				return new Bitmap((Gfl.Bitmap)Marshal.PtrToStructure(ptr, typeof(Gfl.Bitmap)), this.info);
			}else{
				throw new InvalidOperationException();
			}
		}

		public Bitmap EnhanceDetail(){
			IntPtr ptr = IntPtr.Zero;
			if(Gfl.EnhanceDetail(ref this.bitmap, ref ptr) == Gfl.Error.None){
				return new Bitmap((Gfl.Bitmap)Marshal.PtrToStructure(ptr, typeof(Gfl.Bitmap)), this.info);
			}else{
				throw new InvalidOperationException();
			}
		}

		public Bitmap EnhanceForcus(){
			IntPtr ptr = IntPtr.Zero;
			if(Gfl.EnhanceFocus(ref this.bitmap, ref ptr) == Gfl.Error.None){
				return new Bitmap((Gfl.Bitmap)Marshal.PtrToStructure(ptr, typeof(Gfl.Bitmap)), this.info);
			}else{
				throw new InvalidOperationException();
			}
		}

		public Bitmap FocusRestoration(){
			IntPtr ptr = IntPtr.Zero;
			if(Gfl.FocusRestoration(ref this.bitmap, ref ptr) == Gfl.Error.None){
				return new Bitmap((Gfl.Bitmap)Marshal.PtrToStructure(ptr, typeof(Gfl.Bitmap)), this.info);
			}else{
				throw new InvalidOperationException();
			}
		}

		public Bitmap EdgeDetectLight(){
			IntPtr ptr = IntPtr.Zero;
			if(Gfl.EdgeDetectLight(ref this.bitmap, ref ptr) == Gfl.Error.None){
				return new Bitmap((Gfl.Bitmap)Marshal.PtrToStructure(ptr, typeof(Gfl.Bitmap)), this.info);
			}else{
				throw new InvalidOperationException();
			}
		}

		public Bitmap EdgeDetectMedium(){
			IntPtr ptr = IntPtr.Zero;
			if(Gfl.EdgeDetectMedium(ref this.bitmap, ref ptr) == Gfl.Error.None){
				return new Bitmap((Gfl.Bitmap)Marshal.PtrToStructure(ptr, typeof(Gfl.Bitmap)), this.info);
			}else{
				throw new InvalidOperationException();
			}
		}

		public Bitmap EdgeDetectHeavy(){
			IntPtr ptr = IntPtr.Zero;
			if(Gfl.EdgeDetectHeavy(ref this.bitmap, ref ptr) == Gfl.Error.None){
				return new Bitmap((Gfl.Bitmap)Marshal.PtrToStructure(ptr, typeof(Gfl.Bitmap)), this.info);
			}else{
				throw new InvalidOperationException();
			}
		}

		public Bitmap Emboss(){
			IntPtr ptr = IntPtr.Zero;
			if(Gfl.Emboss(ref this.bitmap, ref ptr) == Gfl.Error.None){
				return new Bitmap((Gfl.Bitmap)Marshal.PtrToStructure(ptr, typeof(Gfl.Bitmap)), this.info);
			}else{
				throw new InvalidOperationException();
			}
		}

		#endregion

		[DllImport("KERNEL32.DLL", EntryPoint = "RtlMoveMemory", CharSet = CharSet.Auto)]
		private static extern void CopyMemory(IntPtr dst, IntPtr src, int length);

		public Drawing.Bitmap ToGdiBitmap(){
			int length = (int)this.bitmap.BytesPerLine * this.bitmap.Height;
			
			Drawing.Bitmap bitmap = new Drawing.Bitmap(
				this.bitmap.Width,
				this.bitmap.Height,
				Imaging.PixelFormat.Format32bppArgb);
			
			Imaging.BitmapData bitmapData = bitmap.LockBits(
				new Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
				Imaging.ImageLockMode.ReadWrite,
				bitmap.PixelFormat);
			CopyMemory(bitmapData.Scan0, this.bitmap.Data, length);
			bitmap.UnlockBits(bitmapData);
			
			return bitmap;
		}
		
		public ImageInfo FileInfo{
			get{
				return this.info;
			}
		}
		
		private Exif exif = null;
		public Exif Exif{
			get{
				if(this.exif == null){
					this.exif = new Exif(Gfl.GetExif(ref this.bitmap, Gfl.GetExifOptions.None));
				}
				return this.exif;
			}
		}

		public void Dispose(){
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		~Bitmap(){
			this.Dispose(false);
		}
		
		private bool disposed = false;
		protected virtual void Dispose(bool disposing){
			if(!(this.disposed)){
				Gfl.FreeBitmapData(ref this.bitmap);
				Gfl.FreeBitmap(ref this.bitmap);
				this.disposed = true;
			}
		}
	}
}
