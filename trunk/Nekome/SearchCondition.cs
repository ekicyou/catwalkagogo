/*
	$Id$
*/
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Linq;
using CatWalk;
using Nekome.Search;

namespace Nekome{
	[Serializable]
	public class SearchCondition{
		public string Path{get; set;}
		public string Mask{get; set;}
		public string Pattern{get; set;}
		public bool IsIgnoreCase{get; set;}
		public bool IsUseRegex{get; set;}
		public SearchOption SearchOption{get; set;}
		public ExcludingTargets ExcludingTargets{get; set;}
		public string ExcludingMask{get; set;}
		public Range<ulong> FileSizeRange{get; set;}
		public Range<DateTime> FileModifiedDateRange{get; set;}
		public Range<DateTime> FileCreatedDateRange{get; set;}
		
		public SearchCondition(){
		}

		public static SearchCondition GetDefaultCondition(){
			var cond = new SearchCondition();
			cond.Path = Program.Settings.DirectoryHistory.EmptyIfNull()
				.Concat(new string[]{Environment.CurrentDirectory}).First();
			cond.Mask = Program.Settings.FileMaskHistory.EmptyIfNull()
				.Concat(new string[]{"*.*"}).First();
			cond.IsIgnoreCase = Program.Settings.IsIgnoreCase;
			cond.IsUseRegex = Program.Settings.IsUseRegex;
			cond.SearchOption = Program.Settings.SearchOption;
			cond.ExcludingMask = Program.Settings.ExcludingMask;
			cond.ExcludingTargets = Program.Settings.ExcludingTargets;
			return cond;
		}

		public Regex GetRegex(){
			var regexOptions = this.IsIgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
			var regex = new Regex((this.IsUseRegex) ? this.Pattern : Regex.Escape(this.Pattern), regexOptions);
			return regex;
		}
	}

	[Flags]
	public enum ExcludingTargets{
		None = 0x00,
		Search = 0x01,
		Grep = 0x02,
		All = Search | Grep,
	}
}