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
	public class GrepResult : ResultBase{
		public ObservableCollection<GrepMatch> Matches{get; private set;}
		
		public GrepResult(SearchCondition cond) : base(cond){
			this.Matches = new ObservableCollection<GrepMatch>();
		}
	}
}