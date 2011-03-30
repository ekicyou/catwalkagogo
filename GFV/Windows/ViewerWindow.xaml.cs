/*
	$Id$
*/
using System;
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
using CatWalk.Windows;
using CatWalk.Windows.Input;
using GFV.Properties;
using GFV.ViewModel;

namespace GFV.Windows{
	using Gfl = GflNet;

	/// <summary>
	/// Interaction logic for ViewerWindow.xaml
	/// </summary>
	public partial class ViewerWindow : Window{
		private ContextMenu _ContextMenu;
		private int _Id;
		private static List<int> _UsedIds = new List<int>();
		private WindowSettings _Settings;

		public ViewerWindow(){
			this.InitializeComponent();
			this.WindowStartupLocation = WindowStartupLocation.Manual;

			this._Id = GetId();
			this._Settings = new WindowSettings(this.GetSettingsKey());
			this._Settings.RestoreWindow(this);

			this._ContextMenu = new ContextMenu();
			this._ContextMenu.ItemsSource = (IEnumerable)this.Resources["MainMenu"];
			this._Viewer.ContextMenu = this._ContextMenu;
		}

		private static int GetId(){
			return Enumerable.Range(0, Int32.MaxValue).Where(i => !_UsedIds.Contains(i)).FirstOrDefault();
		}

		private string GetSettingsKey(){
			return "ViewerWindow_" + this._Id;
		}

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
			this._ContextMenu.DataContext = e.NewValue;
		}

		protected override void OnClosed(EventArgs e){
			base.OnClosed(e);
			this._Settings.Save();
		}

		#endregion
	}
}
