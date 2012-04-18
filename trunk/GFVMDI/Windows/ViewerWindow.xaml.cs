using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using CatWalk.Mvvm;
using GFV.Messaging;
using WPF.MDI;

namespace GFV.Windows {
	public partial class ViewerWindow : ResourceDictionary{
		private MdiChild _MdiChild;

		public ViewerWindow(){
			this.InitializeComponent();
		}

		public void OnLoaded(object sender, RoutedEventArgs e){
			var elm = (MdiChild)sender;
			this._MdiChild = elm;
			elm.DataContextChanged += this.OnDataContextChanged;
			var dc = elm.DataContext;
			if(dc != null){
				Messenger.Default.Register<RequestRestoreBoundsMessage>(this.ReceiveRequestRestoreBoundsMessage, dc);
			}
		}

		public void OnUnloaded(object sender, RoutedEventArgs e){
			var elm = (FrameworkElement)sender;
			elm.DataContextChanged -= this.OnDataContextChanged;
		}


		private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if(e.OldValue != null){
				Messenger.Default.Unregister<RequestRestoreBoundsMessage>(this.ReceiveRequestRestoreBoundsMessage, e.OldValue);
			}
			if(e.NewValue != null){
				Messenger.Default.Register<RequestRestoreBoundsMessage>(this.ReceiveRequestRestoreBoundsMessage, e.NewValue);
			}
		}

		private void ReceiveRequestRestoreBoundsMessage(RequestRestoreBoundsMessage m){
			m.Bounds = this._MdiChild.RestoreBounds;
		}
	}
}
