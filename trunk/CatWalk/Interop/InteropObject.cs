using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace CatWalk.Interop {
	public class InteropObject : DisposableObject{
		protected IntPtr Handle{get; private set;}

		public InteropObject(string dllName) : this(Win32.LoadLibrary(dllName)){}
		public InteropObject(IntPtr handle){
			if(handle == IntPtr.Zero){
				throw new ArgumentException("handle");
			}
			this.Handle = handle;
		}

		protected void ThrowIfDidposed(){
			if(this._IsDisposed){
				throw new ObjectDisposedException("Handle");
			}
		}

		private bool _IsDisposed = false;
		protected override void Dispose(bool disposing) {
			if(!this._IsDisposed){
				Win32.FreeLibrary(this.Handle);
				this._IsDisposed = true;
			}
		}

		protected T LoadMethod<T>(string name) where T : class{
			return LoadMethod<T>(name, this.Handle);
		}

		private static T LoadMethod<T>(string name, IntPtr hModule) where T : class{
			return Marshal.GetDelegateForFunctionPointer(Win32.GetProcAddress(hModule, name), typeof(T)) as T;
		}
	}
}
