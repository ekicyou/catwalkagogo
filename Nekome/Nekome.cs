/*
	$Id$
*/
using System;
using System.Windows;
using CatWalk;

namespace Nekome{
	public partial class Program : Application{
		private MainForm mainForm;
		
		protected override void OnStartup(StartupEventArgs e){
			var cmdline = new CommandLine();
			
			this.mainForm = new MainForm();
			this.mainForm.Show();
			
			
		}
	}
}