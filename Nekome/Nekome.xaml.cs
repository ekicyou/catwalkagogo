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
using System.Resources;
using System.Configuration;
using CatWalk;
using CatWalk.Net;
using CatWalk.Windows;
using Nekome.Search;
using Nekome.Windows;

namespace Nekome{
	using Prop = Nekome.Properties;

	public partial class Program : Application{
		private MainForm mainForm;
		private ApplicationSettings settings;
		private ObservableCollection<ExternalTool> grepTools;
		private ObservableCollection<ExternalTool> findTools;
		private JumpList jumpList;
		//private ResourceManager resourceManager;
		
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
			base.OnStartup(e);
			var cmdParser = new CommandLineParser("/", ":", StringComparer.OrdinalIgnoreCase);
			var cmdOption = cmdParser.Parse<CommandLineOption>(e.Args);

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
					this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate{
						var cond = GetSearchCondition(option);
						if(this.MainWindow != null){
							((MainForm)this.MainWindow).FindDialog(cond);
						}
					}));
				}));
				ApplicationProcess.Actions.Add("FindImmediately", new Action<CommandLineOption>(delegate(CommandLineOption option){
					this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate{
						var cond = GetSearchCondition(option);
						if(this.MainWindow != null){
							if(cond.Pattern == null){
								this.mainForm.FindFiles(cond);
							}else{
								this.mainForm.GrepFiles(cond);
							}
						}
					}));
				}));
				
				this.settings = (ApplicationSettings)SettingsBase.Synchronized(new ApplicationSettings());
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
				openTool.Name = "_Open";
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
						this.mainForm.Show();
						if(String.IsNullOrEmpty(cond.Pattern)){
							this.mainForm.FindFiles(cond);
						}else{
							this.mainForm.GrepFiles(cond);
						}
					}else{
						var form = new SearchForm(cond);
						form.ShowInTaskbar = true;
						if(form.ShowDialog().Value){
							this.mainForm.Show();
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
				}else{
					this.mainForm.Show();
				}

				// アップデートチェック
				if(Program.Settings.IsCheckUpdatesOnStartUp &&
					((DateTime.Now - Program.Settings.LastCheckUpdatesDateTime).Days > 0)){
					ThreadPool.QueueUserWorkItem(new WaitCallback(delegate{
						UpdatePackage[] packages = null;
						try{
							packages = Program.GetUpdates(false);
						}catch(WebException){
						}
						if(packages != null && packages.Length > 0){
							var package = packages[0];
							if(MessageBox.Show(
								String.Format(Prop::Resources.FoundUpdateDialog, package.Version), 
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
						}
					}));
				}
			}
		}
		
		protected override void OnExit(ExitEventArgs e){
			base.OnExit(e);
			if(this.settings != null){
				this.settings.FindTools = findTools.ToArray();
				this.settings.GrepTools = grepTools.ToArray();
				this.settings.Save();
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
				cond.FileSearchOption = (cmdOption.Recursive.Value) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
			}
			if(cmdOption.Mask != null){
				cond.Mask = cmdOption.Mask;
			}
			if(cmdOption.Files.Length > 0){
				cond.Path = cmdOption.Files[0];
			}
			return cond;
		}
		
		public static UpdatePackage[] GetUpdates(){
			return GetUpdates(true);
		}

		public static UpdatePackage[] GetUpdates(bool isShowProgress){
			var progWin = (isShowProgress) ? new ProgressWindow() : null;
			try{
				if(isShowProgress){
					progWin.Message = "Checking Updates";
					progWin.Owner = MainForm;
					progWin.IsIndeterminate = true;
					progWin.Show();
				}
				var currVer = new Version(Assembly.GetEntryAssembly().GetInformationalVersion());
				var updater = new AutoUpdater(new Uri("http://nekoaruki.com/updater/nekome/packages.xml"));
				Program.Settings.LastCheckUpdatesDateTime = DateTime.Now;
				return updater.CheckUpdates().Where(p => p.InformationalVersion > currVer).OrderByDescending(p => p.Version).ToArray();
			}finally{
				if(isShowProgress){
					progWin.Close();
				}
			}
		}
		
		public static void Update(UpdatePackage package){
			Application.Current.Dispatcher.BeginInvoke(new Action(delegate{
				var progress = new ProgressWindow();
				progress.Message = "Downloading Update Files.";
				progress.IsIndeterminate = false;
				progress.Owner = MainForm;
				progress.Show();
			
				package.DownloadInstallerAsync(delegate(object sender, DownloadProgressChangedEventArgs e2){
					Application.Current.Dispatcher.Invoke(new Action(delegate{
						progress.Value = (double)e2.ProgressPercentage;
					}));
				}, delegate(object sender, AsyncCompletedEventArgs e2){
					Application.Current.Dispatcher.Invoke(new Action(delegate{
						var file = (string)e2.UserState;
						progress.Close();
						MessageBox.Show("Starting Installer.");
						Process.Start(file);
						Application.Current.Shutdown();
					}));
				});
			}));
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