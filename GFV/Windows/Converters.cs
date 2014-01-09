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
using CatWalk.Win32;
using CatWalk.Win32.Shell;
using GFV.ViewModel;
using System.Reflection;

namespace GFV.Windows{
	using Gdi = System.Drawing;
	using GdiImaging = System.Drawing.Imaging;
	using Gfl = GflNet;
	using IO = System.IO;

	public class GflBitmapToBitmapSourceConverter : IValueConverter{
		#region IValueConverter Members

		//[DllImport("Gdi32.dll")]
		//private static extern bool DeleteObject(IntPtr handle);

		public virtual object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var gflBitmap = (Gfl::Bitmap)value;
			if(gflBitmap != null){
				var length = gflBitmap.BytesPerLine * gflBitmap.Height;
				var pixels = new byte[length];
				Marshal.Copy(gflBitmap.Scan0, pixels, 0, length);
				var bmp = BitmapSource.Create(gflBitmap.Width, gflBitmap.Height, 96, 96, PixelFormats.Bgra32, null, pixels, gflBitmap.BytesPerLine);
				bmp.Freeze();
				return bmp;
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
				var size = (parameter != null) ? ImageListSize.Large : (ImageListSize)parameter;
				using(var il = new ImageList(size)){
					var icon = il.GetIcon(Assembly.GetExecutingAssembly().Location);
					var image = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(icon.Handle, new System.Windows.Int32Rect(0, 0, icon.Width, icon.Height), BitmapSizeOptions.FromEmptyOptions());
					image.Freeze();
					return image;
				}
			}
		}

		#endregion
	}

	public class BitmapSourceOrIconConverter : IValueConverter{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(value != null){
				return value;
			}else{
				var size = (parameter == null) ? ImageListSize.Large :
				           (parameter is ImageListSize) ? (ImageListSize)parameter :
				           (parameter.ToString().Equals("max", StringComparison.OrdinalIgnoreCase)) ? ImageList.MaxSize :
				           ImageListSize.Small;
				using(var il = new ImageList(size)){
					var icon = il.GetIcon(Assembly.GetExecutingAssembly().Location);
					var image = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(icon.Handle, new System.Windows.Int32Rect(0, 0, icon.Width, icon.Height), BitmapSizeOptions.FromEmptyOptions());
					image.Freeze();
					return image;
				}
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

		#endregion
	}

	public class ShellIconConverter : IValueConverter{
		#region IValueConverter Members

		private ImageList _ImageList = new ImageList(ImageListSize.Small);

		public virtual object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var file = (string)value;
			if(!String.IsNullOrEmpty(file)){
				try{
					int overlay;
					var idx = this._ImageList.GetIconIndexWithOverlay(file, out overlay);
					using(var bitmap = this._ImageList.Draw(idx, overlay, ImageListDrawOptions.Transparent)){
						var image = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
						image.Freeze();
						return image;
					}
				}catch{
					System.Drawing.Icon icon = null;
					try{
						icon = ShellIcon.GetUnknownIconImage(IconSize.Small);
						var image = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(icon.Handle, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
						image.Freeze();
						User32.DestroyIcon(icon.Handle);
						return image;
					}catch{
					}finally{
						if(icon != null){
							User32.DestroyIcon(icon.Handle);
						}
					}
				}
			}
			return null;
		}

		public virtual object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

		#endregion
	}

	public class ShellIconImageConverter : ShellIconConverter{
		#region IValueConverter Members

		public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var image = (ImageSource)base.Convert(value, targetType, parameter, culture);
			return new Image(){Source=image};
		}

		public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
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
				case ImageFittingMode.ShorterEdge:
				case ImageFittingMode.ShorterEdgeLargeOnly:
				case ImageFittingMode.LongerEdge:
				case ImageFittingMode.LongerEdgeLargeOnly:
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
				case ImageFittingMode.ShorterEdge:
				case ImageFittingMode.ShorterEdgeLargeOnly:
				case ImageFittingMode.LongerEdge:
				case ImageFittingMode.LongerEdgeLargeOnly:
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
			var d = (double)value;
			if(Double.IsNaN(d)){
				return "";
			}else{
				return Math.Round(d * 100) + "%";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			return (double)value / 100;
		}
		#endregion
	}

	public class InputBindingsToTextConverter : IValueConverter{
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var inputBindings = value as IEnumerable<InputBinding>;
			if(inputBindings != null){
				var list = new List<string>();
				foreach(var binding in inputBindings.Where(bind => bind.CommandParameter == parameter)){
					var converter = TypeDescriptor.GetConverter(binding.Gesture.GetType());
					list.Add((string)converter.ConvertTo(binding.Gesture, typeof(string)));
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
				bool isShow = (bShow == null) ? !SystemParameters.IsGlassEnabled : bShow.Value;
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

	public class ScaleToScalingModeConverter : IValueConverter {
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var scale = (double)value;
			if(scale > 1){
				return BitmapScalingMode.NearestNeighbor;
			}else{
				return BitmapScalingMode.Fant;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

		#endregion
	}

	public class RoundNumberConverter : IValueConverter{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(value is double){
				return Math.Round((double)value);
			}else if(value is float){
				return Math.Round((float)value);
			}else if(value is decimal){
				return Math.Round((decimal)value);
			}else{
				return value;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value;
		}

		#endregion
	}

	public class IsNaNConverter : IValueConverter {
		#region IValueConverter

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return Double.IsNaN((double)value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

		#endregion
	}

	public class AlphaColorConverter : IValueConverter{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var color = (Color)value;
			return Color.FromArgb((byte)parameter, color.R, color.G, color.B);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

		#endregion
	}

	public class SortOrderConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(targetType == typeof(bool)){
				return ((SortOrder)value == SortOrder.Descending);
			}else if(targetType == typeof(bool?)){
				return new Nullable<bool>((SortOrder)value == SortOrder.Descending);
			}else{
				return value;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(targetType == typeof(SortOrder)){
				return (bool)value ? SortOrder.Descending : SortOrder.Ascending;
			}else{
				return value;
			}
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