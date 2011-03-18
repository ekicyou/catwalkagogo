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

	public class Bitmap : IDisposable{
		private Gfl.GflBitmap bitmap;
		private Gfl gfl;
		
		internal Bitmap(Gfl gfl, ref Gfl.GflBitmap bitmap){
			this.gfl = gfl;
			this.bitmap = bitmap;
			this.gfl.LoadedBitmap.Add(new WeakReference(this));
		}

		#region Functions

		public Bitmap Resize(int width, int height, ResizeMethod method){
			var dstPt = IntPtr.Zero;
			this.gfl.ThrowIfError(this.gfl.Resize(ref this.bitmap, ref dstPt, width, height, method));

			var dst = (Gfl.GflBitmap)Marshal.PtrToStructure(dstPt, typeof(Gfl.GflBitmap));
			return new Bitmap(this.gfl, ref dst);
		}

		public Bitmap ResizeCanvas(int width, int height, ResizeMethod method, ResizeCanvasOrigin origin){
			var bg = new Gfl.GflColor();
			var dstPt = IntPtr.Zero;
			this.gfl.ThrowIfError(this.gfl.ResizeCanvas(ref this.bitmap, ref dstPt, width, height, method, origin, ref bg));

			var dst = (Gfl.GflBitmap)Marshal.PtrToStructure(dstPt, typeof(Gfl.GflBitmap));
			return new Bitmap(this.gfl, ref dst);
		}

		public Bitmap Rotate(int angle, Color background){
			var bg = background.ToGflColor();
			var dstPt = IntPtr.Zero;
			this.gfl.ThrowIfError(this.gfl.Rotate(ref this.bitmap, ref dstPt, angle, ref bg));

			var dst = (Gfl.GflBitmap)Marshal.PtrToStructure(dstPt, typeof(Gfl.GflBitmap));
			return new Bitmap(this.gfl, ref dst);
		}

		public Bitmap RotateFine(double angle, Color background){
			var bg = background.ToGflColor();
			var dstPt = IntPtr.Zero;
			this.gfl.ThrowIfError(this.gfl.RotateFine(ref this.bitmap, ref dstPt, angle, ref bg));

			var dst = (Gfl.GflBitmap)Marshal.PtrToStructure(dstPt, typeof(Gfl.GflBitmap));
			return new Bitmap(this.gfl, ref dst);
		}

		public Bitmap FlipHorizontal(){
			var dstPt = IntPtr.Zero;
			this.gfl.ThrowIfError(this.gfl.FlipHorizontal(ref this.bitmap, ref dstPt));

			var dst = (Gfl.GflBitmap)Marshal.PtrToStructure(dstPt, typeof(Gfl.GflBitmap));
			return new Bitmap(this.gfl, ref dst);
		}

		public Bitmap FlipVertical(){
			var dstPt = IntPtr.Zero;
			this.gfl.ThrowIfError(this.gfl.FlipVertical(ref this.bitmap, ref dstPt));

			var dst = (Gfl.GflBitmap)Marshal.PtrToStructure(dstPt, typeof(Gfl.GflBitmap));
			return new Bitmap(this.gfl, ref dst);
		}
		/*
		private IntPtr GetDestBitmap(){
			//var dstPt = this.gfl.AllockBitmap(this.bitmap.Type, this.bitmap.Width, this.bitmap.Height, this.bitmap.LinePadding, ref bg);
			var dst = new Gfl.GflBitmap();
			var dstPt = Marshal.AllocHGlobal(Marshal.SizeOf(dst));
			Marshal.StructureToPtr(dst, dstPt, true);
			return dstPt;
		}
		*/
		public void ExportIntoClipboard(){
			this.gfl.ThrowIfError(this.gfl.ExportIntoClipboard(ref this.bitmap));
		}

		#endregion

		#region Effects
		/*
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
		*/
		#endregion
		/*
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
		*/
		#region Property

		public int BytesPerLine{
			get{
				return (int)this.bitmap.BytesPerLine;
			}
		}

		public int BytesPerPixel{
			get{
				return (int)this.bitmap.BytesPerPixel;
			}
		}

		public int Width{
			get{
				return this.bitmap.Width;
			}
		}

		public int Height{
			get{
				return this.bitmap.Height;
			}
		}

		public IntPtr Scan0{
			get{
				return this.bitmap.Data;
			}
		}

		public int TransparentIndex{
			get{
				return this.bitmap.TransparentIndex;
			}
		}

		public int BitsPerComponent{
			get{
				return this.bitmap.BitsPerComponent;
			}
		}

		public int LinePadding{
			get{
				return this.bitmap.LinePadding;
			}
		}

		public Origin Origin{
			get{
				return this.bitmap.Origin;
			}
		}

		public int UsedColorCount{
			get{
				return this.bitmap.ColorUsed;
			}
		}

		private ColorMap colorMap = null;
		public ColorMap ColorMap{
			get{
				if((this.colorMap == null) && (this.bitmap.ColorMap != IntPtr.Zero)){
					this.colorMap = new ColorMap(this.bitmap.ColorMap);
				}
				return this.colorMap;
			}
		}
		
		private Exif exif = null;
		public Exif Exif{
			get{
				if(this.exif == null && this.bitmap.MetaData != IntPtr.Zero){
					var ptr = this.gfl.BitmapGetEXIF(ref this.bitmap, Gfl.GetExifOptions.WantMakerNotes);
					var exifData = new Gfl.GflExifData();
					Marshal.PtrToStructure(ptr, exifData);
					this.exif = new Exif(exifData);
					this.gfl.FreeEXIF(ref exifData);
				}
				return this.exif;
			}
		}

		public void SaveBitmap(string filename){
			var prms = new Gfl.GflSaveParams();
			this.gfl.GetDefaultSaveParams(ref prms);
			this.gfl.ThrowIfError(this.gfl.SaveBitmap(filename, ref this.bitmap, ref prms));
		}

		#endregion

		#region IDisposable

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
				this.gfl.FreeBitmapData(ref this.bitmap);
				this.gfl.FreeBitmap(ref this.bitmap);
				this.gfl.LoadedBitmap.RemoveAll(wref => wref.Target == this);
				this.disposed = true;
			}
		}

		#endregion
	}
}
