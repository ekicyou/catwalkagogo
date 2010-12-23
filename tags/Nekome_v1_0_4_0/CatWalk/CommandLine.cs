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

		/// <summary>
		/// コマンドライン解析を実行する。
		/// </summary>
		/// <param name="option">コマンドラインの名前と値を定義したオブジェクト</param>
		/// <param name="arguments">コマンドライン</param>
		/// <param name="comparer">使用するStringComparer</param>
		/// <remarks>
		/// option引数のオブジェクトには複数の任意のstring型とbool?型プロパティを定義します。
		/// これらのプロパティのユニークな先頭数文字がコマンドラインオプション名になります。
		/// bool?型のプロパティはオプションのOn/Off/省略を取得できます(/name(+|-))。
		/// string型のプロパティはオプションの文字列を取得できます(/name:value)。
		/// 
		/// また、一つのstring[]型のプロパティを定義することで、オプション以外の文字列(ファイル名など)を取得できます。
		/// 
		/// <code>
		/// class CommandLineOption{
		/// 	public string[] Files{get; set;}
		/// 	public string Mask{get; set;} // /m:ファイルマスク
		/// 	public bool? Recursive{get; set;} // /rec(+|-)
		/// 	public bool? Regex{get; set;} // /reg(+|-)
		/// }
		/// </code>
		/// <code>
		/// var option = new CommandLineOption();
		/// CommandLineParser.Parse(option, args, StringComparer.OrdinalIgnoreCase);
		/// </code>
		/// </remarks>
		public static void Parse(object option, string[] arguments, StringComparer comparer){
			option.ThrowIfNull("option");
			arguments.ThrowIfNull("arguments");
			comparer.ThrowIfNull("comparer");
			
			var comp = new CustomComparer<char>((x, y) => comparer.Compare(x.ToString(), y.ToString()));
			var dicOption = new PrefixDictionary<Tuple<PropertyInfo, Action<string>>>(comp);
			var altOptions = new Dictionary<PropertyInfo, AlternativeCommandLineOptionNameAttribute>();
			var list = new List<string>();
			PropertyInfo listProp = null;

			foreach(var prop in option.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
			                          .Where(prop => prop.CanWrite && prop.CanRead)){
				var thisProp = prop;
				if(prop.PropertyType == typeof(string)){
					dicOption.Add(prop.Name, new Tuple<PropertyInfo, Action<string>>(
						thisProp,
						new Action<string>(delegate(string arg){
							thisProp.SetValue(option, arg, null);
						})));
				}else if(prop.PropertyType == typeof(Nullable<bool>)){
					dicOption.Add(prop.Name, new Tuple<PropertyInfo, Action<string>>(
						thisProp,
						new Action<string>(delegate(string arg){
							thisProp.SetValue(option, (arg.IsNullOrEmpty() || arg == "+"), null);
						})));
				}else if(prop.PropertyType == typeof(string[])){
					if(listProp != null){
						throw new ArgumentException("option");
					}
					listProp = thisProp;
				}
				var attrs = prop.GetCustomAttributes(typeof(AlternativeCommandLineOptionNameAttribute), true)
					.Cast<AlternativeCommandLineOptionNameAttribute>()
					.ToArray();
				if(attrs.Length > 0){
					altOptions.Add(prop, attrs[0]);
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

					var founds = dicOption.Search(key).ToArray();
					if(founds.Length > 1){
						var attrs = founds.Where(pair => altOptions.ContainsKey(pair.Value.Item1))
						                  .Select(pair => new Tuple<AlternativeCommandLineOptionNameAttribute, Action<string>>(altOptions[pair.Value.Item1], pair.Value.Item2));
						var founds2 = attrs.Where(attr => attr.Item1.Name.StartsWith(key, attr.Item1.StringComparison))
						                   .Select(attr => attr.Item2).ToArray();
						if(founds2.Length == 1){
							founds2[0](value);
						}
					}else if(founds.Length == 1){
						founds[0].Value.Item2(value);
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

		/// <summary>
		/// コマンドライン引数に使用する文字列をエスケープする。
		/// </summary>
		/// <param name="text">エスケープする文字列</param>
		/// <returns>エスケープ済みの文字列</returns>
		/// <remarks>
		/// " を \" に、% を ^% に置き換えます。
		/// </remarks>
		public static string Escape(string text){
			return text.Replace("\"", "\\\"").Replace("%", "^%");
		}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class AlternativeCommandLineOptionNameAttribute : Attribute{
		public string Name{get; set;}
		public StringComparison StringComparison{get; set;}

		public AlternativeCommandLineOptionNameAttribute(string name){
			this.Name = name;
			this.StringComparison = StringComparison.Ordinal;
		}

		public AlternativeCommandLineOptionNameAttribute(string name, StringComparison comparison){
			this.Name = name;
			this.StringComparison = comparison;
		}
	}
}