using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;

namespace GFV.Windows{
	using Gfl = GflNet;

	public class GflBitmapToBitmapSourceConverter : IValueConverter{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var bitmap = (Gfl::Bitmap)value;
			return Imaging.CreateBitmapSourceFromMemorySection(
				bitmap.Scan0,
				bitmap.Width,
				bitmap.Height,
				PixelFormats.Bgra32,
				bitmap.BytesPerLine,
				0);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
