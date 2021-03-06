﻿/*
	$Id$
*/
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using System.IO;
using System.Linq;
using System.Threading;
using System.Text.RegularExpressions;
using System.Net;
using System.Configuration;
using System.Threading.Tasks;
using System.Diagnostics;
using Nekome.Search;
using CatWalk;
using CatWalk.Net;
using CatWalk.Windows;

namespace Nekome.Windows{
	using Prop = Nekome.Properties;

	public partial class MainForm : Window{
		private WindowSettings settings = (WindowSettings)SettingsBase.Synchronized(new WindowSettings("MainForm"));
		private ProgressManager progressManager = new ProgressManager();
		private ObservableCollection<ResultTab> resultTabs = new ObservableCollection<ResultTab>();
		private WindowState restoreState;
		
		public MainForm(){
			this.InitializeComponent();
			this.resultTabControl.ItemsSource = this.resultTabs;
			
			// 初期処理
			this.settings.UpgradeOnce();
			this.settings.RestoreWindow(this);
			this.restoreState = this.WindowState;
			this.IsCheckUpdatesOnStartUp = Program.Settings.IsCheckUpdatesOnStartUp;

			// イベント
			this.Loaded += this.LoadedListener;
		}
		
		#region 関数
		
		public void ShowWindow(){
			this.Show();
			this.Activate();
			this.WindowState = this.restoreState;
		}
		
