/*
	$Id: Account.cs 51 2010-01-28 21:55:33Z catwalk $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MazeGenSL.Views {
	public partial class MainPage : UserControl {
		public MainPage() {
			InitializeComponent();
		}

		private Point _DragStartPos;
		private bool _IsDragging = false;
		private void ScrollViewer_RightMouseButtonDown(object sender, MouseButtonEventArgs e) {
			//((FrameworkElement)sender).MouseMove += this.ScrollViewer_MouseMove;
			var elm = (FrameworkElement)sender;
			elm.CaptureMouse();
			this._IsDragging = true;
			this._DragStartPos = e.GetPosition(this._ScrollViewer);
			e.Handled = true;
		}

		private void ScrollViewer_RightMouseButtonUp(object sender, MouseButtonEventArgs e) {
			//((FrameworkElement)sender).MouseMove -= this.ScrollViewer_MouseMove;
			var elm = (FrameworkElement)sender;
			elm.ReleaseMouseCapture();
			this._IsDragging = false;
			e.Handled = true;
		}

		private void ScrollViewer_MouseMove(object sender, MouseEventArgs e) {
			const double alpha = 2;
			if(!this._IsDragging){
				return;
			}
			var pos = e.GetPosition(this._ScrollViewer);
			var deltaX = (pos.X - this._DragStartPos.X) * alpha;
			var deltaY = (pos.Y - this._DragStartPos.Y) * alpha;
			this._ScrollViewer.ScrollToHorizontalOffset(this._ScrollViewer.HorizontalOffset - deltaX);
			this._ScrollViewer.ScrollToVerticalOffset(this._ScrollViewer.VerticalOffset - deltaY);
			this._DragStartPos = pos;
		}

		private void ScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e) {
			var offsetX = this._ScrollViewer.HorizontalOffset;
			var offsetY = this._ScrollViewer.VerticalOffset;
			var width = this._ScrollViewer.ViewportWidth;
			var height = this._ScrollViewer.ViewportWidth;
			var center = e.GetPosition(this._ScrollViewer);

			var zoom = this._ScrollViewer_ScaleTransform.ScaleX;
			var newZoom = (e.Delta > 0) ? zoom / 1.1 : zoom * 1.1;
			//zoom = Math.Min(Math.Max(zoom, 0.1), 8);
			this._ScrollViewer_ScaleTransform.ScaleX = this._ScrollViewer_ScaleTransform.ScaleY = newZoom;
			this._ScrollViewer_LayoutTransformer.ApplyLayoutTransform();

			this._ScrollViewer.ScrollToHorizontalOffset(offsetX / zoom * newZoom);
			this._ScrollViewer.ScrollToVerticalOffset(offsetY / zoom * newZoom);
			e.Handled = true;
		}

		private int _ErrorCount = 0;
		private void OnBindingValidationError(object sender, ValidationErrorEventArgs e) {
			switch(e.Action){
				case ValidationErrorEventAction.Added:
					this._ErrorCount++;
					break;
				case ValidationErrorEventAction.Removed:
					this._ErrorCount--;
					break;
			}
			if(this.DataContext != null){
				Messenger.Default.Send<ValidationErrorMessage>(new ValidationErrorMessage(this, e.Action, this._ErrorCount), this.DataContext);
			}
		}

		private void Test(object sender, MouseButtonEventArgs e) {
			MessageBox.Show("unko");
		}
	}
}
