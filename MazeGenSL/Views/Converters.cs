/*
	$Id: Account.cs 51 2010-01-28 21:55:33Z catwalk $
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
using System.Windows.Data;

namespace MazeGenSL.Views {
	public class BooleanToVisibilityConverter : IValueConverter{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var b = (bool)value;
			return b ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var v = (Visibility)value;
			return (v == Visibility.Visible) ? true : false;
		}

		#endregion
	}

	public class NotConverter : IValueConverter{

		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return !(bool)value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return !(bool)value;
		}

		#endregion
	}
}
