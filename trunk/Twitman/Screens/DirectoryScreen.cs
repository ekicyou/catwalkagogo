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

		public DirectoryScreen(ISystemDirectory directory) : this(directory,new SystemEntryMenuItemTemplate()){}
		public DirectoryScreen(ISystemDirectory directory, ConsoleMenuItemTemplate itemTemplate){
			this.Directory = directory;
			this.MessageLabel = new ConsoleLabel(new Int32Point(0, 0), new Int32Size(Screen.Size.Width, 1));
			this.Menu = new ConsoleMenu(new Int32Point(0, 1), new Int32Size(Screen.Size.Width, Screen.Size.Height - 2));
			this.PromptBox = new ConsoleTextBox(new Int32Point(0, Screen.Size.Height - 1), new Int32Size(Screen.Size.Width, 1));
			this.Controls.Add(this.MessageLabel);
			this.Controls.Add(this.Menu);
			this.Controls.Add(this.PromptBox);

			this.Menu.ItemTemplate = itemTemplate;
			this.Menu.ItemsSource = directory.Children;
			this.Menu.Focus();
		}

		protected override void OnKeyPress(ConsoleKeyEventArgs e) {
			if(!e.IsHandled){
				if(e.Modifiers == 0){
					switch(e.Key){
						case ConsoleKey.RightArrow:{
							if(this.Menu.FocusedItem != null){
								this.OpenMenuItem(this.Menu.FocusedItem);
							}
							e.IsHandled = true;
							break;
						}
					}
				}
			}
			base.OnKeyPress(e);
		}

		protected virtual void OpenMenuItem(ConsoleMenuItem item){
			var dir = item.Value as ISystemDirectory;
			if(dir != null){
				this.OpenDirectory(dir);
			}
		}

		protected virtual void OpenDirectory(ISystemDirectory dir){
			ConsoleApplication.SetScreen(new DirectoryScreen(dir), true);
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
