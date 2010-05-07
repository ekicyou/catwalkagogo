/*
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
using Nekome.Search;
using CatWalk.Windows;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace Nekome{
	public partial class MainForm : GlassWindow{
		private WindowSettings settings = new WindowSettings("MainForm");
		private ProgressManager progressManager = new ProgressManager();
		private ObservableCollection<ResultTab> resultTabs = new ObservableCollection<ResultTab>();
		private WindowState restoreState;
		
		public MainForm(){
			this.InitializeComponent();
			this.resultTabControl.ItemsSource = this.resultTabs;
			
			// 初期処理
			this.settings.UpgradeOnce();
			this.settings.RestoreWindow(this);
			
			// イベント
			this.Loaded += this.LoadedListener;
			this.AeroGlassCompositionChanged += this.AeroGlassCompositionChangedListener;
			if(TaskbarManager.IsPlatformSupported){
				this.progressManager.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e){
					bool isBusy = (bool)e.UserState;
					TaskbarManager.Instance.SetProgressState(
						(isBusy) ? TaskbarProgressBarState.Normal : TaskbarProgressBarState.NoProgress,
						this);
					TaskbarManager.Instance.SetProgressValue(e.ProgressPercentage, 100, this);
				};
			}
		}
		
		#region 関数
		
		public void ShowWindow(){
			this.Show();
			this.Activate();
			this.WindowState = this.restoreState;
		}
		
		private void FindFiles(SearchCondition cond){
			var result = new FindResult(cond.Path, cond.Mask, cond.SearchOption);
			var resultList = result.Files;
			var worker = new FileListWorker(cond.Path, cond.Mask, cond.SearchOption);
			worker.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e){
				var files = e.UserState as string[];
				if(files != null){
					foreach(var file in files){
						if(!Directory.Exists(file)){
							resultList.Add(file);
						}
					}
					try{
						this.progressManager.ReportProgress(result, e.ProgressPercentage);
					}catch{
					}
				}
			};
			worker.RunWorkerCompleted += delegate{
				this.progressManager.Complete(result);
				CommandManager.InvalidateRequerySuggested();
			};
			
			var resultTab = new ResultTab(result, worker);
			this.resultTabs.Add(resultTab);
			this.resultTabControl.SelectedValue = resultTab;
			
			this.progressManager.Start(result);
			worker.Start();
			CommandManager.InvalidateRequerySuggested();
		}
		
		private void GrepFiles(SearchCondition cond){
			var result = new GrepResult(cond.Path, cond.Mask, cond.SearchOption, cond.Regex);
			var resultList = result.Matches;
			var worker = new FileListWorker(cond.Path, cond.Mask, cond.SearchOption);
			worker.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e){
				var files = e.UserState as string[];
				if(files != null){
					ThreadPool.QueueUserWorkItem(new WaitCallback(delegate{
						foreach(var file in files){
							try{
								var matches = Grep.Match(cond.Regex, file);
								this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate{
									foreach(var match in matches){
										resultList.Add(match);
									}
									try{
										this.progressManager.ReportProgress(result, e.ProgressPercentage);
									}catch{
									}
								}));
							}catch(IOException){
							}catch(UnauthorizedAccessException){
							}
						}
					}));
				}
			};
			worker.RunWorkerCompleted += delegate{
				this.progressManager.Complete(result);
				CommandManager.InvalidateRequerySuggested();
			};
			
			var resultTab = new ResultTab(result, worker);
			this.resultTabs.Add(resultTab);
			this.resultTabControl.SelectedValue = resultTab;
			
			this.progressManager.Start(result);
			worker.Start();
			CommandManager.InvalidateRequerySuggested();
		}
		
		#endregion
		
		#region イベント処理
		
		private void LoadedListener(object sender, RoutedEventArgs e){
			if(this.AeroGlassCompositionEnabled){
				this.SetAeroGlassTransparency();
			}
			this.searchButton.Focus();
		}
		
		private void AeroGlassCompositionChangedListener(object sender, AeroGlassCompositionChangedEvenArgs e){
			if(e.GlassAvailable){
				this.SetAeroGlassTransparency();
				this.InvalidateVisual();
			}
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
			e.CanExecute = this.resultTabControl.HasItems && (this.resultTabControl.SelectedIndex >= 0);
		}
		
		private void CloseTab_Executed(object sender, ExecutedRoutedEventArgs e){
			if(this.resultTabs[this.resultTabControl.SelectedIndex].Worker.IsBusy){
				this.resultTabs[this.resultTabControl.SelectedIndex].Worker.Stop();
			}
			this.resultTabs.RemoveAt(this.resultTabControl.SelectedIndex);
		}
		
		private void Search_CanExecute(object sender, CanExecuteRoutedEventArgs e){
			e.CanExecute = true;
		}
		
		private void Search_Executed(object sender, ExecutedRoutedEventArgs e){
			var cond = new SearchCondition();
			cond.Path = Environment.CurrentDirectory;
			cond.Mask = @"*.*";
			var form = new SearchForm(cond);
			form.Owner = this;
			if(form.ShowDialog().Value){
				cond = form.SearchCondition;
				if(cond.Regex == null){
					this.FindFiles(cond);
				}else{
					this.GrepFiles(cond);
				}
			}
		}
		
		private void Abort_CanExecute(object sender, CanExecuteRoutedEventArgs e){
			e.CanExecute = (this.resultTabControl.SelectedIndex >= 0) &&
			                this.resultTabs[this.resultTabControl.SelectedIndex].Worker.IsBusy;
		}
		
		private void Abort_Executed(object sender, ExecutedRoutedEventArgs e){
			this.resultTabs[this.resultTabControl.SelectedIndex].Worker.Stop();
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
			dialog.Title = "外部ツール編集";
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
		
		#endregion
		
		#region プロパティ
		
		public ProgressManager ProgressManager{
			get{
				return this.progressManager;
			}
		}
		
		#endregion
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
	
	public class TrimStringConverter : IValueConverter{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			var text = (string)value;
			var length = Int32.Parse((string)parameter);
			return ((text != null) && (length > 0) && (text.Length > length)) ? text.Substring(0, length) : text;
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			throw new NotImplementedException();
		}
	}
}