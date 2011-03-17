/*
	$Id$
*/
using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Linq;

namespace GflNet{
	public partial class Gfl : IDisposable{
		protected IntPtr GflHandle{get; set;}
		internal List<WeakReference> LoadedBitmap = new List<WeakReference>();

		public Gfl(string dllName){
			this.GflHandle = LoadLibrary(dllName);
			if(this.GflHandle == IntPtr.Zero){
				throw new IOException();
			}

			Gfl.Error error = this.LibraryInit();
			if(error != Gfl.Error.None){
				throw new InvalidOperationException();
			}
			this.IsEnableLZW = true;
		}
		
		public string VersionString{
			get{
				this.ThrowIfDisposed();

				IntPtr ptr = this.GetVersion();
				return Marshal.PtrToStringAnsi(ptr);
			}
		}
		
		private bool isEnableLZW;
		public bool IsEnableLZW{
			get{
				return this.isEnableLZW;
			}
			set{
				this.ThrowIfDisposed();

				this.isEnableLZW = value;
				this.EnableLZW(value);
			}
		}
		
		private string pluginPath = null;
		public string PluginPath{
			get{
				return this.pluginPath;
			}
			set{
				this.ThrowIfDisposed();

				this.pluginPath = value;
				this.SetPluginPathname(value);
			}
		}
		
		public Format[] GetFormats(){
			this.ThrowIfDisposed();

			int num = this.GetNumberOfFormat();
			Format[] formats = new Format[num];
			for(int i = 0; i < num; i++){
				formats[i] = this.GetGflFormat(i);
			}
			return formats;
		}
		
		internal Format GetGflFormat(int index){
			this.ThrowIfDisposed();

			string name = this.GetFormatNameByIndex(index);
			string defaultSuffix = this.GetDefaultFormatSuffixByIndex(index);
			bool readable = this.FormatIsReadableByIndex(index);
			bool writable = this.FormatIsWritableByIndex(index);
			string description = this.GetFormatDescriptionByIndex(index);
			
			return new Format(name, defaultSuffix, readable, writable, description);
		}

		public Bitmap LoadBitmap(string path){
			return this.LoadBitmap(path, 0, this.GetDefaultLoadParameters());
		}

		public Bitmap LoadBitmap(string path, int frameIndex){
			return this.LoadBitmap(path, frameIndex, this.GetDefaultLoadParameters());
		}

		public Bitmap LoadBitmap(string path, int frameIndex, LoadParameters parameters){
			this.ThrowIfDisposed();
			path = Path.GetFullPath(path);
			if(parameters == null){
				throw new ArgumentNullException("parameters");
			}

			Gfl.GflLoadParams prms = new Gfl.GflLoadParams();
			this.GetDefaultLoadParams(ref prms);
			prms.Options = parameters.Options;
			prms.ColorModel = parameters.BitmapType;
			prms.Origin = parameters.Origin;
			prms.ImageWanted = frameIndex;
			prms.Callbacks.Progress = parameters.ProgressCallback;
			
			IntPtr ptr = IntPtr.Zero;
			Gfl.GflFileInformation info = new Gfl.GflFileInformation();
			Gfl.Error error = this.LoadBitmap(path, ref ptr, ref prms, ref info);
			this.ThrowIfError(error);

			var bitmap = new GflNet.Bitmap(this, (GflBitmap)Marshal.PtrToStructure(ptr, typeof(GflBitmap)), new ImageInfo(info, this.GetGflFormat(info.FormatIndex)));
			this.FreeFileInformation(ref info);
			return bitmap;
		}

		internal void ThrowIfError(Error error){
			this.ThrowIfDisposed();

			switch(error){
				case Gfl.Error.FileOpen:
				case Gfl.Error.FileRead:
				case Gfl.Error.FileCreate:
				case Gfl.Error.FileWrite:
					throw new IOException(this.GetErrorString(error));
				case Gfl.Error.NoMemory:
					throw new OutOfMemoryException(this.GetErrorString(error));
				case Gfl.Error.BadBitmap:
				case Gfl.Error.BadFormatIndex:
				case Gfl.Error.UnknownFormat:
					throw new FormatException(this.GetErrorString(error));
				case Gfl.Error.BadParameters:
					throw new ArgumentException(this.GetErrorString(error));
				default:
					throw new ApplicationException(this.GetErrorString(error));
			}
		}

		public MultiBitmap LoadMultiBitmap(string filename){
			this.ThrowIfDisposed();

			filename = Path.GetFullPath(filename);
			var info = this.GetImageInfo(filename);
			return new MultiBitmap(this, filename, info);
		}

		public ImageInfo GetImageInfo(string filename){
			return this.GetImageInfo(filename, 0);
		}

		public ImageInfo GetImageInfo(string filename, int index){
			this.ThrowIfDisposed();

			filename = Path.GetFullPath(filename);
			var info = new GflFileInformation();
			this.GetFileInformation(filename, index, ref info);
			try{
				return new ImageInfo(info, this.GetGflFormat(info.FormatIndex));
			}finally{
				this.FreeFileInformation(ref info);
			}
		}

		public LoadParameters GetDefaultLoadParameters(){
			this.ThrowIfDisposed();

			Gfl.GflLoadParams prms = new Gfl.GflLoadParams();
			this.GetDefaultLoadParams(ref prms);
			return new LoadParameters(prms);
		}

		protected void ThrowIfDisposed(){
			if(this.disposed){
				throw new ObjectDisposedException("Gfl");
			}
		}

		#region IDisposable

		public void Dispose(){
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		~Gfl(){
			this.Dispose(false);
		}
		
		private bool disposed = false;
		protected virtual void Dispose(bool disposing){
			if(!(this.disposed)){
				foreach(var bitmapRef in this.LoadedBitmap.Where(wref => wref.IsAlive).ToArray()){
					((GflNet.Bitmap)bitmapRef.Target).Dispose();
				}
				if(this.GflHandle != IntPtr.Zero){
					this.LibraryExit();
					FreeLibrary(this.GflHandle);
				}
				this.disposed = true;
			}
		}

		#endregion
	}
}
