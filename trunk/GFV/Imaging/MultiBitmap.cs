using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Threading;

namespace GFV.Imaging {
	public interface IMultiBitmap : IEnumerable<BitmapSource>{
		bool IsLoading{get;}
		int FrameCount{get;}
		BitmapSource this[int index]{get;}
		BitmapSource GetThumbnail();
	}
}
