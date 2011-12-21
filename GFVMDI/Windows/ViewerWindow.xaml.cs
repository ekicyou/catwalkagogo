using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using CatWalk.Mvvm;
using GFV.Messaging;

namespace GFV.Windows {
	public partial class ViewerWindow : ResourceDictionary{
		public ViewerWindow(){
			this.InitializeComponent();
		}

		private void MdiChild_Closed(object sender, EventArgs e){
			Messenger.Default.Send(new CloseMessage(this));
		}
	}
}
