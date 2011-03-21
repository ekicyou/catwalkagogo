/*
	$Id$
*/
using System;
using System.Windows;
using System.Windows.Input;
using GFV.Properties;
using GFV.ViewModel;
using GFV.Windows;

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

		[STAThread]
		protected override void OnStartup(StartupEventArgs e) {
			base.OnStartup(e);

			if(!Settings.Default.IsUpgraded){
				Settings.Default.Upgrade();
				Settings.Default.IsUpgraded = true;
			}

			if(Environment.Is64BitProcess){
				this.gfl = new Gfl::Gfl("libgfl340_64.dll");
			}else{
				this.gfl = new Gfl::Gfl("libgfl340.dll");
			}

			var vw = new ViewerWindow();
			var vwm = new ViewerWindowViewModel(this.gfl);
			vwm.OpenFileDialog = new OpenFileDialog(vw);
			vwm.RequestClose += delegate{
				vw.Close();
			};
			this.MainWindow = vw;
			this.MainWindow.DataContext = vwm;
			this.MainWindow.Show();
		}

		protected override void OnExit(ExitEventArgs e) {
			base.OnExit(e);

			Settings.Default.Save();
			this.gfl.Dispose();
		}

		#region Command

		private ICommand _AboutCommand;
		public static ICommand AboutCommand{
			get{
				var prog = (Program)Application.Current;
				if(prog != null){
					return (prog._AboutCommand == null) ? (prog._AboutCommand = new RoutedCommand()) : prog._AboutCommand;
				}else{
					return null;
				}
			}
		}

		private DelegateCommand _ExitCommand;
		public static ICommand ExitCommand{
			get{
				var prog = (Program)Application.Current;
				if(prog != null){
					if(prog._ExitCommand == null){
						prog._ExitCommand = new DelegateCommand(delegate{
							Application.Current.Shutdown();
						});
					}
					return prog._ExitCommand;
				}else{
					return null;
				}
			}
		}

		#endregion
	}
}
