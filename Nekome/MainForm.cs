/*
	$Id$
*/
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.IO;
using System.Threading;
using Nekome.Search;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace Nekome{
	public partial class MainForm{
		private WindowSettings settings = new WindowSettings("MainForm");
		private ProgressManager progressManager = new ProgressManager();
		private ObservableCollection<object> results = new ObservableCollection<object>();
		
		public MainForm(){
			this.InitializeComponent();
			this.resultTabControl.ItemsSource = this.results;
			
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
		
		#region イベント処理
		
		private void LoadedListener(object sender, RoutedEventArgs e){
			if(this.AeroGlassCompositionEnabled){
				this.SetAeroGlassTransparency();
			}
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
		
		#endregion
		
		#region コマンド
		
		private void Close_CanExecute(object sender, CanExecuteRoutedEventArgs e){
			e.CanExecute = true;
		}
		
		private void Close_Executed(object sender, ExecutedRoutedEventArgs e){
			this.Close();
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
				if(cond.Regex == null){
					this.FindFiles(cond);
				}else{
					this.GrepFiles(cond);
				}
			}
		}
		
		#endregion
		
		#region 関数
		
		private void FindFiles(SearchCondition cond){
			var result = new FindResult(cond.Path, cond.Mask, cond.SearchOption);
			var resultList = result.Files;
			this.results.Add(result);
			this.resultTabControl.SelectedValue = result;
			this.progressManager.Start(result);
			
			ThreadPool.QueueUserWorkItem(new WaitCallback(delegate{
				var worker = new FileListWorker(cond.Path, cond.Mask, cond.SearchOption);
				worker.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e){
					var files = e.UserState as string[];
					if(files != null){
						this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate{
							foreach(var file in files){
								resultList.Add(file);
							}
							try{
								this.progressManager.ReportProgress(result, e.ProgressPercentage);
							}catch{
							}
						}));
					}
				};
				worker.RunWorkerCompleted += delegate{
					this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate{
						this.progressManager.Complete(result);
					}));
				};
				worker.Start();
			}));
			
		}
		
		private void GrepFiles(SearchCondition cond){
			var result = new GrepResult(cond.Path, cond.Mask, cond.SearchOption, cond.Regex);
			var resultList = result.Matches;
			this.results.Add(result);
			this.resultTabControl.SelectedValue = result;
			this.progressManager.Start(result);
			
			ThreadPool.QueueUserWorkItem(new WaitCallback(delegate{
				var worker = new FileListWorker(cond.Path, cond.Mask, cond.SearchOption);
				worker.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e){
					var files = e.UserState as string[];
					if(files != null){
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
					}
				};
				worker.RunWorkerCompleted += delegate{
					this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate{
						this.progressManager.Complete(result);
					}));
				};
				worker.Start();
			}));
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
			if(item is FindResult){
				return (DataTemplate)frm.FindResource("FindResultContentTemplate");
			}else if(item is GrepResult){
				return (DataTemplate)frm.FindResource("GrepResultContentTemplate");
			}else{
				throw new InvalidOperationException();
			}
		}
	}
	
	public class ResultItemTemplateSelector : DataTemplateSelector{
		public override DataTemplate SelectTemplate(object item, DependencyObject container){
			var frm = (FrameworkElement)container;
			if(item is FindResult){
				return (DataTemplate)frm.FindResource("FindResultItemTemplate");
			}else if(item is GrepResult){
				return (DataTemplate)frm.FindResource("GrepResultItemTemplate");
			}else{
				throw new InvalidOperationException();
			}
		}
	}
}