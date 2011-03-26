/*
	$Id$
*/
using System;
using System.Collections;
using System.Collections.Generic;
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
using GFV.ViewModel;
using GFV.Properties;
using CatWalk.Windows;
using CatWalk.Shell;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Shell;

namespace GFV.Windows{
	using Gfl = GflNet;

	/// <summary>
	/// Interaction logic for ViewerWindow.xaml
	/// </summary>
	public partial class ViewerWindow : Window{
		private ContextMenu _ContextMenu;

		public ViewerWindow(){
			this.InitializeComponent();

			this.WindowStartupLocation = WindowStartupLocation.Manual;
			this.Width = Settings.Default.ViewerWindowRestoreBounds.Width;
			this.Height = Settings.Default.ViewerWindowRestoreBounds.Height;
			this.Left = Settings.Default.ViewerWindowRestoreBounds.Left;
			this.Top = Settings.Default.ViewerWindowRestoreBounds.Top;
			this.WindowState = Settings.Default.ViewerWindowState;

			this._ContextMenu = new ContextMenu();
			this._ContextMenu.ItemsSource = (IEnumerable)this.Resources["MainMenu"];
			this._Viewer.ContextMenu = this._ContextMenu;
		}

		private void About_Executed(object sender, ExecutedRoutedEventArgs e){
			var dialog = new AboutBox();
			var addInfo = new ObservableCollection<KeyValuePair<string, string>>();
			addInfo.Add(new KeyValuePair<string,string>("", ""));
			addInfo.Add(new KeyValuePair<string,string>("Graphic File Library", Program.Gfl.DllName));
			addInfo.Add(new KeyValuePair<string,string>("Copyright", "Copyright © 1991-2009 Pierre-e Gougelet"));
			addInfo.Add(new KeyValuePair<string,string>("Version", Program.Gfl.VersionString));
			addInfo.Add(new KeyValuePair<string,string>("", ""));
			addInfo.Add(new KeyValuePair<string,string>("Supported Formats:", ""));
			foreach(var fmt in Program.Gfl.Formats.OrderBy(fmt => fmt.Description)){
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
			//dialog.AppIcon = new BitmapImage(new Uri());

			dialog.Owner = this;
			dialog.ShowDialog();
		}

		#region EventHandlers

		protected override void OnStateChanged(EventArgs e){
			base.OnStateChanged(e);
			if(this.WindowState != WindowState.Minimized){
				Settings.Default.ViewerWindowState = this.WindowState;
			}
		}

		protected override void OnLocationChanged(EventArgs e){
			base.OnLocationChanged(e);
			Settings.Default.ViewerWindowRestoreBounds = this.RestoreBounds;
		}

		private void Window_SizeChanged(object sender, SizeChangedEventArgs e){
			Settings.Default.ViewerWindowRestoreBounds = this.RestoreBounds;
			var viewerRect = VisualTreeHelper.GetContentBounds(this._Viewer);
			this.TaskbarItemInfo.ThumbnailClipMargin = new Thickness(
				viewerRect.Left,
				viewerRect.Top,
				viewerRect.Right,
				viewerRect.Bottom);
		}

		private void Window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			this._ContextMenu.DataContext = e.NewValue;
		}

		#endregion
	}
}
