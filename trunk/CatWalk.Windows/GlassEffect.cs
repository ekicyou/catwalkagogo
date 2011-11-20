/*
	$Id$
*/
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace CatWalk.Windows{
	public static class GlassEffect{
		[DllImport("Dwmapi.dll")]
		public static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref Margins margin);
		[DllImport("Dwmapi.dll")]
		private static extern bool DwmIsCompositionEnabled();

		[StructLayout(LayoutKind.Sequential)]
		public struct Margins{
			public int LeftWidth;
			public int RightWidth;
			public int TopHeight;
			public int BottomHeight;
		}

		public static readonly DependencyProperty IsEnabledProperty =
			DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(GlassEffect),
				new FrameworkPropertyMetadata(OnIsEnabledChanged));

		[AttachedPropertyBrowsableForType(typeof(Window))]
		public static void SetIsEnabled(DependencyObject element, Boolean value){
			element.SetValue(IsEnabledProperty, value);
		}

		[AttachedPropertyBrowsableForType(typeof(Window))]
		public static Boolean GetIsEnabled(DependencyObject element){
			return (Boolean)element.GetValue(IsEnabledProperty);
		}

		private static void OnIsEnabledChanged
			(DependencyObject obj, DependencyPropertyChangedEventArgs args){
			if((bool)args.NewValue == true) {
				Window wnd = obj as Window;
				if(wnd != null){
					wnd.Loaded += new RoutedEventHandler(WindowLoaded);
				}else{
					throw new ArgumentException();
				}
			}
		}

		private static void WindowLoaded(object sender, RoutedEventArgs e){
			if(Environment.OSVersion.Platform == PlatformID.Win32NT &&
				Environment.OSVersion.Version.Major >= 6){
				//DwmIsCompositionEnabled()){ // Windows Vista or later
				Window wnd = (Window)sender;
				wnd.Background = Brushes.Transparent;
				IntPtr mainWindowPtr = new WindowInteropHelper(wnd).Handle;
				HwndSource mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
				mainWindowSrc.CompositionTarget.BackgroundColor = Color.FromArgb(0, 0, 0, 0);
				Margins margins = new Margins();
				margins.LeftWidth = -1;
				margins.RightWidth = -1;
				margins.TopHeight = -1;
				margins.BottomHeight = -1;
				DwmExtendFrameIntoClientArea(mainWindowPtr, ref margins);
			}
		}
	}
}
