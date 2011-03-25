/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using CatWalk.Shell;
using GFV.ViewModel;

namespace GFV.Windows{
	using Gfl = GflNet;
	using Gdi = System.Drawing;
	using GdiImaging = System.Drawing.Imaging;

	public class GflBitmapToBitmapSourceConverter : IValueConverter{
		[DllImport("Gdi32.dll")]
		private static extern bool DeleteObject(IntPtr handle);

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var gflBitmap = (Gfl::Bitmap)value;
			if(gflBitmap != null){
				using(var gdiBitmap = new Gdi::Bitmap(gflBitmap.Width, gflBitmap.Height, GdiImaging::PixelFormat.Format32bppArgb)){
					var bitmapData = gdiBitmap.LockBits(
						new Gdi::Rectangle(0, 0, gflBitmap.Width, gflBitmap.Height),
						GdiImaging::ImageLockMode.ReadWrite,
						gdiBitmap.PixelFormat);
					Win32.CopyMemory(bitmapData.Scan0, gflBitmap.Scan0, (IntPtr)(gflBitmap.BytesPerLine * gflBitmap.Height));
					gdiBitmap.UnlockBits(bitmapData);
					IntPtr hBitmap = IntPtr.Zero;
					try{
						hBitmap = gdiBitmap.GetHbitmap();
						return Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
					}finally{
						DeleteObject(hBitmap);
					}
				}
			}else{
				return null;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}

	public class AddConverter : IValueConverter{
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
	}

	public class ImageFittingModeToHorizontalScrollVarVisibilityConverter : IValueConverter{
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
	}

	public class ImageFittingModeToVerticalScrollVarVisibilityConverter : IValueConverter{
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
	}

	public class DoubleToPercentageConverter : IValueConverter{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			return Math.Round((double)value * 100);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			return (double)value / 100;
		}
	}
}