using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.IO;

namespace GFV.Imaging {
	using Gfl = GflNet;
	public class GflImageLoader : IImageLoader{
		public string Name{get{ return "Graphic File Library";}}

		public Gfl::Gfl Gfl{get; private set;}
		public GflImageLoader(Gfl::Gfl gfl){
			this.Gfl = gfl;
		}

		public IMultiBitmap Load(string file) {
			return this.Load(file, CancellationToken.None);
		}
		public IMultiBitmap Load(string file, CancellationToken token) {
			var bmp = this.Gfl.LoadMultiBitmap(file);
			bmp.LoadParameters.WantCancel += delegate(object sender, CancelEventArgs e){
				if(token.IsCancellationRequested){
					e.Cancel = true;
				}
			};
			return new GflMultiBitmap(bmp);
		}

		public IMultiBitmap Load(Stream stream) {
			return this.Load(stream, CancellationToken.None);
		}

		public IMultiBitmap Load(Stream stream, CancellationToken token) {
			var bmp = this.Gfl.LoadMultiBitmap(stream);
			bmp.LoadParameters.WantCancel += delegate(object sender, CancelEventArgs e){
				if(token.IsCancellationRequested){
					e.Cancel = true;
				}
			};
			return new GflMultiBitmap(bmp);
		}
	}
}
