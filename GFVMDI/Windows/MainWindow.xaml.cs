using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Reflection;
using System.ComponentModel;
using GFV.Messaging;
using GFV.Properties;
using GFV.ViewModel;
using CatWalk.Mvvm;

namespace GFV.Windows {
	using Win32 = CatWalk.Win32;

	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window {
		private int _Id;

		public MainWindow() {
			InitializeComponent();

			this._Id = GetId();
		}

		#region InputBindings / Settings

		private void RefreshInputBindings(){
			var infos = Settings.Default.ViewerWindowInputBindingInfos;
			if(infos != null){
				InputBindingInfo.ApplyInputBindings(this, infos);
			}
		}

		private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e){
			switch(e.PropertyName){
				case "ViewerWindowInputBindingInfos": this.RefreshInputBindings(); break;
			}
		}

		private static HashSet<int> _UsedIds = new HashSet<int>();

		private static int GetId(){
			var id = Enumerable.Range(0, Int32.MaxValue).Where(i => !_UsedIds.Contains(i)).FirstOrDefault();
			_UsedIds.Add(id);
			return id;
		}

		private string GetSettingsKey(){
			return "ViewerWindow_" + this._Id;
		}

		#endregion

		#region Event

		private void Window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if(e.OldValue != null){
				Messenger.Default.Unregister<CloseMessage>(this.ReceiveCloseMessage, e.OldValue);
				Messenger.Default.Unregister<AboutMessage>(this.ReceiveAboutMessage, e.OldValue);
				Messenger.Default.Unregister<ArrangeWindowsMessage>(this.ReceiveArrangeWindowsMessage, e.OldValue);
				Messenger.Default.Unregister<ErrorMessage>(this.ReceiveErrorMessage, e.OldValue);
				Messenger.Default.Unregister<ShowSettingsMessage>(this.ReceiveShowSettingsMessage, e.OldValue);
			}
			if(e.NewValue != null){
				Messenger.Default.Register<CloseMessage>(this.ReceiveCloseMessage, e.NewValue);
				Messenger.Default.Register<AboutMessage>(this.ReceiveAboutMessage, e.NewValue);
				Messenger.Default.Register<ArrangeWindowsMessage>(this.ReceiveArrangeWindowsMessage, e.NewValue);
				Messenger.Default.Register<ErrorMessage>(this.ReceiveErrorMessage, e.NewValue);
				Messenger.Default.Register<ShowSettingsMessage>(this.ReceiveShowSettingsMessage, e.NewValue);
			}
			//this._ContextMenu.DataContext = e.NewValue;
			this.RefreshInputBindings();
		}

		#endregion

				#region Receive Messages

		private void ReceiveCloseMessage(CloseMessage message){
			this.Close();
		}

		private void ReceiveAboutMessage(AboutMessage message){
			var dialog = new CatWalk.Windows.AboutBox();
			try{
				using(var il = new Win32::ImageList(Win32::ImageList.MaxSize)){
					var icon = il.GetIcon(Assembly.GetExecutingAssembly().Location, Win32::ImageListDrawOptions.Transparent);
					dialog.AppIcon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
						icon.Handle,
						Int32Rect.Empty,
						BitmapSizeOptions.FromEmptyOptions());
				}
				using(var il = new Win32::ImageList(Win32::ImageListSize.Small)){
					var icon = il.GetIcon(Assembly.GetExecutingAssembly().Location, Win32.ImageListDrawOptions.Transparent);
					dialog.Icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
						icon.Handle,
						Int32Rect.Empty,
						BitmapSizeOptions.FromEmptyOptions());
				}
			}catch{
			}
			dialog.Icon = this.Icon;
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
			dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			//dialog.AppIcon = new BitmapImage(new Uri());

			dialog.Owner = this;
			dialog.ShowDialog();
		}

		private void ReceiveArrangeWindowsMessage(ArrangeWindowsMessage message){
		}

		private void ReceiveErrorMessage(ErrorMessage message){
			this.Dispatcher.BeginInvoke(new Action<string>(delegate(string mes){
				MessageBox.Show(this, mes, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
			}), message.Messsage);
		}

		private void ReceiveShowSettingsMessage(ShowSettingsMessage message){
			var dialog = new SettingsDialog();
			var vm = new SettingsDialogViewModel(Settings.Default);
			dialog.DataContext = vm;
			dialog.Owner = this;
			dialog.ShowDialog();
		}

		#endregion
	}
}
