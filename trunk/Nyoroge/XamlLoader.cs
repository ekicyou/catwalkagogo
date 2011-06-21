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
using System.Windows.Markup;
using System.Xml;
using System.IO;

namespace Nyoroge {
	public static class XamlLoader {
		public static object LoadXaml(string path){
			var xap = new XmlXapResolver();
			var uri = new Uri(path, UriKind.Relative);
			using(var stream = xap.GetEntity(uri, null, typeof(Stream)) as Stream)
			using(var reader = new StreamReader(stream)){
				return XamlReader.Load(reader.ReadToEnd());
			}
		}
	}
}
