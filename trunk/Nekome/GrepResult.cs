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
		public SearchCondition SearchCondition{get; private set;}
		public ObservableCollection<GrepMatch> Matches{get; private set;}
		
		public GrepResult(SearchCondition cond){
			this.SearchCondition = cond;
			this.Matches = new ObservableCollection<GrepMatch>();
		}
	}
}