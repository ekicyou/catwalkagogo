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
					Win32.CopyMemory(bitmapData.Scan0, gflBitmap.Scan0, gflBitmap.BytesPerLine * gflBitmap.Height);
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

	public class ImageFittingModeCheckConverter : IValueConverter{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var v = (ImageFittingMode)value;
			var mode = (ImageFittingMode)parameter;
			return (v == mode);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var b = (bool)value;
			var mode = (ImageFittingMode)parameter;
			if(b){
				return mode;
			}else{
				return ImageFittingMode.None;
			}
		}
	}
}