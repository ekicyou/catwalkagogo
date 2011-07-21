using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Twitman.IOSystem;
using Twitman.Controls;
using CatWalk;

namespace Twitman.Screens {
	public class TimelineScreen : DirectoryScreen{
		public TimelineSystemDirectory TimelineDirectory{
			get{
				return (TimelineSystemDirectory)this.Directory;
			}
		}

		public TimelineScreen(TimelineSystemDirectory dir) : base(dir, new StatusMenuItemTemplate()){
			this.MessageLabel.Text = new ConsoleRun(this.Directory.DisplayPath, ConsoleColor.Magenta);
			this.PromptBox.Text = new ConsoleRun("Ready!", ConsoleColor.Green);
			this.Menu.FocusedIndexChanged += new FocusedIndexChangedEventHandler(Menu_FocusedIndexChanged);
		}

		void Menu_FocusedIndexChanged(object sender, FocusedIndexChangedEventArgs e) {
			if(this.Menu.Items.Count >= 2 && e.Index == (this.Menu.Items.Count - 1)){
				this.PromptBox.Text = new ConsoleRun("Obtaining statuses...", ConsoleColor.Cyan);
				//this.Menu.ItemsSource = null;
				this.TimelineDirectory.GetOlder(10);
				//this.Menu.ItemsSource = this.Directory.Children;
				this.PromptBox.Text = new ConsoleRun("Ready!", ConsoleColor.Green);
			}
		}

		public class StatusMenuItemTemplate : ConsoleMenuItemTemplate{

			public override ConsoleRun[] GetDisplayText(ConsoleMenuItem item, CatWalk.Int32Size size) {
				var status = ((StatusSystemEntry)item.Value).Status;
				var statusText = new ConsoleRun(status.Text).WordWrap(size.Width);
				return Seq.Make(new ConsoleRun(new []{
					new ConsoleText(status.User.ScreenName, ConsoleColor.Green),
					new ConsoleText(" " + status.CreatedAt, ConsoleColor.Cyan),
				})).Concat(statusText).ToArray();
			}
		}
	}
}
