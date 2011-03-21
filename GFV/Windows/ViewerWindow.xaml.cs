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
using GFV.ViewModel;
using GFV.Properties;
using CatWalk.Windows;

namespace GFV.Windows{
	using Gfl = GflNet;

	/// <summary>
	/// Interaction logic for ViewerWindow.xaml
	/// </summary>
	public partial class ViewerWindow : Window {

		public ViewerWindow(){
			this.InitializeComponent();

			this.WindowStartupLocation = WindowStartupLocation.Manual;
			this.Width = Settings.Default.ViewerWindowRestoreBounds.Width;
			this.Height = Settings.Default.ViewerWindowRestoreBounds.Height;
			this.Left = Settings.Default.ViewerWindowRestoreBounds.Left;
			this.Top = Settings.Default.ViewerWindowRestoreBounds.Top;
			this.WindowState = Settings.Default.ViewerWindowState;
		}

		private void About_Executed(object sender, ExecutedRoutedEventArgs e){
			var dialog = new AboutBox();
			dialog.Owner = this;
			dialog.ShowDialog();
		}

		protected override void OnStateChanged(EventArgs e){
			base.OnStateChanged(e);
			if(this.WindowState != WindowState.Minimized){
				Settings.Default.ViewerWindowState = this.WindowState;
			}
		}

		protected override void OnLocationChanged(EventArgs e){
			base.OnLocationChanged(e);
			Settings.Default.ViewerWindowRestoreBounds = this.RestoreBounds;
		}

		private void Window_SizeChanged(object sender, SizeChangedEventArgs e){
			Settings.Default.ViewerWindowRestoreBounds = this.RestoreBounds;
		}
	}
}
