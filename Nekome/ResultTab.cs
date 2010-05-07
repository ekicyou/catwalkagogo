/*
	$Id$
*/
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;
using Nekome.Search;

namespace Nekome{
	public class ResultTab{
		public object Result{get; set;}
		public FileListWorker Worker{get; set;}
		
		public ResultTab(object result, FileListWorker worker){
			this.Result = result;
			this.Worker = worker;
		}
	}
}