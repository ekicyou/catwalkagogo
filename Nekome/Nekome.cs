/*
	$Id$
*/
using System;
using System.Windows;

namespace Nekome{
	public partial class Program : Application{
		private MainForm mainForm;
		
		protected override void OnStartup(StartupEventArgs e){
			this.mainForm = new MainForm();
			this.mainForm.Show();
		}
	}
}