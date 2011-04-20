using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.ComponentModel;
using System.Reflection;

namespace WPF.MDI {
	public class MdiCaptionButtons : ContentControl{
		private Button _MinimizeButton;
		private Button _MaximizeButton;
		private Button _CloseButton;

		static MdiCaptionButtons(){
			FocusableProperty.OverrideMetadata(typeof(MdiCaptionButtons), new FrameworkPropertyMetadata(false));
		}

		public MdiCaptionButtons(){
		}

		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			this._MaximizeButton = this.Template.FindName("PART_MaximizeButton", this) as Button;
			this._MinimizeButton = this.Template.FindName("PART_MinimizeButton", this) as Button;
			this._CloseButton = this.Template.FindName("PART_CloseButton", this) as Button;

			if(this._MinimizeButton != null){
				this._MinimizeButton.Click += this.MinimizeButton_Clicked;
			}
			if(this._MaximizeButton != null){
				this._MaximizeButton.Click += this.MaximizeButton_Clicked;
			}
			if(this._CloseButton != null){
				this._CloseButton.Click += this.CloseButton_Clicked;
			}
		}

		private void MinimizeButton_Clicked(object sender, RoutedEventArgs e){
			var container = this.Container;
			if(container != null){
				var child = container.ActiveMdiChild;
				if(child != null){
					if(child.WindowState == WindowState.Minimized){
						child.WindowState = WindowState.Normal;
					}else{
						child.WindowState = WindowState.Minimized;
					}
				}
			}
		}

		private void MaximizeButton_Clicked(object sender, RoutedEventArgs e){
			var container = this.Container;
			if(container != null){
				var child = container.ActiveMdiChild;
				if(child != null){
					if(child.WindowState != WindowState.Maximized){
						child.WindowState = WindowState.Maximized;
					}else{
						child.WindowState = WindowState.Normal;
					}
				}
			}
		}

		private void CloseButton_Clicked(object sender, RoutedEventArgs e){
			var container = this.Container;
			if(container != null){
				var child = container.ActiveMdiChild;
				if(child != null){
					child.Close();
				}
			}
		}

		public MdiContainer Container {
			get { return (MdiContainer)GetValue(ContainerProperty); }
			set { SetValue(ContainerProperty, value); }
		}

		public static readonly DependencyProperty ContainerProperty =
			DependencyProperty.Register("Container", typeof(MdiContainer), typeof(MdiCaptionButtons), new UIPropertyMetadata(null));
	}
}
