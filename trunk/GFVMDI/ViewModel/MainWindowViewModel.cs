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

namespace GFV.ViewModel {
	using IO = System.IO;

	[SendMessage(typeof(RequestActiveMdiChildMessage))]
	[ReceiveMessage(typeof(MdiChildClosedMessage))]
	[ReceiveMessage(typeof(ActiveMdiChildChangedMessage))]
	public class MainWindowViewModel : ViewModelBase{
		public ObservableCollection<object> ChildWindows{get; private set;}
		public ProgressManager ProgressManager{get; private set;}

		public MainWindowViewModel(){
			this.ProgressManager = new ProgressManager();
			this.ChildWindows = new ObservableCollection<object>();

			Messenger.Default.Register<MdiChildClosedMessage>(this.ReceiveMdiChildClosedMessage, this);
			Messenger.Default.Register<ActiveMdiChildChangedMessage>(this.ReceiveActiveMdiChildChangedMessage, this);
		}

		#region Property

		public object ActiveMdiChild{
			get{
				var req = new RequestActiveMdiChildMessage(this);
				Messenger.Default.Send(req, this);
				return req.ActiveMdiChild;
			}
		}

		public ViewerWindowViewModel ActiveViewerWindow{
			get{
				var req = new RequestActiveMdiChildMessage(this);
				Messenger.Default.Send(req, this);
				return req.ActiveMdiChild as ViewerWindowViewModel;
			}
		}

		#endregion

		#region Messaging

		private void ReceiveMdiChildClosedMessage(MdiChildClosedMessage message){
			this.ChildWindows.Remove(message.Sender);
		}

		private void ReceiveActiveMdiChildChangedMessage(ActiveMdiChildChangedMessage message){
			this.OnPropertyChanging("ActiveMdiChild");
			this.OnPropertyChanging("ActiveViewerWindow");
			this.OnPropertyChanged("ActiveMdiChild");
			this.OnPropertyChanged("ActiveViewerWindow");
		}

		#endregion

		#region Create ViewerWindow

		public ViewerWindowViewModel CreateViewerWindow(){
			var vm = new ViewerWindowViewModel(this, Program.CurrentProgram.DefaultImageLoader);
			this.ChildWindows.Add(vm);

			return vm;
		}

		public ViewerWindowViewModel CreateViewerWindow(string path){
			var vm = this.CreateViewerWindow();
			try{
				vm.CurrentFilePath = path;
			}catch(ArgumentException){
			}catch(NotSupportedException){
			}catch(IO::PathTooLongException){
			}
			return vm;
		}

		#endregion

		#region OpenNewViewerWindow Command

		private DelegateUICommand _OpenViewerWindowCommand;
		public ICommand OpenViewerWindowCommand{
			get{
				return this._OpenViewerWindowCommand ?? (this._OpenViewerWindowCommand = new DelegateUICommand(this.OpenViewerWindow));
			}
		}

		public void OpenViewerWindow(){
			this.CreateViewerWindow();
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
										this.CreateViewerWindow(file);
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
