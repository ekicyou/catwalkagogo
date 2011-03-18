/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace GFV.Windows{
	using VM = GFV.ViewModel;
	using Win32 = Microsoft.Win32;

	public class OpenFileDialog : VM.IOpenFileDialog{
		public Win32::OpenFileDialog Dialog{get; private set;}
		public Window Owner{get; set;}

		public OpenFileDialog(){
			this.Dialog = new Win32::OpenFileDialog();
		}

		public OpenFileDialog(Window owner){
			this.Dialog = new Win32::OpenFileDialog();
			this.Owner = owner;
		}


		public bool? ShowDialog(){
			return this.Dialog.ShowDialog(this.Owner);
		}

		public string FileName{
			get{
				return this.Dialog.FileName;
			}
		}

		public string[] FileNames{
			get{
				return this.FileNames;
			}
		}
	}

	public class SaveFileDialog : VM.ISaveFileDialog{
		public Win32::SaveFileDialog Dialog{get; private set;}
		public Window Owner{get; set;}

		public SaveFileDialog(){
			this.Dialog = new Win32::SaveFileDialog();
		}

		public SaveFileDialog(Window owner){
			this.Dialog = new Win32::SaveFileDialog();
			this.Owner = owner;
		}


		public bool? ShowDialog(){
			return this.Dialog.ShowDialog(this.Owner);
		}

		public string FileName{
			get{
				return this.Dialog.FileName;
			}
		}

		public string[] FileNames{
			get{
				return this.FileNames;
			}
		}
	}
}
