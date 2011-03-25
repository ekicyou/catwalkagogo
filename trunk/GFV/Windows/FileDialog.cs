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

	#region FileDialog

	public abstract class FileDialog : VM.IFileDialog{
		protected Win32::FileDialog Dialog{get; set;}
		public IList<VM::FileDialogFilter> Filters{get; private set;}
		public Window Owner{get; set;}

		public FileDialog() : this(null){}
		public FileDialog(Window owner){
			this.Owner = owner;
			this.Filters = new List<VM::FileDialogFilter>();
		}

		public virtual void Reset(){
			this.Filters.Clear();
		}

		public virtual bool? ShowDialog(){
			this.Dialog.Filter = this.GetFilterString();
			return this.Dialog.ShowDialog(this.Owner);
		}

		public virtual string FileName{
			get{
				return this.Dialog.FileName;
			}
			set{
				this.Dialog.FileName = value;
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

		public bool IsCheckFileExists{
			get{
				return this.Dialog.CheckFileExists;
			}
			set{
				this.Dialog.CheckFileExists = value;
			}
		}

		public bool IsCheckPathExists{
			get{
				return this.Dialog.CheckPathExists;
			}
			set{
				this.Dialog.CheckPathExists = value;
			}
		}

		public bool IsAddExtension{
			get{
				return this.Dialog.AddExtension;
			}
			set{
				this.Dialog.AddExtension = value;
			}
		}

		public string DefaultExtension{
			get{
				return this.Dialog.DefaultExt;
			}
			set{
				this.Dialog.DefaultExt = value;
			}
		}

		public int FilterIndex{
			get{
				return this.Dialog.FilterIndex;
			}
			set{
				this.Dialog.FilterIndex = value;
			}
		}

		public string InitialDirectory{
			get{
				return this.Dialog.InitialDirectory;
			}
			set{
				this.Dialog.InitialDirectory = value;
			}
		}

		public bool IsValidNames{
			get{
				return this.Dialog.ValidateNames;
			}
			set{
				this.Dialog.ValidateNames = value;
			}
		}
	}

	#endregion

	#region OpenFileDialog

	public class OpenFileDialog : FileDialog, VM::IOpenFileDialog{
		public OpenFileDialog() : this(null){}
		public OpenFileDialog(Window owner) : base(owner){
			this.Dialog = new Win32::OpenFileDialog();
		}

		public override void Reset(){
			base.Reset();
			this.Dialog = new Win32::OpenFileDialog();
		}

		public bool IsMultiselect{
			get{
				return ((Win32::OpenFileDialog)this.Dialog).Multiselect;
			}
			set{
				((Win32::OpenFileDialog)this.Dialog).Multiselect = value;
			}
		}
	}

	#endregion

	#region SaveFileDialog

	public class SaveFileDialog : FileDialog, VM::ISaveFileDialog{
		public SaveFileDialog() : this(null){}
		public SaveFileDialog(Window owner) : base(owner){
			this.Dialog = new Win32::SaveFileDialog();
			this.Owner = owner;
		}

		public override void Reset(){
			base.Reset();
			this.Dialog = new Win32::OpenFileDialog();
		}
	}

	#endregion
}
