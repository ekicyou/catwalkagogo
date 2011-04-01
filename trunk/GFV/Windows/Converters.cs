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
using CatWalk.Interop;
using CatWalk.Windows.Input;
using GFV.ViewModel;

namespace GFV.Windows{
	using Gdi = System.Drawing;
	using GdiImaging = System.Drawing.Imaging;
	using Gfl = GflNet;

	public class GflBitmapToBitmapSourceConverter : IValueConverter{
		[DllImport("Gdi32.dll")]
		private static extern bool DeleteObject(IntPtr handle);

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var gflBitmap = (Gfl::Bitmap)value;
			if(gflBitmap != null){
				var length = gflBitmap.BytesPerLine * gflBitmap.Height;
				var pixels = new byte[length];
				Marshal.Copy(gflBitmap.Scan0, pixels, 0, length);
				return BitmapSource.Create(gflBitmap.Width, gflBitmap.Height, 96, 96, PixelFormats.Bgra32, null, pixels, gflBitmap.BytesPerLine);
				/*
				var hSec = IntPtr.Zero;
				var hMap = IntPtr.Zero;
				try{
					hSec = Win32.CreateFileMapping(new IntPtr(-1), IntPtr.Zero, CreateFileMappingOptions.PageReadWrite, 0, (int)length, null);
					hMap = Win32.MapViewOfFile(hSec, FileMapAccessMode.AllAccess, 0, 0, length);
					Win32.CopyMemory(hMap, gflBitmap.Scan0, length);
					return Imaging.CreateBitmapSourceFromMemorySection(hSec, gflBitmap.Width, gflBitmap.Height, PixelFormats.Bgra32, gflBitmap.BytesPerLine, 0);
				}finally{
					if(hMap != IntPtr.Zero){
						if(!Win32.UnmapViewOfFile(hMap)){
							throw new Win32Exception(Marshal.GetLastWin32Error());
						}
					}
					if(hSec != IntPtr.Zero){
						if(!Win32.CloseHandle(hSec)){
							throw new Win32Exception(Marshal.GetLastWin32Error());
						}
					}
				}
				
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
				 * */
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

	public class InputGesturesToTextConverter : IValueConverter{
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
	}
}