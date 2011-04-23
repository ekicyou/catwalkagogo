/*
	$Id$
*/

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Media;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using System.Windows.Threading;
using CatWalk.Win32;

namespace CatWalk.Windows{
	public class ShellIconConverter : IValueConverter{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			var file = (string)value;
			var mode = (parameter == null) ? ShellIconConverterMode.Normal : (ShellIconConverterMode)parameter;
			if(mode == ShellIconConverterMode.Async){
				var bmp = new WriteableBitmap(16, 16, 96, 96, PixelFormats.Pbgra32, null);
				ThreadPool.QueueUserWorkItem(new WaitCallback(delegate{
					Bitmap icon = null;
					BitmapData bmpData = null;
					try{
						icon = ShellIcon.GetIconBitmap(file, IconSize.Small);
						bmpData = icon.LockBits(new Rectangle(0, 0, icon.Width, icon.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, icon.PixelFormat);
						bmp.Dispatcher.BeginInvoke(new Action(delegate{
							bmp.WritePixels(new Int32Rect(0, 0, icon.Width, icon.Height), bmpData.Scan0, bmpData.Stride * bmpData.Height, bmpData.Stride);
							//bmp.Freeze();
							icon.UnlockBits(bmpData);
							icon.Dispose();
						}), DispatcherPriority.Background);
					}catch{
						if(bmpData != null){
							icon.UnlockBits(bmpData);
						}
						icon.Dispose();
					}
				}));
				return bmp;
			}else{
				IntPtr hIcon;
				ShellIcon.GetIcon(file, IconSize.Small, out hIcon);
				if(hIcon != IntPtr.Zero){
					return Imaging.CreateBitmapSourceFromHIcon(hIcon, new Int32Rect(0, 0, 16, 16), BitmapSizeOptions.FromEmptyOptions());
				}else{
					return null;
				}
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			throw new NotImplementedException();
		}
	}

	public enum ShellIconConverterMode{
		Normal,
		Async
	}
}