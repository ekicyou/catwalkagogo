using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CatWalk;
using CatWalk.Text;

namespace Twitman.Controls {
	public class Screen {
		#region static

		public static Int32Size Size{get; private set;}
		static Screen(){
			Console.WindowTop = Console.WindowLeft = 0;
			Size = new Int32Size(Console.WindowWidth - 1, Console.WindowHeight - 1);
			Console.Clear();
		}

		private static void WriteInternal(int line, int column, string text){
			Console.SetCursorPosition(column, line);
			Console.Write(text, column);
		}

		private static void WriteLineInternal(int line, string text){
			var minLength = (Size.Width < text.Length) ? Size.Width : text.Length;
			Console.SetCursorPosition(0, line);
			Console.WriteLine(text);
		}

		#endregion

		private bool _IsAttached = false;
		private string[] _Buffer;
		public ConsoleControlCollection Controls{get; private set;}
		public ConsoleControl FocusedControl{get; private set;}

		public Screen(){
			this._Buffer = Enumerable.Range(0, Size.Height).Select(i => new String(' ', Size.Width)).ToArray();
			this.Controls = new ConsoleControlCollection(this);
		}

		internal void Attach(){
			this._IsAttached = true;
			ConsoleApplication.KeyPressed += this.KeyPressHandler;
			Console.SetCursorPosition(0, 0);
			foreach(var line in this._Buffer){
				Console.WriteLine(line);
			}
		}

		internal void Detach(){
			this._IsAttached = false;
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

		public void Write(int line, int column, string text){
			if(line < 0 || Size.Height <= line){
				throw new ArgumentNullException("line");
			}
			if(column < 0 || Size.Width <= column){
				throw new ArgumentNullException("column");
			}
			var old = this._Buffer[line];
			var prefix = old.Substring(0, column);
			int viewLength;
			text = text.GetFittedText(Size.Width, out viewLength);
			var postfix = ((viewLength + column) < Size.Width) ? old.ViewSubstring(viewLength + column) : "";

			this._Buffer[line] = prefix + text + postfix;
			if(this._IsAttached){
				WriteInternal(line, column, text);
			}
		}

		public void WriteLine(int line, string text){
			if(line < 0 || Size.Height <= line){
				throw new ArgumentNullException("line");
			}
			var old = this._Buffer[line];
			var newT = text.GetFittedText(Size.Width);
			this._Buffer[line] = newT;
			if(this._IsAttached){
				WriteInternal(line, 0, text);
			}
		}
	}
}
