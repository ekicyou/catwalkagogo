/*
	$Id$
*/
using System;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Shell;
using System.Net;
using System.Threading;
using GFV.Properties;
using GFV.ViewModel;
using GFV.Windows;
using CatWalk;
using CatWalk.Collections;
using CatWalk.Net;
using CatWalk.Windows;
using CatWalk.Mvvm;

namespace GFV{
	using Gfl = GflNet;
	using IO = System.IO;
	using ViewerViewViewModelPair = ViewViewModelPair<ViewerWindow, ViewerWindowViewModel>;
	using Prop = GFV.Properties;

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

		private readonly ObservableList<ViewerViewViewModelPair> _ViewerWindows = new ObservableList<ViewerViewViewModelPair>(new SkipList<ViewerViewViewModelPair>());
		private ReadOnlyObservableList<ViewerViewViewModelPair> _ViewerWindowsReadOnly;
		public ReadOnlyObservableList<ViewerViewViewModelPair> ViewerWindows{
			get{
				if(this._ViewerWindowsReadOnly == null){
					this._ViewerWindowsReadOnly = new ReadOnlyObservableList<ViewerViewViewModelPair>(this._ViewerWindows);
				}
				return this._ViewerWindowsReadOnly;
			}
		}

		public ViewerViewViewModelPair CreateViewerWindow(){
			var pair = new ViewerViewViewModelPair(new ViewerWindow(), new ViewerWindowViewModel(this._Gfl));

			// ViewModel
			pair.ViewModel.OpenFileDialog = new OpenFileDialog(pair.View);
			pair.ViewModel.BitmapLoadFailed += this.ViewerWindow_BitmapLoadFailed;
			
			// View
			pair.View.DataContext = pair.ViewModel;
			pair.View.Closed += delegate{
				this._ViewerWindows.Remove(pair);
				pair.ViewModel.Dispose();
			};

			this._ViewerWindows.Add(pair);
			return pair;
		}

