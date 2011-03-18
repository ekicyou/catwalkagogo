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
	}

	public interface IOpenFileDialog : IFileDialog{}
	public interface ISaveFileDialog : IFileDialog{}
}
