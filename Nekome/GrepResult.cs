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
	public class GrepResult{
		public string Path{get; private set;}
		public string Mask{get; private set;}
		public Regex Regex{get; private set;}
		public SearchOption SearchOption{get; private set;}
		public ObservableCollection<GrepMatch> Matches{get; private set;}
		
		public GrepResult(string path, string mask, SearchOption option, Regex regex){
			this.Path = path;
			this.Mask = mask;
			this.Regex = regex;
			this.SearchOption = option;
			this.Matches = new ObservableCollection<GrepMatch>();
		}
	}
}