		private void ViewerWindow_BitmapLoadFailed(object sender, BitmapLoadFailedEventArgs e){
			MessageBox.Show(String.Join("\n\n", e.Exception.InnerExceptions.Select(ex => ex.Message)), "Loading Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		#endregion

		#region OnStartup

		private class CommandLineOption{
			public string[] Files{get; set;}
		}

		[STAThread]
		protected override void OnStartup(StartupEventArgs e) {
			base.OnStartup(e);

			var parser = new CommandLineParser();
			var option = parser.Parse<CommandLineOption>(e.Args);
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
			//MessageBox.Show(Settings.Default.ViewerInputBindingInfos.Where(info => info.CommandParameter != null).First().ToString());

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

			// アップデートチェック
			if(Settings.Default.IsCheckUpdatesOnStartup &&
				((DateTime.Now - Settings.Default.LastCheckUpdatesDateTime).Days > 0)){
				ThreadPool.QueueUserWorkItem(new WaitCallback(delegate{
					UpdatePackage[] packages = null;
					try{
						packages = Program.GetUpdates(false);
					}catch(WebException){
					}
					if(packages != null && packages.Length > 0){
						var package = packages[0];
						if(MessageBox.Show(
							String.Format(Prop::Resources.AutoUpdate_FoundMessage, package.Version), 
							Prop::Resources.AutoUpdate_Title,
							MessageBoxButton.YesNo) == MessageBoxResult.Yes){
							try{
								Program.Update(package);
							}catch(WebException ex){
								MessageBox.Show(String.Format(Prop::Resources.AutoUpdate_FailedToDownloadInstaller ,ex.Message),
									Prop::Resources.AutoUpdate_Title,
									MessageBoxButton.OK,
									MessageBoxImage.Error);
							}
						}
					}
				}));
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
			/*
			Settings.Default.ViewerWindowInputBindingInfos = new InputBindingInfo[]{
				new InputBindingInfo("OpenNewWindowCommand", new KeyGestureInfo(Key.N, ModifierKeys.Control)),
				new InputBindingInfo("OpenFileCommand", new KeyGestureInfo(Key.O, ModifierKeys.Control)),
				new InputBindingInfo("CloseCommand", new KeyGestureInfo(Key.Escape)),
				new InputBindingInfo("CloseCommand", new KeyGestureInfo(Key.W, ModifierKeys.Control)),
				new InputBindingInfo("OpenFileInNewWindowCommand", new KeyGestureInfo(Key.O, ModifierKeys.Control | ModifierKeys.Shift)),
				new InputBindingInfo("NextFileCommand", new KeyGestureInfo(Key.PageDown)),
				new InputBindingInfo("PreviousFileCommand", new KeyGestureInfo(Key.PageUp)),
				new InputBindingInfo("AboutCommand", new KeyGestureInfo(Key.F1)),
				new InputBindingInfo("ExitCommand", new KeyGestureInfo(Key.Q, ModifierKeys.Control)),
			};
			Settings.Default.ViewerInputBindingInfos = new InputBindingInfo[]{
				new InputBindingInfo("System.Windows.Controls.Primitives.ScrollBar::LineUpCommand", new KeyGestureInfo(Key.Up)),
				new InputBindingInfo("System.Windows.Controls.Primitives.ScrollBar::LineDownCommand", new KeyGestureInfo(Key.Down)),
				new InputBindingInfo("System.Windows.Controls.Primitives.ScrollBar::LineLeftCommand", new KeyGestureInfo(Key.Left)),
				new InputBindingInfo("System.Windows.Controls.Primitives.ScrollBar::LineRightCommand", new KeyGestureInfo(Key.Right)),
				new InputBindingInfo("System.Windows.Controls.Primitives.ScrollBar::ScrollToTopCommand", new KeyGestureInfo(Key.Home)),
				new InputBindingInfo("System.Windows.Controls.Primitives.ScrollBar::ScrollToBottomCommand", new KeyGestureInfo(Key.End)),
			};
			*/
			Settings.Default.Save();
		}

		#endregion

		#region Update

		private ICommand _CheckUpdatesCommand;
		public ICommand CheckUpdatesCommand{
			get{
				return this._CheckUpdatesCommand ?? (this._CheckUpdatesCommand = new DelegateUICommand(CheckUpdate));
			}
		}

		public static void CheckUpdate(){
			UpdatePackage[] packages = null;
			try{
				packages = Program.GetUpdates();
			}catch(WebException ex){
				MessageBox.Show(String.Format(Prop::Resources.AutoUpdate_FailedToCheckUpdates, ex.Message), Prop::Resources.AutoUpdate_Title, MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			if(packages != null && packages.Length > 0){
				var package = packages[0];
				if(MessageBox.Show(
					String.Format(Prop::Resources.AutoUpdate_FoundMessage ,package.Version), 
					Prop::Resources.AutoUpdate_Title,
					MessageBoxButton.YesNo) == MessageBoxResult.Yes){
					try{
						Program.Update(package);
						}catch(WebException ex){
							MessageBox.Show(String.Format(Prop::Resources.AutoUpdate_FailedToDownloadInstaller ,ex.Message),
								Prop::Resources.AutoUpdate_Title,
								MessageBoxButton.OK,
								MessageBoxImage.Error);
						}
				}
			}else{
				MessageBox.Show(Prop::Resources.AutoUpdate_NoUpdatesAvailable);
			}
		}

		public static UpdatePackage[] GetUpdates(){
			return GetUpdates(true);
		}

		public static UpdatePackage[] GetUpdates(bool isShowProgress){
			var progWin = (isShowProgress) ? new ProgressWindow() : null;
			try{
				if(isShowProgress){
					progWin.Message = "Checking Updates";
					progWin.Owner = Current.MainWindow;
					progWin.IsIndeterminate = true;
					progWin.Show();
				}
				var currVer = new Version(Assembly.GetEntryAssembly().GetInformationalVersion());
				var updater = new AutoUpdater(new Uri("http://nekoaruki.com/updator/gfv/packages.xml"));
				Settings.Default.LastCheckUpdatesDateTime = DateTime.Now;
				return updater.CheckUpdates().Where(p => p.InformationalVersion > currVer).OrderByDescending(p => p.Version).ToArray();
			}finally{
				if(isShowProgress){
					progWin.Close();
				}
			}
		}
		
		public static void Update(UpdatePackage package){
			Application.Current.Dispatcher.BeginInvoke(new Action(delegate{
				var progress = new ProgressWindow();
				progress.Message = "Downloading Update Files.";
				progress.IsIndeterminate = false;
				progress.Owner = Current.MainWindow;
				progress.Show();
			
				package.DownloadInstallerAsync(delegate(object sender, DownloadProgressChangedEventArgs e2){
					Application.Current.Dispatcher.Invoke(new Action(delegate{
						progress.Value = (double)e2.ProgressPercentage;
					}));
				}, delegate(object sender, AsyncCompletedEventArgs e2){
					Application.Current.Dispatcher.Invoke(new Action(delegate{
						var file = (string)e2.UserState;
						progress.Close();
						MessageBox.Show("Starting Installer.");
						Process.Start(file);
						Application.Current.Shutdown();
					}));
				});
			}));
		}

		#endregion

		private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e){
			MessageBox.Show(e.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}
}
