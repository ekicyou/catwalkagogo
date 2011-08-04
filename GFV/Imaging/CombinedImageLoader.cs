using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace GFV.Imaging {
	public class CombinedImageLoader : IImageLoader{
		public string Name{get{ return "Combined Image Loader";}}

		private IImageLoader[] _Loaders;

		public CombinedImageLoader(IEnumerable<IImageLoader> loaders){
			this._Loaders = loaders.ToArray();
		}

		public IMultiBitmap Load(string file) {
			return this.Load(file, CancellationToken.None);
		}

		public IMultiBitmap Load(string file, CancellationToken token) {
			var exs = new List<Exception>();
			foreach(var loader in this._Loaders){
				try{
					return loader.Load(file, token);
				}catch(Exception ex){
					exs.Add(ex);
				}
			}
			throw new AggregateException(exs);
		}

		public IMultiBitmap Load(Stream stream) {
			throw new NotImplementedException();
		}

		public IMultiBitmap Load(Stream stream, CancellationToken token) {
			var exs = new List<Exception>(this._Loaders.Length);
			var list = new List<string>(this._Loaders.Length);
			var offset = stream.Position;
			foreach(var loader in this._Loaders){
				try{
					return loader.Load(stream, token);
				}catch(Exception ex){
					exs.Add(ex);
					list.Add(loader.Name + " : " + ex.Message);
					stream.Seek(offset, SeekOrigin.Begin);
				}
			}
			throw new IOException(String.Join("\n", list), new AggregateException(exs));
		}
	}
}
