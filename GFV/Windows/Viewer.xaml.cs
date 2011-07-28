/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;
using System.Threading.Tasks;
using GFV.ViewModel;
using GFV.Properties;
using CatWalk.Mvvm;

namespace GFV.Windows {
	/// <summary>
	/// Interaction logic for Viewer.xaml
	/// </summary>
	[SendMessage(typeof(SizeMessage))]
	[SendMessage(typeof(RequestScaleMessage))]
	public partial class Viewer : UserControl{

		public Viewer(){
			InitializeComponent();

			this.Loaded += this.Viewer_Loaded;
		}

		private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e){
			if(e.NewValue != null){
				this.SendViewerSize();
			}
			this.RefreshInputBindings();
		}

		private void RefreshInputBindings(){
			var infos = Settings.Default.ViewerInputBindingInfos;
			if(infos != null){
				InputBindingInfo.ApplyInputBindings(this, infos);
			}
		}

		#region Viewer Size

		private void Viewer_Loaded(object sender, EventArgs e){
			this.SendViewerSize();
		}

		private void SendViewerSize(){
			//MessageBox.Show(new Size(this._ScrollViewer.ViewportWidth, this._ScrollViewer.ViewportHeight).ToString());
			if(this.DataContext != null){
				Messenger.Default.Send<SizeMessage>(
					new SizeMessage(
						this,
						new Size(this._ScrollViewer.ViewportWidth, this._ScrollViewer.ViewportHeight)),
						this.DataContext);
			}
		}

		private void Viewer_ScrollChanged(object sender, ScrollChangedEventArgs e) {
			var sv = (ScrollViewer)sender;
			// Ignore oscillation that the scroll bar is shown and hide.
			if((e.ViewportHeightChange != 0 || e.ViewportWidthChange != 0) && (this.DataContext != null)){
				this.SendViewerSize();
			}
		}

		#endregion

		#region Scroll

		private void LineUp_Executed(object sender, ExecutedRoutedEventArgs e) {
			this._ScrollViewer.LineUp();
		}

		private void LineDown_Executed(object sender, ExecutedRoutedEventArgs e) {
			this._ScrollViewer.LineDown();
		}

		private void LineLeft_Executed(object sender, ExecutedRoutedEventArgs e) {
			this._ScrollViewer.LineLeft();
		}

		private void LineRight_Executed(object sender, ExecutedRoutedEventArgs e) {
			this._ScrollViewer.LineRight();
		}

		private void PageUp_Executed(object sender, ExecutedRoutedEventArgs e) {
			this._ScrollViewer.PageUp();
		}

		private void PageDown_Executed(object sender, ExecutedRoutedEventArgs e) {
			this._ScrollViewer.PageDown();
		}

		private void ScrollToTop_Executed(object sender, ExecutedRoutedEventArgs e) {
			this._ScrollViewer.ScrollToTop();
		}

		private void ScrollToBottom_Executed(object sender, ExecutedRoutedEventArgs e) {
			this._ScrollViewer.ScrollToBottom();
		}

		#endregion

		#region DragScroll

		private Point _DragStartPos;
		private bool _IsDragging = false;
		private void _PictureBox_MouseDown(object sender, MouseButtonEventArgs e) {
			if(e.ChangedButton == MouseButton.Left){
				var elm = (FrameworkElement)sender;
				elm.MouseMove += this._PictureBox_MouseMove;
				elm.CaptureMouse();
				this._IsDragging = true;
				this._DragStartPos = e.GetPosition(this._ScrollViewer);
				this._ScrollViewer.Cursor = Cursors.ScrollAll;
				e.Handled = true;
			}
		}

		private void _PictureBox_MouseUp(object sender, MouseButtonEventArgs e) {
			if(e.ChangedButton == MouseButton.Left){
				var elm = (FrameworkElement)sender;
				elm.MouseMove -= this._PictureBox_MouseMove;
				elm.ReleaseMouseCapture();
				this._IsDragging = false;
				this._ScrollViewer.Cursor = null;
				e.Handled = true;
			}
		}

		private void _PictureBox_MouseMove(object sender, MouseEventArgs e) {
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
		
		private void _ScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e) {
			if(this.DataContext == null){
				return;
			}

			var offsetX = this._ScrollViewer.HorizontalOffset;
			var offsetY = this._ScrollViewer.VerticalOffset;
			var width = this._ScrollViewer.ViewportWidth;
			var height = this._ScrollViewer.ViewportWidth;
			var center = e.GetPosition(this._ScrollViewer);

			var mes = new RequestScaleMessage(this);
			Messenger.Default.Send(mes, this.DataContext);

			var zoom = mes.Scale;
			var newZoom = (e.Delta > 0) ? zoom / 1.1 : zoom * 1.1;
			newZoom = Math.Min(Math.Max(newZoom, 0.01), 8);
			Messenger.Default.Send(new ScaleMessage(this, newZoom), this.DataContext);

			this._ScrollViewer.ScrollToHorizontalOffset(offsetX / zoom * newZoom);
			this._ScrollViewer.ScrollToVerticalOffset(offsetY / zoom * newZoom);
			e.Handled = true;
		}
		
		#endregion

		#region Focus

		private void ScrollViewer_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
			if(e.NewFocus == this._PictureBox){
				this._PictureBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
				e.Handled = true;
			}
		}

		private void PictureBox_GotFocus(object sender, KeyboardFocusChangedEventArgs e) {
			this._ScrollViewer.Focus();
			e.Handled = true;
		}

		protected override void OnGotFocus(RoutedEventArgs e) {
			this._ScrollViewer.Focus();
			base.OnGotFocus(e);
		}

		private void ScrollViewer_PreviewKeyDown(object sender, KeyEventArgs e) {
			if(e.KeyboardDevice.Modifiers == ModifierKeys.None){
				switch(e.Key){
					case Key.Up:
					case Key.Down:
					case Key.Left:
					case Key.Right:
					case Key.Home:
					case Key.End:
					case Key.PageDown:
					case Key.PageUp:{
						e.Handled = true;
						var newEvent = new KeyEventArgs(e.KeyboardDevice, e.InputSource, e.Timestamp, e.Key);
						newEvent.RoutedEvent = Keyboard.KeyDownEvent;
						this.RaiseEvent(newEvent);
						break;
					}
				}
			}
		}

		#endregion
	}
}

