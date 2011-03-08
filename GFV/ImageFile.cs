using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatWalk;

namespace GFV{
	using IO = System.IO;
	using Gfl = GflNet;
	public class ImageFile : DisposableObject{
		public string Path{get; private set;}
		public Gfl::Bitmap Bitmap{get; private set;}

		public ImageFile(string path, Gfl::Bitmap bitmap){
			this.Path = IO.Path.GetFileName(path);
			this.Bitmap = bitmap;
		}

		private bool disposed = false;
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(!this.disposed){
				this.Bitmap.Dispose();
			}
		}
	}
}
