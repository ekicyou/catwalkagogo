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
using CatWalk.Windows;
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
		public Gfl::Gfl Gfl{
			get{
				return this._Gfl;
			}
		}

		private Gfl::GflExtended _GflExtended;
		public Gfl::GflExtended GflExtended{
			get{
				return this._GflExtended;
			}
		}

		public static Program CurrentProgram{
			get{
				return Application.Current as Program;
			}
		}

		#endregion

		#region ViewerWindow

		private readonly ObservableList<ViewViewModelPair<ViewerWindow, ViewerWindowViewModel>> _ViewerWindows = new ObservableList<ViewViewModelPair<ViewerWindow, ViewerWindowViewModel>>(new SkipList<ViewViewModelPair<ViewerWindow, ViewerWindowViewModel>>());
		private ReadOnlyObservableList<ViewViewModelPair<ViewerWindow, ViewerWindowViewModel>> _ViewerWindowsReadOnly;
		public ReadOnlyObservableList<ViewViewModelPair<ViewerWindow, ViewerWindowViewModel>> ViewerWindows{
			get{
				if(this._ViewerWindowsReadOnly == null){
					this._ViewerWindowsReadOnly = new ReadOnlyObservableList<ViewViewModelPair<ViewerWindow, ViewerWindowViewModel>>(this._ViewerWindows);
				}
				return this._ViewerWindowsReadOnly;
			}
		}

		public ViewViewModelPair<ViewerWindow, ViewerWindowViewModel> CreateViewerWindow(){
			var pair = new ViewViewModelPair<ViewerWindow, ViewerWindowViewModel>(new ViewerWindow(), new ViewerWindowViewModel(this._Gfl));

			// ViewModel
			pair.ViewModel.OpenFileDialog = new OpenFileDialog(pair.View);
			
			// View
			pair.View.DataContext = pair.ViewModel;
			pair.View.Closed += delegate{
				this._ViewerWindows.Remove(pair);
				pair.ViewModel.Dispose();
			};
			return pair;
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

			Settings.Default.UpgradeOnce();
			this.Exit += this.SaveSettingsOnExit;
			
			this.InitGfl();

			if(option.Files.Length > 0){
				foreach(var file in option.Files){
					try{
						var path = IO.Path.GetFullPath(file);
						var vwvwvm = this.CreateViewerWindow();
						vwvwvm.View.Show();
						vwvwvm.ViewModel.CurrentFilePath = path;
					}catch(ArgumentException ex){
						this.ShowErrorDialog(ex.Message + "\n" + file);
					}catch(NotSupportedException ex){
						this.ShowErrorDialog(ex.Message + "\n" + file);
					}catch(IO.PathTooLongException ex){
						this.ShowErrorDialog(ex.Message + "\n" + file);
					}
				}
				if(this.MainWindow == null){
					this.CreateViewerWindow().View.Show();
				}
			}else{
				this.CreateViewerWindow().View.Show();
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
			Settings.Default.ViewerWindowInputBindingInfos = new InputBindingInfo[]{
				new InputBindingInfo("OpenFileCommand", new KeyGestureInfo(Key.O, ModifierKeys.Control)),
				new InputBindingInfo("CloseCommand", new KeyGestureInfo(Key.Escape)),
				new InputBindingInfo("CloseCommand", new KeyGestureInfo(Key.W, ModifierKeys.Control)),
				new InputBindingInfo("OpenFileInNewWindowCommand", new KeyGestureInfo(Key.O, ModifierKeys.Control | ModifierKeys.Shift)),
				new InputBindingInfo("NextFileCommand", new KeyGestureInfo(Key.PageDown)),
				new InputBindingInfo("PreviousFileCommand", new KeyGestureInfo(Key.PageUp)),
				new InputBindingInfo("AboutCommand", new KeyGestureInfo(Key.F1)),
				new InputBindingInfo("ExitCommand", new KeyGestureInfo(Key.Q, ModifierKeys.Control)),
			};
			Settings.Default.Save();
		}

		#endregion

		private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e){
		}
	}
}
