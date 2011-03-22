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
		public IntPtr Handle{get; private set;}
		private Gfl.GflBitmap _GflBitmap;
		private Gfl _Gfl;
		
		internal Bitmap(Gfl gfl, IntPtr handle){
			this.Handle = handle;
			this._Gfl = gfl;
			this._GflBitmap = (Gfl.GflBitmap)Marshal.PtrToStructure(handle, typeof(Gfl.GflBitmap));
			this._Gfl.AddBitmap(this);
		}

		#region Functions

		public Bitmap Resize(int width, int height, ResizeMethod method){
			this.ThrowIfDisposed();
			var pDest = IntPtr.Zero;
			this._Gfl.ThrowIfError(this._Gfl.Resize(this, ref pDest, width, height, method));
			return new Bitmap(this._Gfl, pDest);
		}
		
		public Bitmap ResizeCanvas(int width, int height, ResizeMethod method, ResizeCanvasOrigin origin){
			var bg = new Gfl.GflColor();
			var pDest = IntPtr.Zero;
			this._Gfl.ThrowIfError(this._Gfl.ResizeCanvas(this, ref pDest, width, height, method, origin, ref bg));
			return new Bitmap(this._Gfl, pDest);
		}

		public Bitmap Rotate(int angle, Color background){
			var bg = background.ToGflColor();
			var pDest = IntPtr.Zero;
			this._Gfl.ThrowIfError(this._Gfl.Rotate(this, ref pDest, angle, ref bg));
			return new Bitmap(this._Gfl, pDest);
		}

		public Bitmap RotateFine(double angle, Color background){
			var bg = background.ToGflColor();
			var pDest = IntPtr.Zero;
			this._Gfl.ThrowIfError(this._Gfl.RotateFine(this, ref pDest, angle, ref bg));
			return new Bitmap(this._Gfl, pDest);
		}

		public Bitmap FlipHorizontal(){
			var pDest = IntPtr.Zero;
			this._Gfl.ThrowIfError(this._Gfl.FlipHorizontal(this, ref pDest));
			return new Bitmap(this._Gfl, pDest);
		}

		public Bitmap FlipVertical(){
			var pDest = IntPtr.Zero;
			this._Gfl.ThrowIfError(this._Gfl.FlipVertical(this, ref pDest));
			return new Bitmap(this._Gfl, pDest);
		}
		
		public void ExportIntoClipboard(){
			this.ThrowIfDisposed();
			this._Gfl.ThrowIfError(this._Gfl.ExportIntoClipboard(this));
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

		#region Property

		public int BytesPerLine{
			get{
				this.ThrowIfDisposed();
				return (int)this._GflBitmap.BytesPerLine;
			}
		}

		public int BytesPerPixel{
			get{
				this.ThrowIfDisposed();
				return (int)this._GflBitmap.BytesPerPixel;
			}
		}

		public int Width{
			get{
				this.ThrowIfDisposed();
				return this._GflBitmap.Width;
			}
		}

		public int Height{
			get{
				this.ThrowIfDisposed();
				return this._GflBitmap.Height;
			}
		}

		public IntPtr Scan0{
			get{
				this.ThrowIfDisposed();
				return this._GflBitmap.Data;
			}
		}

		public int TransparentIndex{
			get{
				this.ThrowIfDisposed();
				return this._GflBitmap.TransparentIndex;
			}
		}

		public int BitsPerComponent{
			get{
				this.ThrowIfDisposed();
				return this._GflBitmap.BitsPerComponent;
			}
		}

		public int LinePadding{
			get{
				this.ThrowIfDisposed();
				return this._GflBitmap.LinePadding;
			}
		}

		public Origin Origin{
			get{
				this.ThrowIfDisposed();
				return this._GflBitmap.Origin;
			}
		}

		public int UsedColorCount{
			get{
				this.ThrowIfDisposed();
				return this._GflBitmap.ColorUsed;
			}
		}

		private ColorMap colorMap = null;
		public ColorMap ColorMap{
			get{
				this.ThrowIfDisposed();
				if((this.colorMap == null) && (this._GflBitmap.ColorMap != IntPtr.Zero)){
					this.colorMap = new ColorMap(this._GflBitmap.ColorMap);
				}
				return this.colorMap;
			}
		}
		
		private Exif exif = null;
		public Exif Exif{
			get{
				this.ThrowIfDisposed();
				if(this.exif == null && this._GflBitmap.MetaData != IntPtr.Zero){
					var ptr = IntPtr.Zero;
					try{
						this._Gfl.BitmapGetEXIF(this, Gfl.GetExifOptions.WantMakerNotes);
						var exifData = (Gfl.GflExifData)Marshal.PtrToStructure(ptr, typeof(Gfl.GflExifData));
						this.exif = new Exif(exifData);
					}finally{
						if(ptr != IntPtr.Zero){
							this._Gfl.FreeEXIF(ptr);
						}
					}
				}
				return this.exif;
			}
		}

		public void SaveBitmap(string filename){
			this.ThrowIfDisposed();
			var prms = new Gfl.GflSaveParams();
			this._Gfl.GetDefaultSaveParams(ref prms);
			this._Gfl.ThrowIfError(this._Gfl.SaveBitmap(filename, this, ref prms));
		}

		#endregion

		#region IDisposable

		private void ThrowIfDisposed(){
			if(this.disposed){
				throw new ObjectDisposedException("Bitmap");
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
			if(!this.disposed){
				this._Gfl.DisposeBitmap(this);
				this.disposed = true;
			}
		}

		#endregion
	}
}
