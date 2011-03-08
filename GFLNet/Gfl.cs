﻿/*
	$Id$
*/
using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Linq;

namespace GflNet {
	public partial class Gfl : IDisposable{
		protected IntPtr GflHandle{get; set;}
		internal HashSet<WeakReference> LoadedBitmap = new HashSet<WeakReference>();

		public Gfl(string dllName){
			this.GflHandle = LoadLibrary(dllName);

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

		public GflNet.Bitmap LoadBitmap(string path){
			return this.LoadBitmap(path, null);
		}
		
		public GflNet.Bitmap LoadBitmap(string path, GflProgressCallback progressCallback){
			this.ThrowIfDisposed();

			Gfl.LoadParams prms = new Gfl.LoadParams();
			this.GetDefaultLoadParams(ref prms);
			prms.Options = Gfl.LoadOptions.ForceColorModel | Gfl.LoadOptions.IgnoreReadError;
			prms.ColorModel = Gfl.BitmapType.Bgra;
			if(progressCallback != null){
				prms.Callbacks.Progress = new Gfl.ProgressCallback(delegate(int percent, IntPtr userParams){
					progressCallback(null, new GflProgressEventArgs(percent));
				});
			}
			
			IntPtr ptr = IntPtr.Zero;
			Gfl.FileInformation info = new Gfl.FileInformation();
			try{
				Gfl.Error error = this.LoadBitmap(path, ref ptr, ref prms, ref info);

				switch(error){
					case Gfl.Error.None:
						var bitmap = new GflNet.Bitmap(this, (Gfl.Bitmap)Marshal.PtrToStructure(ptr, typeof(Gfl.Bitmap)), info);
						return bitmap;
					case Gfl.Error.FileOpen:
					case Gfl.Error.FileRead:
					case Gfl.Error.FileCreate:
					case Gfl.Error.FileWrite:
						throw new IOException(this.GetErrorString(error));
					case Gfl.Error.NoMemory:
						throw new OutOfMemoryException(this.GetErrorString(error));
					case Gfl.Error.BadBitmap:
						throw new FormatException(this.GetErrorString(error));
					default:
						throw new ApplicationException(this.GetErrorString(error));
				}
			}finally{
				this.FreeFileInformation(ref info);
			}
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