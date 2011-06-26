using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CatWalk;

namespace Twitman.Controls {
	public class Screen {
		#region static

		public static Int32Size Size{get; private set;}
		static Screen(){
			Console.WindowTop = Console.WindowLeft = 0;
			Size = new Int32Size(Console.WindowWidth, Console.WindowHeight - 1);
			Console.Clear();
		}

		public static void Write(int line, int column, string text){
			var length = Size.Width - column;
			var strLength = (length < text.Length) ? length : text.Length;
			Console.SetCursorPosition(column, line);
			Console.Write(text.Substring(0, strLength));
		}

		public static void WriteLine(int line, string text){
			var minLength = (Size.Width < text.Length) ? Size.Width : text.Length;
			Console.SetCursorPosition(0, line);
			Console.WriteLine(text.Substring(0, minLength).PadRight(Size.Width));
		}

		#endregion

		public ConsoleControlCollection Controls{get; private set;}
		public ConsoleControl FocusedControl{get; private set;}

		public Screen(){
			this.Controls = new ConsoleControlCollection(this);
		}

		internal void Attach(){
			ConsoleApplication.KeyPressed += this.KeyPressHandler;
		}

		internal void Detach(){
			ConsoleApplication.KeyPressed -= this.KeyPressHandler;
		}

		private void KeyPressHandler(object sender, ConsoleKeyEventArgs e){
			if(this.FocusedControl != null){
				this.FocusedControl.FireKeyPress(e);
			}
		}

		internal void OnFocusedControlChanged(ConsoleControl control, bool isFocused){
			if(isFocused){
				foreach(var ctrl in this.Controls){
					ctrl._IsFocused = false;
				}
				this.FocusedControl = control;
			}
		}
	}
}
