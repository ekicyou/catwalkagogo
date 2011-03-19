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
using CatWalk.Windows;

namespace GFV.Windows{
	using Gfl = GflNet;

	/// <summary>
	/// Interaction logic for ViewerWindow.xaml
	/// </summary>
	public partial class ViewerWindow : Window {

		public ViewerWindow(){
			this.InitializeComponent();
		}

		private void About_Executed(object sender, ExecutedRoutedEventArgs e){
			var dialog = new AboutBox();
			dialog.Owner = this;
			dialog.ShowDialog();
		}
	}
}
