using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatWalk;
using Twitman.Controls;

namespace Twitman.Screens {
	public class HelpScreen : Screen{
		private ConsoleLabel _Caption = new ConsoleLabel(new Int32Point(0, 0), new Int32Size(Size.Width, 1));
		private ConsoleTextBox _HelpTextBox = new ConsoleTextBox(new Int32Point(0, 1), new Int32Size(Size.Width, Size.Height - 1));

		public HelpScreen(ConsoleRun caption, ConsoleRun helpMessage){
			this._Caption.Text = caption;
			this._HelpTextBox.Text = helpMessage;
			this.Controls.Add(this._Caption);
			this.Controls.Add(this._HelpTextBox);

		}
	}
}
