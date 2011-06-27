/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
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
			
			var comp = new LambdaComparer<char>((x, y) => comparer.Compare(x.ToString(), y.ToString()));
			var dicOption = new PrefixDictionary<Tuple<PropertyInfo, Action<string>>>(comp);
			var defaultActions = new List<Tuple<CommandLineParemeterOrderAttribute, Action<string>>>();
			var priorities = new List<Tuple<int, PropertyInfo, Action<string>>>();
			var list = new List<string>();
			PropertyInfo listProp = null;

			// プロパティ取得
			foreach(var prop in option.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
			                          .Where(prop => prop.CanWrite && prop.CanRead)){
				var action = GetAction(prop, option, ref listProp);
				if(action != null){
					// dicOptionsに追加
					var entry = new Tuple<PropertyInfo, Action<string>>(prop, action);
					var nameAttrs = prop.GetCustomAttributes(typeof(CommandLineOptionNameAttribute), true)
						.Cast<CommandLineOptionNameAttribute>().ToArray();
					if(nameAttrs.Length > 0){
						foreach(var attr in nameAttrs){
							dicOption.Add(attr.Name, entry);
						}
					}else{
						dicOption.Add(prop.Name, entry);
					}

					// プライオリティ追加
					var prioAttr = prop.GetCustomAttributes(typeof(CommandLineParemeterPriorityAttribute), true)
						.Cast<CommandLineParemeterPriorityAttribute>().FirstOrDefault();
					if(prioAttr != null){
						priorities.Add(new Tuple<int, PropertyInfo, Action<string>>(prioAttr.Priority, prop, action));
					}else{
						priorities.Add(new Tuple<int, PropertyInfo, Action<string>>(0, prop, action));
					}

					// デフォルトパラメータ
					var orderAttr = prop.GetCustomAttributes(typeof(CommandLineParemeterOrderAttribute), true)
						.Cast<CommandLineParemeterOrderAttribute>().FirstOrDefault();
					if(orderAttr != null){
						defaultActions.Add(new Tuple<CommandLineParemeterOrderAttribute, Action<string>>(orderAttr, action));
					}
				}
			}
			defaultActions.Sort(new LambdaComparer<Tuple<CommandLineParemeterOrderAttribute, Action<string>>>(
				(a, b) => a.Item1.Index.CompareTo(b.Item1.Index)));
			priorities.Sort(new LambdaComparer<Tuple<int, PropertyInfo, Action<string>>>(
				(a, b) => a.Item1.CompareTo(b.Item1)));

			// 解析
			foreach(var arg in arguments){
				// スイッチ付き
				if(arg.StartsWith("/")){
					// キーと値を取得
					var a = arg.Substring(1).Split(':');
					string value;
					string key;
					// 値が存在しない、フラグの場合
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

					// 前方一致検索
					var founds = dicOption.Search(key).ToArray();
					// 複数ヒットした場合
					if(founds.Length > 1){
						var foundsProps = new HashSet<PropertyInfo>(founds.Select(pair => pair.Value.Item1));
						// プライオリティから判定
						var props = priorities
							.Where(pair => foundsProps.Contains(pair.Item2))
							.GroupBy(pair => pair.Item1)
							.FirstOrDefault().ToArray();
						if(props.Length == 1){
							props[0].Item3(value);
						}
					// 一個だけヒットした場合
					}else if(founds.Length == 1){
						founds[0].Value.Item2(value);
					}
				}else{ // スイッチ無しの時
					if(defaultActions.Count > 0){
						defaultActions[0].Item2(arg);
						defaultActions.RemoveAt(0);
					}else{
						if(listProp != null){
							list.Add(arg);
						}
					}
				}
			}

			if(listProp != null){
				listProp.SetValue(option, list.ToArray(), null);
			}
		}
		
		private static Action<string> GetAction(PropertyInfo prop, object option, ref PropertyInfo listProp){
			var thisProp = prop;
			if(prop.PropertyType.Equals(typeof(Nullable<bool>))){
				// フラグオプションの場合
				return new Action<string>(delegate(string arg){
					thisProp.SetValue(option, (arg.IsNullOrEmpty() || arg == "+"), null);
				});
			}else if(prop.PropertyType.Equals(typeof(string[]))){
				// リストの場合
				if(listProp != null){
					throw new ArgumentException("option");
				}
				listProp = thisProp;
				return null;
			}else{
				var conv = TypeDescriptor.GetConverter(prop.PropertyType);
				// 値付きオプションの場合
				return new Action<string>(delegate(string arg){
					try{
						thisProp.SetValue(option, conv.ConvertFromString(arg), null);
					}catch(Exception){
					}
				});
			}
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
	public class CommandLineOptionNameAttribute : Attribute{
		public string Name{get; set;}

		public CommandLineOptionNameAttribute(string name){
			this.Name = name;
		}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class CommandLineParemeterOrderAttribute : Attribute{
		public int Index{get; set;}
		public CommandLineParemeterOrderAttribute(int index){
			this.Index = index;
		}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class CommandLineParemeterPriorityAttribute : Attribute{
		public int Priority{get; set;}
		public CommandLineParemeterPriorityAttribute(int prior){
			this.Priority = prior;
		}
	}
}