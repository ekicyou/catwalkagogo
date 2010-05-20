/*
	$Id$
*/
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using CatWalk;

namespace Nekome{
	public partial class Program : Application{
		private MainForm mainForm;
		private ApplicationSettings settings;
		private ObservableCollection<ExternalTool> grepTools;
		private ObservableCollection<ExternalTool> findTools;
		
		protected override void OnStartup(StartupEventArgs e){
			var cmdline = new CommandLine(new string[]{"Exit"}, e.Args);
			if(!ApplicationProcess.IsFirst){
				if(cmdline.Arguments.ContainsKey("Exit")){
					ApplicationProcess.InvokeRemote("Exit");
				}else{
					ApplicationProcess.InvokeRemote("Show");
				}
				this.Shutdown();
			}else{
				if(cmdline.Arguments.ContainsKey("Exit")){
					this.Shutdown();
				}

				ApplicationProcess.Actions.Add("Show", new Action(delegate{
					this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate{
						if(this.MainWindow != null){
							((MainForm)this.MainWindow).ShowWindow();
						}
					}));
				}));
				ApplicationProcess.Actions.Add("Exit", new Action(delegate {
					this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate {
						if(this.MainWindow != null) {
							((MainForm)this.MainWindow).Close();
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
				
				cmdline.Arguments.ContainsKey

				this.mainForm = new MainForm();
				this.mainForm.Show();
			}
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