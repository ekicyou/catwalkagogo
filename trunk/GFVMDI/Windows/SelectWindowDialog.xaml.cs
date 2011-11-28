using System;
using System.Collections;
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
using System.Windows.Interop;
using Microsoft.Windows.Shell;

namespace GFV.Windows {
	using Win32 = CatWalk.Win32;

	/// <summary>
	/// Interaction logic for SelectWindowDialog.xaml
	/// </summary>
	public partial class SelectWindowDialog : Window {
		public SelectWindowDialog() {
			InitializeComponent();
		}

		public IEnumerable ItemsSource {
			get { return (IEnumerable)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		// Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ItemsSourceProperty =
			DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(SelectWindowDialog), new UIPropertyMetadata(null));

		public object SelectedValue {
			get { return (object)GetValue(SelectedValueProperty); }
			set { SetValue(SelectedValueProperty, value); }
		}

		// Using a DependencyProperty as the backing store for SelectedValue.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SelectedValueProperty =
			DependencyProperty.Register("SelectedValue", typeof(object), typeof(SelectWindowDialog), new UIPropertyMetadata(null));

		public ModifierKeys HoldModifiers {
			get { return (ModifierKeys)GetValue(HoldModifiersProperty); }
			set { SetValue(HoldModifiersProperty, value); }
		}

		// Using a DependencyProperty as the backing store for HoldModifiers.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty HoldModifiersProperty =
			DependencyProperty.Register("HoldModifiers", typeof(ModifierKeys), typeof(SelectWindowDialog), new UIPropertyMetadata(ModifierKeys.None));

		private void _this_Loaded(object sender, RoutedEventArgs e) {
			if(SystemParameters2.Current.IsGlassEnabled){
				var src = ((HwndSource)HwndSource.FromVisual(this));
				src.CompositionTarget.BackgroundColor = Color.FromArgb(0, 0, 0, 0);

				var blur = new Win32::DwmBlurBehind();
				blur.Enabled = true;

				Win32::Dwm.EnableBlurBehindWindow(src.Handle, blur);
			}
			var screen = this.GetCurrentScreen();
			this.MaxWidth = screen.ScreenArea.Width / 3d * 2d;
			this.MaxHeight = screen.ScreenArea.Height / 3d * 2d;
			this.AdjustPosition();
		}

		protected override void OnDeactivated(EventArgs e){
			base.OnDeactivated(e);
			try{
				this.Close();
			}catch{
			}
		}

		private bool _IsKeyUp = false;
		protected override void OnPreviewKeyUp(KeyEventArgs e){
			base.OnPreviewKeyDown(e);
			if((e.KeyboardDevice.Modifiers & this.HoldModifiers) != this.HoldModifiers){
				this.Close();
				e.Handled = true;
			}else if(!this._IsKeyUp){
				this._IsKeyUp = true;
				e.Handled = true;
			}
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e){
			base.OnPreviewKeyUp(e);
			if(e.Key == Key.Tab){
				e.Handled = true;
				if(this._IsKeyUp){
					if((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift){
						this.SelectPrevious();
					}else{
						this.SelectNext();
					}
				}
			}
		}

		private void SelectPrevious(){
			if(this._SelectBox.Items.Count > 0){
				var idx = this._SelectBox.SelectedIndex;
				if(idx < 0){
					idx = 0;
				}else{
					idx--;
				}
				if(idx < 0){
					idx = this._SelectBox.Items.Count - 1;
				}
				this._SelectBox.SelectedIndex = idx;
			}
		}

		private void SelectNext(){
			if(this._SelectBox.Items.Count > 0){
				var idx = this._SelectBox.SelectedIndex;
				if(idx < 0){
					idx = 0;
				}else{
					idx++;
				}
				if(this._SelectBox.Items.Count <= idx){
					idx = 0;
				}
				this._SelectBox.SelectedIndex = idx;
			}
		}

		private void _this_SizeChanged(object sender, SizeChangedEventArgs e) {
			this.AdjustPosition();
		}

		private void AdjustPosition(){
			var screen = this.GetCurrentScreen();
			this.Left = (screen.ScreenArea.Width - this.ActualWidth) / 2d + screen.ScreenArea.X;
			this.Top = (screen.ScreenArea.Height - this.ActualHeight) / 2d + screen.ScreenArea.Y;
		}
	}
}
