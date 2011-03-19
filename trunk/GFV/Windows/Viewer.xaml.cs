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

namespace GFV.Windows {
	/// <summary>
	/// Interaction logic for Viewer.xaml
	/// </summary>
	public partial class Viewer : UserControl{
		public Viewer(){
			InitializeComponent();
		}

		private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e){
			var vm = e.NewValue as ViewModelBase;
			if(vm != null){
				vm.View = this;
			}
		}
	}
}
