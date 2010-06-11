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

namespace Nekome{
	public partial class SearchForm : Window{
		public SearchForm() : this(null){
		}
		
		public SearchForm(SearchCondition cond){
			this.InitializeComponent();
			this.pathBox.Loaded += delegate{
				var pathEditBox = (TextBox)this.pathBox.Template.FindName("PART_EditableTextBox", this.pathBox);
				AutoComplete.SetIsEnabled(pathEditBox, true);
				AutoComplete.SetPopup(pathEditBox, this.completePopup);
				AutoComplete.SetCandidatesListBox(pathEditBox, this.completeListBox);
				AutoComplete.SetTokenPattern(pathEditBox, "^");
				AutoComplete.SetPopupOffset(pathEditBox, new Vector(-4, 0));
				AutoComplete.AddQueryCandidates(pathEditBox, AutoComplete.QueryDirectoryCandidatesHandler);
			};

			this.searchWordBox.ItemsSource = Program.Settings.SearchWordHistory;
			this.pathBox.ItemsSource = Program.Settings.DirectoryHistory;
			this.fileMaskBox.ItemsSource = Program.Settings.FileMaskHistory;

			this.Loaded += delegate{
				this.searchWordBox.Focus();
			};
			
			if(cond != null){
				this.searchWordBox.Text = cond.Pattern;
				this.pathBox.Text = cond.Path;
				this.fileMaskBox.Text = cond.Mask;
				this.isSubDirectoriesBox.IsChecked = (cond.SearchOption == SearchOption.AllDirectories);
				this.isIgnoreCaseBox.IsChecked = cond.IsIgnoreCase;
				this.isUseRegexBox.IsChecked = cond.IsUseRegex;
			}
		}
		
		public SearchCondition SearchCondition{get; private set;}
		
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
			var path = this.pathBox.Text.TrimEnd('\\') + "\\";
			var mask = this.fileMaskBox.Text;
			var pattern = this.searchWordBox.Text;
			var option = (this.isSubDirectoriesBox.IsChecked.Value) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

			this.DialogResult = true;
			this.SearchCondition = new SearchCondition();
			this.SearchCondition.Path = path;
			this.SearchCondition.Mask = mask;
			this.SearchCondition.SearchOption = option;
			this.SearchCondition.Pattern = pattern;
			this.SearchCondition.IsIgnoreCase = this.isIgnoreCaseBox.IsChecked.Value;
			this.SearchCondition.IsUseRegex = this.isUseRegexBox.IsChecked.Value;

			Program.Settings.SearchWordHistory = new string[]{this.searchWordBox.Text}.Concat(Program.Settings.SearchWordHistory.EmptyIfNull())
			                                                                          .Where(w => !String.IsNullOrEmpty(w))
			                                                                          .Distinct().ToArray();
			Program.Settings.DirectoryHistory = new string[]{path}.Concat(Program.Settings.DirectoryHistory.EmptyIfNull())
			                                                                   .Where(w => !String.IsNullOrEmpty(w))
			                                                                   .Distinct().ToArray();
			Program.Settings.FileMaskHistory = new string[]{this.fileMaskBox.Text}.Concat(Program.Settings.FileMaskHistory.EmptyIfNull())
			                                                                      .Where(w => !String.IsNullOrEmpty(w))
			                                                                      .Distinct().ToArray();

			var task = new JumpTask();
			task.ApplicationPath = Assembly.GetEntryAssembly().Location;
			task.Arguments = String.Join(" ", new string[]{
				CommandLineParser.Escape(path)});
			var title = path;
			const int thre = 30;
			if(title.Length > thre){
				title = title.Substring(title.Length - thre, thre);
				title = "..." + Regex.Replace(title, @"^[^\\]*", "");
			}
			task.Title = title;
			task.Description = path;
			task.IconResourcePath = @"C:\Windows\System32\shell32.dll";
			task.IconResourceIndex = 3;
			JumpList.AddToRecentCategory(task);
			Program.JumpList.Apply();
			this.Close();
		}
		
		#endregion
	}
}