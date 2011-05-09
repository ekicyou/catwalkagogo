/*
	$Id$
*/
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace MazeGenSL.Views{
	public static class TextBoxEx{
		public static readonly DependencyProperty IsSelectAllOnFocusProperty =
			DependencyProperty.RegisterAttached("IsSelectAllOnFocus", typeof(bool), typeof(TextBoxEx), new PropertyMetadata(false, IsSelectAllOnFocusChanged));
		
		public static bool GetIsSelectAllOnFocus(DependencyObject obj){
			return (bool)obj.GetValue(IsSelectAllOnFocusProperty);
		}
		
		public static void SetIsSelectAllOnFocus(DependencyObject obj, bool value){
			obj.SetValue(IsSelectAllOnFocusProperty, value);
		}
		
		private static void IsSelectAllOnFocusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e){
			TextBox textBox = (TextBox)sender;
			
			bool newValue = (bool)e.NewValue;
			bool oldValue = (bool)e.OldValue;
			if(oldValue){
				textBox.GotFocus -= TextBox_GotFocus;
			}
			if(newValue){
				textBox.GotFocus += TextBox_GotFocus;
			}
		}
		
		private static void TextBox_GotFocus(object sender, RoutedEventArgs e){
			TextBox textBox = (TextBox)sender;
			textBox.Dispatcher.BeginInvoke(new Action(textBox.SelectAll));
		}
	}
}