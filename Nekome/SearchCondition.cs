/*
	$Id$
*/
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Windows;
using System.Linq;
using CatWalk;
using Nekome.Search;

namespace Nekome{
	using IO = System.IO;

	[Serializable]
	public class SearchCondition : DependencyObject, ICloneable{
		public static readonly DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(SearchCondition));
		public string Path{
			get{
				return (string)this.GetValue(PathProperty);
			}
			set{
				this.SetValue(PathProperty, value);
			}
		}

		public static readonly DependencyProperty MaskProperty = DependencyProperty.Register("Mask", typeof(string), typeof(SearchCondition));
		public string Mask{
			get{
				return (string)this.GetValue(MaskProperty);
			}
			set{
				this.SetValue(MaskProperty, value);
			}
		}

		public static readonly DependencyProperty ExcludingMaskProperty = DependencyProperty.Register("ExcludingMask", typeof(string), typeof(SearchCondition));
		public string ExcludingMask{
			get{
				return (string)this.GetValue(ExcludingMaskProperty);
			}
			set{
				this.SetValue(ExcludingMaskProperty, value);
			}
		}

		public static readonly DependencyProperty PatternProperty = DependencyProperty.Register("Pattern", typeof(string), typeof(SearchCondition));
		public string Pattern{
			get{
				return (string)this.GetValue(PatternProperty);
			}
			set{
				this.SetValue(PatternProperty, value);
			}
		}

		public static readonly DependencyProperty IsIgnoreCaseProperty = DependencyProperty.Register("IsIgnoreCase", typeof(bool), typeof(SearchCondition));
		public bool IsIgnoreCase{
			get{
				return (bool)this.GetValue(IsIgnoreCaseProperty);
			}
			set{
				this.SetValue(IsIgnoreCaseProperty, value);
			}
		}

		public static readonly DependencyProperty IsUseRegexProperty = DependencyProperty.Register("IsUseRegex", typeof(bool), typeof(SearchCondition));
		public bool IsUseRegex{
			get{
				return (bool)this.GetValue(IsUseRegexProperty);
			}
			set{
				this.SetValue(IsUseRegexProperty, value);
			}
		}

		public static readonly DependencyProperty FileSearchOptionProperty = DependencyProperty.Register("FileSearchOption", typeof(IO::SearchOption), typeof(SearchCondition));
		public IO::SearchOption FileSearchOption{
			get{
				return (IO::SearchOption)this.GetValue(FileSearchOptionProperty);
			}
			set{
				this.SetValue(FileSearchOptionProperty, value);
			}
		}

		public static readonly DependencyProperty ExcludingTargetsProperty = DependencyProperty.Register("ExcludingTargets", typeof(ExcludingTargets), typeof(SearchCondition));
		public ExcludingTargets ExcludingTargets{
			get{
				return (ExcludingTargets)this.GetValue(ExcludingTargetsProperty);
			}
			set{
				this.SetValue(ExcludingTargetsProperty, value);
			}
		}

		public static readonly DependencyProperty FileSizeRangeProperty = DependencyProperty.Register("FileSizeRange", typeof(Range<decimal>), typeof(SearchCondition));
		public Range<decimal> FileSizeRange{
			get{
				return (Range<decimal>)this.GetValue(FileSizeRangeProperty);
			}
			set{
				this.SetValue(FileSizeRangeProperty, value);
			}
		}
		/*
		public Range<DateTime> FileModifiedDateRange{get; set;}
		public Range<DateTime> FileCreatedDateRange{get; set;}
		*/

		public SearchCondition(){
		}

		public static SearchCondition GetDefaultCondition(){
			var cond = new SearchCondition();
			cond.Pattern = Program.Settings.SearchWordHistory.EmptyIfNull()
				.Concat(Seq.Make("")).First();
			cond.Path = Program.Settings.DirectoryHistory.EmptyIfNull()
				.Concat(Seq.Make(Environment.CurrentDirectory)).First();
			cond.Mask = Program.Settings.FileMaskHistory.EmptyIfNull()
				.Concat(Seq.Make("*.*")).First();
			cond.IsIgnoreCase = Program.Settings.IsIgnoreCase;
			cond.IsUseRegex = Program.Settings.IsUseRegex;
			cond.FileSearchOption = Program.Settings.FileSearchOption;
			cond.ExcludingMask = Program.Settings.ExcludingMaskHistory.EmptyIfNull()
				.Concat(Seq.Make("*.exe;*.dll;*.bmp;*.jpg;*.png;*.gif;*.avi;*.wmv;*.mpg;*.mp3;*.wav;*.ogg")).First();
			cond.ExcludingTargets = Program.Settings.ExcludingTargets;
			cond.FileSizeRange = Program.Settings.FileSizeRange;
			return cond;
		}

		public Regex GetRegex(){
			var regexOptions = this.IsIgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
			var regex = new Regex((this.IsUseRegex) ? this.Pattern : Regex.Escape(this.Pattern), regexOptions);
			return regex;
		}

		public object Clone(){
			var cond = new SearchCondition();
			cond.Path = this.Path;
			cond.Mask = this.Mask;
			cond.IsIgnoreCase = this.IsIgnoreCase;
			cond.IsUseRegex = this.IsUseRegex;
			cond.FileSearchOption = this.FileSearchOption;
			cond.ExcludingMask = this.ExcludingMask;
			cond.ExcludingTargets = this.ExcludingTargets;
			cond.FileSizeRange = this.FileSizeRange;
			return cond;
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