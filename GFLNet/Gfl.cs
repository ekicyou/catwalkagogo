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

		#region Initialize

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

		#endregion

		#region Format

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

		#endregion

		#region Read

		public Bitmap LoadBitmap(string path){
			FileInformation info;
			return this.LoadBitmap(path, 0, this.GetDefaultLoadParameters(), out info);
		}

		public Bitmap LoadBitmap(string path, int frameIndex){
			FileInformation info;
			return this.LoadBitmap(path, frameIndex, this.GetDefaultLoadParameters(), out info);
		}

		public Bitmap LoadBitmap(string path, int frameIndex, LoadParameters parameters){
			FileInformation info;
			return this.LoadBitmap(path, frameIndex, parameters, out info);
		}

		public Bitmap LoadBitmap(string path, int frameIndex, LoadParameters parameters, out FileInformation info){
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
			prms.Callbacks.WantCancel = parameters.WantCancelCallback;

			IntPtr ptr = IntPtr.Zero;
			var gflinfo = new Gfl.GflFileInformation();
			this.ThrowIfError(this.LoadBitmap(path, ref ptr, ref prms, ref gflinfo));

			var gflBitmap = (GflBitmap)Marshal.PtrToStructure(ptr, typeof(GflBitmap));
			var bitmap = new GflNet.Bitmap(this, ref gflBitmap);
			info = new FileInformation(gflinfo, this.GetGflFormat(gflinfo.FormatIndex));
			this.FreeFileInformation(ref gflinfo);
			return bitmap;
		}

		public MultiBitmap LoadMultiBitmap(string filename){
			FileInformation info;
			return this.LoadMultiBitmap(filename, out info);
		}

		public MultiBitmap LoadMultiBitmap(string filename, out FileInformation info){
			this.ThrowIfDisposed();

			filename = Path.GetFullPath(filename);
			info = this.GetFileInformation(filename);
			return new MultiBitmap(this, filename, info.ImageCount);
		}

		public LoadParameters GetDefaultLoadParameters(){
			this.ThrowIfDisposed();

			var prms = new Gfl.GflLoadParams();
			this.GetDefaultLoadParams(ref prms);
			return new LoadParameters(prms);
		}

		public FileInformation GetFileInformation(string filename){
			return this.GetFileInformation(filename, -1);
		}

		public FileInformation GetFileInformation(string filename, int index){
			this.ThrowIfDisposed();

			filename = Path.GetFullPath(filename);
			var info = new GflFileInformation();
			this.ThrowIfError(this.GetFileInformation(filename, index, ref info));
			try{
				return new FileInformation(info, this.GetGflFormat(info.FormatIndex));
			}finally{
				this.FreeFileInformation(ref info);
			}
		}

		#endregion

		#region Exception

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
			}
		}

		protected void ThrowIfDisposed(){
			if(this.disposed){
				throw new ObjectDisposedException("Gfl");
			}
		}

		#endregion

		#region IDisposable

		public void Dispose(){
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		~Gfl(){
			this.Dispose(false);
		}
		
		private readonly object _SyncObject = new object();
		private bool disposed = false;
		protected virtual void Dispose(bool disposing){
			lock(this._SyncObject){
				if(!this.disposed){
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
		}

		internal void DisposeBitmap(ref GflBitmap bitmap){
			lock(this._SyncObject){
				this.FreeBitmapData(ref bitmap);
				this.FreeBitmap(ref bitmap);
				this.LoadedBitmap.RemoveAll(wref => wref.Target == this);
			}
		}

		#endregion
	}
}
