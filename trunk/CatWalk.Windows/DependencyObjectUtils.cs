/*
	$Id: DependencyObjectEx.cs 137 2010-12-19 10:01:30Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Windows;
using System.Windows.Threading;

namespace CatWalk.Windows{
	public static class DependencyObjectUtils{
		public static object SafeGetValue(this DependencyObject obj, DependencyProperty dp){
			if(obj.CheckAccess()){
				return obj.GetValue(dp);
			}else{
				return obj.Dispatcher.Invoke(
					DispatcherPriority.Normal,
					new Func<DependencyProperty, object>(obj.GetValue),
					dp);
			}
		}
		
		public static void SafeSetValue(this DependencyObject obj, DependencyProperty dp, object value){
			if(obj.CheckAccess()){
				obj.SetValue(dp, value);
			}else{
				obj.Dispatcher.Invoke(
					DispatcherPriority.Normal,
					new Action<DependencyProperty, object>(obj.SetValue),
					obj,
					new object[]{value});
			}
		}
		
		public static void SafeSetValueAsync(this DependencyObject obj, DependencyProperty dp, object value){
			if(obj.CheckAccess()){
				obj.SetValue(dp, value);
			}else{
				obj.Dispatcher.BeginInvoke(
					DispatcherPriority.Normal,
					new Action<DependencyProperty, object>(obj.SetValue),
					obj,
					new object[]{value});
			}
		}
	}
}