		public void FindDialog(SearchCondition cond){
			var form = new SearchForm(cond);
			form.Owner = this;
			if(form.ShowDialog().Value) {
				cond = form.SearchCondition;
				try{
					Environment.CurrentDirectory = cond.Path;
					if(String.IsNullOrEmpty(cond.Pattern)) {
						this.FindFiles(cond);
					} else {
						this.GrepFiles(cond);
					}
				}catch(Exception ex) {
					MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}
		
		public void FindFiles(SearchCondition cond){
			if(!cond.Path.IsNullOrEmpty() && !cond.Path.EndsWith("\\")){
				cond.Path = Path.GetFullPath(cond.Path) + "\\";
			}
			var result = new FindResult(cond);
			var resultList = result.Files;
			var tokenSource = new CancellationTokenSource();
			var path = cond.Path;
			var searchOption = cond.FileSearchOption;
			var ui = TaskScheduler.FromCurrentSynchronizationContext();
			var masks = cond.Mask.Split(';');
			var isAdvanced = cond.IsEnableAdvancedFindCondition;
			var exMasks = (isAdvanced) ?
				cond.AdvancedFindCondition.ExcludingMask.Split(new char[]{';'}, StringSplitOptions.RemoveEmptyEntries) : new string[0];
			var range = (isAdvanced) ? cond.AdvancedFindCondition.FileSizeRange : new Range<long>(0, Int64.MaxValue);
			var timer = new Stopwatch();
			var find = new Task(new Action(delegate{
				Parallel.ForEach(Seq.EnumerateFileSystemEntries(path, searchOption), delegate(Tuple<IEnumerable<string>, double> filesProg){
					tokenSource.Token.ThrowIfCancellationRequested();
					var files2 = filesProg.Item1.ToArray();
					if(files2.Length > 0){
						string dir = Path.GetDirectoryName(files2[0]);
						var files = files2
							.Where(file => Path.GetFileName(file)
								.Let(name =>
									(masks.Where(mask => name.IsMatchWildCard(mask)).FirstOrDefault() != null) &&
									(exMasks.Where(mask => name.IsMatchWildCard(mask)).FirstOrDefault() == null)));
						var matchFiles = new List<string>();
						if(isAdvanced){
							foreach(var file in files.TakeWhile(_ => !tokenSource.IsCancellationRequested)){
								FileInfo info = null;
								try{
									info = new FileInfo(file);
								}catch(IOException){
								}catch(UnauthorizedAccessException){
								}
								if((info != null) && range.Contains(info.Length)){
									matchFiles.Add(file);
								}
							}
						}else{
							matchFiles.AddRange(files);
						}
						this.Dispatcher.Invoke(new Action(delegate{
							this.progressManager.ProgressMessage = 
								String.Format(Properties.Resources.MainForm_FileSearchingMessage,
									timer.Elapsed.ToString("g"), dir);
							this.progressManager.ReportProgress(result, filesProg.Item2);
							foreach(var match in matchFiles){
								resultList.Add(match);
							}
						}));
					}
				});
			}), tokenSource.Token);
			// 検索完了時
			find.ContinueWith(delegate{
				timer.Stop();
				this.progressManager.ProgressMessage =
					String.Format(Properties.Resources.MainForm_FileSearchCompleteMessage, timer.Elapsed.ToString("g"));
			}, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, ui);
			// 検索キャンセル時
			find.ContinueWith(delegate{
				timer.Stop();
				this.progressManager.ProgressMessage =
					String.Format(Properties.Resources.MainForm_FileSearchCanceledMessage, timer.Elapsed.ToString("g"));
			}, CancellationToken.None, TaskContinuationOptions.OnlyOnCanceled, ui);
			// 後始末
			find.ContinueWith(delegate{
				this.progressManager.Complete(result);
				CommandManager.InvalidateRequerySuggested();
			}, CancellationToken.None, TaskContinuationOptions.None, ui);

			// タブ追加
			var resultTab = new ResultTab(result, find, tokenSource);
			this.resultTabs.Add(resultTab);
			this.resultTabControl.SelectedValue = resultTab;
			
			// タスク実行
			this.progressManager.Start(result);
			timer.Start();
			find.Start();
			CommandManager.InvalidateRequerySuggested();
		}
		
		public void GrepFiles(SearchCondition cond){
			if(!cond.Path.IsNullOrEmpty() && !cond.Path.EndsWith("\\")){
				cond.Path = Path.GetFullPath(cond.Path) + "\\";
			}
			var result = new GrepResult(cond);
			var resultList = result.Matches;
			var tokenSource = new CancellationTokenSource();
			var path = cond.Path;
			var searchOption = cond.FileSearchOption;
			var regex = cond.GetRegex();
			var ui = TaskScheduler.FromCurrentSynchronizationContext();
			var masks = cond.Mask.Split(';');
			var isAdvanced = cond.IsEnableAdvancedGrepCondition;
			var exMasks = (isAdvanced) ?
				cond.AdvancedGrepCondition.ExcludingMask.Split(new char[]{';'}, StringSplitOptions.RemoveEmptyEntries) : new string[0];
			var range = (isAdvanced) ? cond.AdvancedGrepCondition.FileSizeRange : new Range<long>(0, Int64.MaxValue);
			var timer = new Stopwatch();
			var grep = new Task(new Action(delegate{
				foreach(var filesProg in Seq.EnumerateFiles(path, searchOption)){
					tokenSource.Token.ThrowIfCancellationRequested();
					Parallel.ForEach(
						filesProg.Item1.Where(file => Path.GetFileName(file)
							.Let(name =>
								(masks.Where(mask => name.IsMatchWildCard(mask)).FirstOrDefault() != null) &&
								(exMasks.Where(mask => name.IsMatchWildCard(mask)).FirstOrDefault() == null))),
						delegate(string file, ParallelLoopState state){
						if(tokenSource.IsCancellationRequested){
							state.Break();
						}
						if(isAdvanced){
							FileInfo info = null;
							try{
								info = new FileInfo(file);
							}catch(IOException){
							}catch(UnauthorizedAccessException){
							}
							if((info == null) || !range.Contains(info.Length)){
								return;
							}
						}
						this.Dispatcher.Invoke(new Action(delegate{
							this.progressManager.ProgressMessage =
								String.Format(Properties.Resources.MainForm_GreppingMessage,
									timer.Elapsed.ToString("g"), file);
							this.progressManager.ReportProgress(result, filesProg.Item2);
						}));
						var list = new List<GrepMatch>();
						try{
							foreach(var match in Grep.Match(regex, file, tokenSource)){
								list.Add(match);
								if(list.Count > 15){
									this.Dispatcher.Invoke(new Action(delegate{
										foreach(var match2 in list){
											resultList.Add(match2);
										}
										list.Clear();
									}), DispatcherPriority.Background);
								}
							}
						}catch(IOException){
						}catch(UnauthorizedAccessException){
						}catch(Exception ex){
							MessageBox.Show(ex.ToString());
						}
						if(list.Count > 0){
							this.Dispatcher.Invoke(new Action(delegate{
								foreach(var match2 in list){
									resultList.Add(match2);
								}
							}), DispatcherPriority.Background);
						}
					});
				}
			}), tokenSource.Token);
			// Grep完了時
			grep.ContinueWith(delegate{
				timer.Stop();
				this.progressManager.ProgressMessage =
					String.Format(Properties.Resources.MainForm_GrepCompleteMessage, timer.Elapsed.ToString("g"));
			}, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, ui);
			// Grepキャンセル時
			grep.ContinueWith(delegate{
				timer.Stop();
				this.progressManager.ProgressMessage =
										String.Format(Properties.Resources.MainForm_GrepCanceledMessage, timer.Elapsed.ToString("g"));
			}, CancellationToken.None, TaskContinuationOptions.OnlyOnCanceled, ui);
			// 後始末
			grep.ContinueWith(delegate{
				this.progressManager.Complete(result);
				CommandManager.InvalidateRequerySuggested();
			}, CancellationToken.None, TaskContinuationOptions.None, ui);
			
			// タブ追加
			var resultTab = new ResultTab(result, grep, tokenSource);
			this.resultTabs.Add(resultTab);
			this.resultTabControl.SelectedValue = resultTab;
			
			//タスク実行
			this.progressManager.Start(result);
			timer.Start();
			grep.Start();
			CommandManager.InvalidateRequerySuggested();
		}
		
		#endregion
		
		#region イベント処理
		
		private void LoadedListener(object sender, RoutedEventArgs e){
			this.searchButton.Focus();
		}
		
		protected override void OnClosing(CancelEventArgs e){
			this.settings.SaveWindow(this);
			base.OnClosing(e);
		}
		
		protected override void OnClosed(EventArgs e){
			base.OnClosed(e);
			this.settings.Save();
		}
		
		protected override void OnStateChanged(EventArgs e){
			if(this.WindowState != WindowState.Minimized){
				this.restoreState = this.WindowState;
			}
			base.OnStateChanged(e);
			this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate{
				CommandManager.InvalidateRequerySuggested();
			}));
		}
		
