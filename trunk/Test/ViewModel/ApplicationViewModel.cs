using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using CatWalk;
using CatWalk.IOSystem;
using Test.IOSystem;
using Test.ViewModel.IOSystem;

namespace Test.ViewModel {
	public class ApplicationViewModel : ControlViewModel {
		private SystemEntryViewModel _RootEntry;
		private SystemProvider _RootProvider;

		public ApplicationViewModel()
			: base(null) {

		}

		public void Initialize() {
			this._RootProvider = new RootProvider(this);
			this._RootEntry = new SystemEntryViewModel(null, this._RootProvider, new RootEntry(this));
		}

		private class RootProvider : SystemProvider {
			public ApplicationViewModel Application { get; private set; }
			public IEnumerable<SystemProvider> Providers { get; private set; }
			public RootProvider(ApplicationViewModel app) {
				app.ThrowIfNull("app");
				this.Application = app;
				this.Providers = CollectProviders().ToArray();
			}

			private static IEnumerable<SystemProvider> CollectProviders() {
				return AppDomain.CurrentDomain.GetAssemblies()
					.SelectMany(s => s.GetTypes())
					.Where(t => t.IsAssignableFrom(typeof(SystemProvider)))
					.Select(t => (SystemProvider)Activator.CreateInstance(t));
			}

			public override bool TryParsePath(ISystemEntry root, string path, out ISystemEntry entry) {
				entry = null;
				foreach(var provider in this.Providers) {
					if(provider.TryParsePath(root, path, out entry)) {
						return true;
					}
				}
				return false;
			}

			public override IEnumerable<ISystemEntry> GetRootEntries(ISystemEntry parent) {
				return this.Providers.SelectMany(p => p.GetRootEntries(parent));
			}
		}

		private class RootEntry : SystemEntry {
			public ApplicationViewModel Application { get; private set; }
			public RootEntry(ApplicationViewModel app) : base(null, "") {
				app.ThrowIfNull("app");
				this.Application = app;
			}

			public override bool IsDirectory {
				get {
					return true;
				}
			}

			public override IEnumerable<ISystemEntry> GetChildren(System.Threading.CancellationToken token, IProgress<double> progress) {
				return this.Application._RootProvider.GetRootEntries(this);
			}
		}
	}
}
