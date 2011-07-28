/*
	$Id$
*/
using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using System.Windows.Shell;
using System.Windows.Interop;
using System.Threading;
using System.Threading.Tasks;
using GFV.Properties;
using GFV.ViewModel;
using System.Reflection;
using Microsoft.Windows.Shell;
using CatWalk.Mvvm;

namespace GFV.Windows{
	using Gfl = GflNet;
	using Win32 = CatWalk.Win32;

	/// <summary>
	/// Interaction logic for ViewerWindow.xaml
	/// </summary>
	[RecieveMessage(typeof(CloseMessage))]
	[RecieveMessage(typeof(AboutMessage))]
	[RecieveMessage(typeof(SetRectMessage))]
	public partial class ViewerWindow : Window{
		private ContextMenu _ContextMenu;
		private int _Id;
		private static List<int> _UsedIds = new List<int>();
		private WindowSettings _Settings;

		public ViewerWindow(){
			this.InitializeComponent();
			this.WindowStartupLocation = WindowStartupLocation.Manual;
			
			this._Id = GetId();
			this.Loaded += this.Window_Loaded;
			this._Settings = new WindowSettings(this.GetSettingsKey());
			this._Settings.UpgradeOnce();
			this._Settings.RestoreWindow(this);

			this._ContextMenu = new ContextMenu();
			this._ContextMenu.ItemsSource = (IEnumerable)this.Resources["MainMenu"];
			this._Viewer.ContextMenu = this._ContextMenu;

			Settings.Default.PropertyChanged += this.Settings_PropertyChanged;
			if(Settings.Default.IsShowMenubar == null){
				Settings.Default.IsShowMenubar = !SystemParameters2.Current.IsGlassEnabled;
			}
			this.SetStyle();
		}

		#region Style

		private const string PART_ScaleSliderName = "PART_ScaleSlider";
		private void SetStyle(){
			var style = (!Settings.Default.IsShowMenubar.Value && SystemParameters2.Current.IsGlassEnabled) ? (Style)this.Resources["Chrome"] : null;

			// Avoid changing the scale when the style is changed.
			var slider = (Slider)this.Template.FindName(PART_ScaleSliderName, this);
			if(slider != null){
				slider.ValueChanged -= this.ViewerScaleSlider_ValueChanged;
			}

			this.Style = style;
		}

		public override void OnApplyTemplate(){
			base.OnApplyTemplate();

			// Attach event handler of the scale slider.
			var slider = (Slider)this.Template.FindName(PART_ScaleSliderName, this);
			if(slider != null){
				//slider.ValueChanged += this.ViewerScaleSlider_ValueChanged;
			}
		}

		private void Menubar_VisibilityChanged(object sender, DependencyPropertyChangedEventArgs e) {
			this.SetStyle();
		}

		#endregion

		#region InputBindings / Settings

		private void RefreshInputBindings(){
			var infos = Settings.Default.ViewerWindowInputBindingInfos;
			if(infos != null){
				InputBindingInfo.ApplyInputBindings(this, infos);
			}
		}

		private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e){
			if(e.PropertyName == "ViewerWindowInputBindingInfos"){
				this.RefreshInputBindings();
			}
		}

		private static int GetId(){
			return Enumerable.Range(0, Int32.MaxValue).Where(i => !_UsedIds.Contains(i)).FirstOrDefault();
		}

		private string GetSettingsKey(){
			return "ViewerWindow_" + this._Id;
		}

		#endregion

		#region EventHandlers

		private void Window_Loaded(object sender, EventArgs e){
			_UsedIds.Add(this._Id);
		}

		protected override void OnStateChanged(EventArgs e){
			base.OnStateChanged(e);
			if(this.WindowState != WindowState.Minimized){
				this._Settings.WindowState = this.WindowState;
			}
		}

		protected override void OnLocationChanged(EventArgs e){
			base.OnLocationChanged(e);
			this._Settings.StoreWindow(this);
		}

