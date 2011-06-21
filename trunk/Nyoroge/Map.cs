/*
	$Id$
*/
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using CatWalk.SLGameLib;

namespace Nyoroge {
	using TCell = MapObject;
	public class Map : ViewModelBase{
		private ObservableCollection<TCell> _Data;
		private ReadOnlyObservableCollection<TCell> _ReadOnlyData;
		public ReadOnlyObservableCollection<TCell> Data{
			get{
				return this._ReadOnlyData ?? (this._ReadOnlyData = new ReadOnlyObservableCollection<TCell>(this._Data));
			}
		}
		
		private Int32Size _Size;
		public Int32Size Size{
			get{
				return this._Size;
			}
			set{
				this.Resize(value);
			}
		}

		public Map(int width, int height) : this(new Int32Size(width, height)){}
		public Map(Int32Size size){
			this.Resize(size);
			this.ValueToPixelsConverter = new MapValueToPixelsConverter();
		}

		private void Resize(Int32Size size){
			this._Data = new ObservableCollection<TCell>(new TCell[size.Width * size.Height]);
			this._ReadOnlyData = null;
			this._Size = size;
			this.OnPropertyChanged("Data");
			this.OnPropertyChanged("Size");
		}

		private int GetIndex(int x, int y){
			return this._Size.Width * y + x;
		}

		public void PutObject(TCell obj, Int32Point point){
			this.PutObject(obj, point.X, point.Y);
		}

		public void PutObject(TCell obj, int x, int y){
			var idx = this.GetIndex(x, y);
			if(idx >= this._Data.Count){
				throw new ArgumentOutOfRangeException();
			}

			this._Data[idx] = obj;
		}

		public void RemoveObject(Int32Point point){
			this.RemoveObject(point.X, point.Y);
		}

		public void RemoveObject(int x, int y){
			var idx = this.GetIndex(x, y);
			if(idx >= this._Data.Count){
				throw new ArgumentOutOfRangeException();
			}

			this._Data[idx] = null;
		}

		public IValueConverter ValueToPixelsConverter{get; private set;}

		private class MapValueToPixelsConverter : IValueConverter{
			private static int[] EmptyPixels;
			static MapValueToPixelsConverter(){
				var wbmp = new WriteableBitmap(16, 16);
				EmptyPixels = wbmp.Pixels;
			}

			#region IValueConverter Members

			public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
				var obj = (MapObject)value;
				if(obj != null){
					return obj.Pixels;
				}else{
					return EmptyPixels;
				}
			}

			public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
				throw new NotImplementedException();
			}

			#endregion
		}
	}
}
