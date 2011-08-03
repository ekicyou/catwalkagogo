using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace GFV.Imaging {
	/*
	public class GifImageLoader : IImageLoader{
		#region IImageLoader Members

		public IMultiBitmap Load(string file) {
			return this.Load(File.OpenRead(file), CancellationToken.None);
		}

		public IMultiBitmap Load(string file, CancellationToken token) {
			return this.Load(File.OpenRead(file), token);
		}

		public IMultiBitmap Load(Stream stream) {
			return this.Load(stream, CancellationToken.None);
		}

		public IMultiBitmap Load(Stream stream, CancellationToken token) {
			var header = new byte[6];
			var n = stream.Read(header, 0, 6);
			if(n != 6){
				throw new FileFormatException();
			}
			
			// Header
			if(header[0] == 0x47 && header[1] == 0x49 && header[2] == 0x49 && header[3] == 0x38 && (header[4] == 0x37 || header[4] == 0x39) && header[5] == 0x61){
				stream.Seek(-6, SeekOrigin.Current);
				var bitmap = (Bitmap)Bitmap.FromStream(stream, true, false);
				return new GifMultiBitmap(bitmap);
			}else{
				throw new FileFormatException();
			}
		}

		#endregion
	}
	 * */
}
