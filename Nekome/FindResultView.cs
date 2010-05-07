using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using CatWalk.Shell;

namespace Nekome{
	public partial class FindResultView : UserControl{
		public FindResultView(){
			this.InitializeComponent();
			this.RefreshInputBindings();
			Program.FindTools.CollectionChanged += delegate{
				this.RefreshInputBindings();
			};
			this.listBox.Focus();
		}
		
		private void DeleteFile_CanExecute(object sender, CanExecuteRoutedEventArgs e){
			e.CanExecute = (this.listBox.SelectedItems != null);
		}
		
		private void DeleteFile_Executed(object sender, ExecutedRoutedEventArgs e){
			var files = this.listBox.SelectedItems.Cast<string>().ToArray();
			FileOperation.Delete(files, FileOperationOptions.AllowUndo, new WindowInteropHelper(Program.MainForm).Handle);
		}
		
		private void ExecuteExternalTool_CanExecute(object sender, CanExecuteRoutedEventArgs e){
			e.CanExecute = (e.Parameter != null);
		}
		
		private void ExecuteExternalTool_Executed(object sender, ExecutedRoutedEventArgs e){
			var file = (string)this.listBox.SelectedValue;
			var tool = (ExternalTool)e.Parameter;
			var info = tool.GetProcessStartInfo();
			var eval = new MatchEvaluator(delegate(Match m){
				if(String.IsNullOrEmpty(m.Groups[1].Value)){
					if(file == null){
						return "";
					}else{
						switch(m.Groups[2].Value){
							case "P":
								return file;
							case "N":
								return Path.GetFileName(file);
							case "D":
								return Path.GetDirectoryName(file);
							case "E":
								return Path.GetExtension(file);
							case "n":
								return Path.GetFileNameWithoutExtension(file);
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
			Process.Start(info);
		}
		
		private void RefreshInputBindings(){
			this.listBox.InputBindings.Clear();
			foreach(var bind in Program.FindTools.Where(tool => (tool.Key != Key.None))
			                                     .Select(tool => new KeyBinding(NekomeCommands.ExecuteExternalTool, tool.Key, tool.Modifiers){CommandParameter = tool})){
				this.listBox.InputBindings.Add(bind);
			}
		}
		
		public static readonly DependencyProperty ResultProperty =
			DependencyProperty.Register("Result", typeof(FindResult), typeof(FindResultView), new PropertyMetadata(ResultPropertyChanged));
		public FindResult Result{
			get{
				return (FindResult)this.GetValue(ResultProperty);
			}
			set{
				this.SetValue(ResultProperty, value);
			}
		}
		
		private static void ResultPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e){
			var self = (FindResultView)d;
			var old = (FindResult)e.OldValue;
			var now = (FindResult)e.NewValue;
			if(old != null){
				old.Files.CollectionChanged -= self.FilesChanged;
			}
			if(now != null){
				now.Files.CollectionChanged += self.FilesChanged;
			}
		}
		
		private void FilesChanged(object sender, NotifyCollectionChangedEventArgs e){
			if((this.listBox.SelectedIndex < 0) && (this.listBox.HasItems)){
				this.listBox.SelectedIndex = 0;
				this.listBox.Focus();
			}
		}
	}
}