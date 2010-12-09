/*
	$Id$
*/
using System;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace CatWalk{
	public static class Extensions {
		#region Exception
		public static void ThrowIfNull(this object obj){
			if(obj == null){
				throw new ArgumentNullException();
			}
		}
		
		public static void ThrowIfNull(this object obj, string message){
			if(obj == null){
				throw new ArgumentNullException(message);
			}
		}

		public static void ThrowIfOutOfRange(this int n, int min, int max){
			if((n < min) || (max < n)){
				throw new ArgumentOutOfRangeException();
			}
		}
		
		public static void ThrowIfOutOfRange(this int n, int min, int max, string message){
			if((n < min) || (max < n)){
				throw new ArgumentOutOfRangeException(message);
			}
		}

		public static void ThrowIfOutOfRange(this int n, int min){
			if(n < min){
				throw new ArgumentOutOfRangeException();
			}
		}

		public static void ThrowIfOutOfRange(this int n, int min, string message){
			if(n < min){
				throw new ArgumentOutOfRangeException(message);
			}
		}

		#endregion

		#region object

		public static bool IsNull(this object obj){
			return (obj == null);
		}

		#endregion

		#region Assembly

		public static string GetInformationalVersion(this Assembly asm){
			var ver = asm.GetCustomAttributes(true).OfType<AssemblyInformationalVersionAttribute>().First();
			return (ver != null) ? ver.InformationalVersion : null;
		}

		public static Version GetVersion(this Assembly asm){
			return asm.GetName().Version;
		}

		public static string GetCopyright(this Assembly asm){
			var copy = asm.GetCustomAttributes(true).OfType<AssemblyCopyrightAttribute>().First();
			return (copy != null) ? copy.Copyright : null;
		}

		#endregion

		#region string

		public static bool IsNullOrEmpty(this string str){
			return String.IsNullOrEmpty(str);
		}

		public static bool IsMatchWildCard(this string str, string mask){
			return CatWalk.Shell.Win32.PathMatchSpec(str, mask);
		}

		public static int IndexOfRegex(this string str, string pattern){
			return IndexOfRegex(str, pattern, 0, RegexOptions.None);
		}

		public static int IndexOfRegex(this string str, string pattern, int start){
			return IndexOfRegex(str, pattern, start, RegexOptions.None);
		}

		public static int IndexOfRegex(this string str, string pattern, int start, RegexOptions options){
			var rex = new Regex(pattern);
			var match = rex.Match(str, start);
			return (match.Success) ? match.Index : match.Index;
		}
		public static string ReplaceRegex(this string str, string pattern, string replacement){
			return Regex.Replace(str, pattern, replacement);
		}

		public static string ReplaceRegex(this string str, string pattern, string replacement, RegexOptions option){
			return Regex.Replace(str, pattern, replacement, option);
		}

		public static string ReplaceRegex(this string str, string pattern, MatchEvaluator eval){
			return Regex.Replace(str, pattern, eval);
		}

		public static string ReplaceRegex(this string str, string pattern, MatchEvaluator eval, RegexOptions option){
			return Regex.Replace(str, pattern, eval, option);
		}

		public static string Replace(this string str, string[] oldValues, string newValue){
			oldValues.ForEach(value => str = str.Replace(value, newValue));
			return str;
		}

		#endregion
	}
}