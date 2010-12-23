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
	public class FindResult : ResultBase{
		public ObservableCollection<string> Files{get; private set;}
		
		public FindResult(SearchCondition cond) : base(cond){
			this.Files = new ObservableCollection<string>();
		}
	}
}