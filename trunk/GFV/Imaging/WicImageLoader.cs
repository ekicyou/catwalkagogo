using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Threading;
using System.IO;

namespace GFV.Imaging {
	public class WicImageLoader : IImageLoader{
		public string Name{get{ return "Windows Imaging Component";}}

		public IMultiBitmap Load(string file) {
			return this.Load(file, CancellationToken.None);
		}

		public IMultiBitmap Load(string file, CancellationToken token) {
			return new WicMultiBitmap(BitmapDecoder.Create(new Uri(file), BitmapCreateOptions.None, BitmapCacheOption.None));
		}

		public IMultiBitmap Load(Stream stream) {
			return this.Load(stream, CancellationToken.None);
		}

		public IMultiBitmap Load(Stream stream, CancellationToken token) {
			return new WicMultiBitmap(BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.None));
		}
	}
}
