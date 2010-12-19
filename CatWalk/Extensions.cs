/*
	$Id$
*/
using System;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CatWalk {
	public static partial class Ext{
		#region Exception
		public static void ThrowIfNull(this object obj) {
			if(obj == null) {
				throw new ArgumentNullException();
			}
		}

		public static void ThrowIfNull(this object obj, string message) {
			if(obj == null) {
				throw new ArgumentNullException(message);
			}
		}

		public static void ThrowIfOutOfRange(this int n, int min, int max) {
			if((n < min) || (max < n)) {
				throw new ArgumentOutOfRangeException();
			}
		}

		public static void ThrowIfOutOfRange(this int n, int min, int max, string message) {
			if((n < min) || (max < n)) {
				throw new ArgumentOutOfRangeException(message);
			}
		}

		public static void ThrowIfOutOfRange(this int n, int min) {
			if(n < min) {
				throw new ArgumentOutOfRangeException();
			}
		}

		public static void ThrowIfOutOfRange(this int n, int min, string message) {
			if(n < min) {
				throw new ArgumentOutOfRangeException(message);
			}
		}

		#endregion

		#region object

		public static bool IsNull(this object obj) {
			return (obj == null);
		}

		#endregion

		#region Using
		
		public static TResult Dispose<TResource, TResult>(this TResource resource, Func<TResource, TResult> func) where TResource : class, IDisposable{
			using(resource){
				return func(resource);
			}
		}

		public static IEnumerable<R> Use<T, R>(this T resource, Func<T, IEnumerable<R>> func) where T : IDisposable{
			resource.ThrowIfNull("resource");
			func.ThrowIfNull("func");
			using(resource){
				foreach(var v in func(resource)){
					yield return v;
				}
			}
		}

		public static IEnumerable<T> Use<T>(this T resource) where T : IDisposable{
			using(resource){
				while(true){
					yield return resource;
				}
			}
		}

		#endregion

		#region Assembly

		public static string GetInformationalVersion(this Assembly asm) {
			var ver = asm.GetCustomAttributes(true).OfType<AssemblyInformationalVersionAttribute>().First();
			return (ver != null) ? ver.InformationalVersion : null;
		}

		public static Version GetVersion(this Assembly asm) {
			return asm.GetName().Version;
		}

		public static string GetCopyright(this Assembly asm) {
			var copy = asm.GetCustomAttributes(true).OfType<AssemblyCopyrightAttribute>().First();
			return (copy != null) ? copy.Copyright : null;
		}

		#endregion

		#region string

		public static bool IsNullOrEmpty(this string str) {
			return String.IsNullOrEmpty(str);
		}

		public static bool IsMatchWildCard(this string str, string mask) {
			return PathMatchSpec(str, mask);
		}

		[DllImport("shlwapi.dll", EntryPoint = "PathMatchSpec", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool PathMatchSpec([MarshalAs(UnmanagedType.LPTStr)] string path, [MarshalAs(UnmanagedType.LPTStr)] string spec);

		public static int IndexOfRegex(this string str, string pattern) {
			return IndexOfRegex(str, pattern, 0, RegexOptions.None);
		}

		public static int IndexOfRegex(this string str, string pattern, int start) {
			return IndexOfRegex(str, pattern, start, RegexOptions.None);
		}

		public static int IndexOfRegex(this string str, string pattern, int start, RegexOptions options) {
			var rex = new Regex(pattern);
			var match = rex.Match(str, start);
			return (match.Success) ? match.Index : match.Index;
		}
		public static string ReplaceRegex(this string str, string pattern, string replacement) {
			return Regex.Replace(str, pattern, replacement);
		}

		public static string ReplaceRegex(this string str, string pattern, string replacement, RegexOptions option) {
			return Regex.Replace(str, pattern, replacement, option);
		}

		public static string ReplaceRegex(this string str, string pattern, MatchEvaluator eval) {
			return Regex.Replace(str, pattern, eval);
		}

		public static string ReplaceRegex(this string str, string pattern, MatchEvaluator eval, RegexOptions option) {
			return Regex.Replace(str, pattern, eval, option);
		}

		public static string Replace(this string str, string[] oldValues, string newValue) {
			oldValues.ForEach(value => str = str.Replace(value, newValue));
			return str;
		}

		public static string Join(this IEnumerable<string> source, string separator) {
			return String.Join(separator, source.ToArray());
		}

		public static string Join<T>(this IEnumerable<T> source, string separator) {
			return String.Join(separator, source.Select(o => o.ToString()).ToArray());
		}

		public static bool IsDecimalNumber(this char c){
			return (('0' <= c) && (c <= '9'));
		}

		public static bool IsSmallAlphabet(this char c){
			int n = (int)c;
			return (('a' <= n) && (n <= 'z'));
		}

		public static bool IsLargeAlphabet(this char c){
			int n = (int)c;
			return (('A' <= n) && (n <= 'Z'));
		}

		#endregion

		#region EventHandler

		public static void Invoke(this EventHandler handler, object sender, EventArgs e) {
			if(handler != null) {
				handler(sender, e);
			}
		}

		#endregion

		#region Array

		public static void Shuffle<T>(T[] array){
			var n = array.Length; 
			var rnd = new Random();
			while(n > 1){
				var k = rnd.Next(n); 
				n--; 
				var temp = array[n]; 
				array[n] = array[k]; 
				array[k] = temp; 
			}
		}

		#endregion
	}
}