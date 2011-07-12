using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CatWalk;

namespace Twitman.Controls {
	public static class ConsoleApplication {
		private static bool _Shutdown;
		private static List<Screen> _ScreenHistory = new List<Screen>();
		private static Screen _Screen;

		static ConsoleApplication(){
		}

		public static void Start(Screen screen){
			_Screen = screen;
			screen.Attach();
			MessageLoop();
			Exit();
		}

		public static void Start(){
			MessageLoop();
			Exit();
		}

		private static void MessageLoop(){
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
			if(_Screen != null){
				_Screen.Detach();
				_Screen = null;
			}
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
				_ScreenHistory.Add(_Screen);
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
			var new1 = _ScreenHistory[_ScreenHistory.Count - 1];
			_ScreenHistory.RemoveAt(_ScreenHistory.Count - 1);
			_Screen = new1;
			new1.Attach();
		}

		private static ReadOnlyCollection<Screen> _ScreenHistoryReadOnly;
		public static ReadOnlyCollection<Screen> ScreenHistory{
			get{
				if(_ScreenHistoryReadOnly == null){
					_ScreenHistoryReadOnly = new ReadOnlyCollection<Screen>(_ScreenHistory);
				}
				return _ScreenHistoryReadOnly;
			}
		}

		#endregion
	}

	public delegate void ConsoleKeyEventHandler(object sender, ConsoleKeyEventArgs e);
	public class ConsoleKeyEventArgs : EventArgs{
		public ConsoleKey Key{get; private set;}
		public char KeyChar{get; private set;}
		public ConsoleModifiers Modifiers{get; private set;}
		public bool IsHandled{get; set;}

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
