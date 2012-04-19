/*
	$Id: Program.xaml.cs 297 2011-11-22 20:54:41Z catwalkagogo@gmail.com $
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
using System.Runtime.InteropServices;
using System.Windows.Interop;
using GFV.Properties;
using GFV.ViewModel;
using GFV.Windows;
using CatWalk;
using CatWalk.Collections;
using CatWalk.Net;
using CatWalk.Windows;
using CatWalk.Mvvm;
using GFV.Imaging;

namespace GFV{
	using Gfl = GflNet;
	using IO = System.IO;
	using Prop = GFV.Properties;
	using Win32 = CatWalk.Win32;

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

		public IImageLoader DefaultImageLoader{get; private set;}
		/*
		private Gfl::GflExtended _GflExtended;
		public Gfl::GflExtended GflExtended{
			get{
				return this._GflExtended;
			}
		}
		*/
		public static Program CurrentProgram{
			get{
				return Application.Current as Program;
			}
		}

		private Lazy<HashSet<string>> _SupportedFormatExtensions;

		#endregion

		#region MainWindow

		private ICollection<MainWindow> _MainWindows = new ObservableLinkedList<MainWindow>();

		public ICollection<MainWindow> MainWindows{
			get{
				return this._MainWindows;
			}
		}

		public MainWindow ActiveMainWindow{
			get{
				return this.MainWindows.OrderWindowByZOrder().FirstOrDefault();
			}
		}

		public MainWindow CreateMainWindow(){
			var win = new MainWindow();
			var vm = new MainWindowViewModel();
			win.DataContext = vm;
			// ViewModel
			vm.OpenFileDialog = new OpenFileDialog();

			this._MainWindows.Add(win);
			win.Closed += delegate{
				this._MainWindows.Remove(win);
			};

			return win;
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

			this.InitGfl();

			var mainWindow = this.CreateMainWindow();
			var vm = (MainWindowViewModel)mainWindow.DataContext;

			if(option.Files.Length > 0){
				foreach(var file in option.Files){
					vm.CreateViewerWindow(file);
				}
			}

			mainWindow.Show();

			// アップデートチェック
			/*
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
								MessageBox.Show(
									String.Format(Prop::Resources.AutoUpdate_FailedToDownloadInstaller ,ex.Message),
									Prop::Resources.AutoUpdate_Title,
									MessageBoxButton.OK,
									MessageBoxImage.Error);
							}
						}
					}
				}));
			}
			 * */
		}

		private void OnSecondProsess(CommandLineOption option){
			if(option.Files.Length > 0){
				ApplicationProcess.InvokeRemote(RemoteKeys.Open, new object[]{option.Files});
			}else{
				ApplicationProcess.InvokeRemote(RemoteKeys.NewWindow);
			}
			this.Shutdown();
		}

		private void ShowErrorDialog(string message){
			MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		private void InitGfl(){
			if(Environment.Is64BitProcess){
				this._Gfl = new Gfl::Gfl("libgfl340_64.dll");
				//this._GflExtended = new Gfl::GflExtended("libgfle340_64.dll");
			}else{
				this._Gfl = new Gfl::Gfl("libgfl340.dll");
				//this._GflExtended = new Gfl::GflExtended("libgfle340.dll");
			}
			this.Gfl.PluginPath = IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().FullName) +
				IO.Path.DirectorySeparatorChar + "GFLPlugins";
			this.DefaultImageLoader = new CombinedImageLoader(new IImageLoader[]{
				new GifImageLoader(),
				new WicImageLoader(),
				new GflImageLoader(this._Gfl),
			});
			this._SupportedFormatExtensions = new Lazy<HashSet<string>>(this.GetSupportedFormatExtensions);
		}

		private void RegisterRemoteMethods(){
			ApplicationProcess.Actions.Add(RemoteKeys.Show, new Action(delegate{
				this.Dispatcher.Invoke(new Action(delegate{
					var win = this.ActiveMainWindow;
					if(win != null){
						win.Activate();
					}
				}));
			}));
			ApplicationProcess.Actions.Add(RemoteKeys.Open, new Action<string[]>(delegate(string[] files){
				this.Dispatcher.Invoke(new Action<string[]>(delegate(string[] files2){
					var act = this.ActiveMainWindow;
					if(act != null){
						var vm = act.DataContext as MainWindowViewModel;
						foreach(var file in files2){
							var win = vm.CreateViewerWindow(file);
						}
					}
				}), new object[]{files});
			}));
			ApplicationProcess.Actions.Add(RemoteKeys.Kill, new Action(delegate{
				this.Dispatcher.Invoke(new Action(delegate{
					this.Shutdown();
				}));
			}));
			ApplicationProcess.Actions.Add(RemoteKeys.NewWindow, new Action(delegate{
				this.Dispatcher.Invoke(new Action(delegate{
					var act = this.ActiveMainWindow;
					if(act != null){
						var vm = act.DataContext as MainWindowViewModel;
						vm.CreateViewerWindow();
					}
				}));
			}));
		}

		private static class RemoteKeys{
			public const string Show = "Show";
			public const string Open = "Open";
			public const string Kill = "Kill";
			public const string NewWindow = "NewWindow";
		}

		#endregion

		#region OnExit

		protected override void OnExit(ExitEventArgs e) {
			base.OnExit(e);

			if(this._Gfl != null){
				this._Gfl.Dispose();
			}
			/*
			if(this._GflExtended != null){
				this._GflExtended.Dispose();
			}*/
		}

		private void SaveSettingsOnExit(object sender, ExitEventArgs e){
			Settings.Default.MainWindowInputBindingInfos = new InputBindingInfo[]{
				new InputBindingInfo("OpenNewWindowCommand", new KeyGestureInfo(Key.N, ModifierKeys.Control)),
				new InputBindingInfo("OpenFileCommand", new KeyGestureInfo(Key.O, ModifierKeys.Control)),
				new InputBindingInfo("OpenFileInNewWindowCommand", new KeyGestureInfo(Key.O, ModifierKeys.Control | ModifierKeys.Shift)),
				new InputBindingInfo("AboutCommand", new KeyGestureInfo(Key.F1)),
				new InputBindingInfo("ExitCommand", new KeyGestureInfo(Key.Q, ModifierKeys.Control)),
			};
			Settings.Default.ViewerWindowInputBindingInfos = new InputBindingInfo[]{
				new InputBindingInfo("CloseMdiChildCommand", new KeyGestureInfo(Key.W, ModifierKeys.Control)),
				new InputBindingInfo("NextFileCommand", new KeyGestureInfo(Key.PageDown)),
				new InputBindingInfo("PreviousFileCommand", new KeyGestureInfo(Key.PageUp)),
			};
			Settings.Default.ViewerInputBindingInfos = new InputBindingInfo[]{
				new InputBindingInfo("System.Windows.Controls.Primitives.ScrollBar::LineUpCommand", new KeyGestureInfo(Key.Up)),
				new InputBindingInfo("System.Windows.Controls.Primitives.ScrollBar::LineDownCommand", new KeyGestureInfo(Key.Down)),
				new InputBindingInfo("System.Windows.Controls.Primitives.ScrollBar::LineLeftCommand", new KeyGestureInfo(Key.Left)),
				new InputBindingInfo("System.Windows.Controls.Primitives.ScrollBar::LineRightCommand", new KeyGestureInfo(Key.Right)),
				new InputBindingInfo("System.Windows.Controls.Primitives.ScrollBar::ScrollToTopCommand", new KeyGestureInfo(Key.Home)),
				new InputBindingInfo("System.Windows.Controls.Primitives.ScrollBar::ScrollToBottomCommand", new KeyGestureInfo(Key.End)),
			};
			Settings.Default.Save();
		}

		#endregion
		/*
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
					String.Format(Prop::Resources.AutoUpdate_FoundMessage ,package.Version) + "\n\n" + package.ChangeLog, 
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
					progWin.Owner = CurrentProgram.ActiveViewerWindow;
					progWin.IsIndeterminate = true;
					progWin.Show();
				}
				var currVer = new Version(Assembly.GetEntryAssembly().GetInformationalVersion());
				var updater = new AutoUpdater(new Uri("http://nekoaruki.com/updater/gfv/packages.xml"));
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
				progress.Owner = CurrentProgram.ActiveViewerWindow;
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
		*/
		#region Methods

		private HashSet<string> GetSupportedFormatExtensions(){
			return new HashSet<string>(
				this._Gfl.Formats.Select(fmt => fmt.Extensions)
					.Flatten()
					.Select(ext => '.' + ext)
					.Concat(
						Settings.Default.AdditionalFormatExtensions.EmptyIfNull()
						.Select(ext => ext.Split('|'))
						.Where(ext => ext.Length >= 2)
						.Select(ext => '.' + ext[1].TrimStart('.')))
					.Distinct(StringComparer.OrdinalIgnoreCase),
				StringComparer.OrdinalIgnoreCase);
		}

		public IEnumerable<string> SupportedFormatExtensions{
			get{
				return this._SupportedFormatExtensions.Value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ext">without dot</param>
		/// <returns></returns>
		public bool IsSupportedFormat(string ext){
			return this._SupportedFormatExtensions.Value.Contains(ext);
		}

		#endregion

		private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e){
			MessageBox.Show(e.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}

	public static class WindowUtility{
		public static IEnumerable<T> OrderWindowByZOrder<T>(this IEnumerable<T> windows) where T : Window{
			var byHandle = windows.ToDictionary(win => {
				var source = ((HwndSource)PresentationSource.FromVisual(win));
				if(source == null){
					return IntPtr.Zero;
				}else{
					return source.Handle;
				}
			});
			return Win32::WindowUtility.OrderByZOrder(byHandle.Select(pair => pair.Key)).Select(hwnd => byHandle[hwnd]);
		}

		public static void SetForeground(this Window window){
			Win32::Win32Api.SetForegroundWindow(((HwndSource)PresentationSource.FromVisual(window)).Handle);
		}

		public static void SetTopZOrder(this Window window){
			Win32::Win32Api.SetWindowPos(
				((HwndSource)PresentationSource.FromVisual(window)).Handle,
				Win32::Win32Api.HWND_TOP,
				0,
				0,
				0,
				0,
				Win32::SetWindowPosOptions.NoActivate |
				Win32::SetWindowPosOptions.NoMove |
				Win32::SetWindowPosOptions.NoSize);
		}

		public static Win32::ScreenInfo GetCurrentScreen(this Window win){
			return Win32::Screen.GetCurrentMonitor(
				new CatWalk.Int32Rect(
					(int)(Double.IsNaN(win.RestoreBounds.Left) ? 0 : win.RestoreBounds.Left),
					(int)(Double.IsNaN(win.RestoreBounds.Top) ? 0 : win.RestoreBounds.Top),
					(int)(Double.IsNaN(win.RestoreBounds.Width) || win.RestoreBounds.Width < 0 ? 640 : win.RestoreBounds.Width),
					(int)(Double.IsNaN(win.RestoreBounds.Height) || win.RestoreBounds.Height < 0 ? 480 : win.RestoreBounds.Height)));
		}
	}
}
