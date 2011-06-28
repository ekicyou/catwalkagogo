using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatWalk;
using CatWalk.Text;

namespace Twitman.Controls {
	public class ConsoleTextBox : ConsoleLabel{
		public ConsoleTextBox(Int32Point position, Int32Size size) : base(position, size){}
		public ConsoleTextBox(Int32Point position, Int32Size size, string text) : base(position, size, text){}

		public string Prompt(){
			var x = this.Position.X + this.Text.ViewLength();
			Console.SetCursorPosition(x, this.Position.Y);
			return Console.ReadLine();
		}
	}
}
