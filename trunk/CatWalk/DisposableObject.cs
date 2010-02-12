/*
	$Id$
*/
using System;

namespace CatWalk{
	public abstract class DisposableObject : IDisposable{
		public void Dispose(){
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		protected virtual void Dispose(bool disposing){
		}
		
		~DisposableObject(){
			this.Dispose(false);
		}
	}
}