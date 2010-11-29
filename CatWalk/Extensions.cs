/*
	$Id$
*/
using System;

namespace CatWalk{
	public static class Extensions{
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
		
		public static bool IsNullOrEmpty(this string str){
			return String.IsNullOrEmpty(str);
		}
		
		public static bool IsNull(this object obj){
			return (obj == null);
		}

		public static bool IsMatchWildCard(this string str, string mask){
			return CatWalk.Shell.Win32.PathMatchSpec(str, mask);
		}
	}
}