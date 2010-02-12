/*
	$Id$
*/
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows;
using System.Text.RegularExpressions;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Taskbar;
using CatWalk.Windows;

namespace Nekome{
	public partial class SearchForm : Window{
		public SearchForm() : this(null){
		}
		
		public SearchForm(SearchCondition cond){
			if(cond == null){
				throw new ArgumentNullException();
			}
			
			this.InitializeComponent();
			
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
			if(dialog.ShowDialog(this).Value){
				this.pathBox.Text = dialog.SelectedPath;
			}
		}
		
		#endregion
		
		#region コマンド
		
		private void Close_CanExecute(object sender, CanExecuteRoutedEventArgs e){
			e.CanExecute = true;
		}
		
		private void Close_Executed(object sender, ExecutedRoutedEventArgs e){
			this.DialogResult = false;
			this.Close();
		}
		
		private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e){
			e.CanExecute = true;
		}
		
		private void Save_Executed(object sender, ExecutedRoutedEventArgs e){
			this.DialogResult = true;
			this.SearchCondition = new SearchCondition();
			this.SearchCondition.Path = this.pathBox.Text;
			this.SearchCondition.Mask = this.fileMaskBox.Text;
			this.SearchCondition.SearchOption = (this.isSubDirectoriesBox.IsChecked.Value) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
			this.SearchCondition.Regex = this.GetRegex();
			this.Close();
		}
		
		#endregion
	}
}