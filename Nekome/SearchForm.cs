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
				this.pathBox.Text = cond.Path;
				this.fileMaskBox.Text = cond.Mask;
				this.isSubDirectoriesBox.IsChecked = (cond.SearchOption == SearchOption.AllDirectories);
			}
		}
		
		public SearchCondition SearchCondition{get; private set;}
		
		#region 関数
		
		private Regex GetRegex(){
			if(!String.IsNullOrEmpty(this.searchWordBox.Text)){
				var pattern = (this.isUseRegexBox.IsChecked.Value) ?  this.searchWordBox.Text : Regex.Escape(this.searchWordBox.Text);
				var options = ((this.isIgnoreCaseBox.IsChecked.Value) ? RegexOptions.IgnoreCase : RegexOptions.None);
				return new Regex(pattern, options);
			}else{
				return null;
			}
		}
		
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
			var path = this.pathBox.Text;
			var mask = this.fileMaskBox.Text;
			var option = (this.isSubDirectoriesBox.IsChecked.Value) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

			this.DialogResult = true;
			this.SearchCondition = new SearchCondition();
			this.SearchCondition.Path = path;
			this.SearchCondition.Mask = mask;
			this.SearchCondition.SearchOption = option;
			this.SearchCondition.Regex = this.GetRegex();

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
			task.Arguments = path +
				" \"/mask:" + mask +
				"\" /recursive" + ((this.isSubDirectoriesBox.IsChecked.Value) ? "+" : "-");
			task.Title = path;
			JumpList.AddToRecentCategory(task);
			this.Close();
		}
		
		#endregion
	}
}