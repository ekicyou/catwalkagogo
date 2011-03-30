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
			var vwm = new ViewerWindowViewModel(this._Gfl); this._ViewerWindowViewModels.Add(vwm);
			//vwm.BitmapLoadFailed += delegate(object sender, BitmapLoadFailedEventArgs e2){
				//e2.Exception.Handle((ex) => true);
				//MessageBox.Show("Load failed.\n" + e2.Exception.InnerException.Message, "Loading failed", MessageBoxButton.OK, MessageBoxImage.Error);
			//};

			var vw = new ViewerWindow();
			vwm.OpenFileDialog = new OpenFileDialog(vw);
			vwm.RequestClose += delegate{
				vw.Close();
			};
			vw.DataContext = vwm;
			vw.Closed += delegate{
				this._ViewerWindowViewModels.Remove(vwm);
			};
			//vwm.OpenFileCommand.Execute(null);
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

			Settings.Default.UpgradeOnce();
			this.Exit += this.SaveSettingsOnExit;
			
			this.InitGfl();

			if(option.Files.Length > 0){
				foreach(var file in option.Files){
					try{
						var path = IO.Path.GetFullPath(file);
						var vwvwvm = this.CreateViewerWindowInternal();
						vwvwvm.Item1.Show();
						vwvwvm.Item2.CurrentFilePath = path;
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
			Settings.Default.ViewerWindowInputBindingInfos = new InputBindingInfo[]{
				new InputBindingInfo("OpenFileCommand", new KeyGestureInfo(Key.O, ModifierKeys.Control)),
				new InputBindingInfo("CloseCommand", new KeyGestureInfo(Key.Escape)),
				new InputBindingInfo("CloseCommand", new KeyGestureInfo(Key.W, ModifierKeys.Control)),
				new InputBindingInfo("OpenFileInNewWindowCommand", new KeyGestureInfo(Key.O, ModifierKeys.Control | ModifierKeys.Shift)),
				new InputBindingInfo("NextFileCommand", new KeyGestureInfo(Key.PageDown)),
				new InputBindingInfo("PreviousFileCommand", new KeyGestureInfo(Key.PageUp)),
				new InputBindingInfo("GFV.Program::CurrentProgram.AboutCommand", new KeyGestureInfo(Key.F1)),
				new InputBindingInfo("GFV.Program::CurrentProgram.ExitCommand", new KeyGestureInfo(Key.Q, ModifierKeys.Control)),
			};
			Settings.Default.Save();
		}

		#endregion

		#region Command

		private ICommand _AboutCommand;
		public ICommand AboutCommand{
			get{
				return (this._AboutCommand == null) ? (this._AboutCommand = new DelegateUICommand<Window>(this.About)) : this._AboutCommand;
			}
		}

		public void About(Window owner){
			var dialog = new AboutBox();
			var addInfo = new ObservableCollection<KeyValuePair<string, string>>();
			addInfo.Add(new KeyValuePair<string,string>("", ""));
			addInfo.Add(new KeyValuePair<string,string>("Graphic File Library", Program.CurrentProgram.Gfl.DllName));
			addInfo.Add(new KeyValuePair<string,string>("Copyright", "Copyright © 1991-2009 Pierre-e Gougelet"));
			addInfo.Add(new KeyValuePair<string,string>("Version", Program.CurrentProgram.Gfl.VersionString));
			addInfo.Add(new KeyValuePair<string,string>("", ""));
			addInfo.Add(new KeyValuePair<string,string>("Supported Formats:", ""));
			foreach(var fmt in Program.CurrentProgram.Gfl.Formats.OrderBy(fmt => fmt.Description)){
				var key = fmt.Description + " (" + fmt.DefaultSuffix + ")";
				var list = new List<string>();
				if(fmt.Readable){
					list.Add("Read");
				}
				if(fmt.Writable){
					list.Add("Write");
				}
				addInfo.Add(new KeyValuePair<string,string>(key, String.Join(" / ", list)));
			}
			dialog.AdditionalInformations = addInfo;
			//dialog.AppIcon = new BitmapImage(new Uri());

			dialog.Owner = owner;
			dialog.ShowDialog();
		}

		private DelegateCommand _ExitCommand;
		public ICommand ExitCommand{
			get{
				if(this._ExitCommand == null){
					this._ExitCommand = new DelegateUICommand(this.ExitProgram);
				}
				return this._ExitCommand;
			}
		}

		public void ExitProgram(){
			Application.Current.Shutdown();
		}

		#endregion

		private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e){
		}
	}
}
