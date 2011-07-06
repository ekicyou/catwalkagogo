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
		
		public string Prompt(){
			if(this.Screen == null){
				throw new InvalidOperationException();
			}
			var y = this.DisplayText.Length - 1;
			var x = this.DisplayText[y].Width;
			this.SetCursorPosition(x, y);
			var line = Screen.ReadLine();
			return line;
		}

		public string Prompt(ConsoleRun text){
			if(this.Screen == null){
				throw new InvalidOperationException();
			}
			this.Text = text;
			return this.Prompt();
		}
	}
}
