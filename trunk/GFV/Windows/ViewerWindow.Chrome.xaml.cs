using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Shell;
using System.Windows.Threading;
using CatWalk.Mvvm;
using CatWalk.Windows;
using GFV.Properties;
using GFV.ViewModel;
using GFV.Messaging;
using System.Runtime.InteropServices;

namespace GFV.Windows {
	public partial class ViewerWindow_Chrome : ResourceDictionary{
		public ViewerWindow_Chrome(){
			this.InitializeComponent();
		}

		private void AppMenu_MouseRightButtonUp(object sender, MouseEventArgs e){
			var elm = (FrameworkElement)sender;
			SystemCommands.ShowSystemMenu(Window.GetWindow(elm), elm.PointToScreen(e.GetPosition(elm)));
		}
	}
}
