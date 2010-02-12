/*
	$Id$
*/
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using Nekome.Search;

namespace Nekome{
	public class SearchCondition{
		public string Path{get; set;}
		public string Mask{get; set;}
		public Regex Regex{get; set;}
		public SearchOption SearchOption{get; set;}
		
		public SearchCondition(){
		}
	}
}