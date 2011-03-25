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
		void Reset();
		string FileName{get; set;}
		string[] FileNames{get;}
		IList<FileDialogFilter> Filters{get;}
		bool IsCheckFileExists{get; set;}
		bool IsCheckPathExists{get; set;}
		bool IsAddExtension{get; set;}
		string DefaultExtension{get; set;}
		int FilterIndex{get; set;}
		string InitialDirectory{get; set;}
		bool IsValidNames{get; set;}
	}

	public interface IOpenFileDialog : IFileDialog{
		bool IsMultiselect{get; set;}
	}
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
