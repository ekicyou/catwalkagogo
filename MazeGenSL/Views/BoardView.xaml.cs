/*
	$Id: Account.cs 51 2010-01-28 21:55:33Z catwalk $
*/
using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Xml;
using System.IO;
using MazeGenSL.Models;

namespace MazeGenSL.Views {
	public partial class BoardView : UserControl {
		public BoardView() {
			InitializeComponent();
		}
	}

	public class CellToPixelsConverter : IValueConverter{
		private static WriteableBitmap RoadBitmap = LoadBitmap("Road.png");
		private static WriteableBitmap WallBitmap = LoadBitmap("Wall.png");
		private static WriteableBitmap StartBitmap = LoadBitmap("Start.png");
		private static WriteableBitmap RouteBitmap = LoadBitmap("Route.png");
		private static WriteableBitmap GoalBitmap = LoadBitmap("Goal.png");

		private static WriteableBitmap LoadBitmap(string name){
			var bmp = new BitmapImage();
			var xap = new XmlXapResolver();
			var uri = new Uri(@"Resources/" + name, UriKind.Relative);
			var stream = xap.GetEntity(uri, null, typeof(Stream)) as Stream;
			bmp.SetSource(stream);
			var wbmp = new WriteableBitmap(bmp);
			return wbmp;
		}
	
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var cell = (Cell)value;
			switch(cell){
				case Cell.Goal: return GoalBitmap.Pixels;
				case Cell.Road: return RoadBitmap.Pixels;
				case Cell.Route: return RouteBitmap.Pixels;
				case Cell.Start: return StartBitmap.Pixels;
				case Cell.Wall: return WallBitmap.Pixels;
				default: return null;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

		#endregion
	}
}
