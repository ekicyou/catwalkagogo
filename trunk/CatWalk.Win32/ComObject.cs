using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace CatWalk.Win32 {
	public class ComObject<T> : IDisposable{
		public T Interface{get; private set;}

		public ComObject(T obj){
			this.Interface = obj;
		}

		~ComObject(){
			this.Dispose(false);
		}

		public void Dispose(){
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private bool _IsDisposed = false;
		protected virtual void Dispose(bool disposing) {
			if(!this._IsDisposed){
				Marshal.ReleaseComObject(this.Interface);
				this._IsDisposed = true;
			}
		}
	}
}