		#endregion
		
		#region コマンド
		
		private void Close_CanExecute(object sender, CanExecuteRoutedEventArgs e){
			e.CanExecute = true;
		}
		
		private void Close_Executed(object sender, ExecutedRoutedEventArgs e){
			this.Close();
		}
		
		private void CloseTab_CanExecute(object sender, CanExecuteRoutedEventArgs e){
			e.CanExecute = true;
		}
		
		private void CloseTab_Executed(object sender, ExecutedRoutedEventArgs e){
			if(this.resultTabControl.HasItems && (this.resultTabControl.SelectedIndex >= 0)){
				if(this.resultTabs[this.resultTabControl.SelectedIndex].Task.Status == TaskStatus.Running){
					this.resultTabs[this.resultTabControl.SelectedIndex].CancellationTokenSource.Cancel();
				}
				this.resultTabs.RemoveAt(this.resultTabControl.SelectedIndex);
			}else{
				this.Close();
			}
		}
		
		private void Search_CanExecute(object sender, CanExecuteRoutedEventArgs e){
			e.CanExecute = true;
		}
		
		private void Search_Executed(object sender, ExecutedRoutedEventArgs e){
			var cond = SearchCondition.GetDefaultCondition();
			cond.FileSearchOption = Program.Settings.FileSearchOption;
			this.FindDialog(cond);
		}
		
		private void Abort_CanExecute(object sender, CanExecuteRoutedEventArgs e){
			e.CanExecute = (this.resultTabControl.SelectedIndex >= 0) &&
			               (this.resultTabs[this.resultTabControl.SelectedIndex].Task.Status == TaskStatus.Running) &&
			               !this.resultTabs[this.resultTabControl.SelectedIndex].CancellationTokenSource.IsCancellationRequested;
		}
		
		private void Abort_Executed(object sender, ExecutedRoutedEventArgs e){
			var tokenSource = this.resultTabs[this.resultTabControl.SelectedIndex].CancellationTokenSource;
			tokenSource.Cancel();
		}
		
		private void EditFindTools_CanExecute(object sender, CanExecuteRoutedEventArgs e){
			e.CanExecute = true;
		}
		
		private void EditFindTools_Executed(object sender, ExecutedRoutedEventArgs e){
			this.EditExternalTools(Program.FindTools);
		}
		
		private void EditGrepTools_CanExecute(object sender, CanExecuteRoutedEventArgs e){
			e.CanExecute = true;
		}
		
		private void EditGrepTools_Executed(object sender, ExecutedRoutedEventArgs e){
			this.EditExternalTools(Program.GrepTools);
		}
		
		private void EditExternalTools(IList tools){
			var dialog = new CollectionEditDialog();
			dialog.Title = Prop::Resources.MainForm_EditToolsTitle;
			dialog.Collection = tools;
			dialog.Owner = this;
			dialog.ItemTemplate = (DataTemplate)this.FindResource("ExternalToolItemTemplate");
			dialog.AddItem += delegate(object sender2, ItemEventArgs e2){
				var tool = new ExternalTool(){
					Verb = "open",
				};
				var form = new ExternalToolForm(tool);
				form.Owner = sender2 as Window;
				if(form.ShowDialog().Value){
					e2.Item = form.ExternalTool;
				}else{
					e2.Cancel = true;
				}
			};
			dialog.EditItem += delegate(object sender2, ItemEventArgs e2){
				var form = new ExternalToolForm((ExternalTool)e2.Item);
				form.Owner = sender2 as Window;
				if(form.ShowDialog().Value){
					e2.Item = form.ExternalTool;
				}else{
					e2.Cancel = true;
				}
			};
			if(dialog.ShowDialog().Value){
				// todo
			}
		}
		
