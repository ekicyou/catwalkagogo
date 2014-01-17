using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.Mvvm;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Codeplex.Reactive;
using Codeplex.Reactive.Extensions;

namespace CatWalk.Heron.ViewModel.Windows {
	public class ViewViewModel : ControlViewModel {
		private DisposableLazy<ReactiveProperty<Messenger>> _Messenger;
		private DisposableLazy<ReactiveProperty<MainWindowViewModel>> _MainWindow;
		private DisposableLazy<ReactiveProperty<Application>> _Application;
		private CompositeDisposable _Disposables = new CompositeDisposable();

		public ViewViewModel() {
			this._MainWindow = new DisposableLazy<ReactiveProperty<MainWindowViewModel>>(() => {
				var prop =
					this.ObserveProperty(_ => _.Ancestors)
						.OfType<ViewViewModel>()
						.Select(vvm => vvm.MainWindow)
						.Concat(
							this.ObserveProperty(_ => _.Ancestors)
								.OfType<MainWindowViewModel>()
						)
						.ToReactiveProperty();
				this._Disposables.Add(prop.Subscribe(_ => this.OnPropertyChanged("MainWindow")));
				this._Disposables.Add(prop);
				return prop;
			});
			this._Application = new DisposableLazy<ReactiveProperty<Application>>(() => {
				var prop =
					this.ObserveProperty(_ => _.Ancestors)
						.OfType<ViewViewModel>()
						.Select(vvm => vvm.Application)
						.Concat(
							this.ObserveProperty(_ => _.Ancestors)
								.OfType<Application>()
						)
						.ToReactiveProperty();
				this._Disposables.Add(prop.Subscribe(_ => this.OnPropertyChanged("Application")));
				this._Disposables.Add(prop);
				return prop;
			});
			this._Messenger = new DisposableLazy<ReactiveProperty<Messenger>>(() => {
				var prop =
					this._Application.Value
						.Select(app => app.Messenger)
						.ToReactiveProperty();
				this._Disposables.Add(prop.Subscribe(_ => this.OnPropertyChanged("Messenger")));
				this._Disposables.Add(prop);
				return prop;
			});
		}

		public MainWindowViewModel MainWindow {
			get {
				return this._MainWindow.Value.Value;
			}
		}

		public Application Application {
			get {
				return this._Application.Value.Value;
			}
		}

		public Messenger Messenger {
			get {
				return this._Messenger.Value.Value;
			}
		}

		protected CompositeDisposable Disposables {
			get {
				return this._Disposables;
			}
		}
	}
}
