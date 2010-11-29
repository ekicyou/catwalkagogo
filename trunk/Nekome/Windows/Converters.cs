using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.IO;
using Nekome.Search;
using System.Windows.Shell;

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
