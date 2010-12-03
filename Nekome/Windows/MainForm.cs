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
using System.Net;
using Nekome.Search;
using CatWalk;
using CatWalk.Net;
using CatWalk.Windows;

namespace Nekome.Windows{
	public partial class MainForm : Window{
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
				try {
					Environment.CurrentDirectory = cond.Path;
					if(String.IsNullOrEmpty(cond.Pattern)) {
						this.FindFiles(cond);
					} else {
						this.GrepFiles(cond);
					}
				} catch(Exception ex) {
					MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}
		
		public void FindFiles(SearchCondition cond){
			var result = new FindResult(cond);
			var resultList = result.Files;
			var worker = new FileListWorker(cond.Path, cond.Mask, cond.SearchOption);
			if(cond.ExcludingTargets.HasFlag(ExcludingTargets.Search)){
				worker.ExcludingMask = cond.ExcludingMask;
			}
			worker.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e){
				var stream = e.UserState as IEnumerable<string>;
				if(stream != null){
					var files = stream.ToArray();
					foreach(var file in files){
						if(!Directory.Exists(file)){
							resultList.Add(file);
						}
					}
					if(files.Length > 0){
						this.progressManager.ProgressMessage = "Searching " + files[0].Substring(0, files[0].LastIndexOf("\\") + 1) + " ...";
					}
					this.progressManager.ReportProgress(result, (double)e.ProgressPercentage / 100d);
				}else{
					//MessageBox.Show(e.UserState.ToString());
				}
			};
			worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e){
				this.progressManager.Complete(result);
				CommandManager.InvalidateRequerySuggested();
				if(!e.Cancelled){
					this.progressManager.ProgressMessage = "File search is completed. (" + e.Result.ToString() + ")";
				}else{
					this.progressManager.ProgressMessage = "File search is cancelled.";
				}
			};
			
			var resultTab = new ResultTab(result, worker);
			this.resultTabs.Add(resultTab);
			this.resultTabControl.SelectedValue = resultTab;
			
			this.progressManager.Start(result);
			worker.Start();
			CommandManager.InvalidateRequerySuggested();
		}
		
		public void GrepFiles(SearchCondition cond){
			var result = new GrepResult(cond);
			var resultList = result.Matches;
			var worker = new FileListWorker(cond.Path, cond.Mask, cond.SearchOption);
			if(cond.ExcludingTargets.HasFlag(ExcludingTargets.Grep)){
				worker.ExcludingMask = cond.ExcludingMask;
			}
			var regex = cond.GetRegex();
			var threads = 0;
			worker.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e){
				var stream = e.UserState as IEnumerable<string>;
				if(stream != null){
					Interlocked.Increment(ref threads);
					ThreadPool.QueueUserWorkItem(new WaitCallback(delegate{
						foreach(var file in stream){
							try{
								var matches = Grep.Match(regex, file);
								this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate{
									foreach(var match in matches){
										resultList.Add(match);
									}
									this.progressManager.ProgressMessage = "Greping " + file + " ...";
								}));
							}catch(IOException){
							}catch(UnauthorizedAccessException){
							}
						}
						Interlocked.Decrement(ref threads);
						if((threads == 0) && !worker.IsBusy){
							this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate{
								this.progressManager.ProgressMessage = "Grep is completed. (" + worker.ElapsedTime.ToString() + ")";
							}));
						}
					}));
					this.progressManager.ReportProgress(result, (double)e.ProgressPercentage / 100d);
				}else{
					// exception
				}
			};
			worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e){
				this.progressManager.Complete(result);
				CommandManager.InvalidateRequerySuggested();
				if(threads == 0){
					if(!e.Cancelled){
						this.progressManager.ProgressMessage = "Grep is completed. (" + e.Result.ToString() + ")";
					}else{
						this.progressManager.ProgressMessage = "Grep is cancelled.";
					}
				}
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
				if(this.resultTabs[this.resultTabControl.SelectedIndex].Worker.IsBusy){
					this.resultTabs[this.resultTabControl.SelectedIndex].Worker.Stop();
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
			cond.SearchOption = Program.Settings.SearchOption;
			this.FindDialog(cond);
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
			dialog.Title = "Edit Tools";
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
		}
		
		private void CheckUpdate_CanExecute(object sender, CanExecuteRoutedEventArgs e){
			e.CanExecute = true;
		}
		
		private void CheckUpdate_Executed(object sender, ExecutedRoutedEventArgs e){
			UpdatePackage[] packages = null;
			try{
				packages = Program.GetUpdates();
			}catch(WebException ex){
				MessageBox.Show("Faild to check updates.\n" + ex.Message, "Update", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			if(packages != null && packages.Length > 0){
				var package = packages[0];
				if(MessageBox.Show(
					"Version " + package.Version.ToString() + " is found。Do you install this?", 
					"Update",
					MessageBoxButton.YesNo) == MessageBoxResult.Yes){
					try{
						Program.Update(package);
						}catch(WebException ex){
							MessageBox.Show("Faild to download the installer.\n" + ex.Message,
								"Update",
								MessageBoxButton.OK,
								MessageBoxImage.Error);
						}
				}
			}else{
				MessageBox.Show("No updates are available.");
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