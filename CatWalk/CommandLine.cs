/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using CatWalk.Collections;

namespace CatWalk{
	/// <summary>
	/// コマンドライン引数を解析するクラス。
	/// </summary>
	public class CommandLine{
		private PrefixDictionary<string[]> options;
		private string[] files;
		
		public CommandLine() : this(null, StringComparer.OrdinalIgnoreCase){}
		public CommandLine(string[] arguments) : this(arguments, StringComparer.OrdinalIgnoreCase){}
		public CommandLine(StringComparer comparer) : this(null, comparer){}
		
		public CommandLine(string[] arguments, StringComparer comparer){
			if(comparer == null){
				throw new ArgumentNullException("comparer");
			}
			
			if(arguments == null){
				string[] cmdLine = Environment.GetCommandLineArgs();
				int length = cmdLine.Length - 1;
				arguments = new string[length];
				Array.Copy(cmdLine, 1, arguments, 0, length);
			}
			
			// 解析
			var files = new List<string>();
			var options = new List<Tuple<string, string>>();
			foreach(var arg in arguments.Where(s => !s.IsNullOrEmpty())){
				if(arg.StartsWith("/")){
					var option = arg.Substring(1);
					if(!option.IsNullOrEmpty()){
						var idx = option.IndexOf(":");
						if(idx >= 0){
							var value = option.Substring(idx + 1);
							options.Add(new Tuple<string, string>(option, value));
						}else{
							options.Add(new Tuple<string, string>(option, null));
						}
					}
				}else{
					files.Add(arg);
				}
			}
			
			this.files = files.ToArray();
			this.options = new PrefixDictionary<string[]>(
				options.GroupBy(opt => opt.Item1, opt => opt.Item2)
				       .ToDictionary(grp => grp.Key, grp => grp.ToArray()));
		}
		
		public PrefixDictionary<string[]> Options{
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