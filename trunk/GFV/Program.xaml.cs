/*
	$Id$
*/
using System;
using System.Windows;
using System.Windows.Input;
using GFV.Properties;
using GFV.ViewModel;
using GFV.Windows;
using CatWalk;

namespace GFV{
	using Gfl = GflNet;

	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class Program : Application{
		private class CommandLineOption{
			public string[] Files{get; set;}
		}

		private Gfl::Gfl gfl;
		public static Gfl::Gfl Gfl{
			get{
				return ((Program)Application.Current).gfl;
			}
		}

		[STAThread]
		protected override void OnStartup(StartupEventArgs e) {
			base.OnStartup(e);

			var option = new CommandLineOption();
			CommandLineParser.Parse(option, e.Args);
			if(ApplicationProcess.IsFirst){
				this.OnFirstProsess(option);
			}else{
				this.OnSecondProsess(option);
			}
		}

		private void OnFirstProsess(CommandLineOption option){
			this.RegisterRemoteMethods();

			if(!Settings.Default.IsUpgraded){
				Settings.Default.Upgrade();
				Settings.Default.IsUpgraded = true;
			}
			this.Exit += this.SaveSettingsOnExit;

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
			vwm.BitmapLoadFailed += delegate(object sender, BitmapLoadFailedEventArgs e2){
				e2.Exception.Handle((ex) => true);
				MessageBox.Show("Load failed.\n" + e2.Exception.InnerException.Message, "Loading failed", MessageBoxButton.OK, MessageBoxImage.Error);
			};
			this.MainWindow = vw;
			this.MainWindow.DataContext = vwm;
			this.MainWindow.Show();
		}

		private const string ShowRemoteKey = "Show";
		private const string KillRemoteKey = "Kill";
		private void RegisterRemoteMethods(){
			ApplicationProcess.Actions.Add(ShowRemoteKey, new Action(delegate{
				this.Dispatcher.Invoke(new Action(delegate{
					var win = this.MainWindow;
					if(win != null){
						win.Activate();
					}
				}));
			}));
			ApplicationProcess.Actions.Add(KillRemoteKey, new Action(delegate{
				this.Dispatcher.Invoke(new Action(delegate{
					this.Shutdown();
				}));
			}));
		}

		private void OnSecondProsess(CommandLineOption option){
			if(option.Files.Length == 0){
				ApplicationProcess.InvokeRemote(KillRemoteKey);
			}
			this.Shutdown();
		}

		protected override void OnExit(ExitEventArgs e) {
			base.OnExit(e);

			if(this.gfl != null){
				this.gfl.Dispose();
			}
		}

		private void SaveSettingsOnExit(object sender, ExitEventArgs e){
			Settings.Default.Save();
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
