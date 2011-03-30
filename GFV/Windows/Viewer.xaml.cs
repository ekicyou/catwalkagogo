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
			var vm = e.NewValue as ViewerViewModel;
			var old = e.OldValue as ViewerViewModel;
			if(old != null){
				var oldView = (ViewerView)old.View;
				old.View = null;
				oldView.Dispose();
			}
			if(vm != null){
				vm.View = new ViewerView(this);
			}
		}

		#region ViewerView

		private class ViewerView : IViewerView, IDisposable{
			private Viewer _Self;
			private double _OldWidth;
			private double _OldHeight;
			public ViewerView(Viewer self){
				this._Self = self;
				this._Self._ScrollViewer.ScrollChanged += this.Viewer_ScrollChanged;
			}

			public Size ViewerSize{
				get{
					var sv = this._Self._ScrollViewer;
					var width = sv.ViewportWidth;
					var height = sv.ViewportHeight;
					return new Size(width, height);
				}
			}

			private CancellationTokenSource _SizeChangedCancellationTokenSource;
			private void Viewer_ScrollChanged(object sender, ScrollChangedEventArgs e){
				var sv = (ScrollViewer)sender;
				// Ignore oscillation that the scroll bar is shown and hide.
				if((this._OldWidth != sv.ActualWidth || this._OldHeight != sv.ActualHeight) && (e.ViewportHeightChange != 0 || e.ViewportWidthChange != 0)){
					this._OldWidth = sv.ActualWidth;
					this._OldHeight = sv.ActualHeight;
					var eh = this.SizeChanged;
					if(eh != null){
						if(this._SizeChangedCancellationTokenSource != null){
							this._SizeChangedCancellationTokenSource.Cancel();
						}
						eh(this, new ViewerSizeChangedEventArgs(this.ViewerSize, false));

						this._SizeChangedCancellationTokenSource = new CancellationTokenSource();
						var ui = TaskScheduler.FromCurrentSynchronizationContext();
						var task = new Task(delegate{
							Thread.Sleep(100);
						}, this._SizeChangedCancellationTokenSource.Token);
						task.ContinueWith(delegate{
							eh(this, new ViewerSizeChangedEventArgs(this.ViewerSize, true));
							this._SizeChangedCancellationTokenSource.Dispose();
							this._SizeChangedCancellationTokenSource = null;
						}, this._SizeChangedCancellationTokenSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, ui);
						task.Start();
					}
				}
			}

			public event ViewerSizeChangedEventHandler SizeChanged;

			#region IDisposable

			public void Dispose(){
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}
		
			~ViewerView(){
				this.Dispose(false);
			}
		
			private bool disposed = false;
			protected virtual void Dispose(bool disposing){
				if(!(this.disposed)){
					this._Self.Dispatcher.Invoke(new Action(delegate{
						this._Self._ScrollViewer.ScrollChanged -= this.Viewer_ScrollChanged;
					}));
					this.disposed = true;
				}
			}

			#endregion
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
	}
}
