using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Twitman.Controls;
using CatWalk;
using CatWalk.IOSystem;

namespace Twitman.Screens {
	public class DirectoryScreen : TwitmanScreen{
		public ConsoleLabel MessageLabel{get; private set;}
		public ConsoleMenu Menu{get; private set;}
		public ConsoleTextBox PromptBox{get; private set;}
		public ISystemDirectory Directory{get; private set;}

		public DirectoryScreen(ISystemDirectory directory){
			this.Directory = directory;
			this.MessageLabel = new ConsoleLabel(new Int32Point(0, 0), new Int32Size(Screen.Size.Width, 1));
			this.Menu = new ConsoleMenu(new Int32Point(0, 1), new Int32Size(Screen.Size.Width, Screen.Size.Height - 2));
			this.PromptBox = new ConsoleTextBox(new Int32Point(0, Screen.Size.Height - 1), new Int32Size(Screen.Size.Width, 1));
			this.Controls.Add(this.MessageLabel);
			this.Controls.Add(this.Menu);
			this.Controls.Add(this.PromptBox);

			this.Menu.ItemTemplate = new SystemEntryMenuItemTemplate();
			this.Menu.ItemsSource = directory.Children;
			this.Menu.Focus();
		}
	}

	public class SystemEntryMenuItemTemplate : ConsoleMenuItemTemplate{
		public override ConsoleMenuItem GetMenuItem(object value) {
			var entry = value as ISystemEntry;
			return new ConsoleMenuItem(entry.DisplayName, value);
		}

		public override ConsoleRun[] GetDisplayText(ConsoleMenuItem item, Int32Size size) {
			return base.GetDisplayText(item, size);
		}
	}
}
