/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CatWalk.Collections;

namespace CatWalk{
	/// <summary>
	/// コマンドライン引数を解析するクラス。
	/// </summary>
	public static class CommandLineParser{
		public static void Parse(object option){
			Parse(option, GetArguments(), StringComparer.OrdinalIgnoreCase);
		}
		public static void Parse(object option, string[] arguments){
			Parse(option, arguments, StringComparer.OrdinalIgnoreCase);
		}
		public static void Parse(object option, StringComparer comparer){
			Parse(option, null, comparer);
		}
		public static void Parse(object option, string[] arguments, StringComparer comparer){
			option.ThrowIfNull("option");
			arguments.ThrowIfNull("arguments");
			comparer.ThrowIfNull("comparer");
			
			var dicOption = new PrefixDictionary<Action<string>>(new CustomComparer<char>(delegate(char x, char y){
				return comparer.Compare(x.ToString(), y.ToString());
			}));
			var list = new List<string>();
			PropertyInfo listProp = null;

			foreach(var prop in option.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
			                          .Where(prop => prop.CanWrite && prop.CanRead)){
				var thisProp = prop;
				if(prop.PropertyType == typeof(string)){
					dicOption.Add(prop.Name, new Action<string>(delegate(string arg){
						thisProp.SetValue(option, arg, null);
					}));
				}else if(prop.PropertyType == typeof(Nullable<bool>)){
					dicOption.Add(prop.Name, new Action<string>(delegate(string arg){
						thisProp.SetValue(option, (arg.IsNullOrEmpty() || arg == "+"), null);
					}));
				}else if(prop.PropertyType == typeof(string[])){
					listProp = thisProp;
				}
			}

			foreach(var arg in arguments){
				if(arg.StartsWith("/")){
					var a = arg.Substring(1).Split(':');
					string value;
					string key;
					if(a.Length == 1){
						int last = arg.Length - 1;
						if(arg[last] == '+' || arg[last] == '-'){
							key = arg.Substring(0, last);
							value = arg[last].ToString();
						}else{
							key = arg;
							value = "";
						}
					}else{
						key = a[0];
						value = a[1];
					}

					var founds = dicOption.Search(key).GetEnumerator();
					if(founds.MoveNext()){
						var entry = founds.Current;
						if(!founds.MoveNext()){
							entry.Value(value);
						}
					}
				}else{
					list.Add(arg);
				}
			}

			listProp.SetValue(option, list.ToArray(), null);
		}
		
		private static string[] GetArguments(){
			return Environment.GetCommandLineArgs().Skip(1).ToArray();
		}
	}
}