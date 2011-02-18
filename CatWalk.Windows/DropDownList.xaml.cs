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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CatWalk.Windows {
	public partial class DropDownList : UserControl{
		public DropDownList(){
			InitializeComponent();
		}

		private class DropDownListItem : DependencyObject{
			public static readonly DependencyProperty IsSelectedProperty =
				DependencyProperty.Register("IsSelected", typeof(bool), typeof(DropDownListItem));
			public bool IsSelected{
				get{
					return (bool)this.GetValue(IsSelectedProperty);
				}
				set{
					this.SetValue(IsSelectedProperty, value);
				}
			}

		}
	}
}
