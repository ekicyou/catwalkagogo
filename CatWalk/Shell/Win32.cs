/*
	$Id$
*/

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace CatWalk.Shell{
	/// <summary>
	/// Win32APIを扱う静的クラス。
	/// </summary>
	public static class Win32{
		[DllImport("KERNEL32.DLL", EntryPoint = "RtlMoveMemory", CharSet = CharSet.Auto)]
		public static extern void CopyMemory(IntPtr dst, IntPtr src, IntPtr length);
		
		[DllImport("USER32.DLL", EntryPoint = "GetActiveWindow", CharSet = CharSet.Auto)]
		public static extern IntPtr GetActiveWindow();
		
		[DllImport("USER32.DLL", EntryPoint = "SetActiveWindow", CharSet = CharSet.Auto)]
		public static extern IntPtr SetActiveWindow(IntPtr hwnd);
		
		[DllImport("USER32.DLL", EntryPoint = "GetClassLong", CharSet = CharSet.Auto)]
		public static extern IntPtr GetClassLong(IntPtr hwnd, GetClassLongOption nIndex);
		
		/// <summary>
		/// ウインドウを表示する。
		/// </summary>
		/// <param name="hwnd">表示するウインドウのハンドル</param>
		/// <param name="cmdShow">ウインドウの状態</param>
		/// <returns>int</returns>
		[DllImport("USER32.DLL", EntryPoint = "ShowWindow", CharSet = CharSet.Auto)]
		public static extern int ShowWindow(IntPtr hwnd, ShowWindowCommand cmdShow);
		
		/// <summary>
		/// ウインドウを前面に表示する。
		/// </summary>
		/// <param name="hWnd">前面い表示するウインドウのハンドル</param>
		/// <returns>bool</returns>
		[DllImport("USER32.DLL", EntryPoint = "SetForegroundWindow", CharSet = CharSet.Auto)]
		public static extern bool SetForegroundWindow(IntPtr hWnd);
		
		[DllImport("USER32.DLL", EntryPoint = "GetForegroundWindow", CharSet = CharSet.Auto)]
		public static extern IntPtr GetForegroundWindow();
		
		[DllImport("USER32.DLL", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(IntPtr hwnd, WindowMessage msg, int wParam, int lParam);
		
		[DllImport("USER32.DLL", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(IntPtr hwnd, int msg, int wParam, int lParam);
		
		[DllImport("USER32.DLL", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(IntPtr hwnd, int msg, int wParam, ref HeaderItem lParam);
		
		/// <summary>
		/// IMEのコンテキストを取得する。
		/// </summary>
		/// <param name="hwnd">hwnd</param>
		/// <returns>コンテキストのポインタ</returns>
		[DllImport("imm32.dll", EntryPoint = "ImmGetContext", CharSet = CharSet.Auto)]
		public static extern IntPtr ImmGetContext(IntPtr hwnd);
		
		/// <summary>
		/// IMEの状態を変更する。
		/// </summary>
		/// <param name="hIMC">IMEのコンテキストのポインタ</param>
		/// <param name="fOpen">IMEを開くかどうか</param>
		/// <returns>bool</returns>
		[DllImport("imm32.dll", EntryPoint = "ImmSetOpenStatus", CharSet = CharSet.Auto)]
		public static extern bool ImmSetOpenStatus(IntPtr hIMC, bool fOpen);
		
		public delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);
		
		[DllImport("user32", EntryPoint = "EnumWindows", CharSet = CharSet.Auto)]
		public static extern int EnumWindows(EnumWindowsProc lpEnumFunc, int lParam);
		
		[DllImport("user32", EntryPoint = "GetWindowThreadProcessId", CharSet = CharSet.Auto)]
		public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int ProcessId);
		
		[DllImport("user32", EntryPoint = "GetWindow", CharSet = CharSet.Auto)]
		public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowOption option);
		
		[DllImport("user32", EntryPoint = "GetWindowText", CharSet = CharSet.Auto)]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
		
		[DllImport("user32", EntryPoint = "SetWindowText", CharSet = CharSet.Auto)]
		public static extern bool SetWindowText(IntPtr hWnd, [MarshalAs(UnmanagedType.LPTStr)] string text);
		
		[DllImport("user32", EntryPoint = "SetWindowPos", CharSet = CharSet.Auto)]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hwndAfter, int x, int y, int width, int height, SetWindowPosOptions options);
		
		[DllImport("user32", EntryPoint = "GetWindowRect", CharSet = CharSet.Auto)]
		public static extern int GetWindowRect(IntPtr hWnd, ref Win32.Rectangle rect);
		
		[DllImport("user32", EntryPoint = "IsWindow", CharSet = CharSet.Auto)]
		public static extern bool IsWindow(IntPtr hWnd);
		
		[DllImport("user32", EntryPoint = "GetWindowPlacement", CharSet = CharSet.Auto)]
		public static extern bool GetWindowPlacement(IntPtr Handle, ref WindowPlacement placement);
		
		[DllImport("user32", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
		public static extern uint GetWindowLong(IntPtr Handle, GetWindowLongOption option);
		
		[DllImport("user32", EntryPoint = "IsWindowVisible", CharSet = CharSet.Auto)]
		public static extern bool IsWindowVisible(IntPtr Handle);
		
		[DllImport("user32", EntryPoint = "IsWindowEnabled", CharSet = CharSet.Auto)]
		public static extern bool IsWindowEnabled(IntPtr Handle);
		
		[DllImport("user32", EntryPoint = "EnableWindow", CharSet = CharSet.Auto)]
		public static extern bool IsWindowEnabled(IntPtr Handle, bool enable);
		
		[StructLayout(LayoutKind.Sequential)]
		public struct WindowPlacement{
			public long Length;
			public long Flags;
			public ShowWindowCommand Command;
			public Point MinPosition;
			public Point MaxPosition;
			public Rectangle NormalPosition;
		}
		
		/// <summary>
		/// Win32.WindowPlacement構造体のメンバで使用する構造体。
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct Point{
			public long X;
			public long Y;
		}
		
		/// <summary>
		/// Win32.WindowPlacement構造体のメンバで使用する構造体。
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct Rectangle{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}
		
		[DllImport("user32.dll", EntryPoint = "ToUnicode", CharSet = CharSet.Auto)]
		public static extern int ToUnicode(uint wVirtKey, uint wScanCode, byte[] lpKeyState, StringBuilder pwszBuff, int cchBuff, uint wFlags);
		
		/// <summary>
		/// 仮想キーコードとキーの状態から入力された文字(ユニコード文字列)を取得します。
		/// </summary>
		/// <seealso cref="Win32.GetKeyboardState()">Win32.GetKeyboardState</seealso>
		public static string KeyToUnicode(uint virtualKey, byte[] keyState){
			StringBuilder sb = new StringBuilder(16);
			int result = ToUnicode(virtualKey, 0, keyState, sb, sb.Capacity, 0);
			if(result > 0){
				return sb.ToString();
			}else{
				return null;
			}
		}
		
		[DllImport("user32.dll", EntryPoint = "GetKeyboardState", CharSet = CharSet.Auto)]
		internal static extern int GetKeyboardState(byte[] lpKeyState);
		
		public static byte[] GetKeyboardState(){
			byte[] keyState = new byte[256];
			int result = GetKeyboardState(keyState);
			if(result == 0){
				throw new Win32Exception();
			}
			return keyState;
		}
		
		[DllImport("user32.dll", EntryPoint = "MessageBeep", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool MessageBeep(MessageBeep messageBeep);
		
		[DllImport("shlwapi.dll", EntryPoint = "PathMatchSpec", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool PathMatchSpec([MarshalAs(UnmanagedType.LPTStr)] string path, [MarshalAs(UnmanagedType.LPTStr)] string spec);
		/*
		[DllImport("kernel32.dll", EntryPoint = "ExpandEnvironmentStrings", CharSet = CharSet.Auto)]
		public static extern int ExpandEnvironmentStrings([MarshalAs(UnmanagedType.LPTStr)] string src, StringBuilder dest, int size);
		*/
		[DllImport("user32.dll", EntryPoint = "CreatePopupMenu", CharSet = CharSet.Auto)]
		public static extern IntPtr CreatePopupMenu();
		
		[DllImport("user32.dll", EntryPoint = "TrackPopupMenu", CharSet = CharSet.Auto)]
		public static extern int TrackPopupMenu(IntPtr hMenu, TrackPopupMenuOptions wFlags, int x, int y, int nReserved, IntPtr hwnd, int lprc);
		
		[DllImport("user32.dll", EntryPoint = "DestroyMenu", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DestroyMenu(IntPtr hMenu);
		
		[DllImport("user32.dll", EntryPoint = "GetMenuDefaultItem", CharSet = CharSet.Auto)]
		public static extern int GetMenuDefaultItem(IntPtr hMenu, MenuFoundBy byPos, GetMenuDefaultItemOptions options);
		
		[DllImport("user32.dll", EntryPoint = "GetMenuItemCount", CharSet = CharSet.Auto)]
		public static extern int GetMenuItemCount(IntPtr hMenu);
		
		[DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetProcessWorkingSetSize(IntPtr procHandle, IntPtr min, IntPtr max);
		
		/// <summary>
		/// ワーキングセットの値を減らします。
		/// </summary>
		public static void ReduceWorkingSet(){
			SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, new IntPtr(-1L), new IntPtr(-1L));
		}
		
		[DllImport("user32.dll", EntryPoint = "RegisterHotKey", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public extern static bool RegisterHotKey(IntPtr hwnd, int id, ModifierKeys modKeys, System.Windows.Forms.Keys key);
		
		[DllImport("user32.dll", EntryPoint = "UnregisterHotKey", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public extern static bool UnregisterHotKey(IntPtr hwnd, int id);
		
		[DllImport("user32.dll", EntryPoint = "GetDesktopWindow", CharSet = CharSet.Auto)]
		public static extern IntPtr GetDesktopWindow();
		
		[DllImport("user32.dll", EntryPoint = "GetDC", CharSet = CharSet.Auto)]
		public static extern IntPtr GetDeviceContext(IntPtr hWnd);
		
		[DllImport("user32.dll", EntryPoint = "ReleaseDC", CharSet = CharSet.Auto)]
		public static extern IntPtr ReleaseDeviceContext(IntPtr hWnd, IntPtr hDc);
		
		[DllImport("gdi32.dll", EntryPoint = "BitBlt", CharSet = CharSet.Auto)]
		public static extern bool BitBlt(IntPtr hdcDst, int xDst, int yDsk, int width, int height, IntPtr hdcSrc, int xSrc, int ySrc, int rasterOp);
		
		public static System.Drawing.Rectangle GetTotalBound(){
			int x, y, w, h;
			int minX = 0, maxX = 0, minY = 0, maxY = 0;
			foreach(Screen s in Screen.AllScreens){
				x = s.Bounds.X;
				y = s.Bounds.Y;
				w = s.Bounds.X + s.Bounds.Width;
				h = s.Bounds.Y + s.Bounds.Height;
				if(x < minX)
					minX = x;
				if(y < minY)
					minY = y;
				if(maxX < w)
					maxX = w;
				if(maxY < h)
					maxY = h;
			}
			System.Drawing.Rectangle r = new System.Drawing.Rectangle(minX, minY, maxX - minX, maxY - minY);
			return r;
		}
		
		public static Bitmap GetDesktopBitmap(){
			System.Drawing.Rectangle rect = GetTotalBound();
			Bitmap bitmap = new Bitmap(rect.Width, rect.Height);
			Graphics g = Graphics.FromImage(bitmap);
			IntPtr desktopHandle = IntPtr.Zero;
			IntPtr desktopDeviceContext = IntPtr.Zero;
			IntPtr imageDeviceContext = IntPtr.Zero;
			try{
				desktopHandle = GetDesktopWindow();
				desktopDeviceContext = GetDeviceContext(desktopHandle);
				imageDeviceContext = g.GetHdc();
				BitBlt(imageDeviceContext, 0, 0, rect.Width, rect.Height, desktopDeviceContext, rect.X, rect.Y, 0xCC0020);
			}finally{
				ReleaseDeviceContext(desktopHandle, desktopDeviceContext);
				if(imageDeviceContext != IntPtr.Zero){
					g.ReleaseHdc(imageDeviceContext);
				}
			}
			return bitmap;
		}
		
		[DllImport("USER32.DLL", EntryPoint = "DestroyIcon", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DestroyIcon(IntPtr hIcon);
		
		[DllImport("kernel32", EntryPoint = "GetLogicalDrives", CharSet = CharSet.Auto)]
		internal static extern long InternalGetLogicalDrives();
		
		public static string[] GetLogicalDrives(){
			List<string> drives = new List<string>();
			long d = Win32.InternalGetLogicalDrives();
			for(int i = 0; i < 26; i++){
				if(((1 << i) & d) > 0){
					drives.Add(((char)('A' + i)) + ":\\");
				}
			}
			return drives.ToArray();
		}
		
		[DllImport("kernel32", EntryPoint = "GetVolumeInformation", CharSet = CharSet.Auto)]
		public static extern bool GetVolumeInformation(string drivePath, StringBuilder volumeNameBuffer, int volumeNameBufferSize, int volumeSerialNumber, int maximumComponentLength, int fileSystemFlags, StringBuilder fileSystemNameBuffer, int fileSystemNameSize);
		
		public static bool GetVolumeInformation(string drivePath, out string volumeLabel, out string fileSystemName){
			StringBuilder volumeNameBuffer = new StringBuilder(64);
			StringBuilder fileSystemNameBuffer = new StringBuilder(64);
			bool result = GetVolumeInformation(drivePath, volumeNameBuffer, volumeNameBuffer.Capacity, 0, 260, 0, fileSystemNameBuffer, fileSystemNameBuffer.Capacity);
			volumeLabel = volumeNameBuffer.ToString();
			fileSystemName = fileSystemNameBuffer.ToString();
			return result;
		}
		
		[DllImport("kernel32", EntryPoint = "GetShortPathName", CharSet = CharSet.Auto)]
		public static extern int GetShortPathName(string path, StringBuilder shortName, int bufferSize);
		
		public static string GetShortPathName(string path){
			for(int count = path.Length; count < 32767; count *= 2){
				StringBuilder sb = new StringBuilder(count);
				if(GetShortPathName(path, sb, count) != 0){
					return sb.ToString();
				}
			}
			return null;
		}
		
		[DllImport("kernel32", EntryPoint = "GetLongPathName", CharSet = CharSet.Ansi)]
		public static extern int GetLongPathName(string path, StringBuilder longName, int bufferSize);
		
		public static string GetLongPathName(string path){
			for(int count = path.Length + 256; count < 32767; count *= 2){
				StringBuilder sb = new StringBuilder(count);
				if(GetLongPathName(path, sb, count) != 0){
					return sb.ToString();
				}
			}
			return null;
		}
		
		[DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
		public extern static Int32 SetWindowTheme(IntPtr hwnd, String subAppName, String subIdList);
		
		[DllImport("winmm.dll", CharSet = CharSet.Auto, EntryPoint = "mciSendString")]
		public extern static int MciSendString(string command, StringBuilder returnString, int bufferLength, IntPtr hwndCallback);
	}
	
	public class NativeMenuItem : DisposableObject{
		private IntPtr handle;
		
		public NativeMenuItem(){
			this.handle = Win32.CreatePopupMenu();
		}
		
		public NativeMenuItem(IntPtr handle){
			this.handle = handle;
		}
		
		public int Show(Control control, Point pos, TrackPopupMenuOptions options){
			pos = control.PointToScreen(pos);
			return Win32.TrackPopupMenu(this.handle, options, pos.X, pos.Y, 0, control.Handle, 0);
		}
		
		public int GetDefaultItem(MenuFoundBy byPos, GetMenuDefaultItemOptions options){
			return Win32.GetMenuDefaultItem(this.handle, byPos, options);
		}
		
		public IntPtr Handle{
			get{
				return this.handle;
			}
		}
		
		public int Count{
			get{
				return Win32.GetMenuItemCount(this.handle);
			}
		}
		
		protected override void Dispose(bool disposing){
			if(this.handle != IntPtr.Zero){
				Win32.DestroyMenu(this.handle);
				this.handle = IntPtr.Zero;
			}
		}
	}
	
	public enum GetWindowOption : uint{
		First          = 0,
		Last           = 1,
		Next           = 2,
		Prev           = 3,
		Owner          = 4,
		Child          = 5,
		EnabledPopup   = 6,
	}
	
	public enum GetWindowLongOption : int{
		WndProc         = (-4),
		HInstance       = (-6),
		HwndParent      = (-8),
		Style           = (-16),
		ExStyle         = (-20),
		UserData        = (-21),
		Id              = (-12),
	}
	
	public enum GetClassLongOption : int{
		MenuName      = (-8),
		HbrBackground = (-10),
		HCursor       = (-12),
		HIcon         = (-14),
		HModule       = (-16),
		WndProc       = (-24),
		HIconSmall    = (-34),
	}
	
	[Flags]
	public enum WindowStyle : uint{
		Overlapped       = 0x00000000,
		Popup            = 0x80000000,
		Child            = 0x40000000,
		Minimize         = 0x20000000,
		Visible          = 0x10000000,
		Disabled         = 0x08000000,
		ClipSiblings     = 0x04000000,
		ClipChildren     = 0x02000000,
		Maximize         = 0x01000000,
		Caption          = 0x00C00000,     /* WS_BORDER | WS_DLGFRAME  */
		Border           = 0x00800000,
		DialogFrame      = 0x00400000,
		VScroll          = 0x00200000,
		HScroll          = 0x00100000,
		SystemMenu       = 0x00080000,
		ThickFrame       = 0x00040000,
		Group            = 0x00020000,
		TabStop          = 0x00010000,
		
		MinimizeBox      = 0x00020000,
		MaximizeBox      = 0x00010000,
		
		Tiled            = Overlapped,
		Iconic           = Minimize,
		SizeBox          = ThickFrame,
		TiledWindows     = OverlappedWindow,
		
		/*
		 * Common Window Styles
		 */
		OverlappedWindow = (Overlapped | Caption | SystemMenu | ThickFrame | MinimizeBox | MaximizeBox),
		PopupWindow      = (Popup | Border | SystemMenu),
		ChildWindow      = (Child),
	}
	
	[Flags]
	public enum SetWindowPosOptions : uint{
		NoSize          = 0x0001,
		NoMove          = 0x0002,
		NoZOrder        = 0x0004,
		NoRedraw        = 0x0008,
		NoActivate      = 0x0010,
		FrameChanged    = 0x0020,  /* The frame changed: send WM_NCCALCSIZE */
		ShowWindow      = 0x0040,
		HideWindow      = 0x0080,
		NoCopyBits      = 0x0100,
		NoOwnerZOrder   = 0x0200,  /* Don't do owner Z ordering */
		NoSendChanging  = 0x0400,  /* Don't send WM_WINDOWPOSCHANGING */
		DrawFrane       = FrameChanged,
		NoReposition    = NoOwnerZOrder,
		Defererase      = 0x2000,
		AsyncWindowPosition = 0x4000,
	}
	
	public enum WindowMessage : uint{
		Null = 0x0000,
		Create = 0x0001,
		Destroy = 0x0002,
		Move = 0x0003,
		Size = 0x0005,
		Activate = 0x0006,
		Setfocus = 0x0007,
		Killfocus = 0x0008,
		Enable = 0x000A,
		Setredraw = 0x000B,
		SetText = 0x000C,
		GetText = 0x000D,
		GetTextlength = 0x000E,
		Paint = 0x000F,
		Close = 0x0010,
		QueryEndSession = 0x0011,
		QueryOpen = 0x0013,
		EndSession = 0x0016,
		Quit = 0x0012,
		EraseBackground = 0x0014,
		SystemColorChange = 0x0015,
		ShowWindow = 0x0018,
		WinIniChange = 0x001A,
		DeviceModeChange = 0x001B,
		ActivateApplication = 0x001C,
		FontChange = 0x001D,
		TimeChange = 0x001E,
		CancelMode = 0x001F,
		SetCursor = 0x0020,
		MouseActivate = 0x0021,
		ChildActivate = 0x0022,
		QueueSync = 0x0023,
		GetMinMaxInfo = 0x0024,
		PaintIcon = 0x0026,
		IconEraseBackground = 0x0027,
		NextDialogControl = 0x0028,
		SpoolerStatus = 0x002A,
		DrawItem = 0x002B,
		MeasureItem = 0x002C,
		DeleteItem = 0x002D,
		VKeyToItem = 0x002E,
		CharToItem = 0x002F,
		SetFont = 0x0030,
		GetFont = 0x0031,
		SetHotKey = 0x0032,
		GetHotKey = 0x0033,
		QueryDragIcon = 0x0037,
		CompareItem = 0x0039,
		GetObject = 0x003D,
		Compacting = 0x0041,
		Commnotify = 0x0044  /* no longer suported */,
		WindowPositionChanging = 0x0046,
		WindowPositionChanged = 0x0047,
		Power = 0x0048,
		CopyData = 0x004A,
		CancelJournal = 0x004B,
		Notify = 0x004E,
		InputLanguageChangeRequest = 0x0050,
		InputLanguageChange = 0x0051,
		TCard = 0x0052,
		Help = 0x0053,
		UserChanged = 0x0054,
		NotifyFormat = 0x0055,
		ContextMenu = 0x007B,
		StyleChanging = 0x007C,
		StyleChanged = 0x007D,
		DisplayChange = 0x007E,
		GetIcon = 0x007F,
		SetIcon = 0x0080,
		NCCreate = 0x0081,
		NCDestroy = 0x0082,
		NCCalcSize = 0x0083,
		NCHitTest = 0x0084,
		NCPaint = 0x0085,
		NCActivate = 0x0086,
		GetDialogCode = 0x0087,
		SyncPaint = 0x0088,
		NCMouseMove = 0x00A0,
		NCLButtonDown = 0x00A1,
		NCLButtonUp = 0x00A2,
		NCLButtonDoubleClick = 0x00A3,
		NCRButtonDown = 0x00A4,
		NCRButtonUp = 0x00A5,
		NCRButtonDoubleClick = 0x00A6,
		NCMButtonDown = 0x00A7,
		NCMButtonUp = 0x00A8,
		NCMButtonDoubleClick = 0x00A9,
		NCXButtonDown = 0x00AB,
		NCXButtonUp = 0x00AC,
		NCXButtonDoubleClick = 0x00AD,
		Input = 0x00FF,
		KeyFirst = 0x0100,
		KeyDown = 0x0100,
		KeyUp = 0x0101,
		@Char = 0x0102,
		DeadChar = 0x0103,
		SystemKeyDown = 0x0104,
		SystemKeyUp = 0x0105,
		SystemChar = 0x0106,
		SystemDeadChar = 0x0107,
		UniChar = 0x0109,
		//keyLast = 0x0109,
		KeyLast = 0x0108,
		Ime_StartComposition = 0x010D,
		Ime_EndComposition = 0x010E,
		Ime_Composition = 0x010F,
		Ime_KeyLast = 0x010F,
		InitDialog = 0x0110,
		Command = 0x0111,
		SysCommand = 0x0112,
		Timer = 0x0113,
		HScroll = 0x0114,
		VScroll = 0x0115,
		InitMenu = 0x0116,
		InitMenuPopup = 0x0117,
		MenuSelect = 0x011F,
		MenuChar = 0x0120,
		EnterIdle = 0x0121,
		MenuRButtonUp = 0x0122,
		MenuDrag = 0x0123,
		MenuGetObject = 0x0124,
		UninitMenuPopup = 0x0125,
		MenuCommand = 0x0126,
		ChangeUIState = 0x0127,
		UpdateUIState = 0x0128,
		QueryUIState = 0x0129,
		CtlColorMessageBox = 0x0132,
		CtlColorEdit = 0x0133,
		CtlColorListBox = 0x0134,
		CtlColorButton = 0x0135,
		CtlColorDialog = 0x0136,
		CtlColorScrollbar = 0x0137,
		CtlColorStatic = 0x0138,
//		MouseFirst = 0x0200,
		MouseMove = 0x0200,
		LButtonDown = 0x0201,
		LButtonUp = 0x0202,
		LButtonDoubleClick = 0x0203,
		RButtonDown = 0x0204,
		RButtonUp = 0x0205,
		RButtonDoubleClick = 0x0206,
		MButtonDown = 0x0207,
		MButtonUp = 0x0208,
		MButtonDoubleClick = 0x0209,
		MouseWheel = 0x020A,
		XButtonDown = 0x020B,
		XButtonUp = 0x020C,
		XButtonDoubleClick = 0x020D,
//		MouseLast = 0x020D,
//		MouseLast = 0x020A,
//		MouseLast = 0x0209,
		ParentNotify = 0x0210,
		EnterMenuloop = 0x0211,
		ExitMenuloop = 0x0212,
		NextMenu = 0x0213,
		Sizing = 0x0214,
		CaptureChanged = 0x0215,
		Moving = 0x0216,
		MdiCreate = 0x0220,
		MdiDestroy = 0x0221,
		MdiActivate = 0x0222,
		MdiRestore = 0x0223,
		MdiNext = 0x0224,
		MdiMaximize = 0x0225,
		MdiTile = 0x0226,
		MdiCascade = 0x0227,
		MdiIconArrange = 0x0228,
		MdiGetActive = 0x0229,
		MdiSetMenu = 0x0230,
		EnterSizeMove = 0x0231,
		ExitSizeMove = 0x0232,
		DropFiles = 0x0233,
		MdiRefreshMenu = 0x0234,
		Ime_Setconrext = 0x0281,
		Ime_Notify = 0x0282,
		Ime_Control = 0x0283,
		Ime_CompositionFull = 0x0284,
		Ime_Select = 0x0285,
		Ime_Char = 0x0286,
		Ime_Request = 0x0288,
		Ime_Keydown = 0x0290,
		Ime_Keyup = 0x0291,
		MouseHover = 0x02A1,
		MouseLeave = 0x02A3,
		NcMouseHover = 0x02A0,
		NcMouseLeave = 0x02A2,
		WTSsessionChange = 0x02B1,
		TabletFirst = 0x02c0,
		TabletLast = 0x02df,
		Cut = 0x0300,
		Copy = 0x0301,
		Paste = 0x0302,
		Clear = 0x0303,
		Undo = 0x0304,
		RenderFormat = 0x0305,
		RenderAllFormats = 0x0306,
		DestroyClipboard = 0x0307,
		DrawClipboard = 0x0308,
		PaintClipboard = 0x0309,
		VScrollClipboard = 0x030A,
		SizeClipboard = 0x030B,
		AskCbformAtName = 0x030C,
		ChangeClipboardChain = 0x030D,
		HScrollClipboard = 0x030E,
		QueryNewPalette = 0x030F,
		PaletteIsChanging = 0x0310,
		PaletteChanged = 0x0311,
		HotKey = 0x0312,
		Print = 0x0317,
		PrintClient = 0x0318,
		AppCommand = 0x0319,
		ThemeChanged = 0x031A,
		HandHeldFirst = 0x0358,
		HandHeldLast = 0x035F,
		AfxFirst = 0x0360,
		AfxLast = 0x037F,
		PenWinFirst = 0x0380,
		PenWinLast = 0x038F,
		App = 0x8000,
		User = 0x0400,
	}
	
	public enum ListViewMessage : int{
		First = 0x1000,
		SetSelectedColumn = First + 140,
		GetSelectedColumn = First + 174,
		GetHeader = First + 31,
	}
	
	public enum HeaderMessage : int{
		First = 0x1200,
		GetItemA = First + 3,
		GetItemW = First + 11,
		GetItem = GetItemW,
		SetItemA = First + 4,
		SetItemW = First + 12,
		SetItem = SetItemW,
	}
	
	[Flags]
	public enum ListViewColumnFormat : int{
		Left               = 0x0000,
		Right              = 0x0001,
		Center             = 0x0002,
		JustifyMark        = 0x0003,
		RightToLeftReading = 0x0004,
		Bitmap             = 0x2000,
		String             = 0x4000,
		OwnerDraw          = 0x8000,
		
		// (_WIN32_IE >= 0x0300)
		Image              = 0x0800,
		BitmapOnRight      = 0x1000,
		
		// (_WIN32_WINNT >= 0x0501)
		SortUp             = 0x0400,
		SortDown           = 0x0200,
		
		// _WIN32_WINNT >= 0x0600
		CheckBox           = 0x0040,
		Checked            = 0x0080,
		FixedWidth         = 0x0100,
		SplitButton        = 0x1000000,
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public struct HeaderItem{
		public HeaderItemMask Mask;
		public int CXY;
		public string Text;
		public IntPtr Bitmap;
		public int TextMax;
		public ListViewColumnFormat Format;
		public int LParam;
		public int ImageIndex;
		public int OrderIndex;
		public uint Type;
		public IntPtr Filter;
		public uint State;
	}
	
	[Flags]
	public enum HeaderItemMask : uint{
		Width              = 0x0001,
		Height             = Width,
		Text               = 0x0002,
		Format             = 0x0004,
		LParam             = 0x0008,
		Bitmap             = 0x0010,
		
		// (_WIN32_IE >= 0x0300)
		Image              = 0x0020,
		DISetItem          = 0x0040,
		Order              = 0x0080,
		
		// (_WIN32_IE >= 0x0500)
		Filter             = 0x0100,
		
		// _WIN32_WINNT >= 0x0600
		State             = 0x0200,
	}
	
	public enum MessageBeep : uint{
		Okey = 0x00000000,
		IconHand = 0x00000010,
		IconQuestion = 0x00000020,
		IconExclamation = 0x00000030,
		IconAsterisk = 0x00000040,
		Beep = 0xFFFFFFFF
	}

	public enum ShowWindowCommand : int{
		Hide = 0,
		ShowNormal = 1,
		ShowMinimized = 2,
		ShowMaximized = 3,
		ShowNoActivate = 4,
		Show = 5,
		Minimize = 6,
		ShowMinNoActivete = 7,
		ShowNA = 8,
		Restore = 9,
		ShowDefault = 10,
		ForceMinimize = 11
	}
	
	[Flags]
	public enum TrackPopupMenuOptions : uint{
		LeftButton    = 0x0000,
		RightButton   = 0x0002,
		LeftAlign     = 0x0000,
		CenterAlign   = 0x0004,
		RightAlign    = 0x0008,
		TopAlign      = 0x0000,
		VCenterAlign  = 0x0010,
		BottomAlign   = 0x0020,
		Horizontal    = 0x0000,
		Vertical      = 0x0040,
		Monotify      = 0x0080,
		ReturnCommand = 0x0100,
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public struct CopyDataStruct{
		public IntPtr dwData;
		public int cbData;
		public IntPtr lpData;
	}
	
	[Flags]
	public enum ModifierKeys : int{
		Alt     = 0x01,
		Control = 0x02,
		Shift   = 0x04,
		Windows = 0x08,
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public struct NMHdr{
		public IntPtr Hdr;
		public int IDForm;
		public int Code;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public struct NMListView{
		public IntPtr Hdr;
		public int IDForm;
		public int Code;
		public int Item;
		public int SubItem;
		public uint NewState;
		public uint OldState;
		public uint Changed;
		public GDIPoint Action;
		public IntPtr LParam;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public struct GDIPoint{
		public long X;
		public long Y;
	}
	
	public enum MenuFoundBy : uint{
		Command = 0x0000,
		Position = 0x0400,
	}
	
	[Flags]
	public enum GetMenuDefaultItemOptions : uint{
		Normal = 0x0000,
		UseDisabled = 0x0001,
		GoIntoPopups = 0x0002,
	}
	
	public class PinnedObject : DisposableObject{
		private GCHandle handle;
		private bool disposed = false;
		
		public PinnedObject(object obj){
			this.handle = GCHandle.Alloc(obj, GCHandleType.Pinned);
		}
		
		public IntPtr Address{
			get{
				return this.handle.AddrOfPinnedObject();
			}
		}
		
		protected override void Dispose(bool disposing){
			if(!(this.disposed)){
				this.handle.Free();
				this.disposed = true;
			}
		}
	}
}