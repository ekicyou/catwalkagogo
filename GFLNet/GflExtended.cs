using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace GflNet {
	public partial class GflExtended : IDisposable{
		public string DllName{get; private set;}
		protected IntPtr Handle{get; private set;}
		public GflExtended(string dllName){
			this.DllName = dllName;
			this.Handle = NativeMethods.LoadLibrary(dllName);
			if(this.Handle == IntPtr.Zero){
				throw new IOException();
			}
		}

		#region Filter

		public void Sharpen(Bitmap src, int percentage, out Bitmap dst){
			this.ThrowIfDisposed();
			if(percentage < 0 || percentage >= 100){
				throw new ArgumentOutOfRangeException("percentage");
			}
			src.ThrowIfDisposed();
			var pdst = IntPtr.Zero;
			src.Gfl.ThrowIfError(this.Sharpen(src, ref pdst, percentage));
			dst = new Bitmap(src.Gfl, pdst);
		}

		#endregion

		#region Misc

		public void JpegLosslessTransform(string path, JpegLosslessTransform transform){
			this.ThrowIfDisposed();
			if(this.JpegLosslessTransformInternal(path, transform) != Gfl.Error.None){
				throw new IOException();
			}
		}

		#endregion

		#region IDisposable

		protected void ThrowIfDisposed(){
			if(this._Disposed){
				throw new ObjectDisposedException("GflExtended");
			}
		}

		public void Dispose(){
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		~GflExtended(){
			this.Dispose(false);
		}
		
		private bool _Disposed = false;
		protected virtual void Dispose(bool disposing){
			if(!this._Disposed){
				NativeMethods.FreeLibrary(this.Handle);
				this._Disposed = true;
			}
		}

		#endregion
	}
}
