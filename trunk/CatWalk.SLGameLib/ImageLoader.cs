/*
	$Id$
*/
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Xml;
using System.IO;

namespace CatWalk.SLGameLib {
	public static class ImageLoader {
		public static WriteableBitmap LoadBitmap(string path){
			var bmp = new BitmapImage();
			var xap = new XmlXapResolver();
			var uri = new Uri(path, UriKind.Relative);
			using(var stream = xap.GetEntity(uri, null, typeof(Stream)) as Stream){
				bmp.SetSource(stream);
				var wbmp = new WriteableBitmap(bmp);
				return wbmp;
			}
		}
	}
}
