/*
	$Id$
*/
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.IO;
using System.Windows.Shell;
using System.Text.RegularExpressions;
using System.Reflection;
using System.ComponentModel;
using System.Net;
using System.Threading;
using CatWalk;
using CatWalk.Net;
using CatWalk.Windows;
using Nekome.Search;
using Nekome.Windows;

namespace Nekome{
	public partial class Program : Application{
		private MainForm mainForm;
		private ApplicationSettings settings;
		private ObservableCollection<ExternalTool> grepTools;
		private ObservableCollection<ExternalTool> findTools;
		private JumpList jumpList;

		[Serializable]
		private class CommandLineOption{
			public bool? Kill{get; set;}
			public string Mask{get; set;}
			public string[] Files{get; set;}
			public bool? Recursive{get; set;}
			public bool? Immediately{get; set;}
			public string Pattern{get; set;}
			public bool? Regex{get; set;}
			public bool? IgnoreCase{get; set;}
		}

		protected override void OnStartup(StartupEventArgs e){
			var cmdOption = new CommandLineOption();
			CommandLineParser.Parse(cmdOption, e.Args, StringComparer.OrdinalIgnoreCase);

			// プロセス間通信
			if(!ApplicationProcess.IsFirst){
				if(cmdOption.Kill != null && cmdOption.Kill.Value){
					ApplicationProcess.InvokeRemote("Kill");
				}else{
					ApplicationProcess.InvokeRemote("Show");
				}
				if(cmdOption.Files.Length > 0){
					if(cmdOption.Immediately != null && cmdOption.Immediately.Value){
						ApplicationProcess.InvokeRemote("FindImmediately", cmdOption);
					}else{
						try{
							ApplicationProcess.InvokeRemote("Find", cmdOption);
						}catch(Exception ex){
							MessageBox.Show(ex.ToString());
						}
					}
				}
				this.Shutdown();
			}else{ // 通常起動
				if(cmdOption.Kill != null && cmdOption.Kill.Value){
					this.Shutdown();
				}

				ApplicationProcess.Actions.Add("Show", new Action(delegate{
					this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate{
						if(this.MainWindow != null){
							((MainForm)this.MainWindow).ShowWindow();
						}
					}));
				}));
				ApplicationProcess.Actions.Add("Kill", new Action(delegate{
					this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate {
						if(this.MainWindow != null) {
							((MainForm)this.MainWindow).Close();
						}
					}));
				}));
				ApplicationProcess.Actions.Add("Find", new Action<CommandLineOption>(delegate(CommandLineOption option){
					var cond = GetSearchCondition(option);
					this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate{
						if(this.MainWindow != null){
							((MainForm)this.MainWindow).FindDialog(cond);
						}
					}));
				}));
				ApplicationProcess.Actions.Add("FindImmediately", new Action<CommandLineOption>(delegate(CommandLineOption option){
					var cond = GetSearchCondition(option);
					this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate{
						if(this.MainWindow != null){
							if(cond.Pattern == null){
								this.mainForm.FindFiles(cond);
							}else{
								this.mainForm.GrepFiles(cond);
							}
						}
					}));
				}));
				
				this.settings = new ApplicationSettings();
				this.settings.UpgradeOnce();
				
				// 外部ツール
				this.findTools = new ObservableCollection<ExternalTool>();
				if(this.settings.FindTools != null){
					foreach(var tool in this.settings.FindTools){
						this.findTools.Add(tool);
					}
				}
				this.grepTools = new ObservableCollection<ExternalTool>();
				if(this.settings.GrepTools != null){
					foreach(var tool in this.settings.GrepTools){
						this.grepTools.Add(tool);
					}
				}
				var openTool = new ExternalTool();
				openTool.FileName = "%p";
				openTool.Name = "&Open";
				openTool.Key = Key.Return;
				if(this.grepTools.Count == 0){
					this.grepTools.Add(openTool);
				}
				if(this.findTools.Count == 0){
					this.findTools.Add(openTool);
				}

				this.mainForm = new MainForm();
				this.jumpList = JumpList.GetJumpList(this);
				if(this.jumpList == null){
					this.jumpList = new JumpList();
					this.jumpList.ShowRecentCategory = true;
					this.jumpList.ShowFrequentCategory = true;
					this.jumpList.Apply();
				}

				if(cmdOption.Files.Length > 0){
					var cond = GetSearchCondition(cmdOption);
					if(cmdOption.Immediately != null && cmdOption.Immediately.Value){
						if(String.IsNullOrEmpty(cond.Pattern)){
							this.mainForm.FindFiles(cond);
						}else{
							this.mainForm.GrepFiles(cond);
						}
					}else{
						var form = new SearchForm(cond);
						if(form.ShowDialog().Value){
							cond = form.SearchCondition;
							if(String.IsNullOrEmpty(cond.Pattern)){
								this.mainForm.FindFiles(cond);
							}else{
								this.mainForm.GrepFiles(cond);
							}
						}else{
							this.Shutdown();
						}
					}
				}
				this.mainForm.Show();

				// アップデートチェック
				if(Program.Settings.IsCheckUpdatesOnStartUp){
					ThreadPool.QueueUserWorkItem(new WaitCallback(delegate{
						UpdatePackage[] packages = null;
						try{
							packages = Program.GetUpdates(false);
						}catch(WebException){
						}
						if(packages != null && packages.Length > 0){
							var package = packages[0];
							if(MessageBox.Show(
								"Version " + package.Version.ToString() + " is found. Do you install this?", 
								"update",
								MessageBoxButton.YesNo) == MessageBoxResult.Yes){
								try{
									Program.Update(package);
								}catch(WebException ex){
									MessageBox.Show("Faild to download the installer.\n" + ex.Message,
										"update",
										MessageBoxButton.OK,
										MessageBoxImage.Error);
								}
							}
						}
					}));
				}
			}
		}
		
		#region 関数
		
		private static SearchCondition GetSearchCondition(CommandLineOption cmdOption){
			var cond = SearchCondition.GetDefaultCondition();
			if(cmdOption.IgnoreCase != null){
				cond.IsIgnoreCase = cmdOption.IgnoreCase.Value;
			}
			if(cmdOption.Regex != null){
				cond.IsUseRegex = cmdOption.Regex.Value;
			}
			if(cmdOption.Pattern != null){
				cond.Pattern = cmdOption.Pattern;
			}
			if(cmdOption.Recursive != null){
				cond.SearchOption = (cmdOption.Recursive.Value) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
			}
			if(cmdOption.Mask != null){
				cond.Mask = cmdOption.Mask;
			}
			if(cmdOption.Files.Length > 0){
				cond.Path = cmdOption.Files[0];
			}
			return cond;
		}

		protected override void OnExit(ExitEventArgs e){
			if(this.settings != null){
				this.settings.FindTools = findTools.ToArray();
				this.settings.GrepTools = grepTools.ToArray();
				this.settings.Save();
			}
		}
		
		public static UpdatePackage[] GetUpdates(){
			return GetUpdates(true);
		}

		public static UpdatePackage[] GetUpdates(bool isShowProgress){
			var progWin = (isShowProgress) ? new ProgressWindow() : null;
			try{
				if(isShowProgress){
					progWin.Message = "更新を確認しています。";
					progWin.Owner = MainForm;
					progWin.IsIndeterminate = true;
					progWin.Show();
				}
				var currVer = Assembly.GetEntryAssembly().GetName().Version;
				var updater = new AutoUpdater(new Uri("http://nekoaruki.com/updater/nekome/packages.xml"));
				return updater.CheckUpdates().Where(p => p.Version > currVer).OrderByDescending(p => p.Version).ToArray();
			}finally{
				if(isShowProgress){
					progWin.Close();
				}
			}
		}
		
		public static void Update(UpdatePackage package){
			var progress = new ProgressWindow();
			progress.Message = "更新ファイルダウンロード中";
			progress.IsIndeterminate = false;
			progress.Owner = MainForm;
			progress.Show();
			
			package.DownloadInstallerAsync(delegate(object sender, DownloadProgressChangedEventArgs e2){
				Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate{
					progress.Value = (double)e2.ProgressPercentage;
				}));
			}, delegate(object sender, AsyncCompletedEventArgs e2){
				Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate{
					var file = (string)e2.UserState;
					progress.Close();
					MessageBox.Show("インストーラーを実行します。");
					Process.Start(file);
					Application.Current.Shutdown();
				}));
			});
		}
		
		#endregion
		
		#region プロパティ
		
		public static MainForm MainForm{
			get{
				Program prog = Application.Current as Program;
				return (prog != null) ? prog.mainForm : null;
			}
		}
		
		public static ApplicationSettings Settings{
			get{
				Program prog = Application.Current as Program;
				return (prog != null) ? prog.settings : null;
			}
		}
		
		public static ObservableCollection<ExternalTool> FindTools{
			get{
				Program prog = Application.Current as Program;
				return (prog != null) ? prog.findTools : null;
			}
		}
		
		public static ObservableCollection<ExternalTool> GrepTools{
			get{
				Program prog = Application.Current as Program;
				return (prog != null) ? prog.grepTools : null;
			}
		}

		public static JumpList JumpList{
			get{
				Program prog = Application.Current as Program;
				return (prog != null) ? prog.jumpList : null;
			}
		}
		
		#endregion
	}
}