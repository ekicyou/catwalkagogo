using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using CatWalk;
using CatWalk.Mvvm;
using CatWalk.Collections;
using GFV.Messaging;
using GFV.Properties;
using GFV.Windows;
using System.Windows.Media.Imaging;

namespace GFV.ViewModel {
	using IO = System.IO;

	[SendMessage(typeof(RequestActiveMdiChildMessage))]
	[SendMessage(typeof(ActivateMdiChildMessage))]
	[SendMessage(typeof(CloseMessage))]
	[ReceiveMessage(typeof(MdiChildClosedMessage))]
	[ReceiveMessage(typeof(ActiveMdiChildChangedMessage))]
	[ReceiveMessage(typeof(ClosingMessage))]
	[ReceiveMessage(typeof(LoadedMessage))]
	public class MainWindowViewModel : WindowViewModel{
		public ObservableCollection<object> ChildWindows{get; private set;}
		public ProgressManager ProgressManager{get; private set;}
		private int _Id;
		private static HashSet<int> _UsedIds = new HashSet<int>();
		private WindowSettings _Settings;

		private static BitmapSource _AppIcon;
		public static BitmapSource AppIcon{
			get{
				if(_AppIcon == null){
					_AppIcon = new BitmapImage(new Uri(@"pack://application:,,,/GFV;component/GFV.ico"));
				}
				return _AppIcon;
			}
		}

		public MainWindowViewModel(){
			this.ProgressManager = new ProgressManager();
			this.ChildWindows = new ObservableCollection<object>();
			this.ChildWindows.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(ChildWindows_CollectionChanged);

			this.Title = "GFV";

			this._Id = GetId();

			this._Settings = new WindowSettings(this.GetSettingsKey());
			this._Settings.UpgradeOnce();
			this.RestoreBounds = this._Settings.RestoreBounds;
			this.WindowState = this._Settings.WindowState;

			Settings.Default.PropertyChanged += this.Settings_PropertyChanged;

			Messenger.Default.Register<MdiChildClosedMessage>(this.ReceiveMdiChildClosedMessage, this);
			Messenger.Default.Register<ActiveMdiChildChangedMessage>(this.ReceiveActiveMdiChildChangedMessage, this);
			Messenger.Default.Register<ClosingMessage>(this.ReceiveClosingMessage, this);
			Messenger.Default.Register<LoadedMessage>(this.ReceiveLoadedMessage, this);
			Messenger.Default.Register<CloseMessage>(this.ReceiveCloseMessage, this);
		}

		private static int GetId(){
			var id = Enumerable.Range(0, Int32.MaxValue).Where(i => !_UsedIds.Contains(i)).FirstOrDefault();
			_UsedIds.Add(id);
			return id;
		}

		private string GetSettingsKey(){
			return "Window_" + this._Id;
		}

		private void ReceiveClosingMessage(ClosingMessage m){
			this._Settings.RestoreBounds = this.RestoreBounds;
			this._Settings.Save();
		}

		protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			if(e.PropertyName == "WindowState"){
				if(this.WindowState != System.Windows.WindowState.Minimized){
					this._Settings.WindowState = this.WindowState;
				}
			}
		}

