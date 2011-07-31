using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace CatWalk.Win32 {
	public static class WindowUtility {
		public static IEnumerable<IntPtr> OrderByZOrder(this IEnumerable<IntPtr> windows){
			var hash = new HashSet<IntPtr>(windows);

			for(IntPtr hWnd = GetTopWindow(IntPtr.Zero); hWnd != IntPtr.Zero; hWnd = GetNextWindow(hWnd, GW_HWNDNEXT)){
				if(hash.Contains(hWnd)){
					yield return hWnd;
				}
			}
		}

		private const uint GW_HWNDNEXT = 2;
		[DllImport("User32")]
		private static extern IntPtr GetTopWindow(IntPtr hWnd);
		[DllImport("User32", EntryPoint="GetWindow")]
		private static extern IntPtr GetNextWindow(IntPtr hWnd, uint wCmd);

		public static IEnumerable<IntPtr> GetWindows(){
			var list = new List<IntPtr>();
			Win32Api.EnumWindows(GetWindowsProc, list);
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