		private void Window_SizeChanged(object sender, SizeChangedEventArgs e){
			this._Settings.StoreWindow(this);
			var viewerRect = VisualTreeHelper.GetContentBounds(this._Viewer);
			this.TaskbarItemInfo.ThumbnailClipMargin = new Thickness(
				viewerRect.Left,
				viewerRect.Top,
				viewerRect.Right,
				viewerRect.Bottom);
		}

		private void Window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if(e.OldValue != null){
				Messenger.Default.Unregister<CloseMessage>(this.RecieveCloseMessage, e.OldValue);
				Messenger.Default.Unregister<AboutMessage>(this.RecieveAboutMessage, e.OldValue);
				Messenger.Default.Unregister<SetRectMessage>(this.RecieveSetRectMessage, e.OldValue);
			}
			Messenger.Default.Register<CloseMessage>(this.RecieveCloseMessage, e.NewValue);
			Messenger.Default.Register<AboutMessage>(this.RecieveAboutMessage, e.NewValue);
			Messenger.Default.Register<SetRectMessage>(this.RecieveSetRectMessage, e.NewValue);
			this._ContextMenu.DataContext = e.NewValue;
			this.RefreshInputBindings();
		}

		protected override void OnClosed(EventArgs e){
			base.OnClosed(e);
			this._Settings.Save();
			Settings.Default.PropertyChanged -= this.Settings_PropertyChanged;
			if(this.DataContext != null){
				Messenger.Default.Unregister<CloseMessage>(this.RecieveCloseMessage, this.DataContext);
				Messenger.Default.Unregister<AboutMessage>(this.RecieveAboutMessage, this.DataContext);
			}
		}

		#endregion

		#region Revieve Messages

		private void RecieveCloseMessage(CloseMessage message){
			this.Close();
		}

		private void RecieveAboutMessage(AboutMessage message){
			var dialog = new CatWalk.Windows.AboutBox();
			//try{
				dialog.Icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
					Win32::ShellIcon.GetIcon(Assembly.GetExecutingAssembly().Location, Win32::ShellIconSize.Large).Handle,
					new Int32Rect(0, 0, 256, 256), BitmapSizeOptions.FromEmptyOptions());
			//}catch{
			//}
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

		private void RecieveSetRectMessage(SetRectMessage message){
			this.WindowState = WindowState.Normal;
			this.Top = message.Rect.Top;
			this.Left = message.Rect.Left;
			this.Width = message.Rect.Width;
			this.Height = message.Rect.Height;
		}

		#endregion

		#region Viewer Scale Slider

		private void SendViewerScale(double scale, bool isUpdate){
			if(this._Viewer.DataContext != null){
				Messenger.Default.Send<ScaleMessage>(new ScaleMessage(this._Viewer, scale), this._Viewer.DataContext);
			}
		}

		private CancellationTokenSource _ViewerScaleSlider_ValueChanged_CancellationTokenSource;
		private void ViewerScaleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e){
			var scale = e.NewValue;
			if(scale <= 0){
				return;
			}
			this.SendViewerScale(scale, false);

			if(this._ViewerScaleSlider_ValueChanged_CancellationTokenSource != null){
				this._ViewerScaleSlider_ValueChanged_CancellationTokenSource.Cancel();
			}

			var ui = TaskScheduler.FromCurrentSynchronizationContext();
			this._ViewerScaleSlider_ValueChanged_CancellationTokenSource = new CancellationTokenSource();
			var task = new Task(delegate{
				Thread.Sleep(640);
			}, this._ViewerScaleSlider_ValueChanged_CancellationTokenSource.Token);
			task.ContinueWith(delegate{
				this.SendViewerScale(scale, true);
			}, this._ViewerScaleSlider_ValueChanged_CancellationTokenSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, ui);
		}

		#endregion
	}
}
