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
		public string DllName{get; private set;}
		protected IntPtr Handle{get; set;}
		private LinkedList<WeakReference> _LoadedBitmap = new LinkedList<WeakReference>();
#if DEBUG
		public int LoadedBitmapCount{
			get{
				return this._LoadedBitmap.Count;
			}
		}
#endif
		#region Initialize

		public Gfl(string dllName){
			this.DllName = dllName;
			this.Handle = NativeMethods.LoadLibrary(dllName);
			if(this.Handle == IntPtr.Zero){
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

				return this.GetVersion();
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

		private ReadOnlyCollection<Format> _Formats;
		public ReadOnlyCollection<Format> Formats{
			get{
				if(this._Formats == null){
					this._Formats = new ReadOnlyCollection<Format>(this.GetFormats());
				}
				return this._Formats;
			}
		}

		private Format[] GetFormats(){
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

			var formatInfo = new GflFormatInformation();
			this.ThrowIfError(this.GetFormatInformationByIndex(index, ref formatInfo));
			return new Format(ref formatInfo);
		}

		#endregion

		#region LoadBitmap

		public Bitmap LoadBitmap(string path){
			FileInformation info;
			return this.LoadBitmap(path, 0, this.GetDefaultLoadParameters(), out info, this);
		}

		public Bitmap LoadBitmap(string path, int frameIndex){
			FileInformation info;
			return this.LoadBitmap(path, frameIndex, this.GetDefaultLoadParameters(), out info, this);
		}

		public Bitmap LoadBitmap(string path, int frameIndex, LoadParameters parameters){
			FileInformation info;
			return this.LoadBitmap(path, frameIndex, parameters, out info, this);
		}

		public Bitmap LoadBitmap(string path, int frameIndex, LoadParameters parameters, out FileInformation info){
			return this.LoadBitmap(path, frameIndex, parameters, out info, this);
		}

		internal Bitmap LoadBitmap(string path, int frameIndex, LoadParameters parameters, out FileInformation info, object sender){
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
			prms.FormatIndex = parameters.Format.Index;
			prms.Callbacks.Progress = parameters.GetProgressCallback(sender);
			prms.Callbacks.WantCancel = parameters.GetWantCancelCallback(sender);

			IntPtr pBitmap = IntPtr.Zero;
			var pInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(GflFileInformation)));
			try{
				this.ThrowIfError(this.LoadBitmap(path, ref pBitmap, ref prms, pInfo));

				var bitmap = new Bitmap(this, pBitmap);
				info = new FileInformation(this, pInfo);
				return bitmap;
			}finally{
				Marshal.FreeHGlobal(pInfo);
			}
		}

		#endregion

		#region LoadMultiBitmap

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

		#endregion

		#region GetDefaultLoadParameters

		public LoadParameters GetDefaultLoadParameters(){
			this.ThrowIfDisposed();

			var prms = new Gfl.GflLoadParams();
			this.GetDefaultLoadParams(ref prms);
			return new LoadParameters(prms);
		}

		#endregion

		#region GetFileInformation

		public FileInformation GetFileInformation(string filename){
			return this.GetFileInformation(filename, -1);
		}

		public FileInformation GetFileInformation(string filename, int index){
			this.ThrowIfDisposed();

			filename = Path.GetFullPath(filename);
			//var info = new GflFileInformation();
			var pInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(GflFileInformation)));
			try{
				this.ThrowIfError(this.GetFileInformation(filename, index, pInfo));
				return new FileInformation(this, pInfo);
			}finally{
				Marshal.FreeHGlobal(pInfo);
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
			if(this._Disposed){
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
		private bool _Disposed = false;
		protected virtual void Dispose(bool disposing){
			if(!this._Disposed){
				lock(this._SyncObject){
					foreach(var bitmapRef in this._LoadedBitmap.Where(wref => wref.IsAlive)){
						var bitmap = (Bitmap)bitmapRef.Target;
						if(!bitmap.Disposed){
							this.FreeBitmap(bitmap);
							bitmap.Disposed = true;
						}
					}
					this.LibraryExit();
					NativeMethods.FreeLibrary(this.Handle);
					this._Disposed = true;
				}
			}
		}

		internal void AddBitmap(Bitmap bitmap){
			lock(this._SyncObject){
				this.ThrowIfDisposed();
				this._LoadedBitmap.AddLast(new WeakReference(bitmap));
			}
		}

		internal void DisposeBitmap(Bitmap bitmap){
			lock(this._SyncObject){
				if(!bitmap.Disposed){
					this.ThrowIfDisposed();
					this.FreeBitmap(bitmap);
					bitmap.Disposed = true;
					var node = this._LoadedBitmap.First;
					while(node != null){
						var next = node.Next;
						if(!node.Value.IsAlive || node.Value.Target == bitmap){
							this._LoadedBitmap.Remove(node);
						}
						node = next;
					}
				}
			}
		}

		#endregion
	}
}
