﻿/*
	$Id$
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Shell;
using System.Windows.Threading;
using CatWalk.Mvvm;
using CatWalk.Windows;
using GFV.Properties;
using GFV.ViewModel;
using GFV.Messaging;
using System.Runtime.InteropServices;

namespace GFV.Windows{
	using Gfl = GflNet;
	using Win32 = CatWalk.Win32;
	using Shell = CatWalk.Win32.Shell;

	/// <summary>
	/// Interaction logic for ViewerWindow.xaml
	/// </summary>
	[ReceiveMessage(typeof(CloseMessage))]
	[ReceiveMessage(typeof(AboutMessage))]
	[ReceiveMessage(typeof(ArrangeWindowsMessage))]
	[ReceiveMessage(typeof(ErrorMessage))]
	[ReceiveMessage(typeof(ShowSettingsMessage))]
	public partial class ViewerWindow : Window{
		private ContextMenu _ContextMenu;
		private int _Id;
		private static HashSet<int> _UsedIds = new HashSet<int>();
		private WindowSettings _Settings;

		public ViewerWindow(){
			this.InitializeComponent();
			this.WindowStartupLocation = WindowStartupLocation.Manual;
			this.Activated += this.OnActivated;
			this.Deactivated += this.OnDeactivated;
			this.StateChanged += this.OnWindowStateChanged;

			this._Id = GetId();
			this._Settings = new WindowSettings(this.GetSettingsKey());
			this._Settings.UpgradeOnce();
			this._Settings.RestoreWindow(this);

			this._ContextMenu = new ContextMenu();
			this._ContextMenu.ItemsSource = (IEnumerable)this.Resources["MainMenu"];
			this._Viewer.ContextMenu = this._ContextMenu;

			Settings.Default.PropertyChanged += this.Settings_PropertyChanged;
			if(Settings.Default.IsShowMenubar == null){
				Settings.Default.IsShowMenubar =  !SystemParameters.IsGlassEnabled;
			}
			this.SetStyle();

			this.Loaded += delegate{
				this.Dispatcher.BeginInvoke(new Action(delegate{
					if(this.WindowState == WindowState.Maximized){
						this.Activated -= this.OnActivated;
						this.Deactivated -= this.OnDeactivated;
						this.Hide();
						this.Show();
						this.Activated += this.OnActivated;
						this.Deactivated += this.OnDeactivated;
						this.SetForeground();
					}
				}));
			};
		}

		#region Style

		private const string PART_ScaleSliderName = "PART_ScaleSlider";
		private void SetStyle(){
			var style = (!Settings.Default.IsShowMenubar.Value && SystemParameters.IsGlassEnabled) ? (Style)this.Resources["Chrome"] : null;

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
			switch(e.PropertyName){
				case "IsHideFromAltTab":
				case "IsHideFromTaskbar":{
					this.RestoreHideFromTaskbar();
					if(!this.IsActive){
						if(Settings.Default.IsHideFromTaskbar){
							this.ShowInTaskbar = false;
						}
						if(Settings.Default.IsHideFromAltTab){
							this._OldWindowStyle = this.WindowStyle;
							this.WindowStyle = WindowStyle.ToolWindow;
						}
					}
					break;
				}
				case "ViewerWindowInputBindingInfos": this.RefreshInputBindings(); break;
			}
		}

		private static int GetId(){
			var id = Enumerable.Range(0, Int32.MaxValue).Where(i => !_UsedIds.Contains(i)).FirstOrDefault();
			_UsedIds.Add(id);
			return id;
		}

		private string GetSettingsKey(){
			return "ViewerWindow_" + this._Id;
		}

		#endregion

		#region EventHandlers

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
			this._ContextMenu.DataContext = e.NewValue;
			this.RefreshInputBindings();
		}

		protected override void OnClosing(CancelEventArgs e) {
			base.OnClosing(e);
			if((Settings.Default.IsHideFromTaskbar || Settings.Default.IsHideFromAltTab)
				&& Program.CurrentProgram.ActiveViewerWindow == this){
				var win = Program.CurrentProgram.ViewerWindows
					.OrderWindowByZOrder()
					.Skip(1)
					.FirstOrDefault();
				if(win != null){
					win.RestoreHideFromTaskbar();
				}
			}
		}

		protected override void OnClosed(EventArgs e){
			base.OnClosed(e);
			_UsedIds.Remove(this._Id);
			this._Settings.Save();
			Settings.Default.PropertyChanged -= this.Settings_PropertyChanged;
			if(this.DataContext != null){
				Messenger.Default.Unregister<CloseMessage>(this.ReceiveCloseMessage, this.DataContext);
				Messenger.Default.Unregister<AboutMessage>(this.ReceiveAboutMessage, this.DataContext);
				Messenger.Default.Unregister<ArrangeWindowsMessage>(this.ReceiveArrangeWindowsMessage, this.DataContext);
				Messenger.Default.Unregister<ErrorMessage>(this.ReceiveErrorMessage, this.DataContext);
				Messenger.Default.Unregister<ShowSettingsMessage>(this.ReceiveShowSettingsMessage, this.DataContext);
			}
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e) {
			if(e.Key == Key.Tab && (e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control){
				e.Handled = true;
				Program.Current.Dispatcher.BeginInvoke(
					new Action<bool>(this.ShowSelectWindowDialog),
					(e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift);
			}
			base.OnPreviewKeyDown(e);
		}

		private void ShowSelectWindowDialog(bool prev){
			var windows = Program.CurrentProgram.ViewerWindows.OrderWindowByZOrder().Select(win => win.DataContext).ToArray();
			var screen = this.GetCurrentScreen();

			var sel = new SelectWindowDialog() {
				ItemsSource = windows,
				Left = screen.ScreenArea.X,
				Top = screen.ScreenArea.Y,
			};

			if(prev){
				sel.SelectedValue = (windows.Length > 1) ? windows[windows.Length - 1] : windows[0];
			}else{
				sel.SelectedValue = (windows.Length > 1) ? windows[1] : windows[0];
			}

			sel.ShowDialog();
			var selected = Program.CurrentProgram.ViewerWindows.FirstOrDefault(win => win.DataContext == sel.SelectedValue);
			if(selected != null){
				selected.Activate();
			}
		}

		private void OnWindowStateChanged(object sender, EventArgs e){
			var windowChrome = WindowChrome.GetWindowChrome(this);
			this.Dispatcher.BeginInvoke(new Action(delegate{
				if(windowChrome != null && this.WindowState == WindowState.Maximized){
					this.Activated -= this.OnActivated;
					this.Deactivated -= this.OnDeactivated;
					this.Hide();
					this.Show();
					this.Activated += this.OnActivated;
					this.Deactivated += this.OnDeactivated;
				}
			}));
		}

		#endregion

		#region Taskbar switch

		private void OnDeactivated(object sender, EventArgs e) {
			if(Settings.Default.IsHideFromTaskbar || Settings.Default.IsHideFromAltTab){
				this.Dispatcher.BeginInvoke(new Action(this.HideFromTaskbar));
			}
		}

		private void HideFromTaskbar(){
			if(Program.CurrentProgram.ViewerWindows.Any(win => win.IsActive)){
				if(Settings.Default.IsHideFromTaskbar){
					this.ShowInTaskbar = false;
				}
				if(Settings.Default.IsHideFromAltTab && this.WindowStyle != WindowStyle.ToolWindow){
					this._OldWindowStyle = this.WindowStyle;
					this.WindowStyle = WindowStyle.ToolWindow;
				}
			}
		}

		private void RestoreHideFromTaskbar(){
			if(Settings.Default.IsHideFromTaskbar){
				this.ShowInTaskbar = true;
			}
			if(Settings.Default.IsHideFromAltTab && this._OldWindowStyle != null && this.WindowStyle == WindowStyle.ToolWindow){
				this.WindowStyle = this._OldWindowStyle.Value;
			}
		}

		private WindowStyle? _OldWindowStyle;
		private void OnActivated(object sender, EventArgs e) {
			if(Settings.Default.IsHideFromTaskbar || Settings.Default.IsHideFromAltTab){
				foreach(var win in Program.CurrentProgram.ViewerWindows){
					var source = (HwndSource)PresentationSource.FromVisual(win);
					if(source != null){
						win.SetTopZOrder();
						win.HideFromTaskbar();
					}
				}
				this.RestoreHideFromTaskbar();
			}
		}

		#endregion

		#region Receive Messages

		private void ReceiveCloseMessage(CloseMessage message){
			this.Close();
		}

		private void ReceiveAboutMessage(AboutMessage message){
			var dialog = new CatWalk.Windows.AboutBox();
			try{
				using(var il = new Shell::ImageList(Shell::ImageList.MaxSize)) {
					var icon = il.GetIcon(Assembly.GetExecutingAssembly().Location, Shell::ImageListDrawOptions.Transparent);
					dialog.AppIcon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
						icon.Handle,
						Int32Rect.Empty,
						BitmapSizeOptions.FromEmptyOptions());
				}
				using(var il = new Shell::ImageList(Shell::ImageListSize.Small)) {
					var icon = il.GetIcon(Assembly.GetExecutingAssembly().Location, Shell::ImageListDrawOptions.Transparent);
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
			Arranger arranger = null;
			switch(message.Mode){
				case ArrangeMode.Cascade: arranger = new CascadeArranger(); break;
				case ArrangeMode.TileHorizontal: arranger = new TileHorizontalArranger(); break;
				case ArrangeMode.TileVertical: arranger = new TileVerticalArranger(); break;
				case ArrangeMode.StackHorizontal: arranger = new StackHorizontalArranger(); break;
				case ArrangeMode.StackVertical: arranger = new StackVerticalArranger(); break;
			}

			var window = this;
			var screen = Win32::Screen.GetCurrentMonitor(new CatWalk.Int32Rect((int)window.Left, (int)window.Top, (int)window.Width, (int)window.Height));
			if(screen == null){
				return;
			}

			var windows = Program.CurrentProgram.ViewerWindows.Where(win => win.WindowState != WindowState.Minimized)
				.OrderWindowByZOrder()
				.Where(view => Win32::Screen.GetCurrentMonitor(new CatWalk.Int32Rect((int)view.Left, (int)view.Top, (int)view.Width, (int)view.Height)) == screen).ToArray();
			var size = new Size(screen.WorkingArea.Width, screen.WorkingArea.Height);

			var i = 0;
			foreach(var rect in arranger.Arrange(size, windows.Length)){
				rect.Offset(screen.WorkingArea.Left, screen.WorkingArea.Top);
				var win = windows[i];
				win.WindowState = WindowState.Normal;
				win.Left = rect.Left;
				win.Top = rect.Top;
				win.Width = rect.Width;
				win.Height = rect.Height;
				i++;
			}
			
			foreach(var win in windows.Reverse()){
				win.SetTopZOrder();
			}
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

		#region Drag n Drop

		protected override void OnPreviewDragOver(DragEventArgs e){
			if(e.Data.GetDataPresent(DataFormats.FileDrop)){
				e.Effects = DragDropEffects.Copy;
			}else{
				e.Effects = DragDropEffects.None;
			}
			e.Handled = true;
			base.OnPreviewDragOver(e);
		}

		protected override void OnDrop(DragEventArgs e){
			var data = e.Data.GetData(DataFormats.FileDrop);
			var files = data as string[];
			if(files != null && files.Length > 0){
				var first = files[0];
				Messenger.Default.Send(new OpenFileMessage(this, first), this.DataContext);

				foreach(var file in files.Skip(1)){
					Program.CurrentProgram.CreateViewerWindow(file, true).Show();
				}
			}
			base.OnDrop(e);
		}

		#endregion
	}
}
