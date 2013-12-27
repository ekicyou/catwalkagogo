using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CatWalk.Heron.Configuration;
using CatWalk.Heron.Scripting;
using CatWalk.Heron.View;
using CatWalk.Heron.ViewModel;
using CatWalk.Heron.ViewModel.Windows;
using CatWalk.Heron.IOSystem;
using CatWalk.IO;
using CatWalk.Utils;
using CatWalk.Mvvm;
using CatWalk.Windows.Threading;
using Community.CsharpSqlite.SQLiteClient;
using NLog;

namespace CatWalk.Heron {
	public partial class App : Application {
		private Lazy<CachedStorage> _Configuration;
		private FilePath _ConfigurationFilePath;
		private Lazy<Logger> _Logger = new Lazy<Logger>(() => LogManager.GetCurrentClassLogger());

		#region Property

		private Messenger _Messenger = null;
		public Messenger Messenger {
			get {
				return this._Messenger ?? (this._Messenger = new Messenger(new DispatcherSynchronizeInvoke(this.Dispatcher)));
			}
		}

		private ViewFactory _ViewFactory = null;
		public ViewFactory ViewFactory {
			get {
				return this._ViewFactory ?? (this._ViewFactory = new ViewFactory());
			}
		}

		public IStorage Configuration {
			get {
				return this._Configuration.Value;
			}
		}
		public ApplicationViewModel ViewModel { get; private set; }
		public CommandLineOption StartUpOption { get; private set; }
		public PluginManager PluginManager { get; private set; }

		public static new App Current {
			get {
				return Application.Current as App;
			}
		}

		#endregion

		#region App Life Time Methods

		protected override void OnStartup(StartupEventArgs e) {
			base.OnStartup(e);
			if(ApplicationProcess.IsFirst) {
				this.RegisterRemoteCommands();
				this.OnFirstStartUp(e);
			} else {
				this.OnSecondStartUp(e);
			}
		}

		private void OnFirstStartUp(StartupEventArgs e) {
			AppViewModelBase.AppSynchronizeInvoke = new CatWalk.Windows.Threading.DispatcherSynchronizeInvoke(this.Dispatcher);

			this._Configuration = new Lazy<CachedStorage>(
				() => new CachedStorage(
					256,
					new DBStorage(
						SqliteClientFactory.Instance,
						"",
						"configuration")));

			this._ScriptingHosts = new Lazy<IList<IScriptingHost>>(() => {
				/*return AppDomain.CurrentDomain.GetAssemblies()
					.SelectMany(assm => assm.GetTypes()
						.Where(t => typeof(IScriptingHost).IsAssignableFrom(t)))
					.Select(t => Activator.CreateInstance(t) as IScriptingHost)
					.ToList();*/
				return Seq.Make<IScriptingHost>(new DlrHost()/*, new IronJSHost()*/).ToList();
			});

			var option = new CommandLineOption();
			var parser = new CommandLineParser();
			parser.Parse(option, e.Args);
			this.StartUpOption = option;

			this._ConfigurationFilePath = new FilePath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).Concat("Heron");

			this.ViewFactory.Register<MainWindowViewModel>(new Func<MainWindowViewModel, MainWindow>(vm => {
				var window = new MainWindow();
				window.DataContext = vm;
				return window;
			}));

			this.ViewModel = new ApplicationViewModel(this.Messenger, this.ViewFactory);
			this.ViewModel.StartUp(option);

			this.RegisterReceivers();

			this.PluginManager = new PluginManager(this);
			this.PluginManager.Load();

			this.ExecuteScripts();
		}

		private void OnSecondStartUp(StartupEventArgs e) {
			ApplicationProcess.InvokeRemote(AP_CommandLine, e.Args);
			this.Shutdown();
		}

		protected override void OnExit(ExitEventArgs e) {
			base.OnExit(e);

			if(this._Configuration.IsValueCreated) {
				this._Configuration.Value.Save();
			}
		}

		#endregion

		#region Message

		private void RegisterReceivers() {
			this.ViewModel.ThrowIfNull("ViewModel");
			this.Messenger.Register<ArrangeWindowsMessage>(this.OnArrangeWindowsMessage, this.ViewModel);
		}

		private void OnArrangeWindowsMessage(ArrangeWindowsMessage m) {
			this.ArrangeMainWindows(m.Mode);
		}
		
		public class ArrangeWindowsMessage : MessageBase {
			public ArrangeMode Mode { get; private set; }

			public ArrangeWindowsMessage(object sender, ArrangeMode mode)
				: base(sender) {
				this.Mode = mode;
			}
		}

		#endregion

		#region Plugin

		public void RegisterSystemProvider(Type provider) {
			provider.ThrowIfNull("provider");
			if(!provider.IsSubclassOf(typeof(ISystemProvider))) {
				throw new ArgumentException(provider.Name + " does not implement ISystemProvider interface.");
			}
			var prov = (ISystemProvider)Activator.CreateInstance(provider);

			this.ViewModel.Provider.Providers.Add(prov);
		}

		public void UnregisterSystemProvider(Type provider) {
			this.ViewModel.Provider.Providers.RemoveAll(p => provider == p.GetType());
		}

		public void RegisterEntryOperator(Type op) {
			op.ThrowIfNull("op");
			if(!op.IsSubclassOf(typeof(IEntryOperator))) {
				throw new ArgumentException(op.Name + " does not implement IEntryOperator interface.");
			}
			var obj = (IEntryOperator)Activator.CreateInstance(op);

			this.ViewModel.EntryOperators.Add(obj);
		}

		public void UnregisterEntryOperator(Type op) {
			var c = this.ViewModel.EntryOperators;
			for(var i = c.Count - 1; i > 0; i--) {
				var o = c[i];
				if(o.GetType() == op) {
					c.RemoveAt(i);
				}
			}
		}


		#endregion

		private const string AP_CommandLine = "_CommandLine";

		private void RegisterRemoteCommands() {
			ApplicationProcess.Actions.Add(AP_CommandLine, new Action<string[]>(prms => {
				var option = new CommandLineOption();
				var parser = new CommandLineParser();
				parser.Parse(option, prms);
			}));
		}

		public class CommandLineOption : Dictionary<string, string> {

		}
	}

	public class ViewFactory : Factory<Type, object> {
		public void Register<T>(Delegate d) {
			this.Register(typeof(T), d);
		}

		public object Create(object vm, params object[] args) {
			vm.ThrowIfNull("vm");
			return this.Create(vm.GetType(), args);
		}
	}
}
