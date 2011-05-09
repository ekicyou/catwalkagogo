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
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Collections.Generic;

namespace CatWalk.SLGameLib.Views {
	[Obsolete]
	[TemplatePart(Name="PART_Image", Type=typeof(Image))]
	[ContentProperty("Source")]
	public sealed class SpliteView : Control {
		private Image _Image;
		private WriteableBitmap _Surface;

		public SpliteView() {
			this.DefaultStyleKey = typeof(MapView);
		}

		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			this._Image = this.GetTemplateChild("PART_Image") as Image;
			this._Image.Source = this._Surface;
			this._Image.Width = Math.Max(this.SurfaceWidth, 0);
			this._Image.Height = Math.Max(this.SurfaceHeight, 0);
		}

		public void Invalidate(){
			if(this._Surface != null){
				this._Surface.Invalidate();
			}
		}

		#region SurfaceSize

		public int SurfaceWidth {
			get { return (int)GetValue(SurfaceWidthProperty); }
			set { SetValue(SurfaceWidthProperty, value); }
		}

		public static readonly DependencyProperty SurfaceWidthProperty =
			DependencyProperty.Register("PixelWidth", typeof(int), typeof(SpliteView), new PropertyMetadata(0));

		public int SurfaceHeight {
			get { return (int)GetValue(SurfaceHeightProperty); }
			set { SetValue(SurfaceHeightProperty, value); }
		}

		public static readonly DependencyProperty SurfaceHeightProperty =
			DependencyProperty.Register("PixelHeight", typeof(int), typeof(SpliteView), new PropertyMetadata(0));

		#endregion

		#region Source

		public IEnumerable<ISpliteObject> Source{
			get { return (IEnumerable<ISpliteObject>)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}

		public static readonly DependencyProperty SourceProperty = 
			DependencyProperty.Register("Source", typeof(IEnumerable<ISpliteObject>), typeof(SpliteView), new PropertyMetadata(null));

		#endregion
	}

	public interface ISpliteObject{
		Int32Rect SpliteBounds{get;}
		event BoundsChangedEventHandler SpliteBoundsChanged;
		int[] SplitePixels{get;}
		event EventHandler SplitePixelsChanged;
	}

	public delegate void BoundsChangedEventHandler(object sender, BoundsChangedEventArgs e);

	public class BoundsChangedEventArgs : EventArgs{
		public Int32Rect OldRect{get; private set;}
		public Int32Rect NewRect{get; private set;}

		public BoundsChangedEventArgs(Int32Rect oldRect, Int32Rect newRect){
			this.OldRect = oldRect;
			this.NewRect = newRect;
		}
	}
}
