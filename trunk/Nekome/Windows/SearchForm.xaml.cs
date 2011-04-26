/*
	$Id$
*/
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Reflection;
using CatWalk;
using CatWalk.Windows;
using System.Windows.Shell;

namespace Nekome.Windows{
	public partial class SearchForm : Window{
		public SearchForm() : this(SearchCondition.GetDefaultCondition()){
		}
		
		public SearchForm(SearchCondition cond){
			if(cond == null){
				throw new ArgumentNullException();
			}
			this.DataContext = new ViewModel((SearchCondition)cond.Clone());

			this.InitializeComponent();

			this.minFindFileSizeBox.GetBindingExpression(TextBox.TextProperty).ParentBinding.ValidationRules.Add(
				new FileMinSizeConverter((DependencyObject)this.DataContext, ViewModel.MaxFindFileSizeProperty));
			this.maxFindFileSizeBox.GetBindingExpression(TextBox.TextProperty).ParentBinding.ValidationRules.Add(
				new FileMaxSizeConverter((DependencyObject)this.DataContext, ViewModel.MinFindFileSizeProperty));
			this.minGrepFileSizeBox.GetBindingExpression(TextBox.TextProperty).ParentBinding.ValidationRules.Add(
				new FileMinSizeConverter((DependencyObject)this.DataContext, ViewModel.MaxGrepFileSizeProperty));
			this.maxGrepFileSizeBox.GetBindingExpression(TextBox.TextProperty).ParentBinding.ValidationRules.Add(
				new FileMaxSizeConverter((DependencyObject)this.DataContext, ViewModel.MinGrepFileSizeProperty));

			this.Loaded += delegate{
				//var textBox = (TextBox)this.pathBox.Template.FindName("PART_EditableTextBox", this.pathBox);
				var textBox = this.pathBox;
				AutoComplete.SetIsEnabled(textBox, true);
				AutoComplete.SetPopup(textBox, this.completePopup);
				AutoComplete.SetCandidatesListBox(textBox, this.completeListBox);
				AutoComplete.SetTokenPattern(textBox, "^");
				AutoComplete.SetPopupOffset(textBox, new Vector(-4, 0));
				AutoComplete.SetReplaceWordTypes(textBox, new Type[]{typeof(string)});
				AutoComplete.SetIsInsertAutomatically(textBox, false);
				AutoComplete.AddQueryCandidates(textBox, AutoComplete.QueryDirectoryCandidatesHandler);
			};
			this.Loaded += delegate{
				//var textBox = (TextBox)this.searchWordBox.Template.FindName("PART_EditableTextBox", this.searchWordBox);
				var textBox = this.fileMaskBox;
				AutoComplete.SetIsEnabled(textBox, true);
				AutoComplete.SetPopup(textBox, this.completePopup);
				AutoComplete.SetCandidatesListBox(textBox, this.completeListBox);
				AutoComplete.SetTokenPattern(textBox, "^");
				AutoComplete.SetPopupOffset(textBox, new Vector(-4, 0));
				if(Program.Settings.FileMaskHistory != null){
					AutoComplete.AddCondidates(textBox, Program.Settings.FileMaskHistory);
				}
			};
			this.Loaded += delegate{
				//var textBox = (TextBox)this.searchWordBox.Template.FindName("PART_EditableTextBox", this.searchWordBox);
				var textBox = this.grepExcludingMaskBox;
				AutoComplete.SetIsEnabled(textBox, true);
				AutoComplete.SetPopup(textBox, this.completePopup);
				AutoComplete.SetCandidatesListBox(textBox, this.completeListBox);
				AutoComplete.SetTokenPattern(textBox, "^");
				AutoComplete.SetPopupOffset(textBox, new Vector(-4, 0));
				if(Program.Settings.GrepExcludingMaskHistory != null){
					AutoComplete.AddCondidates(textBox, Program.Settings.GrepExcludingMaskHistory);
				}
			};
			this.Loaded += delegate{
				//var textBox = (TextBox)this.searchWordBox.Template.FindName("PART_EditableTextBox", this.searchWordBox);
				var textBox = this.findExcludingMaskBox;
				AutoComplete.SetIsEnabled(textBox, true);
				AutoComplete.SetPopup(textBox, this.completePopup);
				AutoComplete.SetCandidatesListBox(textBox, this.completeListBox);
				AutoComplete.SetTokenPattern(textBox, "^");
				AutoComplete.SetPopupOffset(textBox, new Vector(-4, 0));
				if(Program.Settings.FindExcludingMaskHistory != null){
					AutoComplete.AddCondidates(textBox, Program.Settings.FindExcludingMaskHistory);
				}
			};
			this.Loaded += delegate{
				//var textBox = (TextBox)this.searchWordBox.Template.FindName("PART_EditableTextBox", this.searchWordBox);
				var textBox = this.searchWordBox;
				AutoComplete.SetIsEnabled(textBox, true);
				AutoComplete.SetPopup(textBox, this.completePopup);
				AutoComplete.SetCandidatesListBox(textBox, this.completeListBox);
				AutoComplete.SetTokenPattern(textBox, "^");
				AutoComplete.SetPopupOffset(textBox, new Vector(-4, 0));
				if(Program.Settings.SearchWordHistory != null){
					AutoComplete.AddCondidates(textBox, Program.Settings.SearchWordHistory);
				}
			};

			this.Loaded += delegate{
				this.searchWordBox.Focus();
			};
		}
		
