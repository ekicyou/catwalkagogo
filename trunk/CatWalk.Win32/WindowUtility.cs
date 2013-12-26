using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace CatWalk.Win32 {
	public static class WindowUtility {
		public static IEnumerable<IntPtr> OrderByZOrder(this IEnumerable<IntPtr> windows){
			var hash = new HashSet<IntPtr>(windows);

			for(IntPtr hWnd = User32.GetTopWindow(IntPtr.Zero); hWnd != IntPtr.Zero; hWnd = User32.GetNextWindow(hWnd, GW_HWNDNEXT)) {
				if(hash.Contains(hWnd)){
					yield return hWnd;
				}
			}
		}

		private const uint GW_HWNDNEXT = 2;

		public static IEnumerable<IntPtr> GetWindows(){
			var list = new List<IntPtr>();
			User32.EnumWindows(GetWindowsProc, list);
			return list;
		}

		private static bool GetWindowsProc(IntPtr hwnd, object o){
			var list = (List<IntPtr>)o;
			list.Add(hwnd);
			return true;
		}

		/*
		public void Show(IntPtr hwnd, ShowWindowCommand cmd){
			Win32Api.ShowWindow(hwnd, cmd);
		}

		public void SetForeground(IntPtr hwnd){
			Win32Api.SetForegroundWindow(hwnd);
		}

		public IntPtr GetForeground(){
			return Win32Api.GetForegroundWindow();
		}

		public void Activate(IntPtr hwnd){
			Win32Api.SetActiveWindow(hwnd);
		}

		public IntPtr GetActive(){
			return Win32Api.GetActiveWindow();
		}
		 * */
	}
}
