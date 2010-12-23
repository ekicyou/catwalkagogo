/*
	$Id$
*/
using System;

namespace CatWalk.Windows{
	using WinForms = System.Windows.Forms;
	using Win32 = Microsoft.Win32;
	public class FolderBrowserDialog : Win32::CommonDialog{
		public string Description{get; set;}
		public Environment.SpecialFolder RootFolder{get; set;}
		public string SelectedPath{get; set;}
		public bool ShowNewFolderButton{get; set;}
		
		public FolderBrowserDialog(){
			this.Reset();
		}
		
		public override void Reset(){
			this.Description = "";
			this.RootFolder = Environment.SpecialFolder.Desktop;
			this.SelectedPath = Environment.CurrentDirectory;
			this.ShowNewFolderButton = true;
		}
		
		private class Win32Window : WinForms::IWin32Window{
			public IntPtr Handle{get; private set;}
			
			public Win32Window (IntPtr handle){
				this.Handle = handle;
			}
		}
		
		protected override bool RunDialog(IntPtr hwndOwner){
			using(var fbd = new WinForms::FolderBrowserDialog()){
				fbd.Description = Description;
				fbd.RootFolder = RootFolder;
				fbd.SelectedPath = SelectedPath;
				fbd.ShowNewFolderButton = ShowNewFolderButton;
				
				if(fbd.ShowDialog(new Win32Window(hwndOwner)) != WinForms::DialogResult.OK){
					return false;
				}
				
				SelectedPath = fbd.SelectedPath;
				return true;
			}
		}
	}
}