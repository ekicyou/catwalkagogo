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

		private class ViewerView : IViewerView, IDisposable{
			private Viewer _Self;
			private Window _Window;
			public ViewerView(Viewer self){
				this._Self = self;
				this._Self._ScrollViewer.SizeChanged += this.Viewer_SizeChanged;
				this._Window = Window.GetWindow(this._Self);
				if(this._Window != null){
					this._Window.Loaded += this.Window_StateChanged;
					this._Window.StateChanged += this.Window_StateChanged;
				}
			}

			public Size ViewerSize{
				get{
					var sv = this._Self._ScrollViewer;
					return new Size(sv.ViewportWidth, sv.ViewportHeight);
				}
			}

			/// <summary>
			/// For a bug the viewport size is not refreshed immediately when the window state is changed.
			/// </summary>
			private void Window_StateChanged(object sender, EventArgs e){
				if(this.SizeChanged != null){
					var ui = TaskScheduler.FromCurrentSynchronizationContext();
					var task = new Task(delegate{
						Thread.Sleep(1);
					});
					task.ContinueWith(delegate{
						this.SizeChanged(this, new ViewerSizeChangedEventArgs(this.ViewerSize));
					}, ui);
					task.Start();
				}
			}

			private void Viewer_SizeChanged(object sender, SizeChangedEventArgs e){
				if(this.SizeChanged != null){
					this.SizeChanged(this, new ViewerSizeChangedEventArgs(this.ViewerSize));
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
						this._Self._ScrollViewer.SizeChanged -= this.Viewer_SizeChanged;
						if(this._Window != null){
							this._Window.Loaded -= this.Window_StateChanged;
							this._Window.SizeChanged -= this.Window_StateChanged;
						}
					}));
					this.disposed = true;
				}
			}

			#endregion
		}
	}
}
