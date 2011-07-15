/*
	$Id$
*/

using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Text.RegularExpressions;
using Nekome.Search;
using CatWalk;
using CatWalk.Win32;

namespace Nekome.Windows{
	public partial class GrepResultView : UserControl{
		public GrepResultView(){
			this.InitializeComponent();
			this.RefreshInputBindings();
			Program.GrepTools.CollectionChanged += delegate{
				this.RefreshInputBindings();
			};
			this.listView.Focus();
			this.SizeChanged += delegate{
				this.AutoSizeColumns();
			};
		}
		
		private void RefreshInputBindings(){
			this.listView.InputBindings.Clear();
			foreach(var bind in Program.GrepTools.Where(tool => (tool.Key != Key.None))
			                                     .Select(tool => new KeyBinding(NekomeCommands.ExecuteExternalTool, tool.Key, tool.Modifiers){CommandParameter = tool})){
				this.listView.InputBindings.Add(bind);
			}
		}
		
		private void DeleteFile_CanExecute(object sender, CanExecuteRoutedEventArgs e){
			e.CanExecute = (this.listView.SelectedItems != null);
		}
		
		private void DeleteFile_Executed(object sender, ExecutedRoutedEventArgs e){
			var files = this.listView.SelectedItems.Cast<string>().ToArray();
			FileOperation.Delete(files, FileOperationOptions.AllowUndo, new WindowInteropHelper(Program.MainForm).Handle);
		}
		
		private void ExecuteExternalTool_CanExecute(object sender, CanExecuteRoutedEventArgs e){
			e.CanExecute = (e.Parameter != null);
		}
		
		private void ExecuteExternalTool_Executed(object sender, ExecutedRoutedEventArgs e){
			var matchIsNull = (this.listView.SelectedValue == null);
			var match = (GrepMatch)this.listView.SelectedValue;
			var tool = (ExternalTool)e.Parameter;
			var info = tool.GetProcessStartInfo();
			var eval = new MatchEvaluator(delegate(Match m){
				if(String.IsNullOrEmpty(m.Groups[1].Value)){
					if(matchIsNull){
						return "";
					}else{
						switch(m.Groups[2].Value){
							case "p":
								return match.Path;
							case "P":
								return CommandLineParser.Escape(match.Path);
							case "N":
								return Path.GetFileName(match.Path);
							case "D":
								return Path.GetDirectoryName(match.Path);
							case "E":
								return Path.GetExtension(match.Path);
							case "n":
								return Path.GetFileNameWithoutExtension(match.Path);
							case "L":
								return match.Line.ToString();
							case "l":
								return (match.Line - 1).ToString();
							case "C":
								return match.Column.ToString();
							case "c":
								return match.Match.Index.ToString();
							default:
								return "";
						}
					}
				}else{ // エスケープ
					return "%" + m.Groups[2].Value;
				}
			});
			var regex = new Regex("(%|)%(.)", RegexOptions.Compiled);
			info.FileName = regex.Replace(info.FileName, eval);
			info.Arguments = regex.Replace(info.Arguments, eval);
			info.WorkingDirectory = regex.Replace(info.WorkingDirectory, eval);
			try{
				Process.Start(info);
			}catch(Exception ex){
				MessageBox.Show(ex.Message);
			}
		}
		
		public static readonly DependencyProperty ResultProperty =
		 DependencyProperty.Register("Result", typeof(GrepResult), typeof(GrepResultView), new PropertyMetadata(ResultPropertyChanged));
		public GrepResult Result{
			get{
				return (GrepResult)this.GetValue(ResultProperty);
			}
			set{
				this.SetValue(ResultProperty, value);
			}
		}
		
		private static void ResultPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e){
			var self = (GrepResultView)d;
			var old = (GrepResult)e.OldValue;
			var now = (GrepResult)e.NewValue;
			if(old != null){
				old.Matches.CollectionChanged -= self.MatchesChanged;
			}
			if(now != null){
				now.Matches.CollectionChanged += self.MatchesChanged;
				var conv = (FilePathConverter)self.Resources["FilePathConverter"];
				if(conv != null){
					conv.BasePath = now.SearchCondition.Path;
				}
			}
		}
		
		private void MatchesChanged(object sender, NotifyCollectionChangedEventArgs e){
			if((this.listView.SelectedIndex < 0) && (this.listView.HasItems)){
				this.listView.SelectedIndex = 0;
				this.listView.Focus();
			}
			this.AutoSizeColumns();
		}

		private void AutoSizeColumns(){
			var gridView = (GridView)this.listView.View;
			var columns = gridView.Columns;
			var autoColumns = columns.Take(columns.Count - 1).ToArray();
			foreach(var column in autoColumns){
				if(double.IsNaN(column.Width)){
					column.Width = column.ActualWidth;
				} 
				column.Width = double.NaN;
			}
			columns[columns.Count - 1].Width = this.listView.ActualWidth - autoColumns.Sum(column => column.ActualWidth);
		}
	}
}