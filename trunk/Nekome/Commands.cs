/*
	$Id$
*/
using System;
using System.Windows.Input;

namespace Nekome{
	public static class NekomeCommands{
		public static readonly RoutedUICommand Search = new RoutedUICommand();
		public static readonly RoutedUICommand Abort = new RoutedUICommand();
		public static readonly RoutedUICommand CloseTab = new RoutedUICommand();
		public static readonly RoutedUICommand NextTab = new RoutedUICommand();
		public static readonly RoutedUICommand PreviousTab = new RoutedUICommand();
		public static readonly RoutedUICommand EditFindTools = new RoutedUICommand();
		public static readonly RoutedUICommand EditGrepTools = new RoutedUICommand();
		public static readonly RoutedUICommand ExecuteExternalTool = new RoutedUICommand();
		public static readonly RoutedUICommand DeleteFile = new RoutedUICommand();
	}
}