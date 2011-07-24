/*
	$Id: Account.cs 51 2010-01-28 21:55:33Z catwalk $
*/
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

namespace MazeGenSL.Views {
	public static class PropertyPathResolver {
		private static WeakReference _DummyObject;
		private static readonly DependencyProperty _DummyProperty =
			DependencyProperty.RegisterAttached("Dummy", typeof(object), typeof(DependencyObject), new PropertyMetadata(null));

		public static object Resolve(object obj, string path) {
			return Resolve(new PropertyPath(path), obj);
		}
		public static object Resolve(this PropertyPath path, object obj) {
			DependencyObject dummy = null;
			if(_DummyObject == null){
				dummy = new DummyDependencyObject();
				_DummyObject = new WeakReference(dummy);
			}else{
				dummy = _DummyObject.Target as DummyDependencyObject;
				if(dummy == null){
					dummy = new DummyDependencyObject();
					_DummyObject = new WeakReference(dummy);
				}
			}

			var binding = new Binding(){Path = path, Source = obj};
			BindingOperations.SetBinding(dummy, _DummyProperty, binding);
			return dummy.GetValue(_DummyProperty);
		}

		private class DummyDependencyObject : DependencyObject{
			public DummyDependencyObject(){}
		}
	}
}