		public SearchCondition SearchCondition{
			get{
				return ((ViewModel)this.DataContext).SearchCondition;
			}
		}
		
		#region 関数
		
		private bool CheckPattern(){
			try{
				if(this.isUseRegexBox.IsChecked.Value){
					var regex = new Regex(this.searchWordBox.Text);
					return true;
				}else{
					return true;
				}
			}catch(ArgumentException){
				return false;
			}
		}
		
		private void OpenPath(object sender, RoutedEventArgs e){
			var dialog = new FolderBrowserDialog();
			if(Directory.Exists(this.pathBox.Text)){
				dialog.SelectedPath = this.pathBox.Text;
			}
			if(dialog.ShowDialog(this).Value){
				this.pathBox.Text = dialog.SelectedPath;
			}
		}
		
		#endregion
		
		#region コマンド
		
		private void Cancel_CanExecute(object sender, CanExecuteRoutedEventArgs e){
			e.CanExecute = true;
		}
		
		private void Cancel_Executed(object sender, ExecutedRoutedEventArgs e){
			this.DialogResult = false;
			this.Close();
		}
		
		private void OK_CanExecute(object sender, CanExecuteRoutedEventArgs e){
			e.CanExecute = true;
		}
		
		private void OK_Executed(object sender, ExecutedRoutedEventArgs e){
			this.DialogResult = true;

			Program.Settings.AddHistory(this.SearchCondition);

			// タスク追加。
			var task = new JumpTask();
			task.ApplicationPath = Assembly.GetEntryAssembly().Location;
			task.Arguments = String.Join(" ", new string[]{
				CommandLineParser.Escape(this.SearchCondition.Path)});
			var title = this.SearchCondition.Path;
			const int thre = 30;
			if(title.Length > thre){
				title = title.Substring(title.Length - thre, thre);
				title = "..." + Regex.Replace(title, @"^[^\\]*", "");
			}
			task.Title = title;
			task.Description = this.SearchCondition.Path;
			task.IconResourcePath = @"C:\Windows\System32\shell32.dll";
			task.IconResourceIndex = 3;
			JumpList.AddToRecentCategory(task);
			Program.JumpList.Apply();
			this.Close();
		}
		
		#endregion
		
		public class ViewModel : DependencyObject{
			public SearchCondition SearchCondition{get; private set;}

			public static readonly DependencyProperty MaxFindFileSizeProperty =
				DependencyProperty.Register("MaxFindFileSize", typeof(long), typeof(ViewModel), new PropertyMetadata(Int64.MaxValue, FindFileSizeRangeChanged));
			public long MaxFindFileSize{
				get{
					return (long)this.GetValue(MaxFindFileSizeProperty);
				}
				set{
					this.SetValue(MaxFindFileSizeProperty, value);
				}
			}
			public static readonly DependencyProperty MinFindFileSizeProperty =
				DependencyProperty.Register("MinFindFileSize", typeof(long), typeof(ViewModel), new PropertyMetadata(0L, FindFileSizeRangeChanged));
			public long MinFindFileSize{
				get{
					return (long)this.GetValue(MinFindFileSizeProperty);
				}
				set{
					this.SetValue(MinFindFileSizeProperty, value);
				}
			}
			private static void FindFileSizeRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e){
				var self = (ViewModel)d;
				self.SearchCondition.AdvancedFindCondition.FileSizeRange =
					new Range<long>(self.MinFindFileSize, self.MaxFindFileSize, false, false);
			}

			public static readonly DependencyProperty MaxGrepFileSizeProperty =
				DependencyProperty.Register("MaxGrepFileSize", typeof(long), typeof(ViewModel), new PropertyMetadata(Int64.MaxValue, GrepFileSizeRangeChanged));
			public long MaxGrepFileSize{
				get{
					return (long)this.GetValue(MaxGrepFileSizeProperty);
				}
				set{
					this.SetValue(MaxGrepFileSizeProperty, value);
				}
			}
			public static readonly DependencyProperty MinGrepFileSizeProperty =
				DependencyProperty.Register("MinGrepFileSize", typeof(long), typeof(ViewModel), new PropertyMetadata(0L, GrepFileSizeRangeChanged));
			public long MinGrepFileSize{
				get{
					return (long)this.GetValue(MinGrepFileSizeProperty);
				}
				set{
					this.SetValue(MinGrepFileSizeProperty, value);
				}
			}
			private static void GrepFileSizeRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e){
				var self = (ViewModel)d;
				self.SearchCondition.AdvancedGrepCondition.FileSizeRange =
					new Range<long>(self.MinGrepFileSize, self.MaxGrepFileSize, false, false);
			}

			public ViewModel(SearchCondition cond){
				this.SearchCondition = cond;
				this.MinFindFileSize = cond.AdvancedFindCondition.FileSizeRange.LowerBound;
				this.MinGrepFileSize = cond.AdvancedGrepCondition.FileSizeRange.LowerBound;
				this.MaxFindFileSize = cond.AdvancedFindCondition.FileSizeRange.UpperBound;
				this.MaxGrepFileSize = cond.AdvancedGrepCondition.FileSizeRange.UpperBound;
			}
		}
	}
}