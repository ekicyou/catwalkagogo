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

		#region ViewerView

		private class ViewerView : IViewerView, IDisposable{
			private Viewer _Self;
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

			private void Viewer_ScrollChanged(object sender, ScrollChangedEventArgs e){
				if(e.ViewportHeightChange != 0 || e.ViewportWidthChange != 0){
					var eh = this.SizeChanged;
					if(eh != null){
						eh(this, new ViewerSizeChangedEventArgs(this.ViewerSize));
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
						/*
						this._Self._ScrollViewer.SizeChanged -= this.Viewer_SizeChanged;
						if(this._Window != null){
							this._Window.Loaded -= this.Window_StateChanged;
							this._Window.SizeChanged -= this.Window_StateChanged;
						}
						 * */
					}));
					this.disposed = true;
				}
			}

			#endregion
		}

		#endregion
	}
}
