/*
	$Id$
*/

using System;
using System.Windows;
using System.Windows.Input;

namespace CatWalk.Windows{
	public static class DialogCommands{
		public static readonly RoutedUICommand OK = new RoutedUICommand();
		public static readonly RoutedUICommand Cancel = new RoutedUICommand();
		public static readonly RoutedUICommand Apply = new RoutedUICommand();
	}
}