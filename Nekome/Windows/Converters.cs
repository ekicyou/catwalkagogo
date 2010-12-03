using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.IO;
using Nekome.Search;
using System.Windows.Shell;
using CatWalk;

namespace Nekome.Windows{

	public class FilePathConverter : IValueConverter{
		public string BasePath{get; set;}

		public FilePathConverter(){
		}

		public FilePathConverter(string basePath){
			this.BasePath = basePath;
		}


		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			var path = (string)value;
			if(path.StartsWith(this.BasePath, StringComparison.OrdinalIgnoreCase)){
				return path.Substring(this.BasePath.Length);
			}else{
				return path;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			throw new NotImplementedException();
		}
	}

	public class FileSizeConverter : IValueConverter{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			var str = ((string)value).ToLower();
			var idx = str.IndexOfRegex("[a-z]");
			var suffix = str.Substring(idx).Trim();
			var numberPart = str.Substring(0, idx).Trim().ReplaceRegex(@"[,]|\w", "");
			decimal number;
			if(Decimal.TryParse(numberPart, out number)){
				switch(suffix){
					case "k": case "kb": return number * 1024m;
					case "m": case "mb": return number * 1024m * 1024m;
					case "g": case "gb": return number * 1024m * 1024m * 1024m;
					case "t": case "tb": return number * 1024m * 1024m * 1024m * 1024m;
					case "ki": case "kib": return number * 1000m;
					case "mi": case "mib": return number * 1000m * 1000m;
					case "gi": case "gib": return number * 1000m * 1000m * 1000m;
					case "ti": case "tib": return number * 1000m * 1000m * 1000m * 1000m;
					default: return number;
				}
			}else{
				throw new FormatException();
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			var size = (decimal)value;

			bool ti = (size % (1000m * 1000m * 1000m * 1000m) == 0);
			bool t  = (size % (1024m * 1024m * 1024m * 1024m) == 0);
			if(t && ti){      return (size / 1024 / 1024 / 1024 / 1024).ToString() + "TB";}
			else if(t && !ti){return (size / 1024 / 1024 / 1024 / 1024).ToString() + "TB";}
			else if(!t && ti){return (size / 1000 / 1000 / 1000 / 1000).ToString() + "TiB";}

			bool gi = (size % (1000 * 1000 * 1000) == 0);
			bool g  = (size % (1024 * 1024 * 1024) == 0);
			if(g && gi){      return (size / 1024 / 1024 / 1024).ToString() + "GB";}
			else if(g && !gi){return (size / 1024 / 1024 / 1024).ToString() + "GB";}
			else if(!g && gi){return (size / 1000 / 1000 / 1000).ToString() + "GiB";}


			bool mi = (size % (1000 * 1000) == 0);
			bool m  = (size % (1024 * 1024) == 0);
			if(m && mi){      return (size / 1024 / 1024).ToString() + "MB";}
			else if(m && !mi){return (size / 1024 / 1024).ToString() + "MB";}
			else if(!m && mi){return (size / 1000 / 1000).ToString() + "MiB";}

			bool ki = (size % 1000 == 0);
			bool k  = (size % 1024 == 0);
			if(k && ki){ return (size / 1024).ToString() + "KB";}
			else if(k && !ki){return (size / 1024).ToString() + "KB";}
			else if(!k && ki){return (size / 1000).ToString() + "KiB";}

			return size.ToString() + "B";
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
}