		private void ChildWindows_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			if(e.OldItems != null){
				foreach(var item in e.OldItems.OfType<ViewModelBase>()){
					item.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(ChildWindow_PropertyChanged);
				}
			}
			if(e.NewItems != null){
				foreach(var item in e.NewItems.OfType<ViewModelBase>()){
					item.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ChildWindow_PropertyChanged);
				}
			}
		}

		private void ChildWindow_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
			if(e.PropertyName == "Title"){
				this.OnPropertyChanging("Title");
				this.OnPropertyChanged("Title");
			}
			if(e.PropertyName == "Icon"){
				this.OnPropertyChanging("Icon");
				this.OnPropertyChanged("Icon");
			}
		}

		#region InputBindings / Settings

		private void RefreshInputBindings(){
			Messenger.Default.Send(new ApplyInputBindingsMessage(this, Settings.Default.MainWindowInputBindingInfos), this);
		}

		private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e){
			switch(e.PropertyName){
				case "MainWindowInputBindingInfos": this.RefreshInputBindings(); break;
			}
		}

		#endregion


		#region Property

		public WindowViewModel ActiveMdiChild{
			get{
				var req = new RequestActiveMdiChildMessage(this);
				Messenger.Default.Send(req, this);
				return (WindowViewModel)req.ActiveMdiChild;
			}
			set{
				Messenger.Default.Send(new ActivateMdiChildMessage(this, value), this);
			}
		}

		public ViewerWindowViewModel ActiveViewerWindow{
			get{
				var req = new RequestActiveMdiChildMessage(this);
				Messenger.Default.Send(req, this);
				return req.ActiveMdiChild as ViewerWindowViewModel;
			}
			set{
				Messenger.Default.Send(new ActivateMdiChildMessage(this, value), this);
			}
		}

		public override string Title {
			get {
				var win = this.ActiveMdiChild;
				if(win != null){
					return win.Title;
				}else{
					return base.Title;
				}
			}
			set {
				base.Title = value;
			}
		}

		public override System.Windows.Media.Imaging.BitmapSource Icon {
			get {
				var win = this.ActiveMdiChild;
				if(win != null){
					return win.Icon;
				}else{
					return AppIcon;
				}
			}
			set {
				base.Icon = value;
			}
		}

		#endregion

		#region Messaging

		private void ReceiveMdiChildClosedMessage(MdiChildClosedMessage message){
			this.ChildWindows.Remove(message.Sender);
			this.OnPropertyChanging("ActiveMdiChild");
			this.OnPropertyChanging("ActiveViewerWindow");
			this.OnPropertyChanging("Title");
			this.OnPropertyChanging("Icon");
			this.OnPropertyChanged("ActiveMdiChild");
			this.OnPropertyChanged("ActiveViewerWindow");
			this.OnPropertyChanged("Title");
			this.OnPropertyChanged("Icon");
		}

		private void ReceiveActiveMdiChildChangedMessage(ActiveMdiChildChangedMessage message){
			this.OnPropertyChanging("ActiveMdiChild");
			this.OnPropertyChanging("ActiveViewerWindow");
			this.OnPropertyChanging("Title");
			this.OnPropertyChanging("Icon");
			this.OnPropertyChanged("ActiveMdiChild");
			this.OnPropertyChanged("ActiveViewerWindow");
			this.OnPropertyChanged("Title");
			this.OnPropertyChanged("Icon");
		}

		private void ReceiveLoadedMessage(LoadedMessage m){
			this.RestoreBounds = this._Settings.RestoreBounds;
			this.RefreshInputBindings();
		}

		private void ReceiveCloseMessage(CloseMessage m){
			_UsedIds.Remove(this._Id);
		}

		#endregion

		#region Create ViewerWindow

		public ViewerWindowViewModel CreateViewerWindow(){
			var vm = new ViewerWindowViewModel(this, Program.CurrentProgram.DefaultImageLoader);
			this.ChildWindows.Add(vm);

			return vm;
		}

		public ViewerWindowViewModel CreateViewerWindow(string path){
			var act = this.ActiveMdiChild;
			var vm = this.CreateViewerWindow();
			try{
				vm.CurrentFilePath = path;
				if(act != null){
					var rect = act.RestoreBounds;
					vm.RestoreBounds = rect;
					if(vm.WindowState == System.Windows.WindowState.Maximized){
						vm.WindowState = System.Windows.WindowState.Maximized;
					}
				}else{
					vm.WindowState = System.Windows.WindowState.Maximized;
				}
			}catch(ArgumentException){
			}catch(NotSupportedException){
			}catch(IO::PathTooLongException){
			}
			return vm;
		}

		#endregion

		#region OpenNewWindow Command

		private DelegateUICommand _OpenNewWindowCommand;
		public ICommand OpenNewWindowCommand{
			get{
				return this._OpenNewWindowCommand ?? (this._OpenNewWindowCommand = new DelegateUICommand(this.OpenNewWindow));
			}
		}

		public void OpenNewWindow(){
			var win = Program.CurrentProgram.CreateMainWindow();
			win.Show();
			win.Activate();
		}
		#endregion

		#region OpenFile

		private bool CanOpenFile(){
			return true;
		}

		private bool CanOpenFile(object dummy){
			return true;
		}

		private IOpenFileDialog _OpenFileDialog;
		public IOpenFileDialog OpenFileDialog{
			get{
				return this._OpenFileDialog;
			}
			set{
				this.OnPropertyChanging("OpenFileDialog");
				this._OpenFileDialog = value;
				this.OnPropertyChanged("OpenFileDialog");
			}
		}

		private DelegateUICommand<string> _OpenFileInNewWindowCommand;
		public ICommand OpenFileInNewWindowCommand{
			get{
				if(this._OpenFileInNewWindowCommand == null){
					this._OpenFileInNewWindowCommand = new DelegateUICommand<string>(this.OpenFileInNewWindow, this.CanOpenFile);
				}
				return this._OpenFileInNewWindowCommand;
			}
		}

		private DelegateUICommand<string> _OpenFileCommand;
		public DelegateUICommand<string> OpenFileCommand{
			get{
				if(this._OpenFileCommand == null){
					this._OpenFileCommand = new DelegateUICommand<string>(this.OpenFile, this.CanOpenFile);
				}
				return this._OpenFileCommand;
			}
		}

		public void OpenFile(string path){this.OpenFile(path, false);}
		public void OpenFileInNewWindow(string path){this.OpenFile(path, true);}
		public void OpenFile(string path, bool newWindow){
			if(String.IsNullOrEmpty(path)){
				if(this.OpenFileDialog != null){
					var dlg = this.OpenFileDialog;
					dlg.Reset();
					var formats = Program.CurrentProgram.Gfl.Formats.Where(fmt => fmt.Readable);
					
					var filters = new List<FileDialogFilter>();

					// Gfl
					foreach(var format in formats){
						var mask = String.Join(";", format.Extensions.Select(ext => "*." + ext));
						filters.Add(new FileDialogFilter(
							format.Description + " (" + mask + ")", mask));
					}
					// Wic
					foreach(var elms in Settings.Default.AdditionalFormatExtensions
						.EmptyIfNull()
						.Select(ext => ext.Split('|'))
						.Where(ext => ext.Length >= 2)){
						var name = elms[0];
						var ext = '.' + elms[1].TrimStart('.');
						var mask = '*' + ext;
						filters.Add(new FileDialogFilter(
							name + " (" + mask + ")", mask));
					}
					filters.Sort(new CatWalk.Collections.LambdaComparer<FileDialogFilter>(
						(a, b) => String.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase)));
					// All
					var allImg = new FileDialogFilter(
						"All Images",
						String.Join(";", Program.CurrentProgram.SupportedFormatExtensions.Select(ext => "*" + ext)));
					var allFile = new FileDialogFilter("All Files (*.*)", "*.*");
					new[]{allImg, allFile}.Concat(filters).ForEach(dlg.Filters.Add);

					dlg.IsCheckFileExists = dlg.IsCheckPathExists = dlg.IsMultiselect = dlg.IsValidNames = dlg.IsAddExtension = true;
					
					var act = this.ActiveViewerWindow;
					if(dlg.ShowDialog().Value){
						if(dlg.FileNames.Length > 0){
							var isFirst = true;
							foreach(var file in dlg.FileNames){
								if(act != null && act.CurrentFilePath == null){
									act.CurrentFilePath = file;
								}else{
									if(act == null || newWindow || !isFirst){
										var vm = this.CreateViewerWindow(file);
										this.ActiveMdiChild = vm;
									}else{
										act.CurrentFilePath = file;
									}
								}
								isFirst = false;
							}
						}
					}
				}
			}else{
				var act = this.ActiveViewerWindow;
				if(act != null){
					act.CurrentFilePath = path;
				}else{
					var vm = this.CreateViewerWindow(path);
					this.ActiveMdiChild = vm;
				}
			}
		}

		#endregion

		#region Exit Command

		private DelegateUICommand _ExitCommand;
		public ICommand ExitCommand{
			get{
				return this._ExitCommand ?? (this._ExitCommand = new DelegateUICommand(this.Exit));
			}
		}

		public void Exit(){
			Program.Current.Shutdown();
		}

		#endregion

		#region About Command

		private ICommand _AboutCommand;
		public ICommand AboutCommand{
			get{
				return this._AboutCommand ?? (this._AboutCommand = new DelegateUICommand(this.About));
			}
		}

		public void About(){
			Messenger.Default.Send<AboutMessage>(new AboutMessage(this), this);
		}

		#endregion

		#region Settings Command

		public DelegateUICommand _ShowSettingsCommand;
		public ICommand ShowSettingsCommand{
			get{
				return this._ShowSettingsCommand ?? (this._ShowSettingsCommand = new DelegateUICommand(this.ShowSettings));
			}
		}

		public void ShowSettings(){
			Messenger.Default.Send(new ShowSettingsMessage(this), this);
		}

		#endregion

		#region Arrange Window Command

		private DelegateUICommand<ArrangeMode> _ArrangeWindowsCommand;
		public ICommand ArrangeWindowsCommand{
			get{
				return this._ArrangeWindowsCommand ?? (this._ArrangeWindowsCommand = new DelegateUICommand<ArrangeMode>(this.ArrangeWindows));
			}
		}

		public void ArrangeWindows(ArrangeMode mode){
			Messenger.Default.Send<ArrangeWindowsMessage>(new ArrangeWindowsMessage(this, mode), this);
		}

		#endregion

		#region Show Menubar Command

		private ICommand _ShowMenubarCommand;
		public ICommand ShowMenubarCommand{
			get{
				return this._ShowMenubarCommand ?? (this._ShowMenubarCommand = new DelegateUICommand<bool?>(this.ShowMenubar));
			}
		}

		public void ShowMenubar(bool? visibility){
			if(Settings.Default.IsShowMenubar == null){
				Settings.Default.IsShowMenubar = false;
			}
			if(visibility == null){	// toggle
				Settings.Default.IsShowMenubar = !Settings.Default.IsShowMenubar;
			}else{
				Settings.Default.IsShowMenubar = visibility.Value;
			}
		}

		#endregion

		#region CloseMdiChild

		private ICommand _CloseMdiChildCommand;
		public ICommand CloseMdiChildCommand{
			get{
				return this._CloseMdiChildCommand ?? (this._CloseMdiChildCommand = new DelegateCommand(this.CloseMdiChild, this.CanCloseMdiChild));
			}
		}

		public void CloseMdiChild(){
			var act = this.ActiveMdiChild;
			if(act != null){
				this.ChildWindows.Remove(act);
			}
		}

		public bool CanCloseMdiChild(){
			var act = this.ActiveMdiChild;
			return act != null;
		}

		#endregion

		#region Close

		private ICommand _CloseCommand;
		public ICommand CloseCommand{
			get{
				return this._CloseCommand ?? (this._CloseCommand = new DelegateCommand(this.Close));
			}
		}

		public void Close(){
			Messenger.Default.Send(new CloseMessage(this));
		}

		#endregion

		#region IDisposable Members

		~MainWindowViewModel(){
			this.Dispose(true);
		}

		public void Dispose(){
			this.Dispose(false);
			GC.SuppressFinalize(this);
		}

		private bool _Dispose = false;
		protected virtual void Dispose(bool disposing){
			if(!this._Dispose){
				foreach(var res in this.ChildWindows.OfType<IDisposable>()){
					res.Dispose();
				}
				this._Dispose = true;
			}
		}

		#endregion
	}
}
