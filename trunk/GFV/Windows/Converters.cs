/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CatWalk;
using CatWalk.Interop;
using GFV.ViewModel;
using System.Reflection;

namespace GFV.Windows{
	using Gdi = System.Drawing;
	using GdiImaging = System.Drawing.Imaging;
	using Gfl = GflNet;
	using IO = System.IO;

	public class GflBitmapToBitmapSourceConverter : IValueConverter{
		#region IValueConverter Members

		[DllImport("Gdi32.dll")]
		private static extern bool DeleteObject(IntPtr handle);

		public virtual object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var gflBitmap = (Gfl::Bitmap)value;
			if(gflBitmap != null){
				var length = gflBitmap.BytesPerLine * gflBitmap.Height;
				var pixels = new byte[length];
				Marshal.Copy(gflBitmap.Scan0, pixels, 0, length);
				return BitmapSource.Create(gflBitmap.Width, gflBitmap.Height, 96, 96, PixelFormats.Bgra32, null, pixels, gflBitmap.BytesPerLine);
			}else{
				return null;
			}
		}

		public virtual object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

		#endregion
	}

	public class GflBitmapToBitmapSourceOrIconConverter : GflBitmapToBitmapSourceConverter {
		#region IValueConverter Members

		public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(value != null){
				return base.Convert(value, targetType, parameter, culture);
			}else{
				return ShellIcon.GetIconImageSource(Assembly.GetEntryAssembly().Location, IconSize.Large);
			}
		}

		#endregion
	}
	
	public class AddConverter : IValueConverter{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var v = (double)value;
			var a = Double.Parse((string)parameter);
			return v + a;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var v = (double)value;
			var a = Double.Parse((string)parameter);
			return v - a;
		}

		#endregion
	}

	public class ImageFittingModeToHorizontalScrollVarVisibilityConverter : IValueConverter{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var mode = (ImageFittingMode)value;
			switch(mode){
				case ImageFittingMode.None:
				case ImageFittingMode.WindowHeight:
				case ImageFittingMode.WindowHeightLargeOnly:
					return ScrollBarVisibility.Auto;
				case ImageFittingMode.Window:
				case ImageFittingMode.WindowLargeOnly:
				case ImageFittingMode.WindowWidth:
				case ImageFittingMode.WindowWidthLargeOnly:
					return ScrollBarVisibility.Hidden;
				default:
					return ScrollBarVisibility.Disabled;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

		#endregion
	}

	public class ImageFittingModeToVerticalScrollVarVisibilityConverter : IValueConverter {
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var mode = (ImageFittingMode)value;
			switch(mode){
				case ImageFittingMode.None:
				case ImageFittingMode.WindowWidth:
				case ImageFittingMode.WindowWidthLargeOnly:
					return ScrollBarVisibility.Auto;
				case ImageFittingMode.Window:
				case ImageFittingMode.WindowLargeOnly:
				case ImageFittingMode.WindowHeight:
				case ImageFittingMode.WindowHeightLargeOnly:
					return ScrollBarVisibility.Hidden;
				default:
					return ScrollBarVisibility.Disabled;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}

	public class DoubleToPercentageConverter : IValueConverter{
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			return Math.Round((double)value * 100);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			return (double)value / 100;
		}
		#endregion
	}

	public class InputGesturesToTextConverter : IValueConverter{
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var inputGestures = value as IEnumerable<InputGesture>;
			if(inputGestures != null){
				var list = new List<string>();
				foreach(var gesture in inputGestures){
					var converter = TypeDescriptor.GetConverter(gesture.GetType());
					list.Add((string)converter.ConvertTo(gesture, typeof(string)));
				}
				return String.Join(" / ", list);
			}else{
				return null;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}

	public class BoolToVisibilityConverter : IValueConverter{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(value is bool){
				return (bool)value ? Visibility.Visible : Visibility.Collapsed;
			}else if(value is bool?){
				var bShow = (bool?)value;
				bool isShow = (bShow == null) ? !Microsoft.Windows.Shell.SystemParameters2.Current.IsGlassEnabled : bShow.Value;
				return isShow ? Visibility.Visible : Visibility.Collapsed;
			}else{
				return Visibility.Visible;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

		#endregion
	}

	public class NullableConverter : IValueConverter{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(value == null){
				return parameter;
			}else{
				return value;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

		#endregion
	}

	public class FilePathConverter : IValueConverter {
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			var path = (string)value;
			switch((FilePathTransform)parameter){
				case FilePathTransform.FileName:
					return IO.Path.GetFileName(path);
				case FilePathTransform.DirectoryName:
					return IO.Path.GetDirectoryName(path);
				case FilePathTransform.ExtensionName:
					return IO.Path.GetExtension(path);
				case FilePathTransform.ExtensionNameWithoutDot:
					return IO.Path.GetExtension(path).TrimStart('.');
				case FilePathTransform.PathRoot:
					return IO.Path.GetPathRoot(path);
				default:
					return path;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			throw new NotImplementedException();
		}

		#endregion
	}

	public class ScaleSliderTicksConverter : IValueConverter{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return new DoubleCollection(
				Enumerable.Range(1, 9).Select(n => (double)n * 0.01).Concat(
				Enumerable.Range(0, 79).Select(n => 0.1 + n * 0.1)));
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

		#endregion
	}

	public class RecentFilesMenuItemConverter : IValueConverter{
		private static readonly char[] CharMap = new char[]{'1','2','3','4','5','6','7','8','9','0',
			'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'};

		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var files = (string[])value;
			return files.EmptyIfNull().Select((file, idx) => new KeyValuePair<char, string>(CharMap[idx % CharMap.Length], file));
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

		#endregion
	}

	public enum FilePathTransform{
		None,
		FileName,
		DirectoryName,
		ExtensionName,
		ExtensionNameWithoutDot,
		PathRoot,
	}
}