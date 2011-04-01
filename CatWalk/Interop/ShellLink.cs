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
		// IShellLink�C���^�[�t�F�C�X
		private IShellLink shellLink;
		
		// �J�����g�t�@�C��
		private string currentFile;
		
		// �e��萔
		internal const int MAX_PATH = 260;
		
		internal const uint SLGP_SHORTPATH   = 0x0001; // �Z���`��(8.3�`��)�̃t�@�C�������擾����
		internal const uint SLGP_UNCPRIORITY = 0x0002; // UNC�p�X�����擾����
		internal const uint SLGP_RAWPATH	 = 0x0004; // ���ϐ��Ȃǂ��ϊ�����Ă��Ȃ��p�X�����擾����
		
		#region "�R���X�g���N�V�����E�f�X�g���N�V����"
		public ShellLink(){
			this.currentFile = "";
			this.shellLink = null;
			
			try{
				// Unicode��
				this.shellLink = (IShellLink)(new ShellLinkObject());
			}catch(Exception ex){
				throw new COMException("Can't get a IShellLink object.", ex);
			}
		}

		/// <summary>
		/// �R���X�g���N�^
		/// </summary>
		/// <param name="linkFile">�V���[�g�J�b�g�t�@�C��</param>
		public ShellLink(string linkFile) : this(){
			this.Load(linkFile);
		}

		/// <summary>
		/// �f�X�g���N�^
		/// </summary>
		~ShellLink(){
			Dispose();
		}

		/// <summary>
		/// ���̃C���X�^���X���g�p���Ă��郊�\�[�X��������܂��B
		/// </summary>
		public void Dispose(){
			if(this.shellLink != null){
				Marshal.ReleaseComObject(this.shellLink);
				this.shellLink = null;
				GC.SuppressFinalize(this);
			}
		}

		#endregion

		#region "�v���p�e�B"

		/// <summary>
		/// �J�����g�t�@�C���B
		/// </summary>
		public string CurrentFile{
			get{
				return currentFile;
			}
		}

		/// <summary>
		/// �V���[�g�J�b�g�̃����N��B
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
		/// ��ƃf�B���N�g���B
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
		/// �R�}���h���C�������B
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
		/// �V���[�g�J�b�g�̐����B
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
		/// �A�C�R���̃t�@�C���B
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
		/// �A�C�R���̃C���f�b�N�X�B
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
		/// �A�C�R���̃t�@�C���ƃC���f�b�N�X���擾����
		/// </summary>
		/// <param name="iconFile">�A�C�R���̃t�@�C��</param>
		/// <param name="iconIndex">�A�C�R���̃C���f�b�N�X</param>
		private void GetIconLocation(out string iconFile, out int iconIndex){
			StringBuilder iconFileBuffer = new StringBuilder(MAX_PATH, MAX_PATH);
			this.shellLink.GetIconLocation(iconFileBuffer, iconFileBuffer.Capacity, out iconIndex);
			iconFile = iconFileBuffer.ToString();
		}

		/// <summary>
		/// �A�C�R���̃t�@�C���ƃC���f�b�N�X��ݒ肷��
		/// </summary>
		/// <param name="iconFile">�A�C�R���̃t�@�C��</param>
		/// <param name="iconIndex">�A�C�R���̃C���f�b�N�X</param>
		private void SetIconLocation(string iconFile, int iconIndex){
			this.shellLink.SetIconLocation(iconFile, iconIndex);
		}

		/// <summary>
		/// ���s���̃E�B���h�E�̑傫���B
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
		/// �z�b�g�L�[�B
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

		#region "�ۑ��Ɠǂݍ���"

		/// <summary>
		/// IShellLink�C���^�[�t�F�C�X����L���X�g���ꂽIPersistFile�C���^�[�t�F�C�X���擾���܂��B
		/// </summary>
		/// <returns>IPersistFile�C���^�[�t�F�C�X�B�@�擾�ł��Ȃ������ꍇ��null�B</returns>
		private ComTypes.IPersistFile GetIPersistFile(){
			return this.shellLink as ComTypes.IPersistFile;
		}

		/// <summary>
		/// �J�����g�t�@�C���ɃV���[�g�J�b�g��ۑ����܂��B
		/// </summary>
		/// <exception cref="COMException">IPersistFile�C���^�[�t�F�C�X���擾�ł��܂���ł����B</exception>
		public void Save()
		{
			this.Save(currentFile);
		}

		/// <summary>
		/// �w�肵���t�@�C���ɃV���[�g�J�b�g��ۑ����܂��B
		/// </summary>
		/// <param name="linkFile">�V���[�g�J�b�g��ۑ�����t�@�C��</param>
		/// <exception cref="COMException">IPersistFile�C���^�[�t�F�C�X���擾�ł��܂���ł����B</exception>
		public void Save(string linkFile){
			// IPersistFile�C���^�[�t�F�C�X���擾���ĕۑ�
			ComTypes.IPersistFile persistFile = GetIPersistFile();
			if(persistFile == null) throw new COMException("IPersistFile�C���^�[�t�F�C�X���擾�ł��܂���ł����B");
			persistFile.Save(linkFile, true);
			
			// �J�����g�t�@�C����ۑ�
			this.currentFile = linkFile;
		}

		/// <summary>
		/// �w�肵���t�@�C������V���[�g�J�b�g��ǂݍ��݂܂��B
		/// </summary>
		/// <param name="linkFile">�V���[�g�J�b�g��ǂݍ��ރt�@�C��</param>
		/// <exception cref="FileNotFoundException">�t�@�C����������܂���B</exception>
		/// <exception cref="COMException">IPersistFile�C���^�[�t�F�C�X���擾�ł��܂���ł����B</exception>
		public void Load(string linkFile){
			this.Load(linkFile, IntPtr.Zero, ShellLinkResolveFlags.SLR_ANY_MATCH | ShellLinkResolveFlags.SLR_NO_UI, 1);
		}

		/// <summary>
		/// �w�肵���t�@�C������V���[�g�J�b�g��ǂݍ��݂܂��B
		/// </summary>
		/// <param name="linkFile">�V���[�g�J�b�g��ǂݍ��ރt�@�C��</param>
		/// <param name="hWnd">���̃R�[�h���Ăяo�����I�[�i�[�̃E�B���h�E�n���h��</param>
		/// <param name="resolveFlags">�V���[�g�J�b�g���̉����Ɋւ��铮���\���t���O</param>
		/// <exception cref="FileNotFoundException">�t�@�C����������܂���B</exception>
		/// <exception cref="COMException">IPersistFile�C���^�[�t�F�C�X���擾�ł��܂���ł����B</exception>
		public void Load(string linkFile, IntPtr hWnd, ShellLinkResolveFlags resolveFlags){
			this.Load(linkFile, hWnd, resolveFlags, 1);
		}

		/// <summary>
		/// �w�肵���t�@�C������V���[�g�J�b�g��ǂݍ��݂܂��B
		/// </summary>
		/// <param name="linkFile">�V���[�g�J�b�g��ǂݍ��ރt�@�C��</param>
		/// <param name="hWnd">���̃R�[�h���Ăяo�����I�[�i�[�̃E�B���h�E�n���h��</param>
		/// <param name="resolveFlags">�V���[�g�J�b�g���̉����Ɋւ��铮���\���t���O</param>
		/// <param name="timeOut">SLR_NO_UI���w�肵���Ƃ��̃^�C���A�E�g�l(�~���b)</param>
		/// <exception cref="FileNotFoundException">�t�@�C����������܂���B</exception>
		/// <exception cref="COMException">IPersistFile�C���^�[�t�F�C�X���擾�ł��܂���ł����B</exception>
		public void Load(string linkFile, IntPtr hWnd, ShellLinkResolveFlags resolveFlags, TimeSpan timeOut){
			this.Load(linkFile, hWnd, resolveFlags, (int)timeOut.TotalMilliseconds);
		}
		
		/// <summary>
		/// �w�肵���t�@�C������V���[�g�J�b�g��ǂݍ��݂܂��B
		/// </summary>
		/// <param name="linkFile">�V���[�g�J�b�g��ǂݍ��ރt�@�C��</param>
		/// <param name="hWnd">���̃R�[�h���Ăяo�����I�[�i�[�̃E�B���h�E�n���h��</param>
		/// <param name="resolveFlags">�V���[�g�J�b�g���̉����Ɋւ��铮���\���t���O</param>
		/// <param name="timeOutMilliseconds">SLR_NO_UI���w�肵���Ƃ��̃^�C���A�E�g�l(�~���b)</param>
		/// <exception cref="FileNotFoundException">�t�@�C����������܂���B</exception>
		/// <exception cref="COMException">IPersistFile�C���^�[�t�F�C�X���擾�ł��܂���ł����B</exception>
		public void Load(string linkFile, IntPtr hWnd, ShellLinkResolveFlags resolveFlags, int timeOutMilliseconds){
			if(!File.Exists(linkFile)) throw new FileNotFoundException("�t�@�C����������܂���B", linkFile);

			// IPersistFile�C���^�[�t�F�C�X���擾
			ComTypes.IPersistFile persistFile = GetIPersistFile();

			if(persistFile == null) throw new COMException("IPersistFile�C���^�[�t�F�C�X���擾�ł��܂���ł����B");

			// �ǂݍ���
			persistFile.Load(linkFile, 0x00000000);

			// �t���O������
			uint flags = (uint)resolveFlags;

			if((resolveFlags & ShellLinkResolveFlags.SLR_NO_UI) == ShellLinkResolveFlags.SLR_NO_UI){
				flags |= (uint)(timeOutMilliseconds << 16);
			}

			this.shellLink.Resolve(hWnd, flags);

			// �J�����g�t�@�C�����w��
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
	/// ���s���̃E�B���h�E�̕\�����@��\���񋓌^�ł��B
	/// </summary>
	public enum ShellLinkDisplayMode : int{
		/// <summary>�ʏ�̑傫���̃E�B���h�E�ŋN�����܂��B</summary>
		Normal = 1,
		/// <summary>�ő剻���ꂽ��ԂŋN�����܂��B</summary>
		Maximized = 3,
		/// <summary>�ŏ������ꂽ��ԂŋN�����܂��B</summary>
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