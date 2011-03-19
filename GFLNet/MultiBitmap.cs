/*
	$Id$
*/
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GflNet {
	using IO = System.IO;

	public class MultiBitmap : IDisposable, IEnumerable<Bitmap>{
		private Bitmap[] frames;
		private Gfl gfl;
		private string path;
		private FileInformation[] infos;
		public ReadOnlyCollection<FileInformation> FileInformations{get; private set;}

		internal MultiBitmap(Gfl gfl, string path, int frameCount){
			this.path = IO.Path.GetFullPath(path);
			this.frames = new Bitmap[frameCount];
			this.infos = new FileInformation[frameCount];
			this.FileInformations = new ReadOnlyCollection<FileInformation>(this.infos);
			this.gfl = gfl;
			this.LoadParameters = this.gfl.GetDefaultLoadParameters();
		}

		private void LoadFrame(int index){
			if(!IO.File.Exists(path)){
				throw new IO.FileNotFoundException(this.path);
			}
			FileInformation info;
			var bitmap = this.gfl.LoadBitmap(this.path, index, this.LoadParameters, out info);
			this.infos[index] = info;
			this.frames[index] = bitmap;
		}

		public Bitmap this[int index]{
			get{
				if(index < 0 || index >= this.frames.Length){
					throw new ArgumentOutOfRangeException();
				}
				if(this.frames[index] == null){
					this.LoadFrame(index);
				}
				return this.frames[index];
			}
		}

		private LoadParameters _LoadParameters;
		public LoadParameters LoadParameters{
			get{
				return this._LoadParameters;
			}
			set{
				if(value == null){
					throw new ArgumentNullException();
				}
				this._LoadParameters = value;
			}
		}

		public int FrameCount{
			get{
				return this.frames.Length;
			}
		}

		#region IDisposable

		public void Dispose(){
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		~MultiBitmap(){
			this.Dispose(false);
		}
		
		private bool disposed = false;
		protected virtual void Dispose(bool disposing){
			if(!(this.disposed)){
				foreach(var bitmap in this.frames.Where(bmp => bmp != null)){
					bitmap.Dispose();
				}
				this.disposed = true;
			}
		}

		#endregion

		#region IEnumerable<Bitmap> Members

		public IEnumerator<Bitmap> GetEnumerator() {
			for(var i = 0; i < this.frames.Length; i++){
				if(this.frames[i] == null){
					this.LoadFrame(i);
				}
				yield return this.frames[i];
			}
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

		#endregion
	}
}
