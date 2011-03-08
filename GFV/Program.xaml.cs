using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using GFV.Windows;
using GFV.ViewModel;

namespace GFV{
	using Gfl = GflNet;

	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class Program : Application{
		private Gfl::Gfl gfl;
		public static Gfl::Gfl Gfl{
			get{
				return ((Program)Application.Current).gfl;
			}
		}

		protected override void OnStartup(StartupEventArgs e) {
			base.OnStartup(e);

			if(Environment.Is64BitProcess){
				this.gfl = new Gfl::Gfl("libgfl340_64.dll");
			}else{
				this.gfl = new Gfl::Gfl("libgfl340.dll");
			}

			this.MainWindow = new ViewerWindow();
			this.MainWindow.DataContext = new ViewerWindowViewModel();
		}

		protected override void OnExit(ExitEventArgs e) {
			base.OnExit(e);

			this.gfl.Dispose();
		}
	}
}
