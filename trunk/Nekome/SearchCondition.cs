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

		public static readonly DependencyProperty IsEnableAdvancedGrepConditionProperty = DependencyProperty.Register("IsEnableAdvancedGrepCondition", typeof(bool), typeof(SearchCondition));
		public bool IsEnableAdvancedGrepCondition{
			get{
				return (bool)this.GetValue(IsEnableAdvancedGrepConditionProperty);
			}
			set{
				this.SetValue(IsEnableAdvancedGrepConditionProperty, value);
			}
		}

		public static readonly DependencyProperty IsEnableAdvancedFindConditionProperty = DependencyProperty.Register("IsEnableAdvancedFindCondition", typeof(bool), typeof(SearchCondition));
		public bool IsEnableAdvancedFindCondition{
			get{
				return (bool)this.GetValue(IsEnableAdvancedFindConditionProperty);
			}
			set{
				this.SetValue(IsEnableAdvancedFindConditionProperty, value);
			}
		}

		public AdvancedSearchCondition AdvancedGrepCondition{get; private set;}
		public AdvancedSearchCondition AdvancedFindCondition{get; private set;}

		public SearchCondition(){
			this.AdvancedFindCondition = new AdvancedSearchCondition();
			this.AdvancedGrepCondition = new AdvancedSearchCondition();
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
			cond.IsEnableAdvancedGrepCondition = Program.Settings.IsEnableAdvancedGrepCondition;
			cond.IsEnableAdvancedFindCondition = Program.Settings.IsEnableAdvancedFindCondition;
			cond.AdvancedFindCondition = AdvancedSearchCondition.GetDefaultFindCondition();
			cond.AdvancedGrepCondition = AdvancedSearchCondition.GetDefaultGrepCondition();
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
			cond.IsEnableAdvancedGrepCondition = this.IsEnableAdvancedGrepCondition;
			cond.IsEnableAdvancedFindCondition = this.IsEnableAdvancedFindCondition;
			cond.AdvancedGrepCondition = (AdvancedSearchCondition)this.AdvancedGrepCondition.Clone();
			cond.AdvancedFindCondition = (AdvancedSearchCondition)this.AdvancedFindCondition.Clone();
			return cond;
		}
	}

	public class AdvancedSearchCondition : DependencyObject{
		public AdvancedSearchCondition(){
			this.FileSizeRange = new Range<long>(0, Int64.MaxValue, false, false);
		}

		public object Clone(){
			var cond = new AdvancedSearchCondition();
			cond.FileSizeRange = this.FileSizeRange;
			cond.ExcludingMask = this.ExcludingMask;
			return cond;
		}

		public static readonly DependencyProperty FileSizeRangeProperty = DependencyProperty.Register("FileSizeRange", typeof(Range<long>), typeof(AdvancedSearchCondition));
		public Range<long> FileSizeRange{
			get{
				return (Range<long>)this.GetValue(FileSizeRangeProperty);
			}
			set{
				this.SetValue(FileSizeRangeProperty, value);
			}
		}

		public static readonly DependencyProperty ExcludingMaskProperty = DependencyProperty.Register("ExcludingMask", typeof(string), typeof(AdvancedSearchCondition));
		public string ExcludingMask{
			get{
				return (string)this.GetValue(ExcludingMaskProperty);
			}
			set{
				this.SetValue(ExcludingMaskProperty, value);
			}
		}
		
		public static AdvancedSearchCondition GetDefaultGrepCondition(){
			var cond = new AdvancedSearchCondition();
			cond.ExcludingMask = Program.Settings.GrepExcludingMaskHistory.EmptyIfNull()
				.Concat(Seq.Make("*.exe;*.dll;*.bmp;*.jpg;*.png;*.gif;*.avi;*.wmv;*.mpg;*.mp3;*.wav;*.ogg")).First();
			cond.FileSizeRange = Program.Settings.GrepFileSizeRange;
			return cond;
		}

		public static readonly DependencyProperty FileAttributesProperty = DependencyProperty.Register("FileAttributes", typeof(IO.FileAttributes), typeof(AdvancedSearchCondition));
		public IO.FileAttributes FileAttributes{
			get{
				return (IO.FileAttributes)this.GetValue(FileAttributesProperty);
			}
			set{
				this.SetValue(FileAttributesProperty, value);
			}
		}

		public static AdvancedSearchCondition GetDefaultFindCondition(){
			var cond = new AdvancedSearchCondition();
			cond.ExcludingMask = Program.Settings.FindExcludingMaskHistory.EmptyIfNull()
				.Concat(Seq.Make("")).First();
			cond.FileSizeRange = Program.Settings.FindFileSizeRange;
			cond.FileAttributes = IO.FileAttributes.Archive | IO.FileAttributes.Compressed | IO.FileAttributes.Device |
				IO.FileAttributes.Directory | IO.FileAttributes.Encrypted | IO.FileAttributes.Hidden |
				IO.FileAttributes.NotContentIndexed | IO.FileAttributes.Offline | IO.FileAttributes.ReadOnly |
				IO.FileAttributes.ReparsePoint | IO.FileAttributes.SparseFile | IO.FileAttributes.System |
				IO.FileAttributes.Temporary;
			return cond;
		}
	}
}