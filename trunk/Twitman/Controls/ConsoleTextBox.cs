using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatWalk;
using CatWalk.Text;

namespace Twitman.Controls {
	public class ConsoleTextBox : ConsoleLabel{
		public ConsoleTextBox(Int32Point position, Int32Size size) : base(position, size){}
		public ConsoleTextBox(Int32Point position, Int32Size size, ConsoleRun text) : base(position, size, text){}

		#region Prompt

		public string Prompt(){
			return this.Prompt(true, null, null);
		}
		public string Prompt(bool leaveMessage){
			return this.Prompt(leaveMessage, null, null);
		}
		public string Prompt(bool leaveMessage, ConsoleColor? foreground){
			return this.Prompt(leaveMessage, null, null);
		}
		public string Prompt(bool leaveMessage, ConsoleColor? foreground, ConsoleColor? background){
			if(this.Screen == null){
				throw new InvalidOperationException();
			}
			var y = this.DisplayText.Length - 1;
			var x = this.DisplayText[y].Width;
			this.SetCursorPosition(x, y);
			if(foreground != null){
				Console.ForegroundColor = foreground.Value;
			}
			if(background != null){
				Console.BackgroundColor = background.Value;
			}
			var line = Screen.ReadLine();
			if(leaveMessage){
				this.Text += new ConsoleText(line, foreground, background);
			}
			return line;
		}

		public string Prompt(ConsoleRun text){
			return this.Prompt(text, true, null, null);
		}
		public string Prompt(ConsoleRun text, bool leaveMessage){
			return this.Prompt(text, leaveMessage, null, null);
		}
		public string Prompt(ConsoleRun text, bool leaveMessage, ConsoleColor? foreground){
			return this.Prompt(text, leaveMessage, foreground, null);
		}
		public string Prompt(ConsoleRun text, bool leaveMessage, ConsoleColor? foreground, ConsoleColor? background){
			if(this.Screen == null){
				throw new InvalidOperationException();
			}
			this.Text = text;
			return this.Prompt(leaveMessage, foreground, background);
		}

		public string Prompt(ConsoleText text){
			return this.Prompt(text, true, null, null);
		}
		public string Prompt(ConsoleText text, bool leaveMessage){
			return this.Prompt(text, leaveMessage, null, null);
		}
		public string Prompt(ConsoleText text, bool leaveMessage, ConsoleColor? foreground){
			return this.Prompt(text, leaveMessage, foreground, null);
		}
		public string Prompt(ConsoleText text, bool leaveMessage, ConsoleColor? foreground, ConsoleColor? background){
			if(this.Screen == null){
				throw new InvalidOperationException();
			}
			this.Text += text;
			return this.Prompt(leaveMessage, foreground, background);
		}

		#endregion
	}
}
