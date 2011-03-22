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

	public abstract class FileDialog : VM.IFileDialog{
		public Win32::FileDialog Dialog{get; protected set;}
		public IList<VM::FileDialogFilter> Filters{get; private set;}
		public Window Owner{get; set;}

		public FileDialog() : this(null){}
		public FileDialog(Window owner){
			this.Owner = owner;
			this.Filters = new List<VM::FileDialogFilter>();
		}

		public virtual bool? ShowDialog(){
			this.Dialog.Filter = this.GetFilterString();
			return this.Dialog.ShowDialog(this.Owner);
		}

		public virtual string FileName{
			get{
				return this.Dialog.FileName;
			}
		}

		public string[] FileNames{
			get{
				return this.Dialog.FileNames;
			}
		}

		protected virtual string GetFilterString(){
			return String.Join("|", this.Filters.Select(filter => filter.Name + "|" + filter.Mask));
		}
	}

	public class OpenFileDialog : FileDialog, VM::IOpenFileDialog{
		public OpenFileDialog() : this(null){}
		public OpenFileDialog(Window owner) : base(owner){
			this.Dialog = new Win32::OpenFileDialog();
		}
	}

	public class SaveFileDialog : FileDialog, VM::ISaveFileDialog{
		public SaveFileDialog() : this(null){}
		public SaveFileDialog(Window owner) : base(owner){
			this.Dialog = new Win32::SaveFileDialog();
			this.Owner = owner;
		}
	}
}
