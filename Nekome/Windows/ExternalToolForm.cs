/*
	$Id$
*/
using System;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using CatWalk;
using CatWalk.Windows;
using Microsoft.Win32;

namespace Nekome.Windows{
	public partial class ExternalToolForm : Window{
		public static readonly RoutedUICommand InsertMacro = new RoutedUICommand();
		
		public ExternalToolForm() : this(null){
		}
		
		public ExternalToolForm(ExternalTool tool){
			this.InitializeComponent();
			AutoComplete.AddQueryCandidates(this.workingDirectoryBox, AutoComplete.QueryDirectoryCandidatesHandler);
			AutoComplete.AddQueryCandidates(this.fileNameBox, AutoComplete.GetQueryFilesCandidatesHandler("*.*"));
			this.Loaded += delegate{
				this.nameBox.Focus();
			};
			
			this.windowStyleBox.ItemsSource = new[]{
				new Tuple<string, ProcessWindowStyle>("通常", ProcessWindowStyle.Normal),
				new Tuple<string, ProcessWindowStyle>("非表示", ProcessWindowStyle.Hidden),
				new Tuple<string, ProcessWindowStyle>("最小化", ProcessWindowStyle.Minimized),
				new Tuple<string, ProcessWindowStyle>("最大化", ProcessWindowStyle.Maximized),
			};
			
			var tool2 = new ExternalTool();
			if(tool != null){
				CopyExterlanTool(tool, tool2);
			}
			this.ExternalTool = tool2;
		}
		
		#region 関数
		
		private static void CopyExterlanTool(ExternalTool src, ExternalTool dst){
			dst.Name = src.Name;
			dst.Verb = src.Verb;
			dst.FileName = src.FileName;
			dst.Arguments = src.Arguments;
			dst.WorkingDirectory = src.WorkingDirectory;
			dst.WindowStyle = src.WindowStyle;
			dst.Modifiers = src.Modifiers;
			dst.Key = src.Key;
		}
		
		
		private static void QueryDirectoryCandidates(object sender, QueryCandidatesEventArgs e){
			string path = e.Query;
			if(!String.IsNullOrEmpty(path)){
				var idx = path.LastIndexOf(Path.DirectorySeparatorChar.ToString());
				if(idx > 0){
					var dir = path.Substring(0, idx + 1);
					var name = path.Substring(idx + 1);
					var mask = name + "*";
					try{
						e.Candidates = Directory.GetDirectories(dir, mask).Select(d => new KeyValuePair<string, object>(d, d)).ToArray();
					}catch{
					}
				}
			}
		}
		
		private void OpenFile(object sender, RoutedEventArgs e){
			var dlg = new OpenFileDialog();
			dlg.CheckFileExists = dlg.CheckPathExists = true;
			if(dlg.ShowDialog(this).Value){
				this.fileNameBox.Text = dlg.FileName;
				if(String.IsNullOrEmpty(this.workingDirectoryBox.Text)){
					this.workingDirectoryBox.Text = Path.GetDirectoryName(dlg.FileName);
				}
			}
		}
		
		private void OpenDirectory(object sender, RoutedEventArgs e){
			var dlg = new FolderBrowserDialog();
			if(dlg.ShowDialog(this).Value){
				this.workingDirectoryBox.Text = dlg.SelectedPath;
			}
		}
		
		#endregion
		
		#region プロパティ
		
		public static readonly DependencyProperty ExternalToolProperty = DependencyProperty.Register("ExternalTool", typeof(ExternalTool), typeof(ExternalToolForm));
		public ExternalTool ExternalTool{
			get{
				return (ExternalTool)this.GetValue(ExternalToolProperty);
			}
			private set{
				this.SetValue(ExternalToolProperty, value);
			}
		}
		
		#endregion
		
		#region コマンド
		
		private void InsertMacroFileName_CanExecute(object sender, CanExecuteRoutedEventArgs e){
			e.CanExecute = (e.Parameter != null);
		}
		
		private void InsertMacroFileName_Executed(object sender, ExecutedRoutedEventArgs e){
			var macro = (string)e.Parameter;
			var left = this.fileNameBox.Text.Substring(0, this.fileNameBox.CaretIndex);
			var right = this.fileNameBox.Text.Substring(this.fileNameBox.CaretIndex);
			this.fileNameBox.Text = left + macro + right;
		}
		
		private void InsertMacroArguments_CanExecute(object sender, CanExecuteRoutedEventArgs e){
			e.CanExecute = (e.Parameter != null);
		}
		
		private void InsertMacroArguments_Executed(object sender, ExecutedRoutedEventArgs e){
			var macro = (string)e.Parameter;
			var left = this.argumentsBox.Text.Substring(0, this.argumentsBox.CaretIndex);
			var right = this.argumentsBox.Text.Substring(this.argumentsBox.CaretIndex);
			this.argumentsBox.Text = left + macro + right;
		}
		
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
			this.Close();
		}
		
		#endregion
	}
}