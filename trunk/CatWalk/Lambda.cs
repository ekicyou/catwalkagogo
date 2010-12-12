/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk{
	public static class Lambda{
		#region Memoize

		public static Func<T, TResult> Memoize<T, TResult>(this Func<T, TResult> func){
			func.ThrowIfNull("func");
			var memo = new Dictionary<T, TResult>();
			return n =>{
				TResult o;
				if(memo.TryGetValue(n, out o)){
					return o;
				} else{
					var result = func(n);
					memo.Add(n, result);
					return result;
				}
			};
		}

		#endregion

		#region Bind

		public static Action Bind<T1>(this Action<T1> action, T1 value1){
			return () => action(value1);
		}

		public static Action<T2> Bind1<T1, T2>(this Action<T1, T2> action, T1 value1){
			return (T2 value2) => action(value1, value2);
		}

		public static Action<T1> Bind2<T1, T2>(this Action<T1, T2> action, T2 value2){
			return (T1 value1) => action(value1, value2);
		}

		public static Action<T2, T3> Bind1<T1, T2, T3>(this Action<T1, T2, T3> action, T1 value1){
			return (T2 value2, T3 value3) => action(value1, value2, value3);
		}

		public static Action<T1, T3> Bind2<T1, T2, T3>(this Action<T1, T2, T3> action, T2 value2){
			return (T1 value1, T3 value3) => action(value1, value2, value3);
		}

		public static Action<T1, T2> Bind3<T1, T2, T3>(this Action<T1, T2, T3> action, T3 value3){
			return (T1 value1, T2 value2) => action(value1, value2, value3);
		}

		public static Action<T2, T3, T4> Bind1<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 value1){
			return (T2 value2, T3 value3, T4 value4) => action(value1, value2, value3, value4);
		}

		public static Action<T1, T3, T4> Bind2<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T2 value2){
			return (T1 value1, T3 value3, T4 value4) => action(value1, value2, value3, value4);
		}

		public static Action<T1, T2, T4> Bind3<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T3 value3){
			return (T1 value1, T2 value2, T4 value4) => action(value1, value2, value3, value4);
		}

		public static Action<T1, T2, T3> Bind4<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T4 value4){
			return (T1 value1, T2 value2, T3 value3) => action(value1, value2, value3, value4);
		}

		public static Action<T2, T3, T4, T5> Bind1<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, T1 value1){
			return (T2 value2, T3 value3, T4 value4, T5 value5) => action(value1, value2, value3, value4, value5);
		}

		public static Action<T1, T3, T4, T5> Bind2<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, T2 value2){
			return (T1 value1, T3 value3, T4 value4, T5 value5) => action(value1, value2, value3, value4, value5);
		}

		public static Action<T1, T2, T4, T5> Bind3<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, T3 value3){
			return (T1 value1, T2 value2, T4 value4, T5 value5) => action(value1, value2, value3, value4, value5);
		}

		public static Action<T1, T2, T3, T5> Bind4<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, T4 value4){
			return (T1 value1, T2 value2, T3 value3, T5 value5) => action(value1, value2, value3, value4, value5);
		}

		public static Action<T1, T2, T3, T4> Bind5<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, T5 value5){
			return (T1 value1, T2 value2, T3 value3, T4 value4) => action(value1, value2, value3, value4, value5);
		}

		public static Action<T2, T3, T4, T5, T6> Bind1<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, T1 value1){
			return (T2 value2, T3 value3, T4 value4, T5 value5, T6 value6) => action(value1, value2, value3, value4, value5, value6);
		}

		public static Action<T1, T3, T4, T5, T6> Bind2<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, T2 value2){
			return (T1 value1, T3 value3, T4 value4, T5 value5, T6 value6) => action(value1, value2, value3, value4, value5, value6);
		}

		public static Action<T1, T2, T4, T5, T6> Bind3<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, T3 value3){
			return (T1 value1, T2 value2, T4 value4, T5 value5, T6 value6) => action(value1, value2, value3, value4, value5, value6);
		}

		public static Action<T1, T2, T3, T5, T6> Bind4<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, T4 value4){
			return (T1 value1, T2 value2, T3 value3, T5 value5, T6 value6) => action(value1, value2, value3, value4, value5, value6);
		}

		public static Action<T1, T2, T3, T4, T6> Bind5<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, T5 value5){
			return (T1 value1, T2 value2, T3 value3, T4 value4, T6 value6) => action(value1, value2, value3, value4, value5, value6);
		}

		public static Action<T1, T2, T3, T4, T5> Bind6<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, T6 value6){
			return (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5) => action(value1, value2, value3, value4, value5, value6);
		}

		public static Action<T2, T3, T4, T5, T6, T7> Bind1<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, T1 value1){
			return (T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7) => action(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Action<T1, T3, T4, T5, T6, T7> Bind2<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, T2 value2){
			return (T1 value1, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7) => action(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Action<T1, T2, T4, T5, T6, T7> Bind3<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, T3 value3){
			return (T1 value1, T2 value2, T4 value4, T5 value5, T6 value6, T7 value7) => action(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Action<T1, T2, T3, T5, T6, T7> Bind4<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, T4 value4){
			return (T1 value1, T2 value2, T3 value3, T5 value5, T6 value6, T7 value7) => action(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Action<T1, T2, T3, T4, T6, T7> Bind5<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, T5 value5){
			return (T1 value1, T2 value2, T3 value3, T4 value4, T6 value6, T7 value7) => action(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Action<T1, T2, T3, T4, T5, T7> Bind6<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, T6 value6){
			return (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T7 value7) => action(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Action<T1, T2, T3, T4, T5, T6> Bind7<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, T7 value7){
			return (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6) => action(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Action<T2, T3, T4, T5, T6, T7, T8> Bind1<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 value1){
			return (T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8) => action(value1, value2, value3, value4, value5, value6, value7, value8);
		}

		public static Action<T1, T3, T4, T5, T6, T7, T8> Bind2<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T2 value2){
			return (T1 value1, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8) => action(value1, value2, value3, value4, value5, value6, value7, value8);
		}

		public static Action<T1, T2, T4, T5, T6, T7, T8> Bind3<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T3 value3){
			return (T1 value1, T2 value2, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8) => action(value1, value2, value3, value4, value5, value6, value7, value8);
		}

		public static Action<T1, T2, T3, T5, T6, T7, T8> Bind4<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T4 value4){
			return (T1 value1, T2 value2, T3 value3, T5 value5, T6 value6, T7 value7, T8 value8) => action(value1, value2, value3, value4, value5, value6, value7, value8);
		}

		public static Action<T1, T2, T3, T4, T6, T7, T8> Bind5<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T5 value5){
			return (T1 value1, T2 value2, T3 value3, T4 value4, T6 value6, T7 value7, T8 value8) => action(value1, value2, value3, value4, value5, value6, value7, value8);
		}

		public static Action<T1, T2, T3, T4, T5, T7, T8> Bind6<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T6 value6){
			return (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T7 value7, T8 value8) => action(value1, value2, value3, value4, value5, value6, value7, value8);
		}

		public static Action<T1, T2, T3, T4, T5, T6, T8> Bind7<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T7 value7){
			return (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T8 value8) => action(value1, value2, value3, value4, value5, value6, value7, value8);
		}

		public static Action<T1, T2, T3, T4, T5, T6, T7> Bind8<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T8 value8){
			return (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7) => action(value1, value2, value3, value4, value5, value6, value7, value8);
		}

		public static Func<T2, TResult> Bind1<T1, T2, TResult>(this Func<T1, T2, TResult> func, T1 value1){
			return (T2 value2) => func(value1, value2);
		}

		public static Func<T1, TResult> Bind2<T1, T2, TResult>(this Func<T1, T2, TResult> func, T2 value2){
			return (T1 value1) => func(value1, value2);
		}

		public static Func<T2, T3, TResult> Bind1<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func, T1 value1){
			return (T2 value2, T3 value3) => func(value1, value2, value3);
		}

		public static Func<T1, T3, TResult> Bind2<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func, T2 value2){
			return (T1 value1, T3 value3) => func(value1, value2, value3);
		}

		public static Func<T1, T2, TResult> Bind3<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func, T3 value3){
			return (T1 value1, T2 value2) => func(value1, value2, value3);
		}

		public static Func<T2, T3, T4, TResult> Bind1<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> func, T1 value1){
			return (T2 value2, T3 value3, T4 value4) => func(value1, value2, value3, value4);
		}

		public static Func<T1, T3, T4, TResult> Bind2<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> func, T2 value2){
			return (T1 value1, T3 value3, T4 value4) => func(value1, value2, value3, value4);
		}

		public static Func<T1, T2, T4, TResult> Bind3<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> func, T3 value3){
			return (T1 value1, T2 value2, T4 value4) => func(value1, value2, value3, value4);
		}

		public static Func<T1, T2, T3, TResult> Bind4<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> func, T4 value4){
			return (T1 value1, T2 value2, T3 value3) => func(value1, value2, value3, value4);
		}

		public static Func<T2, T3, T4, T5, TResult> Bind1<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, TResult> func, T1 value1){
			return (T2 value2, T3 value3, T4 value4, T5 value5) => func(value1, value2, value3, value4, value5);
		}

		public static Func<T1, T3, T4, T5, TResult> Bind2<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, TResult> func, T2 value2){
			return (T1 value1, T3 value3, T4 value4, T5 value5) => func(value1, value2, value3, value4, value5);
		}

		public static Func<T1, T2, T4, T5, TResult> Bind3<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, TResult> func, T3 value3){
			return (T1 value1, T2 value2, T4 value4, T5 value5) => func(value1, value2, value3, value4, value5);
		}

		public static Func<T1, T2, T3, T5, TResult> Bind4<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, TResult> func, T4 value4){
			return (T1 value1, T2 value2, T3 value3, T5 value5) => func(value1, value2, value3, value4, value5);
		}

		public static Func<T1, T2, T3, T4, TResult> Bind5<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, TResult> func, T5 value5){
			return (T1 value1, T2 value2, T3 value3, T4 value4) => func(value1, value2, value3, value4, value5);
		}

		public static Func<T2, T3, T4, T5, T6, TResult> Bind1<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 value1){
			return (T2 value2, T3 value3, T4 value4, T5 value5, T6 value6) => func(value1, value2, value3, value4, value5, value6);
		}

		public static Func<T1, T3, T4, T5, T6, TResult> Bind2<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, TResult> func, T2 value2){
			return (T1 value1, T3 value3, T4 value4, T5 value5, T6 value6) => func(value1, value2, value3, value4, value5, value6);
		}

		public static Func<T1, T2, T4, T5, T6, TResult> Bind3<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, TResult> func, T3 value3){
			return (T1 value1, T2 value2, T4 value4, T5 value5, T6 value6) => func(value1, value2, value3, value4, value5, value6);
		}

		public static Func<T1, T2, T3, T5, T6, TResult> Bind4<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, TResult> func, T4 value4){
			return (T1 value1, T2 value2, T3 value3, T5 value5, T6 value6) => func(value1, value2, value3, value4, value5, value6);
		}

		public static Func<T1, T2, T3, T4, T6, TResult> Bind5<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, TResult> func, T5 value5){
			return (T1 value1, T2 value2, T3 value3, T4 value4, T6 value6) => func(value1, value2, value3, value4, value5, value6);
		}

		public static Func<T1, T2, T3, T4, T5, TResult> Bind6<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, TResult> func, T6 value6){
			return (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5) => func(value1, value2, value3, value4, value5, value6);
		}

		public static Func<T2, T3, T4, T5, T6, T7, TResult> Bind1<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 value1){
			return (T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7) => func(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<T1, T3, T4, T5, T6, T7, TResult> Bind2<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T2 value2){
			return (T1 value1, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7) => func(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<T1, T2, T4, T5, T6, T7, TResult> Bind3<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T3 value3){
			return (T1 value1, T2 value2, T4 value4, T5 value5, T6 value6, T7 value7) => func(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<T1, T2, T3, T5, T6, T7, TResult> Bind4<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T4 value4){
			return (T1 value1, T2 value2, T3 value3, T5 value5, T6 value6, T7 value7) => func(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<T1, T2, T3, T4, T6, T7, TResult> Bind5<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T5 value5){
			return (T1 value1, T2 value2, T3 value3, T4 value4, T6 value6, T7 value7) => func(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<T1, T2, T3, T4, T5, T7, TResult> Bind6<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T6 value6){
			return (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T7 value7) => func(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<T1, T2, T3, T4, T5, T6, TResult> Bind7<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T7 value7){
			return (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6) => func(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<T2, T3, T4, T5, T6, T7, T8, TResult> Bind1<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 value1){
			return (T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8) => func(value1, value2, value3, value4, value5, value6, value7, value8);
		}

		public static Func<T1, T3, T4, T5, T6, T7, T8, TResult> Bind2<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T2 value2){
			return (T1 value1, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8) => func(value1, value2, value3, value4, value5, value6, value7, value8);
		}

		public static Func<T1, T2, T4, T5, T6, T7, T8, TResult> Bind3<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T3 value3){
			return (T1 value1, T2 value2, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8) => func(value1, value2, value3, value4, value5, value6, value7, value8);
		}

		public static Func<T1, T2, T3, T5, T6, T7, T8, TResult> Bind4<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T4 value4){
			return (T1 value1, T2 value2, T3 value3, T5 value5, T6 value6, T7 value7, T8 value8) => func(value1, value2, value3, value4, value5, value6, value7, value8);
		}

		public static Func<T1, T2, T3, T4, T6, T7, T8, TResult> Bind5<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T5 value5){
			return (T1 value1, T2 value2, T3 value3, T4 value4, T6 value6, T7 value7, T8 value8) => func(value1, value2, value3, value4, value5, value6, value7, value8);
		}

		public static Func<T1, T2, T3, T4, T5, T7, T8, TResult> Bind6<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T6 value6){
			return (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T7 value7, T8 value8) => func(value1, value2, value3, value4, value5, value6, value7, value8);
		}

		public static Func<T1, T2, T3, T4, T5, T6, T8, TResult> Bind7<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T7 value7){
			return (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T8 value8) => func(value1, value2, value3, value4, value5, value6, value7, value8);
		}

		public static Func<T1, T2, T3, T4, T5, T6, T7, TResult> Bind8<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T8 value8){
			return (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7) => func(value1, value2, value3, value4, value5, value6, value7, value8);
		}

		#endregion

		#region Curry

		public static Func<T1, Action<T2>> Curry1<T1, T2>(this Action<T1, T2> action){
			return (T1 value1) => (T2 value2) => action(value1, value2);
		}

		public static Func<T2, Action<T1>> Curry2<T1, T2>(this Action<T1, T2> action){
			return (T2 value2) => (T1 value1) => action(value1, value2);
		}

		public static Func<TC1, Func<T1, Action>> Curry<TC1, T1>(this Func<TC1, Action<T1>> action){
			return (TC1 valueC1) => (T1 value1) => () => action(valueC1)(value1);
		}

		public static Func<T1, Action<T2, T3>> Curry1<T1, T2, T3>(this Action<T1, T2, T3> action){
			return (T1 value1) => (T2 value2, T3 value3) => action(value1, value2, value3);
		}

		public static Func<T2, Action<T1, T3>> Curry2<T1, T2, T3>(this Action<T1, T2, T3> action){
			return (T2 value2) => (T1 value1, T3 value3) => action(value1, value2, value3);
		}

		public static Func<T3, Action<T1, T2>> Curry3<T1, T2, T3>(this Action<T1, T2, T3> action){
			return (T3 value3) => (T1 value1, T2 value2) => action(value1, value2, value3);
		}

		public static Func<TC1, Func<T1, Action<T2>>> Curry1<TC1, T1, T2>(this Func<TC1, Action<T1, T2>> action){
			return (TC1 valueC1) => (T1 value1) => (T2 value2) => action(valueC1)(value1, value2);
		}

		public static Func<TC1, Func<T2, Action<T1>>> Curry2<TC1, T1, T2>(this Func<TC1, Action<T1, T2>> action){
			return (TC1 valueC1) => (T2 value2) => (T1 value1) => action(valueC1)(value1, value2);
		}

		public static Func<TC1, Func<TC2, Func<T1, Action>>> Curry<TC1, TC2, T1>(this Func<TC1, Func<TC2, Action<T1>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (T1 value1) => () => action(valueC1)(valueC2)(value1);
		}

		public static Func<T1, Action<T2, T3, T4>> Curry1<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action){
			return (T1 value1) => (T2 value2, T3 value3, T4 value4) => action(value1, value2, value3, value4);
		}

		public static Func<T2, Action<T1, T3, T4>> Curry2<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action){
			return (T2 value2) => (T1 value1, T3 value3, T4 value4) => action(value1, value2, value3, value4);
		}

		public static Func<T3, Action<T1, T2, T4>> Curry3<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action){
			return (T3 value3) => (T1 value1, T2 value2, T4 value4) => action(value1, value2, value3, value4);
		}

		public static Func<T4, Action<T1, T2, T3>> Curry4<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action){
			return (T4 value4) => (T1 value1, T2 value2, T3 value3) => action(value1, value2, value3, value4);
		}

		public static Func<TC1, Func<T1, Action<T2, T3>>> Curry1<TC1, T1, T2, T3>(this Func<TC1, Action<T1, T2, T3>> action){
			return (TC1 valueC1) => (T1 value1) => (T2 value2, T3 value3) => action(valueC1)(value1, value2, value3);
		}

		public static Func<TC1, Func<T2, Action<T1, T3>>> Curry2<TC1, T1, T2, T3>(this Func<TC1, Action<T1, T2, T3>> action){
			return (TC1 valueC1) => (T2 value2) => (T1 value1, T3 value3) => action(valueC1)(value1, value2, value3);
		}

		public static Func<TC1, Func<T3, Action<T1, T2>>> Curry3<TC1, T1, T2, T3>(this Func<TC1, Action<T1, T2, T3>> action){
			return (TC1 valueC1) => (T3 value3) => (T1 value1, T2 value2) => action(valueC1)(value1, value2, value3);
		}

		public static Func<TC1, Func<TC2, Func<T1, Action<T2>>>> Curry1<TC1, TC2, T1, T2>(this Func<TC1, Func<TC2, Action<T1, T2>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (T1 value1) => (T2 value2) => action(valueC1)(valueC2)(value1, value2);
		}

		public static Func<TC1, Func<TC2, Func<T2, Action<T1>>>> Curry2<TC1, TC2, T1, T2>(this Func<TC1, Func<TC2, Action<T1, T2>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (T2 value2) => (T1 value1) => action(valueC1)(valueC2)(value1, value2);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<T1, Action>>>> Curry<TC1, TC2, TC3, T1>(this Func<TC1, Func<TC2, Func<TC3, Action<T1>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (T1 value1) => () => action(valueC1)(valueC2)(valueC3)(value1);
		}

		public static Func<T1, Action<T2, T3, T4, T5>> Curry1<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action){
			return (T1 value1) => (T2 value2, T3 value3, T4 value4, T5 value5) => action(value1, value2, value3, value4, value5);
		}

		public static Func<T2, Action<T1, T3, T4, T5>> Curry2<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action){
			return (T2 value2) => (T1 value1, T3 value3, T4 value4, T5 value5) => action(value1, value2, value3, value4, value5);
		}

		public static Func<T3, Action<T1, T2, T4, T5>> Curry3<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action){
			return (T3 value3) => (T1 value1, T2 value2, T4 value4, T5 value5) => action(value1, value2, value3, value4, value5);
		}

		public static Func<T4, Action<T1, T2, T3, T5>> Curry4<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action){
			return (T4 value4) => (T1 value1, T2 value2, T3 value3, T5 value5) => action(value1, value2, value3, value4, value5);
		}

		public static Func<T5, Action<T1, T2, T3, T4>> Curry5<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action){
			return (T5 value5) => (T1 value1, T2 value2, T3 value3, T4 value4) => action(value1, value2, value3, value4, value5);
		}

		public static Func<TC1, Func<T1, Action<T2, T3, T4>>> Curry1<TC1, T1, T2, T3, T4>(this Func<TC1, Action<T1, T2, T3, T4>> action){
			return (TC1 valueC1) => (T1 value1) => (T2 value2, T3 value3, T4 value4) => action(valueC1)(value1, value2, value3, value4);
		}

		public static Func<TC1, Func<T2, Action<T1, T3, T4>>> Curry2<TC1, T1, T2, T3, T4>(this Func<TC1, Action<T1, T2, T3, T4>> action){
			return (TC1 valueC1) => (T2 value2) => (T1 value1, T3 value3, T4 value4) => action(valueC1)(value1, value2, value3, value4);
		}

		public static Func<TC1, Func<T3, Action<T1, T2, T4>>> Curry3<TC1, T1, T2, T3, T4>(this Func<TC1, Action<T1, T2, T3, T4>> action){
			return (TC1 valueC1) => (T3 value3) => (T1 value1, T2 value2, T4 value4) => action(valueC1)(value1, value2, value3, value4);
		}

		public static Func<TC1, Func<T4, Action<T1, T2, T3>>> Curry4<TC1, T1, T2, T3, T4>(this Func<TC1, Action<T1, T2, T3, T4>> action){
			return (TC1 valueC1) => (T4 value4) => (T1 value1, T2 value2, T3 value3) => action(valueC1)(value1, value2, value3, value4);
		}

		public static Func<TC1, Func<TC2, Func<T1, Action<T2, T3>>>> Curry1<TC1, TC2, T1, T2, T3>(this Func<TC1, Func<TC2, Action<T1, T2, T3>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (T1 value1) => (T2 value2, T3 value3) => action(valueC1)(valueC2)(value1, value2, value3);
		}

		public static Func<TC1, Func<TC2, Func<T2, Action<T1, T3>>>> Curry2<TC1, TC2, T1, T2, T3>(this Func<TC1, Func<TC2, Action<T1, T2, T3>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (T2 value2) => (T1 value1, T3 value3) => action(valueC1)(valueC2)(value1, value2, value3);
		}

		public static Func<TC1, Func<TC2, Func<T3, Action<T1, T2>>>> Curry3<TC1, TC2, T1, T2, T3>(this Func<TC1, Func<TC2, Action<T1, T2, T3>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (T3 value3) => (T1 value1, T2 value2) => action(valueC1)(valueC2)(value1, value2, value3);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<T1, Action<T2>>>>> Curry1<TC1, TC2, TC3, T1, T2>(this Func<TC1, Func<TC2, Func<TC3, Action<T1, T2>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (T1 value1) => (T2 value2) => action(valueC1)(valueC2)(valueC3)(value1, value2);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<T2, Action<T1>>>>> Curry2<TC1, TC2, TC3, T1, T2>(this Func<TC1, Func<TC2, Func<TC3, Action<T1, T2>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (T2 value2) => (T1 value1) => action(valueC1)(valueC2)(valueC3)(value1, value2);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<T1, Action>>>>> Curry<TC1, TC2, TC3, TC4, T1>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Action<T1>>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (T1 value1) => () => action(valueC1)(valueC2)(valueC3)(valueC4)(value1);
		}

		public static Func<T1, Action<T2, T3, T4, T5, T6>> Curry1<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action){
			return (T1 value1) => (T2 value2, T3 value3, T4 value4, T5 value5, T6 value6) => action(value1, value2, value3, value4, value5, value6);
		}

		public static Func<T2, Action<T1, T3, T4, T5, T6>> Curry2<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action){
			return (T2 value2) => (T1 value1, T3 value3, T4 value4, T5 value5, T6 value6) => action(value1, value2, value3, value4, value5, value6);
		}

		public static Func<T3, Action<T1, T2, T4, T5, T6>> Curry3<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action){
			return (T3 value3) => (T1 value1, T2 value2, T4 value4, T5 value5, T6 value6) => action(value1, value2, value3, value4, value5, value6);
		}

		public static Func<T4, Action<T1, T2, T3, T5, T6>> Curry4<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action){
			return (T4 value4) => (T1 value1, T2 value2, T3 value3, T5 value5, T6 value6) => action(value1, value2, value3, value4, value5, value6);
		}

		public static Func<T5, Action<T1, T2, T3, T4, T6>> Curry5<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action){
			return (T5 value5) => (T1 value1, T2 value2, T3 value3, T4 value4, T6 value6) => action(value1, value2, value3, value4, value5, value6);
		}

		public static Func<T6, Action<T1, T2, T3, T4, T5>> Curry6<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action){
			return (T6 value6) => (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5) => action(value1, value2, value3, value4, value5, value6);
		}

		public static Func<TC1, Func<T1, Action<T2, T3, T4, T5>>> Curry1<TC1, T1, T2, T3, T4, T5>(this Func<TC1, Action<T1, T2, T3, T4, T5>> action){
			return (TC1 valueC1) => (T1 value1) => (T2 value2, T3 value3, T4 value4, T5 value5) => action(valueC1)(value1, value2, value3, value4, value5);
		}

		public static Func<TC1, Func<T2, Action<T1, T3, T4, T5>>> Curry2<TC1, T1, T2, T3, T4, T5>(this Func<TC1, Action<T1, T2, T3, T4, T5>> action){
			return (TC1 valueC1) => (T2 value2) => (T1 value1, T3 value3, T4 value4, T5 value5) => action(valueC1)(value1, value2, value3, value4, value5);
		}

		public static Func<TC1, Func<T3, Action<T1, T2, T4, T5>>> Curry3<TC1, T1, T2, T3, T4, T5>(this Func<TC1, Action<T1, T2, T3, T4, T5>> action){
			return (TC1 valueC1) => (T3 value3) => (T1 value1, T2 value2, T4 value4, T5 value5) => action(valueC1)(value1, value2, value3, value4, value5);
		}

		public static Func<TC1, Func<T4, Action<T1, T2, T3, T5>>> Curry4<TC1, T1, T2, T3, T4, T5>(this Func<TC1, Action<T1, T2, T3, T4, T5>> action){
			return (TC1 valueC1) => (T4 value4) => (T1 value1, T2 value2, T3 value3, T5 value5) => action(valueC1)(value1, value2, value3, value4, value5);
		}

		public static Func<TC1, Func<T5, Action<T1, T2, T3, T4>>> Curry5<TC1, T1, T2, T3, T4, T5>(this Func<TC1, Action<T1, T2, T3, T4, T5>> action){
			return (TC1 valueC1) => (T5 value5) => (T1 value1, T2 value2, T3 value3, T4 value4) => action(valueC1)(value1, value2, value3, value4, value5);
		}

		public static Func<TC1, Func<TC2, Func<T1, Action<T2, T3, T4>>>> Curry1<TC1, TC2, T1, T2, T3, T4>(this Func<TC1, Func<TC2, Action<T1, T2, T3, T4>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (T1 value1) => (T2 value2, T3 value3, T4 value4) => action(valueC1)(valueC2)(value1, value2, value3, value4);
		}

		public static Func<TC1, Func<TC2, Func<T2, Action<T1, T3, T4>>>> Curry2<TC1, TC2, T1, T2, T3, T4>(this Func<TC1, Func<TC2, Action<T1, T2, T3, T4>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (T2 value2) => (T1 value1, T3 value3, T4 value4) => action(valueC1)(valueC2)(value1, value2, value3, value4);
		}

		public static Func<TC1, Func<TC2, Func<T3, Action<T1, T2, T4>>>> Curry3<TC1, TC2, T1, T2, T3, T4>(this Func<TC1, Func<TC2, Action<T1, T2, T3, T4>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (T3 value3) => (T1 value1, T2 value2, T4 value4) => action(valueC1)(valueC2)(value1, value2, value3, value4);
		}

		public static Func<TC1, Func<TC2, Func<T4, Action<T1, T2, T3>>>> Curry4<TC1, TC2, T1, T2, T3, T4>(this Func<TC1, Func<TC2, Action<T1, T2, T3, T4>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (T4 value4) => (T1 value1, T2 value2, T3 value3) => action(valueC1)(valueC2)(value1, value2, value3, value4);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<T1, Action<T2, T3>>>>> Curry1<TC1, TC2, TC3, T1, T2, T3>(this Func<TC1, Func<TC2, Func<TC3, Action<T1, T2, T3>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (T1 value1) => (T2 value2, T3 value3) => action(valueC1)(valueC2)(valueC3)(value1, value2, value3);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<T2, Action<T1, T3>>>>> Curry2<TC1, TC2, TC3, T1, T2, T3>(this Func<TC1, Func<TC2, Func<TC3, Action<T1, T2, T3>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (T2 value2) => (T1 value1, T3 value3) => action(valueC1)(valueC2)(valueC3)(value1, value2, value3);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<T3, Action<T1, T2>>>>> Curry3<TC1, TC2, TC3, T1, T2, T3>(this Func<TC1, Func<TC2, Func<TC3, Action<T1, T2, T3>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (T3 value3) => (T1 value1, T2 value2) => action(valueC1)(valueC2)(valueC3)(value1, value2, value3);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<T1, Action<T2>>>>>> Curry1<TC1, TC2, TC3, TC4, T1, T2>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Action<T1, T2>>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (T1 value1) => (T2 value2) => action(valueC1)(valueC2)(valueC3)(valueC4)(value1, value2);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<T2, Action<T1>>>>>> Curry2<TC1, TC2, TC3, TC4, T1, T2>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Action<T1, T2>>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (T2 value2) => (T1 value1) => action(valueC1)(valueC2)(valueC3)(valueC4)(value1, value2);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Func<T1, Action>>>>>> Curry<TC1, TC2, TC3, TC4, TC5, T1>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Action<T1>>>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (TC5 valueC5) => (T1 value1) => () => action(valueC1)(valueC2)(valueC3)(valueC4)(valueC5)(value1);
		}

		public static Func<T1, Action<T2, T3, T4, T5, T6, T7>> Curry1<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action){
			return (T1 value1) => (T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7) => action(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<T2, Action<T1, T3, T4, T5, T6, T7>> Curry2<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action){
			return (T2 value2) => (T1 value1, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7) => action(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<T3, Action<T1, T2, T4, T5, T6, T7>> Curry3<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action){
			return (T3 value3) => (T1 value1, T2 value2, T4 value4, T5 value5, T6 value6, T7 value7) => action(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<T4, Action<T1, T2, T3, T5, T6, T7>> Curry4<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action){
			return (T4 value4) => (T1 value1, T2 value2, T3 value3, T5 value5, T6 value6, T7 value7) => action(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<T5, Action<T1, T2, T3, T4, T6, T7>> Curry5<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action){
			return (T5 value5) => (T1 value1, T2 value2, T3 value3, T4 value4, T6 value6, T7 value7) => action(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<T6, Action<T1, T2, T3, T4, T5, T7>> Curry6<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action){
			return (T6 value6) => (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T7 value7) => action(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<T7, Action<T1, T2, T3, T4, T5, T6>> Curry7<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action){
			return (T7 value7) => (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6) => action(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<TC1, Func<T1, Action<T2, T3, T4, T5, T6>>> Curry1<TC1, T1, T2, T3, T4, T5, T6>(this Func<TC1, Action<T1, T2, T3, T4, T5, T6>> action){
			return (TC1 valueC1) => (T1 value1) => (T2 value2, T3 value3, T4 value4, T5 value5, T6 value6) => action(valueC1)(value1, value2, value3, value4, value5, value6);
		}

		public static Func<TC1, Func<T2, Action<T1, T3, T4, T5, T6>>> Curry2<TC1, T1, T2, T3, T4, T5, T6>(this Func<TC1, Action<T1, T2, T3, T4, T5, T6>> action){
			return (TC1 valueC1) => (T2 value2) => (T1 value1, T3 value3, T4 value4, T5 value5, T6 value6) => action(valueC1)(value1, value2, value3, value4, value5, value6);
		}

		public static Func<TC1, Func<T3, Action<T1, T2, T4, T5, T6>>> Curry3<TC1, T1, T2, T3, T4, T5, T6>(this Func<TC1, Action<T1, T2, T3, T4, T5, T6>> action){
			return (TC1 valueC1) => (T3 value3) => (T1 value1, T2 value2, T4 value4, T5 value5, T6 value6) => action(valueC1)(value1, value2, value3, value4, value5, value6);
		}

		public static Func<TC1, Func<T4, Action<T1, T2, T3, T5, T6>>> Curry4<TC1, T1, T2, T3, T4, T5, T6>(this Func<TC1, Action<T1, T2, T3, T4, T5, T6>> action){
			return (TC1 valueC1) => (T4 value4) => (T1 value1, T2 value2, T3 value3, T5 value5, T6 value6) => action(valueC1)(value1, value2, value3, value4, value5, value6);
		}

		public static Func<TC1, Func<T5, Action<T1, T2, T3, T4, T6>>> Curry5<TC1, T1, T2, T3, T4, T5, T6>(this Func<TC1, Action<T1, T2, T3, T4, T5, T6>> action){
			return (TC1 valueC1) => (T5 value5) => (T1 value1, T2 value2, T3 value3, T4 value4, T6 value6) => action(valueC1)(value1, value2, value3, value4, value5, value6);
		}

		public static Func<TC1, Func<T6, Action<T1, T2, T3, T4, T5>>> Curry6<TC1, T1, T2, T3, T4, T5, T6>(this Func<TC1, Action<T1, T2, T3, T4, T5, T6>> action){
			return (TC1 valueC1) => (T6 value6) => (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5) => action(valueC1)(value1, value2, value3, value4, value5, value6);
		}

		public static Func<TC1, Func<TC2, Func<T1, Action<T2, T3, T4, T5>>>> Curry1<TC1, TC2, T1, T2, T3, T4, T5>(this Func<TC1, Func<TC2, Action<T1, T2, T3, T4, T5>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (T1 value1) => (T2 value2, T3 value3, T4 value4, T5 value5) => action(valueC1)(valueC2)(value1, value2, value3, value4, value5);
		}

		public static Func<TC1, Func<TC2, Func<T2, Action<T1, T3, T4, T5>>>> Curry2<TC1, TC2, T1, T2, T3, T4, T5>(this Func<TC1, Func<TC2, Action<T1, T2, T3, T4, T5>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (T2 value2) => (T1 value1, T3 value3, T4 value4, T5 value5) => action(valueC1)(valueC2)(value1, value2, value3, value4, value5);
		}

		public static Func<TC1, Func<TC2, Func<T3, Action<T1, T2, T4, T5>>>> Curry3<TC1, TC2, T1, T2, T3, T4, T5>(this Func<TC1, Func<TC2, Action<T1, T2, T3, T4, T5>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (T3 value3) => (T1 value1, T2 value2, T4 value4, T5 value5) => action(valueC1)(valueC2)(value1, value2, value3, value4, value5);
		}

		public static Func<TC1, Func<TC2, Func<T4, Action<T1, T2, T3, T5>>>> Curry4<TC1, TC2, T1, T2, T3, T4, T5>(this Func<TC1, Func<TC2, Action<T1, T2, T3, T4, T5>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (T4 value4) => (T1 value1, T2 value2, T3 value3, T5 value5) => action(valueC1)(valueC2)(value1, value2, value3, value4, value5);
		}

		public static Func<TC1, Func<TC2, Func<T5, Action<T1, T2, T3, T4>>>> Curry5<TC1, TC2, T1, T2, T3, T4, T5>(this Func<TC1, Func<TC2, Action<T1, T2, T3, T4, T5>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (T5 value5) => (T1 value1, T2 value2, T3 value3, T4 value4) => action(valueC1)(valueC2)(value1, value2, value3, value4, value5);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<T1, Action<T2, T3, T4>>>>> Curry1<TC1, TC2, TC3, T1, T2, T3, T4>(this Func<TC1, Func<TC2, Func<TC3, Action<T1, T2, T3, T4>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (T1 value1) => (T2 value2, T3 value3, T4 value4) => action(valueC1)(valueC2)(valueC3)(value1, value2, value3, value4);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<T2, Action<T1, T3, T4>>>>> Curry2<TC1, TC2, TC3, T1, T2, T3, T4>(this Func<TC1, Func<TC2, Func<TC3, Action<T1, T2, T3, T4>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (T2 value2) => (T1 value1, T3 value3, T4 value4) => action(valueC1)(valueC2)(valueC3)(value1, value2, value3, value4);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<T3, Action<T1, T2, T4>>>>> Curry3<TC1, TC2, TC3, T1, T2, T3, T4>(this Func<TC1, Func<TC2, Func<TC3, Action<T1, T2, T3, T4>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (T3 value3) => (T1 value1, T2 value2, T4 value4) => action(valueC1)(valueC2)(valueC3)(value1, value2, value3, value4);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<T4, Action<T1, T2, T3>>>>> Curry4<TC1, TC2, TC3, T1, T2, T3, T4>(this Func<TC1, Func<TC2, Func<TC3, Action<T1, T2, T3, T4>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (T4 value4) => (T1 value1, T2 value2, T3 value3) => action(valueC1)(valueC2)(valueC3)(value1, value2, value3, value4);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<T1, Action<T2, T3>>>>>> Curry1<TC1, TC2, TC3, TC4, T1, T2, T3>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Action<T1, T2, T3>>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (T1 value1) => (T2 value2, T3 value3) => action(valueC1)(valueC2)(valueC3)(valueC4)(value1, value2, value3);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<T2, Action<T1, T3>>>>>> Curry2<TC1, TC2, TC3, TC4, T1, T2, T3>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Action<T1, T2, T3>>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (T2 value2) => (T1 value1, T3 value3) => action(valueC1)(valueC2)(valueC3)(valueC4)(value1, value2, value3);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<T3, Action<T1, T2>>>>>> Curry3<TC1, TC2, TC3, TC4, T1, T2, T3>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Action<T1, T2, T3>>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (T3 value3) => (T1 value1, T2 value2) => action(valueC1)(valueC2)(valueC3)(valueC4)(value1, value2, value3);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Func<T1, Action<T2>>>>>>> Curry1<TC1, TC2, TC3, TC4, TC5, T1, T2>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Action<T1, T2>>>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (TC5 valueC5) => (T1 value1) => (T2 value2) => action(valueC1)(valueC2)(valueC3)(valueC4)(valueC5)(value1, value2);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Func<T2, Action<T1>>>>>>> Curry2<TC1, TC2, TC3, TC4, TC5, T1, T2>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Action<T1, T2>>>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (TC5 valueC5) => (T2 value2) => (T1 value1) => action(valueC1)(valueC2)(valueC3)(valueC4)(valueC5)(value1, value2);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Func<TC6, Func<T1, Action>>>>>>> Curry<TC1, TC2, TC3, TC4, TC5, TC6, T1>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Func<TC6, Action<T1>>>>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (TC5 valueC5) => (TC6 valueC6) => (T1 value1) => () => action(valueC1)(valueC2)(valueC3)(valueC4)(valueC5)(valueC6)(value1);
		}

		public static Func<T1, Action<T2, T3, T4, T5, T6, T7, T8>> Curry1<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action){
			return (T1 value1) => (T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8) => action(value1, value2, value3, value4, value5, value6, value7, value8);
		}

		public static Func<T2, Action<T1, T3, T4, T5, T6, T7, T8>> Curry2<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action){
			return (T2 value2) => (T1 value1, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8) => action(value1, value2, value3, value4, value5, value6, value7, value8);
		}

		public static Func<T3, Action<T1, T2, T4, T5, T6, T7, T8>> Curry3<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action){
			return (T3 value3) => (T1 value1, T2 value2, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8) => action(value1, value2, value3, value4, value5, value6, value7, value8);
		}

		public static Func<T4, Action<T1, T2, T3, T5, T6, T7, T8>> Curry4<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action){
			return (T4 value4) => (T1 value1, T2 value2, T3 value3, T5 value5, T6 value6, T7 value7, T8 value8) => action(value1, value2, value3, value4, value5, value6, value7, value8);
		}

		public static Func<T5, Action<T1, T2, T3, T4, T6, T7, T8>> Curry5<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action){
			return (T5 value5) => (T1 value1, T2 value2, T3 value3, T4 value4, T6 value6, T7 value7, T8 value8) => action(value1, value2, value3, value4, value5, value6, value7, value8);
		}

		public static Func<T6, Action<T1, T2, T3, T4, T5, T7, T8>> Curry6<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action){
			return (T6 value6) => (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T7 value7, T8 value8) => action(value1, value2, value3, value4, value5, value6, value7, value8);
		}

		public static Func<T7, Action<T1, T2, T3, T4, T5, T6, T8>> Curry7<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action){
			return (T7 value7) => (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T8 value8) => action(value1, value2, value3, value4, value5, value6, value7, value8);
		}

		public static Func<T8, Action<T1, T2, T3, T4, T5, T6, T7>> Curry8<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action){
			return (T8 value8) => (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7) => action(value1, value2, value3, value4, value5, value6, value7, value8);
		}

		public static Func<TC1, Func<T1, Action<T2, T3, T4, T5, T6, T7>>> Curry1<TC1, T1, T2, T3, T4, T5, T6, T7>(this Func<TC1, Action<T1, T2, T3, T4, T5, T6, T7>> action){
			return (TC1 valueC1) => (T1 value1) => (T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7) => action(valueC1)(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<TC1, Func<T2, Action<T1, T3, T4, T5, T6, T7>>> Curry2<TC1, T1, T2, T3, T4, T5, T6, T7>(this Func<TC1, Action<T1, T2, T3, T4, T5, T6, T7>> action){
			return (TC1 valueC1) => (T2 value2) => (T1 value1, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7) => action(valueC1)(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<TC1, Func<T3, Action<T1, T2, T4, T5, T6, T7>>> Curry3<TC1, T1, T2, T3, T4, T5, T6, T7>(this Func<TC1, Action<T1, T2, T3, T4, T5, T6, T7>> action){
			return (TC1 valueC1) => (T3 value3) => (T1 value1, T2 value2, T4 value4, T5 value5, T6 value6, T7 value7) => action(valueC1)(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<TC1, Func<T4, Action<T1, T2, T3, T5, T6, T7>>> Curry4<TC1, T1, T2, T3, T4, T5, T6, T7>(this Func<TC1, Action<T1, T2, T3, T4, T5, T6, T7>> action){
			return (TC1 valueC1) => (T4 value4) => (T1 value1, T2 value2, T3 value3, T5 value5, T6 value6, T7 value7) => action(valueC1)(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<TC1, Func<T5, Action<T1, T2, T3, T4, T6, T7>>> Curry5<TC1, T1, T2, T3, T4, T5, T6, T7>(this Func<TC1, Action<T1, T2, T3, T4, T5, T6, T7>> action){
			return (TC1 valueC1) => (T5 value5) => (T1 value1, T2 value2, T3 value3, T4 value4, T6 value6, T7 value7) => action(valueC1)(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<TC1, Func<T6, Action<T1, T2, T3, T4, T5, T7>>> Curry6<TC1, T1, T2, T3, T4, T5, T6, T7>(this Func<TC1, Action<T1, T2, T3, T4, T5, T6, T7>> action){
			return (TC1 valueC1) => (T6 value6) => (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T7 value7) => action(valueC1)(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<TC1, Func<T7, Action<T1, T2, T3, T4, T5, T6>>> Curry7<TC1, T1, T2, T3, T4, T5, T6, T7>(this Func<TC1, Action<T1, T2, T3, T4, T5, T6, T7>> action){
			return (TC1 valueC1) => (T7 value7) => (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6) => action(valueC1)(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<TC1, Func<TC2, Func<T1, Action<T2, T3, T4, T5, T6>>>> Curry1<TC1, TC2, T1, T2, T3, T4, T5, T6>(this Func<TC1, Func<TC2, Action<T1, T2, T3, T4, T5, T6>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (T1 value1) => (T2 value2, T3 value3, T4 value4, T5 value5, T6 value6) => action(valueC1)(valueC2)(value1, value2, value3, value4, value5, value6);
		}

		public static Func<TC1, Func<TC2, Func<T2, Action<T1, T3, T4, T5, T6>>>> Curry2<TC1, TC2, T1, T2, T3, T4, T5, T6>(this Func<TC1, Func<TC2, Action<T1, T2, T3, T4, T5, T6>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (T2 value2) => (T1 value1, T3 value3, T4 value4, T5 value5, T6 value6) => action(valueC1)(valueC2)(value1, value2, value3, value4, value5, value6);
		}

		public static Func<TC1, Func<TC2, Func<T3, Action<T1, T2, T4, T5, T6>>>> Curry3<TC1, TC2, T1, T2, T3, T4, T5, T6>(this Func<TC1, Func<TC2, Action<T1, T2, T3, T4, T5, T6>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (T3 value3) => (T1 value1, T2 value2, T4 value4, T5 value5, T6 value6) => action(valueC1)(valueC2)(value1, value2, value3, value4, value5, value6);
		}

		public static Func<TC1, Func<TC2, Func<T4, Action<T1, T2, T3, T5, T6>>>> Curry4<TC1, TC2, T1, T2, T3, T4, T5, T6>(this Func<TC1, Func<TC2, Action<T1, T2, T3, T4, T5, T6>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (T4 value4) => (T1 value1, T2 value2, T3 value3, T5 value5, T6 value6) => action(valueC1)(valueC2)(value1, value2, value3, value4, value5, value6);
		}

		public static Func<TC1, Func<TC2, Func<T5, Action<T1, T2, T3, T4, T6>>>> Curry5<TC1, TC2, T1, T2, T3, T4, T5, T6>(this Func<TC1, Func<TC2, Action<T1, T2, T3, T4, T5, T6>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (T5 value5) => (T1 value1, T2 value2, T3 value3, T4 value4, T6 value6) => action(valueC1)(valueC2)(value1, value2, value3, value4, value5, value6);
		}

		public static Func<TC1, Func<TC2, Func<T6, Action<T1, T2, T3, T4, T5>>>> Curry6<TC1, TC2, T1, T2, T3, T4, T5, T6>(this Func<TC1, Func<TC2, Action<T1, T2, T3, T4, T5, T6>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (T6 value6) => (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5) => action(valueC1)(valueC2)(value1, value2, value3, value4, value5, value6);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<T1, Action<T2, T3, T4, T5>>>>> Curry1<TC1, TC2, TC3, T1, T2, T3, T4, T5>(this Func<TC1, Func<TC2, Func<TC3, Action<T1, T2, T3, T4, T5>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (T1 value1) => (T2 value2, T3 value3, T4 value4, T5 value5) => action(valueC1)(valueC2)(valueC3)(value1, value2, value3, value4, value5);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<T2, Action<T1, T3, T4, T5>>>>> Curry2<TC1, TC2, TC3, T1, T2, T3, T4, T5>(this Func<TC1, Func<TC2, Func<TC3, Action<T1, T2, T3, T4, T5>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (T2 value2) => (T1 value1, T3 value3, T4 value4, T5 value5) => action(valueC1)(valueC2)(valueC3)(value1, value2, value3, value4, value5);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<T3, Action<T1, T2, T4, T5>>>>> Curry3<TC1, TC2, TC3, T1, T2, T3, T4, T5>(this Func<TC1, Func<TC2, Func<TC3, Action<T1, T2, T3, T4, T5>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (T3 value3) => (T1 value1, T2 value2, T4 value4, T5 value5) => action(valueC1)(valueC2)(valueC3)(value1, value2, value3, value4, value5);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<T4, Action<T1, T2, T3, T5>>>>> Curry4<TC1, TC2, TC3, T1, T2, T3, T4, T5>(this Func<TC1, Func<TC2, Func<TC3, Action<T1, T2, T3, T4, T5>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (T4 value4) => (T1 value1, T2 value2, T3 value3, T5 value5) => action(valueC1)(valueC2)(valueC3)(value1, value2, value3, value4, value5);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<T5, Action<T1, T2, T3, T4>>>>> Curry5<TC1, TC2, TC3, T1, T2, T3, T4, T5>(this Func<TC1, Func<TC2, Func<TC3, Action<T1, T2, T3, T4, T5>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (T5 value5) => (T1 value1, T2 value2, T3 value3, T4 value4) => action(valueC1)(valueC2)(valueC3)(value1, value2, value3, value4, value5);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<T1, Action<T2, T3, T4>>>>>> Curry1<TC1, TC2, TC3, TC4, T1, T2, T3, T4>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Action<T1, T2, T3, T4>>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (T1 value1) => (T2 value2, T3 value3, T4 value4) => action(valueC1)(valueC2)(valueC3)(valueC4)(value1, value2, value3, value4);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<T2, Action<T1, T3, T4>>>>>> Curry2<TC1, TC2, TC3, TC4, T1, T2, T3, T4>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Action<T1, T2, T3, T4>>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (T2 value2) => (T1 value1, T3 value3, T4 value4) => action(valueC1)(valueC2)(valueC3)(valueC4)(value1, value2, value3, value4);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<T3, Action<T1, T2, T4>>>>>> Curry3<TC1, TC2, TC3, TC4, T1, T2, T3, T4>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Action<T1, T2, T3, T4>>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (T3 value3) => (T1 value1, T2 value2, T4 value4) => action(valueC1)(valueC2)(valueC3)(valueC4)(value1, value2, value3, value4);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<T4, Action<T1, T2, T3>>>>>> Curry4<TC1, TC2, TC3, TC4, T1, T2, T3, T4>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Action<T1, T2, T3, T4>>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (T4 value4) => (T1 value1, T2 value2, T3 value3) => action(valueC1)(valueC2)(valueC3)(valueC4)(value1, value2, value3, value4);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Func<T1, Action<T2, T3>>>>>>> Curry1<TC1, TC2, TC3, TC4, TC5, T1, T2, T3>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Action<T1, T2, T3>>>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (TC5 valueC5) => (T1 value1) => (T2 value2, T3 value3) => action(valueC1)(valueC2)(valueC3)(valueC4)(valueC5)(value1, value2, value3);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Func<T2, Action<T1, T3>>>>>>> Curry2<TC1, TC2, TC3, TC4, TC5, T1, T2, T3>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Action<T1, T2, T3>>>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (TC5 valueC5) => (T2 value2) => (T1 value1, T3 value3) => action(valueC1)(valueC2)(valueC3)(valueC4)(valueC5)(value1, value2, value3);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Func<T3, Action<T1, T2>>>>>>> Curry3<TC1, TC2, TC3, TC4, TC5, T1, T2, T3>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Action<T1, T2, T3>>>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (TC5 valueC5) => (T3 value3) => (T1 value1, T2 value2) => action(valueC1)(valueC2)(valueC3)(valueC4)(valueC5)(value1, value2, value3);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Func<TC6, Func<T1, Action<T2>>>>>>>> Curry1<TC1, TC2, TC3, TC4, TC5, TC6, T1, T2>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Func<TC6, Action<T1, T2>>>>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (TC5 valueC5) => (TC6 valueC6) => (T1 value1) => (T2 value2) => action(valueC1)(valueC2)(valueC3)(valueC4)(valueC5)(valueC6)(value1, value2);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Func<TC6, Func<T2, Action<T1>>>>>>>> Curry2<TC1, TC2, TC3, TC4, TC5, TC6, T1, T2>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Func<TC6, Action<T1, T2>>>>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (TC5 valueC5) => (TC6 valueC6) => (T2 value2) => (T1 value1) => action(valueC1)(valueC2)(valueC3)(valueC4)(valueC5)(valueC6)(value1, value2);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Func<TC6, Func<TC7, Func<T1, Action>>>>>>>> Curry<TC1, TC2, TC3, TC4, TC5, TC6, TC7, T1>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Func<TC6, Func<TC7, Action<T1>>>>>>>> action){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (TC5 valueC5) => (TC6 valueC6) => (TC7 valueC7) => (T1 value1) => () => action(valueC1)(valueC2)(valueC3)(valueC4)(valueC5)(valueC6)(valueC7)(value1);
		}

		public static Func<T1, Func<T2, TResult>> Curry1<T1, T2, TResult>(this Func<T1, T2, TResult> func){
			return (T1 value1) => (T2 value2) => func(value1, value2);
		}

		public static Func<T2, Func<T1, TResult>> Curry2<T1, T2, TResult>(this Func<T1, T2, TResult> func){
			return (T2 value2) => (T1 value1) => func(value1, value2);
		}

		public static Func<TC1, Func<T1, Func<TResult>>> Curry<TC1, T1, TResult>(this Func<TC1, Func<T1, TResult>> func){
			return (TC1 valueC1) => (T1 value1) => () => func(valueC1)(value1);
		}

		public static Func<T1, Func<T2, T3, TResult>> Curry1<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func){
			return (T1 value1) => (T2 value2, T3 value3) => func(value1, value2, value3);
		}

		public static Func<T2, Func<T1, T3, TResult>> Curry2<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func){
			return (T2 value2) => (T1 value1, T3 value3) => func(value1, value2, value3);
		}

		public static Func<T3, Func<T1, T2, TResult>> Curry3<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func){
			return (T3 value3) => (T1 value1, T2 value2) => func(value1, value2, value3);
		}

		public static Func<TC1, Func<T1, Func<T2, TResult>>> Curry1<TC1, T1, T2, TResult>(this Func<TC1, Func<T1, T2, TResult>> func){
			return (TC1 valueC1) => (T1 value1) => (T2 value2) => func(valueC1)(value1, value2);
		}

		public static Func<TC1, Func<T2, Func<T1, TResult>>> Curry2<TC1, T1, T2, TResult>(this Func<TC1, Func<T1, T2, TResult>> func){
			return (TC1 valueC1) => (T2 value2) => (T1 value1) => func(valueC1)(value1, value2);
		}

		public static Func<TC1, Func<TC2, Func<T1, Func<TResult>>>> Curry<TC1, TC2, T1, TResult>(this Func<TC1, Func<TC2, Func<T1, TResult>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (T1 value1) => () => func(valueC1)(valueC2)(value1);
		}

		public static Func<T1, Func<T2, T3, T4, TResult>> Curry1<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> func){
			return (T1 value1) => (T2 value2, T3 value3, T4 value4) => func(value1, value2, value3, value4);
		}

		public static Func<T2, Func<T1, T3, T4, TResult>> Curry2<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> func){
			return (T2 value2) => (T1 value1, T3 value3, T4 value4) => func(value1, value2, value3, value4);
		}

		public static Func<T3, Func<T1, T2, T4, TResult>> Curry3<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> func){
			return (T3 value3) => (T1 value1, T2 value2, T4 value4) => func(value1, value2, value3, value4);
		}

		public static Func<T4, Func<T1, T2, T3, TResult>> Curry4<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> func){
			return (T4 value4) => (T1 value1, T2 value2, T3 value3) => func(value1, value2, value3, value4);
		}

		public static Func<TC1, Func<T1, Func<T2, T3, TResult>>> Curry1<TC1, T1, T2, T3, TResult>(this Func<TC1, Func<T1, T2, T3, TResult>> func){
			return (TC1 valueC1) => (T1 value1) => (T2 value2, T3 value3) => func(valueC1)(value1, value2, value3);
		}

		public static Func<TC1, Func<T2, Func<T1, T3, TResult>>> Curry2<TC1, T1, T2, T3, TResult>(this Func<TC1, Func<T1, T2, T3, TResult>> func){
			return (TC1 valueC1) => (T2 value2) => (T1 value1, T3 value3) => func(valueC1)(value1, value2, value3);
		}

		public static Func<TC1, Func<T3, Func<T1, T2, TResult>>> Curry3<TC1, T1, T2, T3, TResult>(this Func<TC1, Func<T1, T2, T3, TResult>> func){
			return (TC1 valueC1) => (T3 value3) => (T1 value1, T2 value2) => func(valueC1)(value1, value2, value3);
		}

		public static Func<TC1, Func<TC2, Func<T1, Func<T2, TResult>>>> Curry1<TC1, TC2, T1, T2, TResult>(this Func<TC1, Func<TC2, Func<T1, T2, TResult>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (T1 value1) => (T2 value2) => func(valueC1)(valueC2)(value1, value2);
		}

		public static Func<TC1, Func<TC2, Func<T2, Func<T1, TResult>>>> Curry2<TC1, TC2, T1, T2, TResult>(this Func<TC1, Func<TC2, Func<T1, T2, TResult>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (T2 value2) => (T1 value1) => func(valueC1)(valueC2)(value1, value2);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<T1, Func<TResult>>>>> Curry<TC1, TC2, TC3, T1, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<T1, TResult>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (T1 value1) => () => func(valueC1)(valueC2)(valueC3)(value1);
		}

		public static Func<T1, Func<T2, T3, T4, T5, TResult>> Curry1<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, TResult> func){
			return (T1 value1) => (T2 value2, T3 value3, T4 value4, T5 value5) => func(value1, value2, value3, value4, value5);
		}

		public static Func<T2, Func<T1, T3, T4, T5, TResult>> Curry2<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, TResult> func){
			return (T2 value2) => (T1 value1, T3 value3, T4 value4, T5 value5) => func(value1, value2, value3, value4, value5);
		}

		public static Func<T3, Func<T1, T2, T4, T5, TResult>> Curry3<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, TResult> func){
			return (T3 value3) => (T1 value1, T2 value2, T4 value4, T5 value5) => func(value1, value2, value3, value4, value5);
		}

		public static Func<T4, Func<T1, T2, T3, T5, TResult>> Curry4<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, TResult> func){
			return (T4 value4) => (T1 value1, T2 value2, T3 value3, T5 value5) => func(value1, value2, value3, value4, value5);
		}

		public static Func<T5, Func<T1, T2, T3, T4, TResult>> Curry5<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, TResult> func){
			return (T5 value5) => (T1 value1, T2 value2, T3 value3, T4 value4) => func(value1, value2, value3, value4, value5);
		}

		public static Func<TC1, Func<T1, Func<T2, T3, T4, TResult>>> Curry1<TC1, T1, T2, T3, T4, TResult>(this Func<TC1, Func<T1, T2, T3, T4, TResult>> func){
			return (TC1 valueC1) => (T1 value1) => (T2 value2, T3 value3, T4 value4) => func(valueC1)(value1, value2, value3, value4);
		}

		public static Func<TC1, Func<T2, Func<T1, T3, T4, TResult>>> Curry2<TC1, T1, T2, T3, T4, TResult>(this Func<TC1, Func<T1, T2, T3, T4, TResult>> func){
			return (TC1 valueC1) => (T2 value2) => (T1 value1, T3 value3, T4 value4) => func(valueC1)(value1, value2, value3, value4);
		}

		public static Func<TC1, Func<T3, Func<T1, T2, T4, TResult>>> Curry3<TC1, T1, T2, T3, T4, TResult>(this Func<TC1, Func<T1, T2, T3, T4, TResult>> func){
			return (TC1 valueC1) => (T3 value3) => (T1 value1, T2 value2, T4 value4) => func(valueC1)(value1, value2, value3, value4);
		}

		public static Func<TC1, Func<T4, Func<T1, T2, T3, TResult>>> Curry4<TC1, T1, T2, T3, T4, TResult>(this Func<TC1, Func<T1, T2, T3, T4, TResult>> func){
			return (TC1 valueC1) => (T4 value4) => (T1 value1, T2 value2, T3 value3) => func(valueC1)(value1, value2, value3, value4);
		}

		public static Func<TC1, Func<TC2, Func<T1, Func<T2, T3, TResult>>>> Curry1<TC1, TC2, T1, T2, T3, TResult>(this Func<TC1, Func<TC2, Func<T1, T2, T3, TResult>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (T1 value1) => (T2 value2, T3 value3) => func(valueC1)(valueC2)(value1, value2, value3);
		}

		public static Func<TC1, Func<TC2, Func<T2, Func<T1, T3, TResult>>>> Curry2<TC1, TC2, T1, T2, T3, TResult>(this Func<TC1, Func<TC2, Func<T1, T2, T3, TResult>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (T2 value2) => (T1 value1, T3 value3) => func(valueC1)(valueC2)(value1, value2, value3);
		}

		public static Func<TC1, Func<TC2, Func<T3, Func<T1, T2, TResult>>>> Curry3<TC1, TC2, T1, T2, T3, TResult>(this Func<TC1, Func<TC2, Func<T1, T2, T3, TResult>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (T3 value3) => (T1 value1, T2 value2) => func(valueC1)(valueC2)(value1, value2, value3);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<T1, Func<T2, TResult>>>>> Curry1<TC1, TC2, TC3, T1, T2, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<T1, T2, TResult>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (T1 value1) => (T2 value2) => func(valueC1)(valueC2)(valueC3)(value1, value2);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<T2, Func<T1, TResult>>>>> Curry2<TC1, TC2, TC3, T1, T2, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<T1, T2, TResult>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (T2 value2) => (T1 value1) => func(valueC1)(valueC2)(valueC3)(value1, value2);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<T1, Func<TResult>>>>>> Curry<TC1, TC2, TC3, TC4, T1, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<T1, TResult>>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (T1 value1) => () => func(valueC1)(valueC2)(valueC3)(valueC4)(value1);
		}

		public static Func<T1, Func<T2, T3, T4, T5, T6, TResult>> Curry1<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, TResult> func){
			return (T1 value1) => (T2 value2, T3 value3, T4 value4, T5 value5, T6 value6) => func(value1, value2, value3, value4, value5, value6);
		}

		public static Func<T2, Func<T1, T3, T4, T5, T6, TResult>> Curry2<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, TResult> func){
			return (T2 value2) => (T1 value1, T3 value3, T4 value4, T5 value5, T6 value6) => func(value1, value2, value3, value4, value5, value6);
		}

		public static Func<T3, Func<T1, T2, T4, T5, T6, TResult>> Curry3<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, TResult> func){
			return (T3 value3) => (T1 value1, T2 value2, T4 value4, T5 value5, T6 value6) => func(value1, value2, value3, value4, value5, value6);
		}

		public static Func<T4, Func<T1, T2, T3, T5, T6, TResult>> Curry4<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, TResult> func){
			return (T4 value4) => (T1 value1, T2 value2, T3 value3, T5 value5, T6 value6) => func(value1, value2, value3, value4, value5, value6);
		}

		public static Func<T5, Func<T1, T2, T3, T4, T6, TResult>> Curry5<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, TResult> func){
			return (T5 value5) => (T1 value1, T2 value2, T3 value3, T4 value4, T6 value6) => func(value1, value2, value3, value4, value5, value6);
		}

		public static Func<T6, Func<T1, T2, T3, T4, T5, TResult>> Curry6<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, TResult> func){
			return (T6 value6) => (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5) => func(value1, value2, value3, value4, value5, value6);
		}

		public static Func<TC1, Func<T1, Func<T2, T3, T4, T5, TResult>>> Curry1<TC1, T1, T2, T3, T4, T5, TResult>(this Func<TC1, Func<T1, T2, T3, T4, T5, TResult>> func){
			return (TC1 valueC1) => (T1 value1) => (T2 value2, T3 value3, T4 value4, T5 value5) => func(valueC1)(value1, value2, value3, value4, value5);
		}

		public static Func<TC1, Func<T2, Func<T1, T3, T4, T5, TResult>>> Curry2<TC1, T1, T2, T3, T4, T5, TResult>(this Func<TC1, Func<T1, T2, T3, T4, T5, TResult>> func){
			return (TC1 valueC1) => (T2 value2) => (T1 value1, T3 value3, T4 value4, T5 value5) => func(valueC1)(value1, value2, value3, value4, value5);
		}

		public static Func<TC1, Func<T3, Func<T1, T2, T4, T5, TResult>>> Curry3<TC1, T1, T2, T3, T4, T5, TResult>(this Func<TC1, Func<T1, T2, T3, T4, T5, TResult>> func){
			return (TC1 valueC1) => (T3 value3) => (T1 value1, T2 value2, T4 value4, T5 value5) => func(valueC1)(value1, value2, value3, value4, value5);
		}

		public static Func<TC1, Func<T4, Func<T1, T2, T3, T5, TResult>>> Curry4<TC1, T1, T2, T3, T4, T5, TResult>(this Func<TC1, Func<T1, T2, T3, T4, T5, TResult>> func){
			return (TC1 valueC1) => (T4 value4) => (T1 value1, T2 value2, T3 value3, T5 value5) => func(valueC1)(value1, value2, value3, value4, value5);
		}

		public static Func<TC1, Func<T5, Func<T1, T2, T3, T4, TResult>>> Curry5<TC1, T1, T2, T3, T4, T5, TResult>(this Func<TC1, Func<T1, T2, T3, T4, T5, TResult>> func){
			return (TC1 valueC1) => (T5 value5) => (T1 value1, T2 value2, T3 value3, T4 value4) => func(valueC1)(value1, value2, value3, value4, value5);
		}

		public static Func<TC1, Func<TC2, Func<T1, Func<T2, T3, T4, TResult>>>> Curry1<TC1, TC2, T1, T2, T3, T4, TResult>(this Func<TC1, Func<TC2, Func<T1, T2, T3, T4, TResult>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (T1 value1) => (T2 value2, T3 value3, T4 value4) => func(valueC1)(valueC2)(value1, value2, value3, value4);
		}

		public static Func<TC1, Func<TC2, Func<T2, Func<T1, T3, T4, TResult>>>> Curry2<TC1, TC2, T1, T2, T3, T4, TResult>(this Func<TC1, Func<TC2, Func<T1, T2, T3, T4, TResult>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (T2 value2) => (T1 value1, T3 value3, T4 value4) => func(valueC1)(valueC2)(value1, value2, value3, value4);
		}

		public static Func<TC1, Func<TC2, Func<T3, Func<T1, T2, T4, TResult>>>> Curry3<TC1, TC2, T1, T2, T3, T4, TResult>(this Func<TC1, Func<TC2, Func<T1, T2, T3, T4, TResult>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (T3 value3) => (T1 value1, T2 value2, T4 value4) => func(valueC1)(valueC2)(value1, value2, value3, value4);
		}

		public static Func<TC1, Func<TC2, Func<T4, Func<T1, T2, T3, TResult>>>> Curry4<TC1, TC2, T1, T2, T3, T4, TResult>(this Func<TC1, Func<TC2, Func<T1, T2, T3, T4, TResult>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (T4 value4) => (T1 value1, T2 value2, T3 value3) => func(valueC1)(valueC2)(value1, value2, value3, value4);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<T1, Func<T2, T3, TResult>>>>> Curry1<TC1, TC2, TC3, T1, T2, T3, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<T1, T2, T3, TResult>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (T1 value1) => (T2 value2, T3 value3) => func(valueC1)(valueC2)(valueC3)(value1, value2, value3);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<T2, Func<T1, T3, TResult>>>>> Curry2<TC1, TC2, TC3, T1, T2, T3, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<T1, T2, T3, TResult>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (T2 value2) => (T1 value1, T3 value3) => func(valueC1)(valueC2)(valueC3)(value1, value2, value3);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<T3, Func<T1, T2, TResult>>>>> Curry3<TC1, TC2, TC3, T1, T2, T3, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<T1, T2, T3, TResult>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (T3 value3) => (T1 value1, T2 value2) => func(valueC1)(valueC2)(valueC3)(value1, value2, value3);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<T1, Func<T2, TResult>>>>>> Curry1<TC1, TC2, TC3, TC4, T1, T2, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<T1, T2, TResult>>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (T1 value1) => (T2 value2) => func(valueC1)(valueC2)(valueC3)(valueC4)(value1, value2);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<T2, Func<T1, TResult>>>>>> Curry2<TC1, TC2, TC3, TC4, T1, T2, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<T1, T2, TResult>>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (T2 value2) => (T1 value1) => func(valueC1)(valueC2)(valueC3)(valueC4)(value1, value2);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Func<T1, Func<TResult>>>>>>> Curry<TC1, TC2, TC3, TC4, TC5, T1, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Func<T1, TResult>>>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (TC5 valueC5) => (T1 value1) => () => func(valueC1)(valueC2)(valueC3)(valueC4)(valueC5)(value1);
		}

		public static Func<T1, Func<T2, T3, T4, T5, T6, T7, TResult>> Curry1<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, TResult> func){
			return (T1 value1) => (T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7) => func(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<T2, Func<T1, T3, T4, T5, T6, T7, TResult>> Curry2<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, TResult> func){
			return (T2 value2) => (T1 value1, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7) => func(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<T3, Func<T1, T2, T4, T5, T6, T7, TResult>> Curry3<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, TResult> func){
			return (T3 value3) => (T1 value1, T2 value2, T4 value4, T5 value5, T6 value6, T7 value7) => func(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<T4, Func<T1, T2, T3, T5, T6, T7, TResult>> Curry4<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, TResult> func){
			return (T4 value4) => (T1 value1, T2 value2, T3 value3, T5 value5, T6 value6, T7 value7) => func(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<T5, Func<T1, T2, T3, T4, T6, T7, TResult>> Curry5<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, TResult> func){
			return (T5 value5) => (T1 value1, T2 value2, T3 value3, T4 value4, T6 value6, T7 value7) => func(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<T6, Func<T1, T2, T3, T4, T5, T7, TResult>> Curry6<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, TResult> func){
			return (T6 value6) => (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T7 value7) => func(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<T7, Func<T1, T2, T3, T4, T5, T6, TResult>> Curry7<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, TResult> func){
			return (T7 value7) => (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6) => func(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<TC1, Func<T1, Func<T2, T3, T4, T5, T6, TResult>>> Curry1<TC1, T1, T2, T3, T4, T5, T6, TResult>(this Func<TC1, Func<T1, T2, T3, T4, T5, T6, TResult>> func){
			return (TC1 valueC1) => (T1 value1) => (T2 value2, T3 value3, T4 value4, T5 value5, T6 value6) => func(valueC1)(value1, value2, value3, value4, value5, value6);
		}

		public static Func<TC1, Func<T2, Func<T1, T3, T4, T5, T6, TResult>>> Curry2<TC1, T1, T2, T3, T4, T5, T6, TResult>(this Func<TC1, Func<T1, T2, T3, T4, T5, T6, TResult>> func){
			return (TC1 valueC1) => (T2 value2) => (T1 value1, T3 value3, T4 value4, T5 value5, T6 value6) => func(valueC1)(value1, value2, value3, value4, value5, value6);
		}

		public static Func<TC1, Func<T3, Func<T1, T2, T4, T5, T6, TResult>>> Curry3<TC1, T1, T2, T3, T4, T5, T6, TResult>(this Func<TC1, Func<T1, T2, T3, T4, T5, T6, TResult>> func){
			return (TC1 valueC1) => (T3 value3) => (T1 value1, T2 value2, T4 value4, T5 value5, T6 value6) => func(valueC1)(value1, value2, value3, value4, value5, value6);
		}

		public static Func<TC1, Func<T4, Func<T1, T2, T3, T5, T6, TResult>>> Curry4<TC1, T1, T2, T3, T4, T5, T6, TResult>(this Func<TC1, Func<T1, T2, T3, T4, T5, T6, TResult>> func){
			return (TC1 valueC1) => (T4 value4) => (T1 value1, T2 value2, T3 value3, T5 value5, T6 value6) => func(valueC1)(value1, value2, value3, value4, value5, value6);
		}

		public static Func<TC1, Func<T5, Func<T1, T2, T3, T4, T6, TResult>>> Curry5<TC1, T1, T2, T3, T4, T5, T6, TResult>(this Func<TC1, Func<T1, T2, T3, T4, T5, T6, TResult>> func){
			return (TC1 valueC1) => (T5 value5) => (T1 value1, T2 value2, T3 value3, T4 value4, T6 value6) => func(valueC1)(value1, value2, value3, value4, value5, value6);
		}

		public static Func<TC1, Func<T6, Func<T1, T2, T3, T4, T5, TResult>>> Curry6<TC1, T1, T2, T3, T4, T5, T6, TResult>(this Func<TC1, Func<T1, T2, T3, T4, T5, T6, TResult>> func){
			return (TC1 valueC1) => (T6 value6) => (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5) => func(valueC1)(value1, value2, value3, value4, value5, value6);
		}

		public static Func<TC1, Func<TC2, Func<T1, Func<T2, T3, T4, T5, TResult>>>> Curry1<TC1, TC2, T1, T2, T3, T4, T5, TResult>(this Func<TC1, Func<TC2, Func<T1, T2, T3, T4, T5, TResult>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (T1 value1) => (T2 value2, T3 value3, T4 value4, T5 value5) => func(valueC1)(valueC2)(value1, value2, value3, value4, value5);
		}

		public static Func<TC1, Func<TC2, Func<T2, Func<T1, T3, T4, T5, TResult>>>> Curry2<TC1, TC2, T1, T2, T3, T4, T5, TResult>(this Func<TC1, Func<TC2, Func<T1, T2, T3, T4, T5, TResult>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (T2 value2) => (T1 value1, T3 value3, T4 value4, T5 value5) => func(valueC1)(valueC2)(value1, value2, value3, value4, value5);
		}

		public static Func<TC1, Func<TC2, Func<T3, Func<T1, T2, T4, T5, TResult>>>> Curry3<TC1, TC2, T1, T2, T3, T4, T5, TResult>(this Func<TC1, Func<TC2, Func<T1, T2, T3, T4, T5, TResult>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (T3 value3) => (T1 value1, T2 value2, T4 value4, T5 value5) => func(valueC1)(valueC2)(value1, value2, value3, value4, value5);
		}

		public static Func<TC1, Func<TC2, Func<T4, Func<T1, T2, T3, T5, TResult>>>> Curry4<TC1, TC2, T1, T2, T3, T4, T5, TResult>(this Func<TC1, Func<TC2, Func<T1, T2, T3, T4, T5, TResult>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (T4 value4) => (T1 value1, T2 value2, T3 value3, T5 value5) => func(valueC1)(valueC2)(value1, value2, value3, value4, value5);
		}

		public static Func<TC1, Func<TC2, Func<T5, Func<T1, T2, T3, T4, TResult>>>> Curry5<TC1, TC2, T1, T2, T3, T4, T5, TResult>(this Func<TC1, Func<TC2, Func<T1, T2, T3, T4, T5, TResult>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (T5 value5) => (T1 value1, T2 value2, T3 value3, T4 value4) => func(valueC1)(valueC2)(value1, value2, value3, value4, value5);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<T1, Func<T2, T3, T4, TResult>>>>> Curry1<TC1, TC2, TC3, T1, T2, T3, T4, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<T1, T2, T3, T4, TResult>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (T1 value1) => (T2 value2, T3 value3, T4 value4) => func(valueC1)(valueC2)(valueC3)(value1, value2, value3, value4);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<T2, Func<T1, T3, T4, TResult>>>>> Curry2<TC1, TC2, TC3, T1, T2, T3, T4, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<T1, T2, T3, T4, TResult>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (T2 value2) => (T1 value1, T3 value3, T4 value4) => func(valueC1)(valueC2)(valueC3)(value1, value2, value3, value4);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<T3, Func<T1, T2, T4, TResult>>>>> Curry3<TC1, TC2, TC3, T1, T2, T3, T4, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<T1, T2, T3, T4, TResult>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (T3 value3) => (T1 value1, T2 value2, T4 value4) => func(valueC1)(valueC2)(valueC3)(value1, value2, value3, value4);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<T4, Func<T1, T2, T3, TResult>>>>> Curry4<TC1, TC2, TC3, T1, T2, T3, T4, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<T1, T2, T3, T4, TResult>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (T4 value4) => (T1 value1, T2 value2, T3 value3) => func(valueC1)(valueC2)(valueC3)(value1, value2, value3, value4);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<T1, Func<T2, T3, TResult>>>>>> Curry1<TC1, TC2, TC3, TC4, T1, T2, T3, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<T1, T2, T3, TResult>>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (T1 value1) => (T2 value2, T3 value3) => func(valueC1)(valueC2)(valueC3)(valueC4)(value1, value2, value3);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<T2, Func<T1, T3, TResult>>>>>> Curry2<TC1, TC2, TC3, TC4, T1, T2, T3, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<T1, T2, T3, TResult>>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (T2 value2) => (T1 value1, T3 value3) => func(valueC1)(valueC2)(valueC3)(valueC4)(value1, value2, value3);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<T3, Func<T1, T2, TResult>>>>>> Curry3<TC1, TC2, TC3, TC4, T1, T2, T3, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<T1, T2, T3, TResult>>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (T3 value3) => (T1 value1, T2 value2) => func(valueC1)(valueC2)(valueC3)(valueC4)(value1, value2, value3);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Func<T1, Func<T2, TResult>>>>>>> Curry1<TC1, TC2, TC3, TC4, TC5, T1, T2, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Func<T1, T2, TResult>>>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (TC5 valueC5) => (T1 value1) => (T2 value2) => func(valueC1)(valueC2)(valueC3)(valueC4)(valueC5)(value1, value2);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Func<T2, Func<T1, TResult>>>>>>> Curry2<TC1, TC2, TC3, TC4, TC5, T1, T2, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Func<T1, T2, TResult>>>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (TC5 valueC5) => (T2 value2) => (T1 value1) => func(valueC1)(valueC2)(valueC3)(valueC4)(valueC5)(value1, value2);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Func<TC6, Func<T1, Func<TResult>>>>>>>> Curry<TC1, TC2, TC3, TC4, TC5, TC6, T1, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Func<TC6, Func<T1, TResult>>>>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (TC5 valueC5) => (TC6 valueC6) => (T1 value1) => () => func(valueC1)(valueC2)(valueC3)(valueC4)(valueC5)(valueC6)(value1);
		}

		public static Func<T1, Func<T2, T3, T4, T5, T6, T7, T8, TResult>> Curry1<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func){
			return (T1 value1) => (T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8) => func(value1, value2, value3, value4, value5, value6, value7, value8);
		}

		public static Func<T2, Func<T1, T3, T4, T5, T6, T7, T8, TResult>> Curry2<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func){
			return (T2 value2) => (T1 value1, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8) => func(value1, value2, value3, value4, value5, value6, value7, value8);
		}

		public static Func<T3, Func<T1, T2, T4, T5, T6, T7, T8, TResult>> Curry3<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func){
			return (T3 value3) => (T1 value1, T2 value2, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8) => func(value1, value2, value3, value4, value5, value6, value7, value8);
		}

		public static Func<T4, Func<T1, T2, T3, T5, T6, T7, T8, TResult>> Curry4<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func){
			return (T4 value4) => (T1 value1, T2 value2, T3 value3, T5 value5, T6 value6, T7 value7, T8 value8) => func(value1, value2, value3, value4, value5, value6, value7, value8);
		}

		public static Func<T5, Func<T1, T2, T3, T4, T6, T7, T8, TResult>> Curry5<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func){
			return (T5 value5) => (T1 value1, T2 value2, T3 value3, T4 value4, T6 value6, T7 value7, T8 value8) => func(value1, value2, value3, value4, value5, value6, value7, value8);
		}

		public static Func<T6, Func<T1, T2, T3, T4, T5, T7, T8, TResult>> Curry6<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func){
			return (T6 value6) => (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T7 value7, T8 value8) => func(value1, value2, value3, value4, value5, value6, value7, value8);
		}

		public static Func<T7, Func<T1, T2, T3, T4, T5, T6, T8, TResult>> Curry7<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func){
			return (T7 value7) => (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T8 value8) => func(value1, value2, value3, value4, value5, value6, value7, value8);
		}

		public static Func<T8, Func<T1, T2, T3, T4, T5, T6, T7, TResult>> Curry8<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func){
			return (T8 value8) => (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7) => func(value1, value2, value3, value4, value5, value6, value7, value8);
		}

		public static Func<TC1, Func<T1, Func<T2, T3, T4, T5, T6, T7, TResult>>> Curry1<TC1, T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<TC1, Func<T1, T2, T3, T4, T5, T6, T7, TResult>> func){
			return (TC1 valueC1) => (T1 value1) => (T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7) => func(valueC1)(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<TC1, Func<T2, Func<T1, T3, T4, T5, T6, T7, TResult>>> Curry2<TC1, T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<TC1, Func<T1, T2, T3, T4, T5, T6, T7, TResult>> func){
			return (TC1 valueC1) => (T2 value2) => (T1 value1, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7) => func(valueC1)(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<TC1, Func<T3, Func<T1, T2, T4, T5, T6, T7, TResult>>> Curry3<TC1, T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<TC1, Func<T1, T2, T3, T4, T5, T6, T7, TResult>> func){
			return (TC1 valueC1) => (T3 value3) => (T1 value1, T2 value2, T4 value4, T5 value5, T6 value6, T7 value7) => func(valueC1)(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<TC1, Func<T4, Func<T1, T2, T3, T5, T6, T7, TResult>>> Curry4<TC1, T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<TC1, Func<T1, T2, T3, T4, T5, T6, T7, TResult>> func){
			return (TC1 valueC1) => (T4 value4) => (T1 value1, T2 value2, T3 value3, T5 value5, T6 value6, T7 value7) => func(valueC1)(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<TC1, Func<T5, Func<T1, T2, T3, T4, T6, T7, TResult>>> Curry5<TC1, T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<TC1, Func<T1, T2, T3, T4, T5, T6, T7, TResult>> func){
			return (TC1 valueC1) => (T5 value5) => (T1 value1, T2 value2, T3 value3, T4 value4, T6 value6, T7 value7) => func(valueC1)(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<TC1, Func<T6, Func<T1, T2, T3, T4, T5, T7, TResult>>> Curry6<TC1, T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<TC1, Func<T1, T2, T3, T4, T5, T6, T7, TResult>> func){
			return (TC1 valueC1) => (T6 value6) => (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T7 value7) => func(valueC1)(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<TC1, Func<T7, Func<T1, T2, T3, T4, T5, T6, TResult>>> Curry7<TC1, T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<TC1, Func<T1, T2, T3, T4, T5, T6, T7, TResult>> func){
			return (TC1 valueC1) => (T7 value7) => (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6) => func(valueC1)(value1, value2, value3, value4, value5, value6, value7);
		}

		public static Func<TC1, Func<TC2, Func<T1, Func<T2, T3, T4, T5, T6, TResult>>>> Curry1<TC1, TC2, T1, T2, T3, T4, T5, T6, TResult>(this Func<TC1, Func<TC2, Func<T1, T2, T3, T4, T5, T6, TResult>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (T1 value1) => (T2 value2, T3 value3, T4 value4, T5 value5, T6 value6) => func(valueC1)(valueC2)(value1, value2, value3, value4, value5, value6);
		}

		public static Func<TC1, Func<TC2, Func<T2, Func<T1, T3, T4, T5, T6, TResult>>>> Curry2<TC1, TC2, T1, T2, T3, T4, T5, T6, TResult>(this Func<TC1, Func<TC2, Func<T1, T2, T3, T4, T5, T6, TResult>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (T2 value2) => (T1 value1, T3 value3, T4 value4, T5 value5, T6 value6) => func(valueC1)(valueC2)(value1, value2, value3, value4, value5, value6);
		}

		public static Func<TC1, Func<TC2, Func<T3, Func<T1, T2, T4, T5, T6, TResult>>>> Curry3<TC1, TC2, T1, T2, T3, T4, T5, T6, TResult>(this Func<TC1, Func<TC2, Func<T1, T2, T3, T4, T5, T6, TResult>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (T3 value3) => (T1 value1, T2 value2, T4 value4, T5 value5, T6 value6) => func(valueC1)(valueC2)(value1, value2, value3, value4, value5, value6);
		}

		public static Func<TC1, Func<TC2, Func<T4, Func<T1, T2, T3, T5, T6, TResult>>>> Curry4<TC1, TC2, T1, T2, T3, T4, T5, T6, TResult>(this Func<TC1, Func<TC2, Func<T1, T2, T3, T4, T5, T6, TResult>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (T4 value4) => (T1 value1, T2 value2, T3 value3, T5 value5, T6 value6) => func(valueC1)(valueC2)(value1, value2, value3, value4, value5, value6);
		}

		public static Func<TC1, Func<TC2, Func<T5, Func<T1, T2, T3, T4, T6, TResult>>>> Curry5<TC1, TC2, T1, T2, T3, T4, T5, T6, TResult>(this Func<TC1, Func<TC2, Func<T1, T2, T3, T4, T5, T6, TResult>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (T5 value5) => (T1 value1, T2 value2, T3 value3, T4 value4, T6 value6) => func(valueC1)(valueC2)(value1, value2, value3, value4, value5, value6);
		}

		public static Func<TC1, Func<TC2, Func<T6, Func<T1, T2, T3, T4, T5, TResult>>>> Curry6<TC1, TC2, T1, T2, T3, T4, T5, T6, TResult>(this Func<TC1, Func<TC2, Func<T1, T2, T3, T4, T5, T6, TResult>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (T6 value6) => (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5) => func(valueC1)(valueC2)(value1, value2, value3, value4, value5, value6);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<T1, Func<T2, T3, T4, T5, TResult>>>>> Curry1<TC1, TC2, TC3, T1, T2, T3, T4, T5, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<T1, T2, T3, T4, T5, TResult>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (T1 value1) => (T2 value2, T3 value3, T4 value4, T5 value5) => func(valueC1)(valueC2)(valueC3)(value1, value2, value3, value4, value5);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<T2, Func<T1, T3, T4, T5, TResult>>>>> Curry2<TC1, TC2, TC3, T1, T2, T3, T4, T5, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<T1, T2, T3, T4, T5, TResult>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (T2 value2) => (T1 value1, T3 value3, T4 value4, T5 value5) => func(valueC1)(valueC2)(valueC3)(value1, value2, value3, value4, value5);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<T3, Func<T1, T2, T4, T5, TResult>>>>> Curry3<TC1, TC2, TC3, T1, T2, T3, T4, T5, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<T1, T2, T3, T4, T5, TResult>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (T3 value3) => (T1 value1, T2 value2, T4 value4, T5 value5) => func(valueC1)(valueC2)(valueC3)(value1, value2, value3, value4, value5);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<T4, Func<T1, T2, T3, T5, TResult>>>>> Curry4<TC1, TC2, TC3, T1, T2, T3, T4, T5, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<T1, T2, T3, T4, T5, TResult>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (T4 value4) => (T1 value1, T2 value2, T3 value3, T5 value5) => func(valueC1)(valueC2)(valueC3)(value1, value2, value3, value4, value5);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<T5, Func<T1, T2, T3, T4, TResult>>>>> Curry5<TC1, TC2, TC3, T1, T2, T3, T4, T5, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<T1, T2, T3, T4, T5, TResult>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (T5 value5) => (T1 value1, T2 value2, T3 value3, T4 value4) => func(valueC1)(valueC2)(valueC3)(value1, value2, value3, value4, value5);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<T1, Func<T2, T3, T4, TResult>>>>>> Curry1<TC1, TC2, TC3, TC4, T1, T2, T3, T4, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<T1, T2, T3, T4, TResult>>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (T1 value1) => (T2 value2, T3 value3, T4 value4) => func(valueC1)(valueC2)(valueC3)(valueC4)(value1, value2, value3, value4);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<T2, Func<T1, T3, T4, TResult>>>>>> Curry2<TC1, TC2, TC3, TC4, T1, T2, T3, T4, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<T1, T2, T3, T4, TResult>>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (T2 value2) => (T1 value1, T3 value3, T4 value4) => func(valueC1)(valueC2)(valueC3)(valueC4)(value1, value2, value3, value4);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<T3, Func<T1, T2, T4, TResult>>>>>> Curry3<TC1, TC2, TC3, TC4, T1, T2, T3, T4, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<T1, T2, T3, T4, TResult>>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (T3 value3) => (T1 value1, T2 value2, T4 value4) => func(valueC1)(valueC2)(valueC3)(valueC4)(value1, value2, value3, value4);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<T4, Func<T1, T2, T3, TResult>>>>>> Curry4<TC1, TC2, TC3, TC4, T1, T2, T3, T4, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<T1, T2, T3, T4, TResult>>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (T4 value4) => (T1 value1, T2 value2, T3 value3) => func(valueC1)(valueC2)(valueC3)(valueC4)(value1, value2, value3, value4);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Func<T1, Func<T2, T3, TResult>>>>>>> Curry1<TC1, TC2, TC3, TC4, TC5, T1, T2, T3, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Func<T1, T2, T3, TResult>>>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (TC5 valueC5) => (T1 value1) => (T2 value2, T3 value3) => func(valueC1)(valueC2)(valueC3)(valueC4)(valueC5)(value1, value2, value3);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Func<T2, Func<T1, T3, TResult>>>>>>> Curry2<TC1, TC2, TC3, TC4, TC5, T1, T2, T3, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Func<T1, T2, T3, TResult>>>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (TC5 valueC5) => (T2 value2) => (T1 value1, T3 value3) => func(valueC1)(valueC2)(valueC3)(valueC4)(valueC5)(value1, value2, value3);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Func<T3, Func<T1, T2, TResult>>>>>>> Curry3<TC1, TC2, TC3, TC4, TC5, T1, T2, T3, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Func<T1, T2, T3, TResult>>>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (TC5 valueC5) => (T3 value3) => (T1 value1, T2 value2) => func(valueC1)(valueC2)(valueC3)(valueC4)(valueC5)(value1, value2, value3);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Func<TC6, Func<T1, Func<T2, TResult>>>>>>>> Curry1<TC1, TC2, TC3, TC4, TC5, TC6, T1, T2, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Func<TC6, Func<T1, T2, TResult>>>>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (TC5 valueC5) => (TC6 valueC6) => (T1 value1) => (T2 value2) => func(valueC1)(valueC2)(valueC3)(valueC4)(valueC5)(valueC6)(value1, value2);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Func<TC6, Func<T2, Func<T1, TResult>>>>>>>> Curry2<TC1, TC2, TC3, TC4, TC5, TC6, T1, T2, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Func<TC6, Func<T1, T2, TResult>>>>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (TC5 valueC5) => (TC6 valueC6) => (T2 value2) => (T1 value1) => func(valueC1)(valueC2)(valueC3)(valueC4)(valueC5)(valueC6)(value1, value2);
		}

		public static Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Func<TC6, Func<TC7, Func<T1, Func<TResult>>>>>>>>> Curry<TC1, TC2, TC3, TC4, TC5, TC6, TC7, T1, TResult>(this Func<TC1, Func<TC2, Func<TC3, Func<TC4, Func<TC5, Func<TC6, Func<TC7, Func<T1, TResult>>>>>>>> func){
			return (TC1 valueC1) => (TC2 valueC2) => (TC3 valueC3) => (TC4 valueC4) => (TC5 valueC5) => (TC6 valueC6) => (TC7 valueC7) => (T1 value1) => () => func(valueC1)(valueC2)(valueC3)(valueC4)(valueC5)(valueC6)(valueC7)(value1);
		}

		#endregion

		#region Uncurry

		public static Action Uncurry(this Func<Action> func){
			return () => func()();
		}

		public static Action<TB1> Uncurry<TB1>(this Func<Action<TB1>> func){
			return (TB1 valueB1) => func()(valueB1);
		}

		public static Action<TB1, TB2> Uncurry<TB1, TB2>(this Func<Action<TB1, TB2>> func){
			return (TB1 valueB1, TB2 valueB2) => func()(valueB1, valueB2);
		}

		public static Action<TB1, TB2, TB3> Uncurry<TB1, TB2, TB3>(this Func<Action<TB1, TB2, TB3>> func){
			return (TB1 valueB1, TB2 valueB2, TB3 valueB3) => func()(valueB1, valueB2, valueB3);
		}

		public static Action<TB1, TB2, TB3, TB4> Uncurry<TB1, TB2, TB3, TB4>(this Func<Action<TB1, TB2, TB3, TB4>> func){
			return (TB1 valueB1, TB2 valueB2, TB3 valueB3, TB4 valueB4) => func()(valueB1, valueB2, valueB3, valueB4);
		}

		public static Action<TB1, TB2, TB3, TB4, TB5> Uncurry<TB1, TB2, TB3, TB4, TB5>(this Func<Action<TB1, TB2, TB3, TB4, TB5>> func){
			return (TB1 valueB1, TB2 valueB2, TB3 valueB3, TB4 valueB4, TB5 valueB5) => func()(valueB1, valueB2, valueB3, valueB4, valueB5);
		}

		public static Action<TB1, TB2, TB3, TB4, TB5, TB6> Uncurry<TB1, TB2, TB3, TB4, TB5, TB6>(this Func<Action<TB1, TB2, TB3, TB4, TB5, TB6>> func){
			return (TB1 valueB1, TB2 valueB2, TB3 valueB3, TB4 valueB4, TB5 valueB5, TB6 valueB6) => func()(valueB1, valueB2, valueB3, valueB4, valueB5, valueB6);
		}

		public static Action<TB1, TB2, TB3, TB4, TB5, TB6, TB7> Uncurry<TB1, TB2, TB3, TB4, TB5, TB6, TB7>(this Func<Action<TB1, TB2, TB3, TB4, TB5, TB6, TB7>> func){
			return (TB1 valueB1, TB2 valueB2, TB3 valueB3, TB4 valueB4, TB5 valueB5, TB6 valueB6, TB7 valueB7) => func()(valueB1, valueB2, valueB3, valueB4, valueB5, valueB6, valueB7);
		}

		public static Action<TB1, TB2, TB3, TB4, TB5, TB6, TB7, TB8> Uncurry<TB1, TB2, TB3, TB4, TB5, TB6, TB7, TB8>(this Func<Action<TB1, TB2, TB3, TB4, TB5, TB6, TB7, TB8>> func){
			return (TB1 valueB1, TB2 valueB2, TB3 valueB3, TB4 valueB4, TB5 valueB5, TB6 valueB6, TB7 valueB7, TB8 valueB8) => func()(valueB1, valueB2, valueB3, valueB4, valueB5, valueB6, valueB7, valueB8);
		}

		public static Action<TA1> Uncurry<TA1>(this Func<TA1, Action> func){
			return (TA1 valueA1) => func(valueA1)();
		}

		public static Action<TA1, TB1> Uncurry<TA1, TB1>(this Func<TA1, Action<TB1>> func){
			return (TA1 valueA1, TB1 valueB1) => func(valueA1)(valueB1);
		}

		public static Action<TA1, TB1, TB2> Uncurry<TA1, TB1, TB2>(this Func<TA1, Action<TB1, TB2>> func){
			return (TA1 valueA1, TB1 valueB1, TB2 valueB2) => func(valueA1)(valueB1, valueB2);
		}

		public static Action<TA1, TB1, TB2, TB3> Uncurry<TA1, TB1, TB2, TB3>(this Func<TA1, Action<TB1, TB2, TB3>> func){
			return (TA1 valueA1, TB1 valueB1, TB2 valueB2, TB3 valueB3) => func(valueA1)(valueB1, valueB2, valueB3);
		}

		public static Action<TA1, TB1, TB2, TB3, TB4> Uncurry<TA1, TB1, TB2, TB3, TB4>(this Func<TA1, Action<TB1, TB2, TB3, TB4>> func){
			return (TA1 valueA1, TB1 valueB1, TB2 valueB2, TB3 valueB3, TB4 valueB4) => func(valueA1)(valueB1, valueB2, valueB3, valueB4);
		}

		public static Action<TA1, TB1, TB2, TB3, TB4, TB5> Uncurry<TA1, TB1, TB2, TB3, TB4, TB5>(this Func<TA1, Action<TB1, TB2, TB3, TB4, TB5>> func){
			return (TA1 valueA1, TB1 valueB1, TB2 valueB2, TB3 valueB3, TB4 valueB4, TB5 valueB5) => func(valueA1)(valueB1, valueB2, valueB3, valueB4, valueB5);
		}

		public static Action<TA1, TB1, TB2, TB3, TB4, TB5, TB6> Uncurry<TA1, TB1, TB2, TB3, TB4, TB5, TB6>(this Func<TA1, Action<TB1, TB2, TB3, TB4, TB5, TB6>> func){
			return (TA1 valueA1, TB1 valueB1, TB2 valueB2, TB3 valueB3, TB4 valueB4, TB5 valueB5, TB6 valueB6) => func(valueA1)(valueB1, valueB2, valueB3, valueB4, valueB5, valueB6);
		}

		public static Action<TA1, TB1, TB2, TB3, TB4, TB5, TB6, TB7> Uncurry<TA1, TB1, TB2, TB3, TB4, TB5, TB6, TB7>(this Func<TA1, Action<TB1, TB2, TB3, TB4, TB5, TB6, TB7>> func){
			return (TA1 valueA1, TB1 valueB1, TB2 valueB2, TB3 valueB3, TB4 valueB4, TB5 valueB5, TB6 valueB6, TB7 valueB7) => func(valueA1)(valueB1, valueB2, valueB3, valueB4, valueB5, valueB6, valueB7);
		}

		public static Action<TA1, TA2> Uncurry<TA1, TA2>(this Func<TA1, TA2, Action> func){
			return (TA1 valueA1, TA2 valueA2) => func(valueA1, valueA2)();
		}

		public static Action<TA1, TA2, TB1> Uncurry<TA1, TA2, TB1>(this Func<TA1, TA2, Action<TB1>> func){
			return (TA1 valueA1, TA2 valueA2, TB1 valueB1) => func(valueA1, valueA2)(valueB1);
		}

		public static Action<TA1, TA2, TB1, TB2> Uncurry<TA1, TA2, TB1, TB2>(this Func<TA1, TA2, Action<TB1, TB2>> func){
			return (TA1 valueA1, TA2 valueA2, TB1 valueB1, TB2 valueB2) => func(valueA1, valueA2)(valueB1, valueB2);
		}

		public static Action<TA1, TA2, TB1, TB2, TB3> Uncurry<TA1, TA2, TB1, TB2, TB3>(this Func<TA1, TA2, Action<TB1, TB2, TB3>> func){
			return (TA1 valueA1, TA2 valueA2, TB1 valueB1, TB2 valueB2, TB3 valueB3) => func(valueA1, valueA2)(valueB1, valueB2, valueB3);
		}

		public static Action<TA1, TA2, TB1, TB2, TB3, TB4> Uncurry<TA1, TA2, TB1, TB2, TB3, TB4>(this Func<TA1, TA2, Action<TB1, TB2, TB3, TB4>> func){
			return (TA1 valueA1, TA2 valueA2, TB1 valueB1, TB2 valueB2, TB3 valueB3, TB4 valueB4) => func(valueA1, valueA2)(valueB1, valueB2, valueB3, valueB4);
		}

		public static Action<TA1, TA2, TB1, TB2, TB3, TB4, TB5> Uncurry<TA1, TA2, TB1, TB2, TB3, TB4, TB5>(this Func<TA1, TA2, Action<TB1, TB2, TB3, TB4, TB5>> func){
			return (TA1 valueA1, TA2 valueA2, TB1 valueB1, TB2 valueB2, TB3 valueB3, TB4 valueB4, TB5 valueB5) => func(valueA1, valueA2)(valueB1, valueB2, valueB3, valueB4, valueB5);
		}

		public static Action<TA1, TA2, TB1, TB2, TB3, TB4, TB5, TB6> Uncurry<TA1, TA2, TB1, TB2, TB3, TB4, TB5, TB6>(this Func<TA1, TA2, Action<TB1, TB2, TB3, TB4, TB5, TB6>> func){
			return (TA1 valueA1, TA2 valueA2, TB1 valueB1, TB2 valueB2, TB3 valueB3, TB4 valueB4, TB5 valueB5, TB6 valueB6) => func(valueA1, valueA2)(valueB1, valueB2, valueB3, valueB4, valueB5, valueB6);
		}

		public static Action<TA1, TA2, TA3> Uncurry<TA1, TA2, TA3>(this Func<TA1, TA2, TA3, Action> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3) => func(valueA1, valueA2, valueA3)();
		}

		public static Action<TA1, TA2, TA3, TB1> Uncurry<TA1, TA2, TA3, TB1>(this Func<TA1, TA2, TA3, Action<TB1>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TB1 valueB1) => func(valueA1, valueA2, valueA3)(valueB1);
		}

		public static Action<TA1, TA2, TA3, TB1, TB2> Uncurry<TA1, TA2, TA3, TB1, TB2>(this Func<TA1, TA2, TA3, Action<TB1, TB2>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TB1 valueB1, TB2 valueB2) => func(valueA1, valueA2, valueA3)(valueB1, valueB2);
		}

		public static Action<TA1, TA2, TA3, TB1, TB2, TB3> Uncurry<TA1, TA2, TA3, TB1, TB2, TB3>(this Func<TA1, TA2, TA3, Action<TB1, TB2, TB3>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TB1 valueB1, TB2 valueB2, TB3 valueB3) => func(valueA1, valueA2, valueA3)(valueB1, valueB2, valueB3);
		}

		public static Action<TA1, TA2, TA3, TB1, TB2, TB3, TB4> Uncurry<TA1, TA2, TA3, TB1, TB2, TB3, TB4>(this Func<TA1, TA2, TA3, Action<TB1, TB2, TB3, TB4>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TB1 valueB1, TB2 valueB2, TB3 valueB3, TB4 valueB4) => func(valueA1, valueA2, valueA3)(valueB1, valueB2, valueB3, valueB4);
		}

		public static Action<TA1, TA2, TA3, TB1, TB2, TB3, TB4, TB5> Uncurry<TA1, TA2, TA3, TB1, TB2, TB3, TB4, TB5>(this Func<TA1, TA2, TA3, Action<TB1, TB2, TB3, TB4, TB5>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TB1 valueB1, TB2 valueB2, TB3 valueB3, TB4 valueB4, TB5 valueB5) => func(valueA1, valueA2, valueA3)(valueB1, valueB2, valueB3, valueB4, valueB5);
		}

		public static Action<TA1, TA2, TA3, TA4> Uncurry<TA1, TA2, TA3, TA4>(this Func<TA1, TA2, TA3, TA4, Action> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TA4 valueA4) => func(valueA1, valueA2, valueA3, valueA4)();
		}

		public static Action<TA1, TA2, TA3, TA4, TB1> Uncurry<TA1, TA2, TA3, TA4, TB1>(this Func<TA1, TA2, TA3, TA4, Action<TB1>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TA4 valueA4, TB1 valueB1) => func(valueA1, valueA2, valueA3, valueA4)(valueB1);
		}

		public static Action<TA1, TA2, TA3, TA4, TB1, TB2> Uncurry<TA1, TA2, TA3, TA4, TB1, TB2>(this Func<TA1, TA2, TA3, TA4, Action<TB1, TB2>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TA4 valueA4, TB1 valueB1, TB2 valueB2) => func(valueA1, valueA2, valueA3, valueA4)(valueB1, valueB2);
		}

		public static Action<TA1, TA2, TA3, TA4, TB1, TB2, TB3> Uncurry<TA1, TA2, TA3, TA4, TB1, TB2, TB3>(this Func<TA1, TA2, TA3, TA4, Action<TB1, TB2, TB3>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TA4 valueA4, TB1 valueB1, TB2 valueB2, TB3 valueB3) => func(valueA1, valueA2, valueA3, valueA4)(valueB1, valueB2, valueB3);
		}

		public static Action<TA1, TA2, TA3, TA4, TB1, TB2, TB3, TB4> Uncurry<TA1, TA2, TA3, TA4, TB1, TB2, TB3, TB4>(this Func<TA1, TA2, TA3, TA4, Action<TB1, TB2, TB3, TB4>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TA4 valueA4, TB1 valueB1, TB2 valueB2, TB3 valueB3, TB4 valueB4) => func(valueA1, valueA2, valueA3, valueA4)(valueB1, valueB2, valueB3, valueB4);
		}

		public static Action<TA1, TA2, TA3, TA4, TA5> Uncurry<TA1, TA2, TA3, TA4, TA5>(this Func<TA1, TA2, TA3, TA4, TA5, Action> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TA4 valueA4, TA5 valueA5) => func(valueA1, valueA2, valueA3, valueA4, valueA5)();
		}

		public static Action<TA1, TA2, TA3, TA4, TA5, TB1> Uncurry<TA1, TA2, TA3, TA4, TA5, TB1>(this Func<TA1, TA2, TA3, TA4, TA5, Action<TB1>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TA4 valueA4, TA5 valueA5, TB1 valueB1) => func(valueA1, valueA2, valueA3, valueA4, valueA5)(valueB1);
		}

		public static Action<TA1, TA2, TA3, TA4, TA5, TB1, TB2> Uncurry<TA1, TA2, TA3, TA4, TA5, TB1, TB2>(this Func<TA1, TA2, TA3, TA4, TA5, Action<TB1, TB2>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TA4 valueA4, TA5 valueA5, TB1 valueB1, TB2 valueB2) => func(valueA1, valueA2, valueA3, valueA4, valueA5)(valueB1, valueB2);
		}

		public static Action<TA1, TA2, TA3, TA4, TA5, TB1, TB2, TB3> Uncurry<TA1, TA2, TA3, TA4, TA5, TB1, TB2, TB3>(this Func<TA1, TA2, TA3, TA4, TA5, Action<TB1, TB2, TB3>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TA4 valueA4, TA5 valueA5, TB1 valueB1, TB2 valueB2, TB3 valueB3) => func(valueA1, valueA2, valueA3, valueA4, valueA5)(valueB1, valueB2, valueB3);
		}

		public static Action<TA1, TA2, TA3, TA4, TA5, TA6> Uncurry<TA1, TA2, TA3, TA4, TA5, TA6>(this Func<TA1, TA2, TA3, TA4, TA5, TA6, Action> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TA4 valueA4, TA5 valueA5, TA6 valueA6) => func(valueA1, valueA2, valueA3, valueA4, valueA5, valueA6)();
		}

		public static Action<TA1, TA2, TA3, TA4, TA5, TA6, TB1> Uncurry<TA1, TA2, TA3, TA4, TA5, TA6, TB1>(this Func<TA1, TA2, TA3, TA4, TA5, TA6, Action<TB1>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TA4 valueA4, TA5 valueA5, TA6 valueA6, TB1 valueB1) => func(valueA1, valueA2, valueA3, valueA4, valueA5, valueA6)(valueB1);
		}

		public static Action<TA1, TA2, TA3, TA4, TA5, TA6, TB1, TB2> Uncurry<TA1, TA2, TA3, TA4, TA5, TA6, TB1, TB2>(this Func<TA1, TA2, TA3, TA4, TA5, TA6, Action<TB1, TB2>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TA4 valueA4, TA5 valueA5, TA6 valueA6, TB1 valueB1, TB2 valueB2) => func(valueA1, valueA2, valueA3, valueA4, valueA5, valueA6)(valueB1, valueB2);
		}

		public static Action<TA1, TA2, TA3, TA4, TA5, TA6, TA7> Uncurry<TA1, TA2, TA3, TA4, TA5, TA6, TA7>(this Func<TA1, TA2, TA3, TA4, TA5, TA6, TA7, Action> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TA4 valueA4, TA5 valueA5, TA6 valueA6, TA7 valueA7) => func(valueA1, valueA2, valueA3, valueA4, valueA5, valueA6, valueA7)();
		}

		public static Action<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TB1> Uncurry<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TB1>(this Func<TA1, TA2, TA3, TA4, TA5, TA6, TA7, Action<TB1>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TA4 valueA4, TA5 valueA5, TA6 valueA6, TA7 valueA7, TB1 valueB1) => func(valueA1, valueA2, valueA3, valueA4, valueA5, valueA6, valueA7)(valueB1);
		}

		public static Action<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8> Uncurry<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8>(this Func<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, Action> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TA4 valueA4, TA5 valueA5, TA6 valueA6, TA7 valueA7, TA8 valueA8) => func(valueA1, valueA2, valueA3, valueA4, valueA5, valueA6, valueA7, valueA8)();
		}

		public static Func<TResult> Uncurry<TResult>(this Func<Func<TResult>> func){
			return () => func()();
		}

		public static Func<TB1, TResult> Uncurry<TB1, TResult>(this Func<Func<TB1, TResult>> func){
			return (TB1 valueB1) => func()(valueB1);
		}

		public static Func<TB1, TB2, TResult> Uncurry<TB1, TB2, TResult>(this Func<Func<TB1, TB2, TResult>> func){
			return (TB1 valueB1, TB2 valueB2) => func()(valueB1, valueB2);
		}

		public static Func<TB1, TB2, TB3, TResult> Uncurry<TB1, TB2, TB3, TResult>(this Func<Func<TB1, TB2, TB3, TResult>> func){
			return (TB1 valueB1, TB2 valueB2, TB3 valueB3) => func()(valueB1, valueB2, valueB3);
		}

		public static Func<TB1, TB2, TB3, TB4, TResult> Uncurry<TB1, TB2, TB3, TB4, TResult>(this Func<Func<TB1, TB2, TB3, TB4, TResult>> func){
			return (TB1 valueB1, TB2 valueB2, TB3 valueB3, TB4 valueB4) => func()(valueB1, valueB2, valueB3, valueB4);
		}

		public static Func<TB1, TB2, TB3, TB4, TB5, TResult> Uncurry<TB1, TB2, TB3, TB4, TB5, TResult>(this Func<Func<TB1, TB2, TB3, TB4, TB5, TResult>> func){
			return (TB1 valueB1, TB2 valueB2, TB3 valueB3, TB4 valueB4, TB5 valueB5) => func()(valueB1, valueB2, valueB3, valueB4, valueB5);
		}

		public static Func<TB1, TB2, TB3, TB4, TB5, TB6, TResult> Uncurry<TB1, TB2, TB3, TB4, TB5, TB6, TResult>(this Func<Func<TB1, TB2, TB3, TB4, TB5, TB6, TResult>> func){
			return (TB1 valueB1, TB2 valueB2, TB3 valueB3, TB4 valueB4, TB5 valueB5, TB6 valueB6) => func()(valueB1, valueB2, valueB3, valueB4, valueB5, valueB6);
		}

		public static Func<TB1, TB2, TB3, TB4, TB5, TB6, TB7, TResult> Uncurry<TB1, TB2, TB3, TB4, TB5, TB6, TB7, TResult>(this Func<Func<TB1, TB2, TB3, TB4, TB5, TB6, TB7, TResult>> func){
			return (TB1 valueB1, TB2 valueB2, TB3 valueB3, TB4 valueB4, TB5 valueB5, TB6 valueB6, TB7 valueB7) => func()(valueB1, valueB2, valueB3, valueB4, valueB5, valueB6, valueB7);
		}

		public static Func<TB1, TB2, TB3, TB4, TB5, TB6, TB7, TB8, TResult> Uncurry<TB1, TB2, TB3, TB4, TB5, TB6, TB7, TB8, TResult>(this Func<Func<TB1, TB2, TB3, TB4, TB5, TB6, TB7, TB8, TResult>> func){
			return (TB1 valueB1, TB2 valueB2, TB3 valueB3, TB4 valueB4, TB5 valueB5, TB6 valueB6, TB7 valueB7, TB8 valueB8) => func()(valueB1, valueB2, valueB3, valueB4, valueB5, valueB6, valueB7, valueB8);
		}

		public static Func<TA1, TResult> Uncurry<TA1, TResult>(this Func<TA1, Func<TResult>> func){
			return (TA1 valueA1) => func(valueA1)();
		}

		public static Func<TA1, TB1, TResult> Uncurry<TA1, TB1, TResult>(this Func<TA1, Func<TB1, TResult>> func){
			return (TA1 valueA1, TB1 valueB1) => func(valueA1)(valueB1);
		}

		public static Func<TA1, TB1, TB2, TResult> Uncurry<TA1, TB1, TB2, TResult>(this Func<TA1, Func<TB1, TB2, TResult>> func){
			return (TA1 valueA1, TB1 valueB1, TB2 valueB2) => func(valueA1)(valueB1, valueB2);
		}

		public static Func<TA1, TB1, TB2, TB3, TResult> Uncurry<TA1, TB1, TB2, TB3, TResult>(this Func<TA1, Func<TB1, TB2, TB3, TResult>> func){
			return (TA1 valueA1, TB1 valueB1, TB2 valueB2, TB3 valueB3) => func(valueA1)(valueB1, valueB2, valueB3);
		}

		public static Func<TA1, TB1, TB2, TB3, TB4, TResult> Uncurry<TA1, TB1, TB2, TB3, TB4, TResult>(this Func<TA1, Func<TB1, TB2, TB3, TB4, TResult>> func){
			return (TA1 valueA1, TB1 valueB1, TB2 valueB2, TB3 valueB3, TB4 valueB4) => func(valueA1)(valueB1, valueB2, valueB3, valueB4);
		}

		public static Func<TA1, TB1, TB2, TB3, TB4, TB5, TResult> Uncurry<TA1, TB1, TB2, TB3, TB4, TB5, TResult>(this Func<TA1, Func<TB1, TB2, TB3, TB4, TB5, TResult>> func){
			return (TA1 valueA1, TB1 valueB1, TB2 valueB2, TB3 valueB3, TB4 valueB4, TB5 valueB5) => func(valueA1)(valueB1, valueB2, valueB3, valueB4, valueB5);
		}

		public static Func<TA1, TB1, TB2, TB3, TB4, TB5, TB6, TResult> Uncurry<TA1, TB1, TB2, TB3, TB4, TB5, TB6, TResult>(this Func<TA1, Func<TB1, TB2, TB3, TB4, TB5, TB6, TResult>> func){
			return (TA1 valueA1, TB1 valueB1, TB2 valueB2, TB3 valueB3, TB4 valueB4, TB5 valueB5, TB6 valueB6) => func(valueA1)(valueB1, valueB2, valueB3, valueB4, valueB5, valueB6);
		}

		public static Func<TA1, TB1, TB2, TB3, TB4, TB5, TB6, TB7, TResult> Uncurry<TA1, TB1, TB2, TB3, TB4, TB5, TB6, TB7, TResult>(this Func<TA1, Func<TB1, TB2, TB3, TB4, TB5, TB6, TB7, TResult>> func){
			return (TA1 valueA1, TB1 valueB1, TB2 valueB2, TB3 valueB3, TB4 valueB4, TB5 valueB5, TB6 valueB6, TB7 valueB7) => func(valueA1)(valueB1, valueB2, valueB3, valueB4, valueB5, valueB6, valueB7);
		}

		public static Func<TA1, TA2, TResult> Uncurry<TA1, TA2, TResult>(this Func<TA1, TA2, Func<TResult>> func){
			return (TA1 valueA1, TA2 valueA2) => func(valueA1, valueA2)();
		}

		public static Func<TA1, TA2, TB1, TResult> Uncurry<TA1, TA2, TB1, TResult>(this Func<TA1, TA2, Func<TB1, TResult>> func){
			return (TA1 valueA1, TA2 valueA2, TB1 valueB1) => func(valueA1, valueA2)(valueB1);
		}

		public static Func<TA1, TA2, TB1, TB2, TResult> Uncurry<TA1, TA2, TB1, TB2, TResult>(this Func<TA1, TA2, Func<TB1, TB2, TResult>> func){
			return (TA1 valueA1, TA2 valueA2, TB1 valueB1, TB2 valueB2) => func(valueA1, valueA2)(valueB1, valueB2);
		}

		public static Func<TA1, TA2, TB1, TB2, TB3, TResult> Uncurry<TA1, TA2, TB1, TB2, TB3, TResult>(this Func<TA1, TA2, Func<TB1, TB2, TB3, TResult>> func){
			return (TA1 valueA1, TA2 valueA2, TB1 valueB1, TB2 valueB2, TB3 valueB3) => func(valueA1, valueA2)(valueB1, valueB2, valueB3);
		}

		public static Func<TA1, TA2, TB1, TB2, TB3, TB4, TResult> Uncurry<TA1, TA2, TB1, TB2, TB3, TB4, TResult>(this Func<TA1, TA2, Func<TB1, TB2, TB3, TB4, TResult>> func){
			return (TA1 valueA1, TA2 valueA2, TB1 valueB1, TB2 valueB2, TB3 valueB3, TB4 valueB4) => func(valueA1, valueA2)(valueB1, valueB2, valueB3, valueB4);
		}

		public static Func<TA1, TA2, TB1, TB2, TB3, TB4, TB5, TResult> Uncurry<TA1, TA2, TB1, TB2, TB3, TB4, TB5, TResult>(this Func<TA1, TA2, Func<TB1, TB2, TB3, TB4, TB5, TResult>> func){
			return (TA1 valueA1, TA2 valueA2, TB1 valueB1, TB2 valueB2, TB3 valueB3, TB4 valueB4, TB5 valueB5) => func(valueA1, valueA2)(valueB1, valueB2, valueB3, valueB4, valueB5);
		}

		public static Func<TA1, TA2, TB1, TB2, TB3, TB4, TB5, TB6, TResult> Uncurry<TA1, TA2, TB1, TB2, TB3, TB4, TB5, TB6, TResult>(this Func<TA1, TA2, Func<TB1, TB2, TB3, TB4, TB5, TB6, TResult>> func){
			return (TA1 valueA1, TA2 valueA2, TB1 valueB1, TB2 valueB2, TB3 valueB3, TB4 valueB4, TB5 valueB5, TB6 valueB6) => func(valueA1, valueA2)(valueB1, valueB2, valueB3, valueB4, valueB5, valueB6);
		}

		public static Func<TA1, TA2, TA3, TResult> Uncurry<TA1, TA2, TA3, TResult>(this Func<TA1, TA2, TA3, Func<TResult>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3) => func(valueA1, valueA2, valueA3)();
		}

		public static Func<TA1, TA2, TA3, TB1, TResult> Uncurry<TA1, TA2, TA3, TB1, TResult>(this Func<TA1, TA2, TA3, Func<TB1, TResult>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TB1 valueB1) => func(valueA1, valueA2, valueA3)(valueB1);
		}

		public static Func<TA1, TA2, TA3, TB1, TB2, TResult> Uncurry<TA1, TA2, TA3, TB1, TB2, TResult>(this Func<TA1, TA2, TA3, Func<TB1, TB2, TResult>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TB1 valueB1, TB2 valueB2) => func(valueA1, valueA2, valueA3)(valueB1, valueB2);
		}

		public static Func<TA1, TA2, TA3, TB1, TB2, TB3, TResult> Uncurry<TA1, TA2, TA3, TB1, TB2, TB3, TResult>(this Func<TA1, TA2, TA3, Func<TB1, TB2, TB3, TResult>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TB1 valueB1, TB2 valueB2, TB3 valueB3) => func(valueA1, valueA2, valueA3)(valueB1, valueB2, valueB3);
		}

		public static Func<TA1, TA2, TA3, TB1, TB2, TB3, TB4, TResult> Uncurry<TA1, TA2, TA3, TB1, TB2, TB3, TB4, TResult>(this Func<TA1, TA2, TA3, Func<TB1, TB2, TB3, TB4, TResult>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TB1 valueB1, TB2 valueB2, TB3 valueB3, TB4 valueB4) => func(valueA1, valueA2, valueA3)(valueB1, valueB2, valueB3, valueB4);
		}

		public static Func<TA1, TA2, TA3, TB1, TB2, TB3, TB4, TB5, TResult> Uncurry<TA1, TA2, TA3, TB1, TB2, TB3, TB4, TB5, TResult>(this Func<TA1, TA2, TA3, Func<TB1, TB2, TB3, TB4, TB5, TResult>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TB1 valueB1, TB2 valueB2, TB3 valueB3, TB4 valueB4, TB5 valueB5) => func(valueA1, valueA2, valueA3)(valueB1, valueB2, valueB3, valueB4, valueB5);
		}

		public static Func<TA1, TA2, TA3, TA4, TResult> Uncurry<TA1, TA2, TA3, TA4, TResult>(this Func<TA1, TA2, TA3, TA4, Func<TResult>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TA4 valueA4) => func(valueA1, valueA2, valueA3, valueA4)();
		}

		public static Func<TA1, TA2, TA3, TA4, TB1, TResult> Uncurry<TA1, TA2, TA3, TA4, TB1, TResult>(this Func<TA1, TA2, TA3, TA4, Func<TB1, TResult>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TA4 valueA4, TB1 valueB1) => func(valueA1, valueA2, valueA3, valueA4)(valueB1);
		}

		public static Func<TA1, TA2, TA3, TA4, TB1, TB2, TResult> Uncurry<TA1, TA2, TA3, TA4, TB1, TB2, TResult>(this Func<TA1, TA2, TA3, TA4, Func<TB1, TB2, TResult>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TA4 valueA4, TB1 valueB1, TB2 valueB2) => func(valueA1, valueA2, valueA3, valueA4)(valueB1, valueB2);
		}

		public static Func<TA1, TA2, TA3, TA4, TB1, TB2, TB3, TResult> Uncurry<TA1, TA2, TA3, TA4, TB1, TB2, TB3, TResult>(this Func<TA1, TA2, TA3, TA4, Func<TB1, TB2, TB3, TResult>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TA4 valueA4, TB1 valueB1, TB2 valueB2, TB3 valueB3) => func(valueA1, valueA2, valueA3, valueA4)(valueB1, valueB2, valueB3);
		}

		public static Func<TA1, TA2, TA3, TA4, TB1, TB2, TB3, TB4, TResult> Uncurry<TA1, TA2, TA3, TA4, TB1, TB2, TB3, TB4, TResult>(this Func<TA1, TA2, TA3, TA4, Func<TB1, TB2, TB3, TB4, TResult>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TA4 valueA4, TB1 valueB1, TB2 valueB2, TB3 valueB3, TB4 valueB4) => func(valueA1, valueA2, valueA3, valueA4)(valueB1, valueB2, valueB3, valueB4);
		}

		public static Func<TA1, TA2, TA3, TA4, TA5, TResult> Uncurry<TA1, TA2, TA3, TA4, TA5, TResult>(this Func<TA1, TA2, TA3, TA4, TA5, Func<TResult>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TA4 valueA4, TA5 valueA5) => func(valueA1, valueA2, valueA3, valueA4, valueA5)();
		}

		public static Func<TA1, TA2, TA3, TA4, TA5, TB1, TResult> Uncurry<TA1, TA2, TA3, TA4, TA5, TB1, TResult>(this Func<TA1, TA2, TA3, TA4, TA5, Func<TB1, TResult>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TA4 valueA4, TA5 valueA5, TB1 valueB1) => func(valueA1, valueA2, valueA3, valueA4, valueA5)(valueB1);
		}

		public static Func<TA1, TA2, TA3, TA4, TA5, TB1, TB2, TResult> Uncurry<TA1, TA2, TA3, TA4, TA5, TB1, TB2, TResult>(this Func<TA1, TA2, TA3, TA4, TA5, Func<TB1, TB2, TResult>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TA4 valueA4, TA5 valueA5, TB1 valueB1, TB2 valueB2) => func(valueA1, valueA2, valueA3, valueA4, valueA5)(valueB1, valueB2);
		}

		public static Func<TA1, TA2, TA3, TA4, TA5, TB1, TB2, TB3, TResult> Uncurry<TA1, TA2, TA3, TA4, TA5, TB1, TB2, TB3, TResult>(this Func<TA1, TA2, TA3, TA4, TA5, Func<TB1, TB2, TB3, TResult>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TA4 valueA4, TA5 valueA5, TB1 valueB1, TB2 valueB2, TB3 valueB3) => func(valueA1, valueA2, valueA3, valueA4, valueA5)(valueB1, valueB2, valueB3);
		}

		public static Func<TA1, TA2, TA3, TA4, TA5, TA6, TResult> Uncurry<TA1, TA2, TA3, TA4, TA5, TA6, TResult>(this Func<TA1, TA2, TA3, TA4, TA5, TA6, Func<TResult>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TA4 valueA4, TA5 valueA5, TA6 valueA6) => func(valueA1, valueA2, valueA3, valueA4, valueA5, valueA6)();
		}

		public static Func<TA1, TA2, TA3, TA4, TA5, TA6, TB1, TResult> Uncurry<TA1, TA2, TA3, TA4, TA5, TA6, TB1, TResult>(this Func<TA1, TA2, TA3, TA4, TA5, TA6, Func<TB1, TResult>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TA4 valueA4, TA5 valueA5, TA6 valueA6, TB1 valueB1) => func(valueA1, valueA2, valueA3, valueA4, valueA5, valueA6)(valueB1);
		}

		public static Func<TA1, TA2, TA3, TA4, TA5, TA6, TB1, TB2, TResult> Uncurry<TA1, TA2, TA3, TA4, TA5, TA6, TB1, TB2, TResult>(this Func<TA1, TA2, TA3, TA4, TA5, TA6, Func<TB1, TB2, TResult>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TA4 valueA4, TA5 valueA5, TA6 valueA6, TB1 valueB1, TB2 valueB2) => func(valueA1, valueA2, valueA3, valueA4, valueA5, valueA6)(valueB1, valueB2);
		}

		public static Func<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TResult> Uncurry<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TResult>(this Func<TA1, TA2, TA3, TA4, TA5, TA6, TA7, Func<TResult>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TA4 valueA4, TA5 valueA5, TA6 valueA6, TA7 valueA7) => func(valueA1, valueA2, valueA3, valueA4, valueA5, valueA6, valueA7)();
		}

		public static Func<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TB1, TResult> Uncurry<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TB1, TResult>(this Func<TA1, TA2, TA3, TA4, TA5, TA6, TA7, Func<TB1, TResult>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TA4 valueA4, TA5 valueA5, TA6 valueA6, TA7 valueA7, TB1 valueB1) => func(valueA1, valueA2, valueA3, valueA4, valueA5, valueA6, valueA7)(valueB1);
		}

		public static Func<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TResult> Uncurry<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TResult>(this Func<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, Func<TResult>> func){
			return (TA1 valueA1, TA2 valueA2, TA3 valueA3, TA4 valueA4, TA5 valueA5, TA6 valueA6, TA7 valueA7, TA8 valueA8) => func(valueA1, valueA2, valueA3, valueA4, valueA5, valueA6, valueA7, valueA8)();
		}

		#endregion
	}
}
