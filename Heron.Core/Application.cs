using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using CatWalk.Heron.Configuration;
using CatWalk.Heron.Scripting;
using CatWalk.Heron.ViewModel;
using CatWalk.Heron.ViewModel.Windows;
using CatWalk.Heron.IOSystem;
using CatWalk.IO;
using CatWalk.Utils;
using CatWalk.Mvvm;
using Community.CsharpSqlite.SQLiteClient;
using NLog;

namespace CatWalk.Heron {
	public partial class Application{
		private Lazy<CachedStorage> _Configuration;
		private FilePath _ConfigurationFilePath;
		private Lazy<Logger> _Logger = new Lazy<Logger>(() => LogManager.GetCurrentClassLogger());
		private ISynchronizeInvoke _SynchronizeInvoke;

		#region static

		private static Application _Current;

		public static Application Current {
			get {
				return _Current;
			}
		}

		#endregion

		#region Run

		public void Run() {
			this.Run(new string[0]);
		}

		public void Run(IReadOnlyList<string> args) {
			args.ThrowIfNull("args");
			if(_Current != null) {
				throw new InvalidOperationException("Application is already running.");
			}
			_Current = this;

			this.OnStartup(new ApplicationStartUpEventArgs(args));
		}

		#endregion

		#region Property

		public ISynchronizeInvoke SynchronizeInvoke {
			get {
				return this._SynchronizeInvoke;
			}
			set {
				this._SynchronizeInvoke = value;
				this.Messenger.SynchronizeInvoke = value;
			}
		}

		private Messenger _Messenger = null;
		public Messenger Messenger {
			get {
				return this._Messenger ?? (this._Messenger = new Messenger(this._SynchronizeInvoke));
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

		#endregion

		#region StartUp

		protected virtual void OnStartup(ApplicationStartUpEventArgs e) {
			if(ApplicationProcess.IsFirst) {
				this.RegisterRemoteCommands();
				this.OnFirstStartUp(e);
			} else {
				this.OnSecondStartUp(e);
			}

			var handler = this.StartUp;
			if(handler != null) {
				handler(this, e);
			}
		}

		protected virtual void OnFirstStartUp(ApplicationStartUpEventArgs e) {
			this._ConfigurationFilePath = new FilePath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).Concat("Heron");
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

			this.ViewModel = new ApplicationViewModel(this.Messenger, this.ViewFactory);
			this.ViewModel.StartUp(option);

			this.RegisterReceivers();

			this.PluginManager = new PluginManager(this);
			this.PluginManager.Load();

			this.ExecuteScripts();
		}

		protected virtual void OnSecondStartUp(ApplicationStartUpEventArgs e) {
			ApplicationProcess.InvokeRemote(AP_CommandLine, e.Args);
			this.Shutdown();
		}

		public event EventHandler<ApplicationStartUpEventArgs> StartUp;

		#endregion

		#region Exit

		protected virtual void OnExit(ApplicationExitEventArgs e) {
			if(this._Configuration.IsValueCreated) {
				this._Configuration.Value.Save();
			}
			var handler = this.Exit;
			if(handler != null) {
				handler(this, e);
			}
		}

		public void Shutdown(int exitCode = 0) {

		}

		public event EventHandler<ApplicationExitEventArgs> Exit;

		#endregion

		#region Message

		private void RegisterReceivers() {
			this.ViewModel.ThrowIfNull("ViewModel");
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

	public class ApplicationStartUpEventArgs : EventArgs {
		public IReadOnlyList<string> Args { get; private set; }

		public ApplicationStartUpEventArgs(IReadOnlyList<string> args) {
			args.ThrowIfNull("args");
			this.Args = args;
		}
	}

	public class ApplicationExitEventArgs : EventArgs {
		public int ApplicationExitCode { get; set; }

		public ApplicationExitEventArgs(int exitCode = 0){
			this.ApplicationExitCode = exitCode;
		}
	}
}
