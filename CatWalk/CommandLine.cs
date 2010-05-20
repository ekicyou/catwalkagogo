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
		private IDictionary<string, string[]> arguments;
		
		public CommandLine(string[] options) : this(options, GetArguments(), StringComparer.OrdinalIgnoreCase){}
		public CommandLine(string[] options, string[] arguments) : this(options, arguments, StringComparer.OrdinalIgnoreCase){}
		public CommandLine(string[] options, StringComparer comparer) : this(options, null, comparer){}
		
		public CommandLine(string[] options, string[] arguments, StringComparer comparer){
			options.ThrowIfNull("options");
			arguments.ThrowIfNull("arguments");
			comparer.ThrowIfNull("comparer");
			
			var dicOption = new PrefixDictionary<List<string>>();
			foreach(var option in options){
				dicOption.Add(option, new List<string>());
			}

			var key = "";
			foreach(var arg in arguments){
				if(arg.StartsWith("-")){
					key = arg.Substring(1);
				}else{
					var founds = dicOption.Search(key).GetEnumerator();
					if(founds.MoveNext()){
						var entry = founds.Current;
						if(!founds.MoveNext()){
							entry.Value.Add(arg);
						}
					}
				}
			}

			this.arguments = dicOption.ToDictionary(entry => entry.Key, entry => entry.Value.ToArray());
		}
		
		private static string[] GetArguments(){
			return Environment.GetCommandLineArgs().Skip(1).ToArray();
		}

		public IDictionary<string, string[]> Arguments{
			get{
				return this.arguments;
			}
		}
	}
}