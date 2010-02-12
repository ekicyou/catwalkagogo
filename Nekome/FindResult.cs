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
	public class FindResult{
		public string Path{get; private set;}
		public string Mask{get; private set;}
		public SearchOption SearchOption{get; private set;}
		public ObservableCollection<string> Files{get; private set;}
		
		public FindResult(string path, string mask, SearchOption option){
			this.Path = path;
			this.Mask = mask;
			this.SearchOption = option;
			this.Files = new ObservableCollection<string>();
		}
	}
}