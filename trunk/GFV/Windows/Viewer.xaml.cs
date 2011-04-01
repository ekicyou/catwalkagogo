/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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

namespace GFV.Windows {
	/// <summary>
	/// Interaction logic for Viewer.xaml
	/// </summary>
	public partial class Viewer : UserControl{

		public Viewer(){
			InitializeComponent();

			this._ScrollViewer.CommandBindings.Clear();
			this._ScrollViewer.InputBindings.Clear();

		}

		private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e){
			if(e.NewValue != null){
				this.SendViewerSize(true);
			}
		}

		private void SendViewerSize(bool isUpdate){
				Messenger.Default.Send<ViewerSizeChangedMessage>(
					new ViewerSizeChangedMessage(
						this,
						new Size(this._ScrollViewer.ViewportWidth, this._ScrollViewer.ViewportHeight), isUpdate),
						this.DataContext);
		}

		private double _OldWidth;
		private double _OldHeight;
		private CancellationTokenSource _SizeChangedCancellationTokenSource;
		private void Viewer_ScrollChanged(object sender, ScrollChangedEventArgs e) {
			var sv = (ScrollViewer)sender;
			// Ignore oscillation that the scroll bar is shown and hide.
			if((this._OldWidth != sv.ActualWidth || this._OldHeight != sv.ActualHeight) && (e.ViewportHeightChange != 0 || e.ViewportWidthChange != 0)){
				this._OldWidth = sv.ActualWidth;
				this._OldHeight = sv.ActualHeight;
				if(this.DataContext != null){
					if(this._SizeChangedCancellationTokenSource != null){
						this._SizeChangedCancellationTokenSource.Cancel();
					}
					this.SendViewerSize(false);

					this._SizeChangedCancellationTokenSource = new CancellationTokenSource();
					var ui = TaskScheduler.FromCurrentSynchronizationContext();
					var task = new Task(delegate{
						Thread.Sleep(100);
					}, this._SizeChangedCancellationTokenSource.Token);
					task.ContinueWith(delegate{
						this.SendViewerSize(true);
						this._SizeChangedCancellationTokenSource.Dispose();
						this._SizeChangedCancellationTokenSource = null;
					}, this._SizeChangedCancellationTokenSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, ui);
					task.Start();
				}
			}
		}

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

	}
}
