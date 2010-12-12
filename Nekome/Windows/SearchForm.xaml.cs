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
			this.SearchCondition = (SearchCondition)cond.Clone();
			
			this.InitializeComponent();

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
				var textBox = this.excludingMaskBox;
				AutoComplete.SetIsEnabled(textBox, true);
				AutoComplete.SetPopup(textBox, this.completePopup);
				AutoComplete.SetCandidatesListBox(textBox, this.completeListBox);
				AutoComplete.SetTokenPattern(textBox, "^");
				AutoComplete.SetPopupOffset(textBox, new Vector(-4, 0));
				if(Program.Settings.ExcludingMaskHistory != null){
					AutoComplete.AddCondidates(textBox, Program.Settings.ExcludingMaskHistory);
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
			
			//this.searchWordBox.ItemsSource = Program.Settings.SearchWordHistory;
			//this.pathBox.ItemsSource = Program.Settings.DirectoryHistory;
			//this.fileMaskBox.ItemsSource = Program.Settings.FileMaskHistory;

			this.Loaded += delegate{
				this.searchWordBox.Focus();
			};
			/*
			if(cond != null){
				this.searchWordBox.Text = cond.Pattern;
				this.pathBox.Text = cond.Path;
				this.fileMaskBox.Text = cond.Mask;
				this.isSubDirectoriesBox.IsChecked = (cond.FileSearchOption == SearchOption.AllDirectories);
				this.isIgnoreCaseBox.IsChecked = cond.IsIgnoreCase;
				this.isUseRegexBox.IsChecked = cond.IsUseRegex;
				this.excludingMaskBox.Text = cond.ExcludingMask;
				this.excludingTargets.SelectedValue = cond.ExcludingTargets;
			}*/
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
			this.DialogResult = true;

			Program.Settings.SearchWordHistory = new string[]{this.SearchCondition.Pattern}.Concat(Program.Settings.SearchWordHistory.EmptyIfNull())
			                                                                               .Where(w => !String.IsNullOrEmpty(w))
			                                                                               .Distinct().ToArray();
			Program.Settings.DirectoryHistory = new string[]{this.SearchCondition.Path}.Concat(Program.Settings.DirectoryHistory.EmptyIfNull())
			                                                                           .Where(w => !String.IsNullOrEmpty(w))
			                                                                           .Distinct().ToArray();
			Program.Settings.FileMaskHistory = new string[]{this.SearchCondition.Mask}.Concat(Program.Settings.FileMaskHistory.EmptyIfNull())
			                                                                          .Where(w => !String.IsNullOrEmpty(w))
			                                                                          .Distinct().ToArray();
			Program.Settings.ExcludingMaskHistory = new string[]{this.SearchCondition.ExcludingMask}.Concat(Program.Settings.ExcludingMaskHistory.EmptyIfNull())
			                                                                                        .Where(w => !String.IsNullOrEmpty(w))
			                                                                                        .Distinct().ToArray();

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
	}
}