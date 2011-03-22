/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GFV.ViewModel{
	public interface IDialog{
		Nullable<bool> ShowDialog();
	}

	public interface IFileDialog : IDialog{
		string FileName{get;}
		string[] FileNames{get;}
		IList<FileDialogFilter> Filters{get;}
	}

	public interface IOpenFileDialog : IFileDialog{}
	public interface ISaveFileDialog : IFileDialog{}

	public struct FileDialogFilter{
		public string Name{get; private set;}
		public string Mask{get; private set;}

		public FileDialogFilter(string name, string mask) : this(){
			this.Name = name;
			this.Mask = mask;
		}
	}
}
