/*
	$Id$
*/
using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CatWalk.Interop{
	using ComTypes = System.Runtime.InteropServices.ComTypes;
	
	// based on http://smdn.invisiblefulmoon.net/ikimasshoy/dotnettips/tips043.html
	public sealed class ShellLink : IDisposable{
		// IShellLinkインターフェイス
		private IShellLink shellLink;
		
		// カレントファイル
		private string currentFile;
		
		// 各種定数
		internal const int MAX_PATH = 260;
		
		internal const uint SLGP_SHORTPATH   = 0x0001; // 短い形式(8.3形式)のファイル名を取得する
		internal const uint SLGP_UNCPRIORITY = 0x0002; // UNCパス名を取得する
		internal const uint SLGP_RAWPATH	 = 0x0004; // 環境変数などが変換されていないパス名を取得する
		
		#region "コンストラクション・デストラクション"
		public ShellLink(){
			this.currentFile = "";
			this.shellLink = null;
			
			try{
				// Unicode環境
				this.shellLink = (IShellLink)(new ShellLinkObject());
			}catch(Exception ex){
				throw new COMException("Can't get a IShellLink object.", ex);
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="linkFile">ショートカットファイル</param>
		public ShellLink(string linkFile) : this(){
			this.Load(linkFile);
		}

		/// <summary>
		/// デストラクタ
		/// </summary>
		~ShellLink(){
			Dispose();
		}

		/// <summary>
		/// このインスタンスが使用しているリソースを解放します。
		/// </summary>
		public void Dispose(){
			if(this.shellLink != null){
				Marshal.ReleaseComObject(this.shellLink);
				this.shellLink = null;
				GC.SuppressFinalize(this);
			}
		}

		#endregion

		#region "プロパティ"

		/// <summary>
		/// カレントファイル。
		/// </summary>
		public string CurrentFile{
			get{
				return currentFile;
			}
		}

		/// <summary>
		/// ショートカットのリンク先。
		/// </summary>
		public string TargetPath{
			get{		
				StringBuilder targetPath = new StringBuilder(MAX_PATH, MAX_PATH);
				ShellLinkFindData data = new ShellLinkFindData();
				this.shellLink.GetPath(targetPath, targetPath.Capacity, ref data, SLGP_UNCPRIORITY);
				return targetPath.ToString();
			}
			set{
				this.shellLink.SetPath(value);
			}
		}

		/// <summary>
		/// 作業ディレクトリ。
		/// </summary>
		public string WorkingDirectory{
			get{
				StringBuilder workingDirectory = new StringBuilder(MAX_PATH, MAX_PATH);
				this.shellLink.GetWorkingDirectory(workingDirectory, workingDirectory.Capacity);
				return workingDirectory.ToString();
			}
			set{
				this.shellLink.SetWorkingDirectory(value);	
			}
		}

		/// <summary>
		/// コマンドライン引数。
		/// </summary>
		public string Arguments{
			get{
				StringBuilder arguments = new StringBuilder(MAX_PATH, MAX_PATH);
				this.shellLink.GetArguments(arguments, arguments.Capacity);
				return arguments.ToString();
			}
			set{
				this.shellLink.SetArguments(value);	
			}
		}

		/// <summary>
		/// ショートカットの説明。
		/// </summary>
		public string Description{
			get{
				StringBuilder description = new StringBuilder(MAX_PATH, MAX_PATH);
				this.shellLink.GetDescription(description, description.Capacity);
				return description.ToString();
			}
			set{
				this.shellLink.SetDescription(value);	
			}
		}

		/// <summary>
		/// アイコンのファイル。
		/// </summary>
		public string IconFile{
			get{
				int iconIndex = 0;
				string iconFile = "";
				this.GetIconLocation(out iconFile, out iconIndex);
				return iconFile;
			}
			set{
				int iconIndex = 0;
				string iconFile = "";
				this.GetIconLocation(out iconFile, out iconIndex);
				this.SetIconLocation(value, iconIndex);
			}
		}

		/// <summary>
		/// アイコンのインデックス。
		/// </summary>
		public int IconIndex{
			get{
				int iconIndex = 0;
				string iconPath = "";
				this.GetIconLocation(out iconPath, out iconIndex);
				return iconIndex;
			}
			set{
				int iconIndex = 0;
				string iconPath = "";
				this.GetIconLocation(out iconPath, out iconIndex);
				this.SetIconLocation(iconPath, value);
			}
		}

		/// <summary>
		/// アイコンのファイルとインデックスを取得する
		/// </summary>
		/// <param name="iconFile">アイコンのファイル</param>
		/// <param name="iconIndex">アイコンのインデックス</param>
		private void GetIconLocation(out string iconFile, out int iconIndex){
			StringBuilder iconFileBuffer = new StringBuilder(MAX_PATH, MAX_PATH);
			this.shellLink.GetIconLocation(iconFileBuffer, iconFileBuffer.Capacity, out iconIndex);
			iconFile = iconFileBuffer.ToString();
		}

		/// <summary>
		/// アイコンのファイルとインデックスを設定する
		/// </summary>
		/// <param name="iconFile">アイコンのファイル</param>
		/// <param name="iconIndex">アイコンのインデックス</param>
		private void SetIconLocation(string iconFile, int iconIndex){
			this.shellLink.SetIconLocation(iconFile, iconIndex);
		}

		/// <summary>
		/// 実行時のウィンドウの大きさ。
		/// </summary>
		public ShellLinkDisplayMode DisplayMode{
			get{
				int showCmd = 0;
				this.shellLink.GetShowCmd(out showCmd);	
				return (ShellLinkDisplayMode)showCmd;
			}
			set
			{
				this.shellLink.SetShowCmd((int)value);
			}
		}

		/// <summary>
		/// ホットキー。
		/// </summary>
		public Keys HotKey{
			get{
				ushort hotKey = 0;
				this.shellLink.GetHotkey(out hotKey);
				return (Keys)hotKey;
			}
			set{
				this.shellLink.SetHotkey((ushort)value);
			}
		}

		#endregion

		#region "保存と読み込み"

		/// <summary>
		/// IShellLinkインターフェイスからキャストされたIPersistFileインターフェイスを取得します。
		/// </summary>
		/// <returns>IPersistFileインターフェイス。　取得できなかった場合はnull。</returns>
		private ComTypes.IPersistFile GetIPersistFile(){
			return this.shellLink as ComTypes.IPersistFile;
		}

		/// <summary>
		/// カレントファイルにショートカットを保存します。
		/// </summary>
		/// <exception cref="COMException">IPersistFileインターフェイスを取得できませんでした。</exception>
		public void Save()
		{
			this.Save(currentFile);
		}

		/// <summary>
		/// 指定したファイルにショートカットを保存します。
		/// </summary>
		/// <param name="linkFile">ショートカットを保存するファイル</param>
		/// <exception cref="COMException">IPersistFileインターフェイスを取得できませんでした。</exception>
		public void Save(string linkFile){
			// IPersistFileインターフェイスを取得して保存
			ComTypes.IPersistFile persistFile = GetIPersistFile();
			if(persistFile == null) throw new COMException("IPersistFileインターフェイスを取得できませんでした。");
			persistFile.Save(linkFile, true);
			
			// カレントファイルを保存
			this.currentFile = linkFile;
		}

		/// <summary>
		/// 指定したファイルからショートカットを読み込みます。
		/// </summary>
		/// <param name="linkFile">ショートカットを読み込むファイル</param>
		/// <exception cref="FileNotFoundException">ファイルが見つかりません。</exception>
		/// <exception cref="COMException">IPersistFileインターフェイスを取得できませんでした。</exception>
		public void Load(string linkFile){
			this.Load(linkFile, IntPtr.Zero, ShellLinkResolveFlags.SLR_ANY_MATCH | ShellLinkResolveFlags.SLR_NO_UI, 1);
		}

		/// <summary>
		/// 指定したファイルからショートカットを読み込みます。
		/// </summary>
		/// <param name="linkFile">ショートカットを読み込むファイル</param>
		/// <param name="hWnd">このコードを呼び出したオーナーのウィンドウハンドル</param>
		/// <param name="resolveFlags">ショートカット情報の解決に関する動作を表すフラグ</param>
		/// <exception cref="FileNotFoundException">ファイルが見つかりません。</exception>
		/// <exception cref="COMException">IPersistFileインターフェイスを取得できませんでした。</exception>
		public void Load(string linkFile, IntPtr hWnd, ShellLinkResolveFlags resolveFlags){
			this.Load(linkFile, hWnd, resolveFlags, 1);
		}

		/// <summary>
		/// 指定したファイルからショートカットを読み込みます。
		/// </summary>
		/// <param name="linkFile">ショートカットを読み込むファイル</param>
		/// <param name="hWnd">このコードを呼び出したオーナーのウィンドウハンドル</param>
		/// <param name="resolveFlags">ショートカット情報の解決に関する動作を表すフラグ</param>
		/// <param name="timeOut">SLR_NO_UIを指定したときのタイムアウト値(ミリ秒)</param>
		/// <exception cref="FileNotFoundException">ファイルが見つかりません。</exception>
		/// <exception cref="COMException">IPersistFileインターフェイスを取得できませんでした。</exception>
		public void Load(string linkFile, IntPtr hWnd, ShellLinkResolveFlags resolveFlags, TimeSpan timeOut){
			this.Load(linkFile, hWnd, resolveFlags, (int)timeOut.TotalMilliseconds);
		}
		
		/// <summary>
		/// 指定したファイルからショートカットを読み込みます。
		/// </summary>
		/// <param name="linkFile">ショートカットを読み込むファイル</param>
		/// <param name="hWnd">このコードを呼び出したオーナーのウィンドウハンドル</param>
		/// <param name="resolveFlags">ショートカット情報の解決に関する動作を表すフラグ</param>
		/// <param name="timeOutMilliseconds">SLR_NO_UIを指定したときのタイムアウト値(ミリ秒)</param>
		/// <exception cref="FileNotFoundException">ファイルが見つかりません。</exception>
		/// <exception cref="COMException">IPersistFileインターフェイスを取得できませんでした。</exception>
		public void Load(string linkFile, IntPtr hWnd, ShellLinkResolveFlags resolveFlags, int timeOutMilliseconds){
			if(!File.Exists(linkFile)) throw new FileNotFoundException("ファイルが見つかりません。", linkFile);

			// IPersistFileインターフェイスを取得
			ComTypes.IPersistFile persistFile = GetIPersistFile();

			if(persistFile == null) throw new COMException("IPersistFileインターフェイスを取得できませんでした。");

			// 読み込み
			persistFile.Load(linkFile, 0x00000000);

			// フラグを処理
			uint flags = (uint)resolveFlags;

			if((resolveFlags & ShellLinkResolveFlags.SLR_NO_UI) == ShellLinkResolveFlags.SLR_NO_UI){
				flags |= (uint)(timeOutMilliseconds << 16);
			}

			this.shellLink.Resolve(hWnd, flags);

			// カレントファイルを指定
			currentFile = linkFile;
		}
		#endregion
	}

	[ComImport]
	[Guid("00021401-0000-0000-C000-000000000046")]
	[ClassInterface(ClassInterfaceType.None)]
	public class ShellLinkObject{
	}
	
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("000214F9-0000-0000-C000-000000000046")]
	public interface IShellLink{
		void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cch, [MarshalAs(UnmanagedType.Struct)] ref ShellLinkFindData pfd, uint fFlags);
		void GetIDList(out IntPtr ppidl);
		void SetIDList(IntPtr pidl);
		void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cch);
		void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
		void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cch);
		void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
		void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cch);
		void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
		void GetHotkey(out ushort pwHotkey);
		void SetHotkey(ushort wHotkey);
		void GetShowCmd(out int piShowCmd);
		void SetShowCmd(int iShowCmd);
		void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cch, out int piIcon);
		void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
		void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, uint dwReserved);
		void Resolve(IntPtr hwnd, uint flags);
		void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
	}
	
	[StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
	public struct ShellLinkFindData{
		public const int MAX_PATH = 260;
		public uint dwFileAttributes;
		public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
		public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
		public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
		public uint nFileSizeHigh;
		public uint nFileSizeLow;
		public uint dwReserved0;
		public uint dwReserved1;
		
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
		public string cFileName;
		
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
		public string cAlternateFileName;
	}
	
	/// <summary>
	/// 実行時のウィンドウの表示方法を表す列挙型です。
	/// </summary>
	public enum ShellLinkDisplayMode : int{
		/// <summary>通常の大きさのウィンドウで起動します。</summary>
		Normal = 1,
		/// <summary>最大化された状態で起動します。</summary>
		Maximized = 3,
		/// <summary>最小化された状態で起動します。</summary>
		Minimized = 7,
	}
	
	/// <summary></summary>
	[Flags]
	public enum ShellLinkResolveFlags : int{
		/// <summary></summary>
		SLR_ANY_MATCH = 0x2,
		/// <summary></summary>
		SLR_INVOKE_MSI = 0x80,
		/// <summary></summary>
		SLR_NOLINKINFO = 0x40,
		/// <summary></summary>
		SLR_NO_UI = 0x1,
		/// <summary></summary>
		SLR_NO_UI_WITH_MSG_PUMP = 0x101,
		/// <summary></summary>
		SLR_NOUPDATE = 0x8,
		/// <summary></summary>
		SLR_NOSEARCH = 0x10,
		/// <summary></summary>
		SLR_NOTRACK = 0x20,
		/// <summary></summary>
		SLR_UPDATE  = 0x4
	}
}