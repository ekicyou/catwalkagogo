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
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Taskbar;
using CatWalk.Windows;

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

			this.searchWordBox.Focus();
			
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
			this.DialogResult = true;
			this.SearchCondition = new SearchCondition();
			this.SearchCondition.Path = this.pathBox.Text;
			this.SearchCondition.Mask = this.fileMaskBox.Text;
			this.SearchCondition.SearchOption = (this.isSubDirectoriesBox.IsChecked.Value) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
			this.SearchCondition.Regex = this.GetRegex();

			Program.Settings.SearchWordHistory = Program.Settings.SearchWordHistory ?? new string[0];
			Program.Settings.DirectoryHistory = Program.Settings.DirectoryHistory ?? new string[0];
			Program.Settings.FileMaskHistory = Program.Settings.FileMaskHistory ?? new string[0];
			Program.Settings.SearchWordHistory = Program.Settings.SearchWordHistory.Concat(new string[]{this.searchWordBox.Text})
			                                                                       .Distinct().ToArray();
			Program.Settings.DirectoryHistory = Program.Settings.DirectoryHistory.Concat(new string[]{this.pathBox.Text})
			                                                                     .Distinct().ToArray();
			Program.Settings.FileMaskHistory = Program.Settings.FileMaskHistory.Concat(new string[]{this.fileMaskBox.Text})
			                                                                   .Distinct().ToArray();
			this.Close();
		}
		
		#endregion
	}
}