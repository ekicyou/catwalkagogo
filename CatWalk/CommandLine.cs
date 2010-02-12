/*
	$Id$
*/
using System;
using System.Collections.Generic;

namespace Hiyoko.Utilities{
	/// <summary>
	/// コマンドライン引数を解析するクラス。
	/// </summary>
	public class CommandLine{
		private Dictionary<string, string> options;
		private string[] files;
		
		private static string[] commandLine = null;
		
		public CommandLine() : this(null, StringComparer.OrdinalIgnoreCase){}
		public CommandLine(string[] arguments) : this(arguments, StringComparer.OrdinalIgnoreCase){}
		public CommandLine(StringComparer comparer) : this(null, comparer){}
		public CommandLine(string[] arguments, StringComparer comparer){
			if(comparer == null){
				throw new ArgumentNullException("comparer");
			}
			if(commandLine == null){
				string[] cmdLine = Environment.GetCommandLineArgs();
				int length = cmdLine.Length - 1;
				commandLine = new string[length];
				Array.Copy(cmdLine, 1, commandLine, 0, length);
			}
			this.options = new Dictionary<string, string>(comparer);
			
			this.Parse();
		}
		
		private void Parse(string[] arguments){
			this.options.Clear();
			List<string> fileList = new List<string>();
			foreach(string arg in arguments){
				if((arg[0] == '/') && (arg.Length > 1)){
					string key;
					string prm = "";
					int n = arg.IndexOf(":");
					if(n != -1){
						key = arg.Substring(1, n - 1);
						prm = arg.Substring(n + 1);
					}else{
						key = arg.Substring(1);
					}
					if(!(this.options.ContainsKey(key))){
						this.options.Add(key, prm);
					}
				}else{
					fileList.Add(arg);
				}
			}
			files = fileList.ToArray();
		}
		
		public Dictionary<string, string> Options{
			get{
				return this.options;
			}
		}
		
		public string[] Files{
			get{
				return this.files;
			}
		}
	}
}