		private void About_CanExecute(object sender, CanExecuteRoutedEventArgs e){
			e.CanExecute = true;
		}
		
		private void About_Executed(object sender, ExecutedRoutedEventArgs e){
			var about = new AboutBox();
			about.Owner = this;
			about.ShowDialog();
		}
		
		private void CheckUpdate_CanExecute(object sender, CanExecuteRoutedEventArgs e){
			e.CanExecute = true;
		}
		
		private void CheckUpdate_Executed(object sender, ExecutedRoutedEventArgs e){
			UpdatePackage[] packages = null;
			try{
				packages = Program.GetUpdates();
			}catch(WebException ex){
				MessageBox.Show(String.Format(Prop::Resources.FaildToCheckUpdates, ex.Message), Prop::Resources.UpdateTitle, MessageBoxButton.OK, MessageBoxImage.Error);
			}
			if(packages != null && packages.Length > 0){
				var package = packages[0];
				if(MessageBox.Show(
					String.Format(Prop::Resources.FoundUpdateDialog ,package.Version), 
					Prop::Resources.UpdateTitle,
					MessageBoxButton.YesNo) == MessageBoxResult.Yes){
					try{
						Program.Update(package);
						}catch(WebException ex){
							MessageBox.Show(String.Format(Prop::Resources.FaildToDownloadInstaller ,ex.Message),
								Prop::Resources.UpdateTitle,
								MessageBoxButton.OK,
								MessageBoxImage.Error);
						}
				}
			}else{
				MessageBox.Show(Prop::Resources.NoUpdatesAvailable);
			}
		}

		private void SettingGrepPreviewFont_CanExecute(object sender, CanExecuteRoutedEventArgs e){
			e.CanExecute = true;
		}

		private void SettingGrepPreviewFont_Executed(object sender, ExecutedRoutedEventArgs e){
			var dialog = new FontDialog();
			dialog.SelectedFont = Program.Settings.GrepPreviewFont;
			if(dialog.ShowDialog().Value){
				Program.Settings.GrepPreviewFont = dialog.SelectedFont;
			}
		}
		
		#endregion
		
		#region プロパティ
		
		public ProgressManager ProgressManager{
			get{
				return this.progressManager;
			}
		}
		
		private static readonly DependencyProperty IsCheckUpdatesOnStartUpProperty =
			DependencyProperty.Register("IsCheckUpdatesOnStartUp", typeof(bool), typeof(MainForm),
				new PropertyMetadata(IsCheckUpdatesOnStartUpChanged));
		private bool IsCheckUpdatesOnStartUp{
			get{
				return (bool)this.GetValue(IsCheckUpdatesOnStartUpProperty);
			}
			set{
				this.SetValue(IsCheckUpdatesOnStartUpProperty, value);
			}
		}

		private static void IsCheckUpdatesOnStartUpChanged(DependencyObject d, DependencyPropertyChangedEventArgs e){
			Program.Settings.IsCheckUpdatesOnStartUp = (bool)e.NewValue;
		}


		#endregion

		private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e) {
			if(e.MouseDevice.MiddleButton == MouseButtonState.Pressed){
				this.resultTabControl.SelectedValue = ((FrameworkElement)sender).DataContext;
				if(this.resultTabs[this.resultTabControl.SelectedIndex].Task.Status == TaskStatus.Running){
					this.resultTabs[this.resultTabControl.SelectedIndex].CancellationTokenSource.Cancel();
				}
				this.resultTabs.RemoveAt(this.resultTabControl.SelectedIndex);
			}
		}
	}
	
	public class ResultContentTemplateSelector : DataTemplateSelector{
		public override DataTemplate SelectTemplate(object item, DependencyObject container){
			var frm = (FrameworkElement)container;
			var tab = (ResultTab)item;
			if(tab.Result is FindResult){
				return (DataTemplate)frm.FindResource("FindResultContentTemplate");
			}else if(tab.Result is GrepResult){
				return (DataTemplate)frm.FindResource("GrepResultContentTemplate");
			}else{
				throw new InvalidOperationException();
			}
		}
	}
	
	public class ResultItemTemplateSelector : DataTemplateSelector{
		public override DataTemplate SelectTemplate(object item, DependencyObject container){
			var frm = (FrameworkElement)container;
			var tab = (ResultTab)item;
			if(tab.Result is FindResult){
				return (DataTemplate)frm.FindResource("FindResultItemTemplate");
			}else if(tab.Result is GrepResult){
				return (DataTemplate)frm.FindResource("GrepResultItemTemplate");
			}else{
				throw new InvalidOperationException();
			}
		}
	}
}