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

		public IMultiBitmap Load(System.IO.Stream stream) {
			throw new NotImplementedException();
		}

		public IMultiBitmap Load(System.IO.Stream stream, CancellationToken token) {
			throw new NotImplementedException();
		}
	}
}
