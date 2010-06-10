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
using CatWalk;
using Nekome.Search;

namespace Nekome{
	public partial class Program : Application{
		private MainForm mainForm;
		private ApplicationSettings settings;
		private ObservableCollection<ExternalTool> grepTools;
		private ObservableCollection<ExternalTool> findTools;

		private class CommandLineOption{
			public bool? Exit{get; set;}
			public string Mask{get; set;}
			public string[] Files{get; set;}
			public bool? Recursive{get; set;}
			public bool? Immediately{get; set;}
			public string Word{get; set;}
		}

		protected override void OnStartup(StartupEventArgs e){
			var cmdOption = new CommandLineOption();
			CommandLineParser.Parse(cmdOption, e.Args, StringComparer.OrdinalIgnoreCase);
			if(!ApplicationProcess.IsFirst){
				if(cmdOption.Exit != null && cmdOption.Exit.Value){
					ApplicationProcess.InvokeRemote("Exit");
				}else{
					ApplicationProcess.InvokeRemote("Show");
				}
				if(cmdOption.Files.Length > 0){
					var cond = GetSearchCondition(cmdOption);
					if(cmdOption.Immediately != null && cmdOption.Immediately.Value){
						ApplicationProcess.InvokeRemote("FindImmediately", cond);
					}else{
						ApplicationProcess.InvokeRemote("Find", cond);
					}
				}
				this.Shutdown();
			}else{
				if(cmdOption.Exit != null && cmdOption.Exit.Value){
					this.Shutdown();
				}

				ApplicationProcess.Actions.Add("Show", new Action(delegate{
					this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate{
						if(this.MainWindow != null){
							((MainForm)this.MainWindow).ShowWindow();
						}
					}));
				}));
				ApplicationProcess.Actions.Add("Exit", new Action(delegate{
					this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate {
						if(this.MainWindow != null) {
							((MainForm)this.MainWindow).Close();
						}
					}));
				}));
				ApplicationProcess.Actions.Add("Find", new Action<SearchCondition>(delegate(SearchCondition cond){
					this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate{
						if(this.MainWindow != null){
							((MainForm)this.MainWindow).FindDialog(cond);
						}
					}));
				}));
				ApplicationProcess.Actions.Add("FindImmediately", new Action<SearchCondition>(delegate(SearchCondition cond){
					this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate{
						if(this.MainWindow != null){
							if(cond.Regex == null){
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
				openTool.FileName = "%P";
				openTool.Name = "開く(&O)";
				openTool.Key = Key.Return;
				if(this.grepTools.Count == 0){
					this.grepTools.Add(openTool);
				}
				if(this.findTools.Count == 0){
					this.findTools.Add(openTool);
				}

				this.mainForm = new MainForm();
				this.mainForm.Show();

				if(cmdOption.Files.Length > 0){
					var cond = GetSearchCondition(cmdOption);
					if(cmdOption.Immediately != null && cmdOption.Immediately.Value){
						if(cond.Regex == null){
							this.mainForm.FindFiles(cond);
						}else{
							this.mainForm.GrepFiles(cond);
						}
					}else{
						var form = new SearchForm(cond);
						if(form.ShowDialog().Value){
							if(cond.Regex == null){
								this.mainForm.FindFiles(cond);
							}else{
								this.mainForm.GrepFiles(cond);
							}
						}else{
							this.Shutdown();
						}
					}
				}
			}
		}
		
		private static SearchCondition GetSearchCondition(CommandLineOption cmdOption){
			var cond = new SearchCondition();
			cond.Path = cmdOption.Files.Concat(new string[]{Environment.CurrentDirectory}).First();
			cond.Mask = (cmdOption.Mask != null) ? cmdOption.Mask : Program.Settings.Mask;
			cond.SearchOption = (cmdOption.Recursive != null) ? ((cmdOption.Recursive.Value) ? SearchOption.AllDirectories
			                                                                                 : SearchOption.TopDirectoryOnly)
			                                                  : Program.Settings.SearchOption;
			return cond;
		}

		protected override void OnExit(ExitEventArgs e){
			if(this.settings != null){
				this.settings.FindTools = findTools.ToArray();
				this.settings.GrepTools = grepTools.ToArray();
				this.settings.Save();
			}
		}

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
	}
}