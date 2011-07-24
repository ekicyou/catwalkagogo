/*
	$Id: Account.cs 51 2010-01-28 21:55:33Z catwalk $
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using System.Windows.Markup;
using System.Collections.Specialized;
using System.Globalization;

namespace MazeGenSL.Views {
	/// <summary>
	/// Display 2 dimentional map as image.
	/// </summary>
	[TemplatePart(Name="PART_Image", Type=typeof(Image))]
	[ContentProperty("Source")]
	public sealed class MapView : Control {
		private Image _Image;
		private WriteableBitmap _Surface;

		public MapView() {
			this.DefaultStyleKey = typeof(MapView);
		}

		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			this._Image = this.GetTemplateChild("PART_Image") as Image;
			this._Image.Source = this._Surface;
			this._Image.Width = Math.Max(this.CellWidth * this.MapWidth, 0);
			this._Image.Height = Math.Max(this.CellHeight * this.MapHeight, 0);
		}

		#region Draw

		/// <summary>
		/// Re-initialize surface and draw all cells.
		/// </summary>
		private void RefreshSurface(){
			// Clear
			this._Surface = null;
			if(this._Image != null){
				this._Image.Source = null;
			}

			// Property check for surface
			var x = this.MapWidth;
			if(x <= 0){
				return;
			}
			var y = this.MapHeight;
			if(y <= 0){
				return;
			}
			var _CellSizeX = this.CellWidth;
			if(_CellSizeX <= 0){
				return;
			}
			var _CellSizeY = this.CellHeight;
			if(_CellSizeY <= 0){
				return;
			}
			this._Surface = new WriteableBitmap(x * _CellSizeX, y * _CellSizeY);
	
			// Property check for source
			if(this.Source == null){
				return;
			}
			var cellConverter = this.ValueToPixelsConverter;
			if(cellConverter == null){
				return;
			}

			var source = this.Source.GetEnumerator();
			for(var j = 0; j < y; j++){
				for(var i = 0; i < x; i++){
					if(source.MoveNext()){
						this.DrawCell(i, j, source.Current, _CellSizeX, _CellSizeY, x, cellConverter);
					}else{
						goto drawLoop;
					}
				}
			}
		drawLoop:

			if(this._Image != null){
				//this._Image.Visibility = Visibility.Visible;
				this._Image.Width = x * _CellSizeX;
				this._Image.Height = y * _CellSizeY;
				this._Image.Source = this._Surface;
			}
		}

		private int GetIndex(int x, int y){
			return x + this.MapWidth * y;
		}

		/// <summary>
		/// Draw cell at the point on surface.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="cell"></param>
		private void DrawCell(int x, int y, object cell, int _CellSizeX, int _CellSizeY, int stride, IValueConverter cellConverter){
			var x2 = x * _CellSizeX;
			var y2 = y * _CellSizeY;
			int[] src = cellConverter.Convert(cell, typeof(int[]), null, CultureInfo.CurrentUICulture) as int[];
			if(src == null){
				return;
			}

			var stridePixel = stride * _CellSizeX;
			var offset = stridePixel * y * _CellSizeY + x * _CellSizeX;
			var srcStart = 0;
			var dstStart = offset;
			for(var i = 0; i < _CellSizeY; i++, srcStart += _CellSizeX, dstStart += stridePixel){
				Array.Copy(src, srcStart, this._Surface.Pixels, dstStart, _CellSizeX);
			}
		}

		#endregion

		#region Source

		/// <summary>
		/// One-dimentional array represents 2D map data.
		/// </summary>
		public IEnumerable Source {
			get { return (IEnumerable)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}

		// Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SourceProperty =
			DependencyProperty.Register("Source", typeof(IEnumerable), typeof(MapView),
			new PropertyMetadata(null, SourceChanged));

		private static void SourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e){
			var self = (MapView)d;
			self.RefreshSurface();
			if(e.OldValue != null){
				var obs = e.OldValue as INotifyCollectionChanged;
				if(obs != null){
					obs.CollectionChanged -= self.Source_CollectionChanged;
				}
			}
			if(e.NewValue != null){
				var obs = e.NewValue as INotifyCollectionChanged;
				if(obs != null){
					obs.CollectionChanged += self.Source_CollectionChanged;
				}
			}
		}

		private void Source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e){
			switch(e.Action){
				case NotifyCollectionChangedAction.Add:{
					this.Redraw(e.NewStartingIndex);
					break;
				}
				case NotifyCollectionChangedAction.Remove:{
					this.Redraw(e.OldStartingIndex);
					break;
				}
				case NotifyCollectionChangedAction.Replace:{
					if(this._Surface == null){
						this.RefreshSurface();
					}else{
						var cellWidth = this.CellWidth;
						var cellHeight = this.CellHeight;
						var cellConverter = this.ValueToPixelsConverter;
						var stride = this.MapWidth;
						var start = e.NewStartingIndex;
						for(var i = 0; i < e.NewItems.Count; i++){
							var idx = start + i;
							var x = idx % stride;
							var y = (int)Math.Floor(idx / stride);
							var py = y * cellHeight;
							if(py >= this._Surface.PixelHeight){
								break;
							}else{
								this.DrawCell(x, y, e.NewItems[i], cellWidth, cellHeight, stride, cellConverter);
							}
						}
						this._Surface.Invalidate();
					}
					break;
				}
				case NotifyCollectionChangedAction.Reset:{
					this.RefreshSurface();
					break;
				}
			}
		}

		private void Redraw(int start){
			if(this._Surface == null){
				return;
			}
			var source = this.Source;
			if(source == null){
				return;
			}
			var cellWidth = this.CellWidth;
			var cellHeight = this.CellHeight;
			var cellConverter = this.ValueToPixelsConverter;
			var data = source.Cast<object>().Skip(start);
			var x = this.MapWidth;
			var y = this.MapHeight;
			if(x <= 0 || y <= 0){
				return;
			}
			foreach(var value in source){
				var mx = start % x;
				var my = start / x;
				var py = my * cellHeight;
				if(py >= this._Surface.PixelHeight){
					break;
				}else{
					this.DrawCell(mx, my, value, cellWidth, cellHeight, x, cellConverter);
				}
			}
			this._Surface.Invalidate();
		}

		#endregion

		#region MapSize

		public int MapWidth {
			get { return (int)GetValue(MapWidthProperty); }
			set { SetValue(MapWidthProperty, value); }
		}

		public static readonly DependencyProperty MapWidthProperty =
			DependencyProperty.Register("MapWidth", typeof(int), typeof(MapView), new PropertyMetadata(0, MapSizeChanged));


		public int MapHeight {
			get { return (int)GetValue(MapHeightProperty); }
			set { SetValue(MapHeightProperty, value); }
		}

		public static readonly DependencyProperty MapHeightProperty =
			DependencyProperty.Register("MapHeight", typeof(int), typeof(MapView), new PropertyMetadata(0, MapSizeChanged));

		private static void MapSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e){
			var self = (MapView)d;
			self.RefreshSurface();
		}

		#endregion

		#region BitmapConverter

		/// <summary>
		/// An IValueConverter converts value of map data to bitmap pixels (int[]).
		/// </summary>
		public IValueConverter ValueToPixelsConverter {
			get { return (IValueConverter)GetValue(ValueToPixelsConverterProperty); }
			set { SetValue(ValueToPixelsConverterProperty, value); }
		}

		public static readonly DependencyProperty ValueToPixelsConverterProperty =
			DependencyProperty.Register("ValueToPixelsConverter", typeof(IValueConverter), typeof(MapView), new PropertyMetadata(null, ValueToPixelsConverterChanged));

		private static void ValueToPixelsConverterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e){
			var self = (MapView)d;
			self.RefreshSurface();
		}

		#endregion

		#region CellSize

		public int CellWidth {
			get { return (int)GetValue(CellWidthProperty); }
			set { SetValue(CellWidthProperty, value); }
		}

		public static readonly DependencyProperty CellWidthProperty =
			DependencyProperty.Register("CellWidth", typeof(int), typeof(MapView), new PropertyMetadata(1, CellSizeChanged));

		public int CellHeight {
			get { return (int)GetValue(CellHeightProperty); }
			set { SetValue(CellHeightProperty, value); }
		}

		public static readonly DependencyProperty CellHeightProperty =
			DependencyProperty.Register("CellHeight", typeof(int), typeof(MapView), new PropertyMetadata(1, CellSizeChanged));

		private static void CellSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e){
			var self = (MapView)d;
			self.RefreshSurface();
		}

		#endregion
	}
}
