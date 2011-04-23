/*
	$Id$
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using System.Windows.Threading;
using CatWalk;
using CatWalk.Win32;
using Nekome.Search;

namespace Nekome.Windows {
	#region Icon
	public class ShellIconConverter : IValueConverter{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			var file = (string)value;
			var mode = (parameter == null) ? ShellIconConverterMode.Normal : (ShellIconConverterMode)parameter;
			//Program.MainForm.ProgressManager.ProgressMessage = file;
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

	#endregion

	#region FilePath

	public class FilePathConverter : DependencyObject, IValueConverter{
		public static readonly DependencyProperty BasePathProperty = DependencyProperty.Register("BasePath", typeof(string), typeof(FilePathConverter));
		public string BasePath{
			get{
				return (string)this.GetValue(BasePathProperty);
			}
			set{
				this.SetValue(BasePathProperty, value);
			}
		}

		public FilePathConverter(){
		}

		public FilePathConverter(string basePath){
			this.BasePath = basePath;
		}


		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			var path = (string)value;
			if((this.BasePath != null) && path.StartsWith(this.BasePath, StringComparison.OrdinalIgnoreCase)){
				return path.Substring(this.BasePath.Length);
			}else{
				return path;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			throw new NotImplementedException();
		}
	}

	#endregion

	#region FileSize

	public class FileSizeConverter : ValidationRule, IValueConverter{
		private static readonly char[] trimChar = new char[]{'0', '.'};

		public virtual object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			var size = (long)value;

			if(size == 0){
				return "0 B";
			}

			bool ti = (size % (1000L * 1000L * 1000L * 1000L) == 0);
			bool t  = (size % (1024L * 1024L * 1024L * 1024L) == 0);
			if(t && ti){      return (size / 1024d / 1024d / 1024d / 1024d).ToString("N").TrimEnd(trimChar) + " TB";}
			else if(t && !ti){return (size / 1024d / 1024d / 1024d / 1024d).ToString("N").TrimEnd(trimChar) + " TB";}
			else if(!t && ti){return (size / 1000d / 1000d / 1000d / 1000d).ToString("N").TrimEnd(trimChar) + " TiB";}

			bool gi = (size % (1000 * 1000 * 1000) == 0);
			bool g  = (size % (1024 * 1024 * 1024) == 0);
			if(g && gi){      return (size / 1024d / 1024d / 1024d).ToString("N").TrimEnd(trimChar) + " GB";}
			else if(g && !gi){return (size / 1024d / 1024d / 1024d).ToString("N").TrimEnd(trimChar) + " GB";}
			else if(!g && gi){return (size / 1000d / 1000d / 1000d).ToString("N").TrimEnd(trimChar) + " GiB";}


			bool mi = (size % (1000 * 1000) == 0);
			bool m  = (size % (1024 * 1024) == 0);
			if(m && mi){      return (size / 1024d / 1024d).ToString("N").TrimEnd(trimChar) + " MB";}
			else if(m && !mi){return (size / 1024d / 1024d).ToString("N").TrimEnd(trimChar) + " MB";}
			else if(!m && mi){return (size / 1000d / 1000d).ToString("N").TrimEnd(trimChar) + " MiB";}

			bool ki = (size % 1000 == 0);
			bool k  = (size % 1024 == 0);
			if(k && ki){ return (size / 1024d).ToString("N").TrimEnd(trimChar) + " KB";}
			else if(k && !ki){return (size / 1024d).ToString("N").TrimEnd(trimChar) + " KB";}
			else if(!k && ki){return (size / 1000d).ToString("N").TrimEnd(trimChar) + " KiB";}

			return size.ToString() + " B";
		}

		public static readonly Regex FormatRegex = new Regex(@"\s*([0-9,.]+)\s*([kmgt]{0,1}i{0,1})b{0,1}\s*", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public virtual object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			var str = ((string)value).ToLower();
			var match = FormatRegex.Match(str);
			if(match.Success){
				var numberPart = match.Groups[1].Value.Replace(",", "");
				var suffix = match.Groups[2].Value;
				//MessageBox.Show(String.Join(", ", match.Groups.Cast<Group>().Select(g => g.Value)));
				long number = Int64.Parse(numberPart);
				switch(suffix){
					case "k": return number * 1024L;
					case "m": return number * 1024L * 1024L;
					case "g": return number * 1024L * 1024L * 1024L;
					case "t": return number * 1024L * 1024L * 1024L * 1024L;
					case "ki": return number * 1000L;
					case "mi": return number * 1000L * 1000L;
					case "gi": return number * 1000L * 1000L * 1000L;
					case "ti": return number * 1000L * 1000L * 1000L * 1000L;
					default: return number;
				}
			}else{
				throw new FormatException("File size format is [number] [unit]");
			}
		}

		public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo) {
			var str = (string)value;
			var match = FormatRegex.Match(str);
			if(match.Success){
				try{
					var numberPart = match.Groups[1].Value;
					long number = Int64.Parse(numberPart);
					return new ValidationResult(true, null);
				}catch(Exception e){
					return new ValidationResult(false, e.Message);
				}
			}else{
				return new ValidationResult(false, "File size format is [number] [unit]");
			}
		}
	}

	public class FileMinSizeConverter : FileSizeConverter{
		public DependencyObject Object{get; set;}
		public DependencyProperty MaxProperty{get; set;}

		public FileMinSizeConverter(){}
		public FileMinSizeConverter(DependencyObject obj, DependencyProperty max){
			this.Object = obj;
			this.MaxProperty = max;
		}

		public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo) {
			long size;
			try{
				size = (long)this.ConvertBack(value, typeof(long), null, cultureInfo);
			}catch(Exception e){
				return new ValidationResult(false, e.Message);
			}
			var max = (long)this.Object.GetValue(this.MaxProperty);
			if(max < size){
				return new ValidationResult(false, "Max size is less than min size");
			}else{
				return new ValidationResult(true, null);
			}
		}

		public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			var str = (string)value;
			if(str.IsNullOrEmpty()){
				return 0L;
			}else{
				return base.ConvertBack(value, targetType, parameter, culture);
			}
		}
	}

	public class FileMaxSizeConverter : FileSizeConverter{
		public DependencyObject Object{get; set;}
		public DependencyProperty MinProperty{get; set;}

		public FileMaxSizeConverter(){}
		public FileMaxSizeConverter(DependencyObject obj, DependencyProperty min){
			this.Object = obj;
			this.MinProperty = min;
		}

		public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo) {
			long size;
			try{
				size = (long)this.ConvertBack(value, typeof(long), null, cultureInfo);
			}catch(Exception e){
				return new ValidationResult(false, e.Message);
			}
			var min = (long)this.Object.GetValue(this.MinProperty);
			if(min > size){
				return new ValidationResult(false, "Min size is greater than max size");
			}else{
				return new ValidationResult(true, null);
			}
		}
	
		public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			var str = (string)value;
			if(str.IsNullOrEmpty()){
				return 1024L * 1024L * 1024L * 1024L * 1024L;
			}else{
				return base.ConvertBack(value, targetType, parameter, culture);
			}
		}
	}

	#endregion

	#region etc

	public class IsEmptyConverter : IValueConverter{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			var str = (string)value;
			return str.IsNullOrEmpty();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}

	public class IsNotEmptyConverter : IValueConverter{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			var str = (string)value;
			return !str.IsNullOrEmpty();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}

	public class PercentageToDoubleConverter : IValueConverter{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			int p = (int)value;
			return ((double)p) / 100d;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			double p = (double)value;
			return (int)(p * 100);
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

	public class TrimStringConverter : IValueConverter{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			var text = (string)value;
			var length = Int32.Parse((string)parameter);
			return ((text != null) && (length > 0) && (text.Length > length)) ? text.Substring(0, length) : text;
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			throw new NotImplementedException();
		}
	}
	
	public class SearchOptionToIsAllDirectoriesConverter : IValueConverter{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			var option = (SearchOption)value;
			return (option == SearchOption.AllDirectories);
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			return ((bool)value) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
		}
	}

	public class BoolToProgressStateConverter : IValueConverter{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			var b = (bool)value;
			return b ? TaskbarItemProgressState.Normal : TaskbarItemProgressState.None;
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			return ((TaskbarItemProgressState)value) != TaskbarItemProgressState.None;
		}
	}

	#endregion

}
