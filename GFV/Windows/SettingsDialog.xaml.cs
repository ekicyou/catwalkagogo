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
using CatWalk.Mvvm;
using GFV.Messaging;

namespace GFV.Windows {
	/// <summary>
	/// SettingsDialog.xaml の相互作用ロジック
	/// </summary>
	[ReceiveMessage(typeof(CloseMessage))]
	public partial class SettingsDialog : Window {
		public SettingsDialog() {
			InitializeComponent();
		}

		private void Window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if(e.OldValue != null){
				Messenger.Default.Unregister<CloseMessage>(this.ReceiveCloseMessage);
			}
			if(e.NewValue != null){
				Messenger.Default.Register<CloseMessage>(this.ReceiveCloseMessage);
			}
		}

		private void ReceiveCloseMessage(CloseMessage message){
			this.Close();
		}
	}
}
