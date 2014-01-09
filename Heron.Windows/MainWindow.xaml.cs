﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Shell;
using System.Windows.Controls.Ribbon;
using System.Windows.Interop;
using CatWalk.Win32;

namespace CatWalk.Heron.Windows {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : RibbonWindow {
		public WindowsPlugin Plugin { get; private set; }
		private HotKeyManager _HotKeyManager;
		private HwndSource _HwndSource;

		public MainWindow(WindowsPlugin plugin) {
			plugin.ThrowIfNull("plugin");
			InitializeComponent();

			this.Plugin = plugin;

			var wint = new WindowInteropHelper(this);
			wint.EnsureHandle();
			this._HwndSource = HwndSource.FromHwnd(wint.Handle);
			this._HotKeyManager = new HotKeyManager(this._HwndSource.Handle);
			this._HwndSource.AddHook(this.WndProc);
		}

		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
			return this._HotKeyManager.WndProc(hwnd, msg, wParam, lParam, ref handled);
		}

		public Ribbon Ribbon {
			get {
				return this._Ribbon;
			}
		}

		public HotKeyManager HotKeyManager {
			get {
				return this._HotKeyManager;
			}
		}

		private void RibbonWindow_Activated(object sender, EventArgs e) {
			WindowUtility.LatestActiveWindow = this;
		}
	}

	public class JobCountToProgressStateConverter : IValueConverter {
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(targetType == typeof(TaskbarItemProgressState)) {
				return ((int)value > 0) ? TaskbarItemProgressState.Normal : TaskbarItemProgressState.None;
			} else {
				return null;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

		#endregion
	}
}
