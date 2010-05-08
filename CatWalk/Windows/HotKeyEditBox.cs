/*
	$Id: HotKeyEditBox.cs 40 2010-01-24 10:10:19Z catwalk $
*/
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CatWalk.Windows{
	public partial class HotKeyEditBox : UserControl{
		public HotKeyEditBox(){
			this.InitializeComponent();
			
			this.RefreshText(this.Modifiers, this.Key);
		}
		
		#region イベント処理
		
		protected override void OnGotFocus(RoutedEventArgs e){
			this.textBox.Focus();
			base.OnGotFocus(e);
		}
		
		private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e){
			if(IsValidKey(e.Key)){
				this.Key = e.Key;
				this.Modifiers = Keyboard.Modifiers |
				                (((Keyboard.GetKeyStates(Key.LWin) & KeyStates.Down) > 0) ||
				                 ((Keyboard.GetKeyStates(Key.RWin) & KeyStates.Down) > 0) ?
				                 ModifierKeys.Windows : ModifierKeys.None);
			}else{
				this.Key = Key.None;
				this.Modifiers = ModifierKeys.None;
			}
			e.Handled = true;
		}
		
		private void TextBox_GotFocus(object sender, RoutedEventArgs e){
			InputMethod.Current.ImeState = InputMethodState.Off;
		}
		
		#endregion
		
		#region 関数
		
		private static bool IsValidKey(Key key){
			return (key != Key.LeftCtrl) &&
			       (key != Key.RightCtrl) &&
			       (key != Key.LeftShift) &&
			       (key != Key.RightShift) &&
			       (key != Key.LeftAlt) &&
			       (key != Key.RightAlt) &&
			       (key != Key.LWin) &&
			       (key != Key.RWin);
		}
		
		private void RefreshText(ModifierKeys mods, Key key){
			this.textBox.Text = GetModifiersString(mods) + GetKeyString(key);
		}
		
		private static string GetKeyString(Key key){
			if((Key.D0 <= key) && (key <= Key.D9)){
				return (key - Key.D0).ToString();
			}else{
				return Enum.GetName(typeof(Key), key);
			}
		}
		
		private static string GetModifiersString(ModifierKeys mods){
			var list = new List<string>(3);
			if((mods & (ModifierKeys.Shift)) > 0){
				list.Add("Shift");
			}
			if((mods & (ModifierKeys.Control)) > 0){
				list.Add("Ctrl");
			}
			if((mods & (ModifierKeys.Alt)) > 0){
				list.Add("Alt");
			}
			if((mods & (ModifierKeys.Windows)) > 0){
				list.Add("Win");
			}
			return String.Join(" + ", list.ToArray()) + ((list.Count > 0) ? " + " : "");
		}
		
		#endregion
		
		#region プロパティ
		
		public static readonly DependencyProperty KeyProperty =
			DependencyProperty.Register("Key", typeof(Key), typeof(HotKeyEditBox),
				new FrameworkPropertyMetadata(Key.None, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, KeyPropertyChanged));
		public Key Key{
			get{
				return (Key)this.GetValue(KeyProperty);
			}
			set{
				this.SetValue(KeyProperty, value);
			}
		}
		
		private static void KeyPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e){
			HotKeyEditBox self = (HotKeyEditBox)sender;
			self.RefreshText(self.Modifiers, (Key)e.NewValue);
		}
		
		public static readonly DependencyProperty ModifiersProperty =
			DependencyProperty.Register("Modifiers", typeof(ModifierKeys), typeof(HotKeyEditBox),
			new FrameworkPropertyMetadata(ModifierKeys.None, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ModifiersPropertyChanged));
		public ModifierKeys Modifiers{
			get{
				return (ModifierKeys)this.GetValue(ModifiersProperty);
			}
			set{
				this.SetValue(ModifiersProperty, value);
			}
		}
		
		private static void ModifiersPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e){
			HotKeyEditBox self = (HotKeyEditBox)sender;
			self.RefreshText((ModifierKeys)e.NewValue, self.Key);
		}
		
		#endregion
	}
}