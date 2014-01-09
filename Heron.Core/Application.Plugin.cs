using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using CatWalk;
using CatWalk.Windows.Input;
using CatWalk.IOSystem;
using CatWalk.Heron.IOSystem;
using CatWalk.Heron.ViewModel.IOSystem;
using CatWalk.Heron.ViewModel.Windows;
using CatWalk.Mvvm;
using CatWalk.Heron.ViewModel;

namespace CatWalk.Heron {
	public abstract partial class Application : ControlViewModel, IJobManagerSite {
		private void InitializePlugin() {
			this.PluginManager = new PluginManager(this);
			this.PluginManager.Load();
		}

		#region Plugin

		private ViewFactory _ViewFactory = null;
		public ViewFactory ViewFactory {
			get {
				return this._ViewFactory ?? (this._ViewFactory = new ViewFactory());
			}
		}

		public PluginManager PluginManager { get; private set; }

		#endregion

		#region Plugin

		public void RegisterSystemProvider(Type provider) {
			provider.ThrowIfNull("provider");
			if(!provider.IsSubclassOf(typeof(ISystemProvider))) {
				throw new ArgumentException(provider.Name + " does not implement ISystemProvider interface.");
			}
			var prov = (ISystemProvider)Activator.CreateInstance(provider);

			this.Provider.Providers.Add(prov);
		}

		public void UnregisterSystemProvider(Type provider) {
			this.Provider.Providers.RemoveAll(p => provider == p.GetType());
		}

		public void RegisterEntryOperator(Type op) {
			op.ThrowIfNull("op");
			if(!op.IsSubclassOf(typeof(IEntryOperator))) {
				throw new ArgumentException(op.Name + " does not implement IEntryOperator interface.");
			}
			var obj = (IEntryOperator)Activator.CreateInstance(op);

			this.EntryOperators.Add(obj);
		}

		public void UnregisterEntryOperator(Type op) {
			var c = this.EntryOperators;
			for(var i = c.Count - 1; i > 0; i--) {
				var o = c[i];
				if(o.GetType() == op) {
					c.RemoveAt(i);
				}
			}
		}

		#endregion
	}
}
