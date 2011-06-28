﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CatWalk;

namespace Twitman.Controls {
	public static class ConsoleApplication {
		private static bool _Shutdown;
		public static ReadOnlyCollection<Screen> _ScreenHistory;
		private static List<Screen> _ScreenHistoryList = new List<Screen>();
		private static Screen _Screen;

		static ConsoleApplication(){
			_ScreenHistory = new ReadOnlyCollection<Screen>(_ScreenHistoryList);
		}

		public static void Start(Screen screen){
			if(_Screen != null){
				_Screen.Detach();
			}
			_Screen = screen;
			screen.Attach();
			Start();
			Exit();
		}

		public static void Start(){
			while(!_Shutdown){
				var info = Console.ReadKey(true);
				var handler = KeyPressed;
				if(handler != null){
					handler(null, new ConsoleKeyEventArgs(info));
				}
			}
		}

		public static void Exit(){
			Exit(0);
		}
		public static void Exit(int code){
			_Shutdown = true;
			var handler = Exited;
			if(handler != null){
				handler(null, new ExitEventArgs(code));
			}
			Environment.Exit(code);
		}

		public static event ConsoleKeyEventHandler KeyPressed;

		public static event ExitEventHandler Exited;

		#region Screen

		public static void SetScreen(Screen screen, bool addHistory){
			if(addHistory && _Screen != null){
				_ScreenHistoryList.Add(_Screen);
			}
			if(_Screen != null){
				_Screen.Detach();
			}
			_Screen = screen;
			screen.Attach();
		}

		public static void RestoreScreen(){
			if(_ScreenHistory.Count == 0){
				throw new InvalidOperationException();
			}
			var old = _Screen;
			old.Detach();
			var new1 = _ScreenHistoryList[_ScreenHistoryList.Count - 1];
			_ScreenHistoryList.RemoveAt(_ScreenHistoryList.Count - 1);
			_Screen = new1;
			new1.Attach();
		}

		#endregion
	}

	public delegate void ConsoleKeyEventHandler(object sender, ConsoleKeyEventArgs e);
	public class ConsoleKeyEventArgs : EventArgs{
		public ConsoleKey Key{get; private set;}
		public char KeyChar{get; private set;}
		public ConsoleModifiers Modifiers{get; private set;}

		public ConsoleKeyEventArgs(ConsoleKeyInfo info){
			this.Key = info.Key;
			this.KeyChar = info.KeyChar;
			this.Modifiers = info.Modifiers;
		}
	}

	public delegate void ExitEventHandler(object sender, ExitEventArgs e);
	public class ExitEventArgs : EventArgs{
		public int ExitCode{get; private set;}
		public ExitEventArgs(int exitCode){
			this.ExitCode = exitCode;
		}
	}
}
