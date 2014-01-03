/*
	$Id$
*/
using System;

namespace CatWalk{
	public abstract class DisposableObject : IDisposable{
		private bool _Disposed = false;
		protected bool IsDisposed{
			get{
				return this._Disposed;
			}
		}

		public void Dispose(){
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		protected virtual void Dispose(bool disposing){
			this._Disposed = true;
		}

		protected void ThrowIfDisposed(){
			throw new ObjectDisposedException("this");
		}
		
		~DisposableObject(){
			this.Dispose(false);
		}
	}
}