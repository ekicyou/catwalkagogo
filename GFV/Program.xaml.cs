/*
	$Id$
*/
using System;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using GFV.Properties;
using GFV.ViewModel;
using GFV.Windows;
using CatWalk;
using CatWalk.Collections;

namespace GFV{
	using Gfl = GflNet;
	using IO = System.IO;

	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class Program : Application{
		#region Properties

		private Gfl::Gfl _Gfl;
		public static Gfl::Gfl Gfl{
			get{
				return ((Program)Application.Current)._Gfl;
			}
		}

		private Gfl::GflExtended _GflExtended;
		public static Gfl::GflExtended GflExtended{
			get{
				return ((Program)Application.Current)._GflExtended;
			}
		}


		#endregion

		#region ViewerWindow

		private readonly ObservableList<ViewerWindowViewModel> _ViewerWindowViewModels = new ObservableList<ViewerWindowViewModel>(new SkipList<ViewerWindowViewModel>());
		private ReadOnlyObservableList<ViewerWindowViewModel> _ViewerWindowsViewModelsReadOnly;
		public static ReadOnlyObservableList<ViewerWindowViewModel> ViewerWindowViewModels{
			get{
				var prog = (Program)Application.Current;
				if(prog != null){
					if(prog._ViewerWindowsViewModelsReadOnly == null){
						prog._ViewerWindowsViewModelsReadOnly = new ReadOnlyObservableList<ViewerWindowViewModel>(prog._ViewerWindowViewModels);
					}
					return prog._ViewerWindowsViewModelsReadOnly;
				}else{
					return null;
				}
			}
		}

		public static Tuple<ViewerWindow, ViewerWindowViewModel> CreateViewerWindow(){
			var prog = (Program)Application.Current;
			if(prog != null){
				return prog.CreateViewerWindowInternal();
			}else{
				throw new InvalidOperationException();
			}
		}

		private Tuple<ViewerWindow, ViewerWindowViewModel> CreateViewerWindowInternal(){
			var vw = new ViewerWindow();
			var vwm = new ViewerWindowViewModel(this._Gfl); this._ViewerWindowViewModels.Add(vwm);
			vwm.OpenFileDialog = new OpenFileDialog(vw);
			vwm.RequestClose += delegate{
				vw.Close();
				this._ViewerWindowViewModels.Remove(vwm);
			};
			//vwm.BitmapLoadFailed += delegate(object sender, BitmapLoadFailedEventArgs e2){
				//e2.Exception.Handle((ex) => true);
				//MessageBox.Show("Load failed.\n" + e2.Exception.InnerException.Message, "Loading failed", MessageBoxButton.OK, MessageBoxImage.Error);
			//};

			vw.DataContext = vwm;
			vw.Closed += delegate{
				this._ViewerWindowViewModels.Remove(vwm);
			};
			return new Tuple<ViewerWindow,ViewerWindowViewModel>(vw, vwm);
		}

		#endregion

		#region OnStartup

		private class CommandLineOption{
			public string[] Files{get; set;}
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

			this.InitGfl();

			if(option.Files.Length > 0){
				foreach(var file in option.Files){
					try{
						var path = IO.Path.GetFullPath(file);
						var vwvwvm = this.CreateViewerWindowInternal();
						vwvwvm.Item1.Show();
						vwvwvm.Item2.Path = path;
					}catch(ArgumentException ex){
						this.ShowErrorDialog(ex.Message + "\n" + file);
					}catch(NotSupportedException ex){
						this.ShowErrorDialog(ex.Message + "\n" + file);
					}catch(IO.PathTooLongException ex){
						this.ShowErrorDialog(ex.Message + "\n" + file);
					}
				}
				if(this.MainWindow == null){
					this.CreateViewerWindowInternal().Item1.Show();
				}
			}else{
				this.CreateViewerWindowInternal().Item1.Show();
			}
		}

		private void ShowErrorDialog(string message){
			MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		private void InitGfl(){
			if(Environment.Is64BitProcess){
				this._Gfl = new Gfl::Gfl("libgfl340_64.dll");
				this._GflExtended = new Gfl::GflExtended("libgfle340_64.dll");
			}else{
				this._Gfl = new Gfl::Gfl("libgfl340.dll");
				this._GflExtended = new Gfl::GflExtended("libgfle340.dll");
			}
			this._Gfl.PluginPath = IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().FullName) +
				IO.Path.DirectorySeparatorChar + "GFLPlugins";
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

		#endregion

		#region OnExit

		protected override void OnExit(ExitEventArgs e) {
			base.OnExit(e);

			if(this._Gfl != null){
				this._Gfl.Dispose();
			}
			if(this._GflExtended != null){
				this._GflExtended.Dispose();
			}
		}

		private void SaveSettingsOnExit(object sender, ExitEventArgs e){
			Settings.Default.Save();
		}

		#endregion

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

		private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e){
		}
	}
}
