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
		}

		public class StatusMenuItemTemplate : ConsoleMenuItemTemplate{

			public override ConsoleRun[] GetDisplayText(ConsoleMenuItem item, CatWalk.Int32Size size) {
				var status = ((StatusSystemEntry)item.Value).Status;
				var statusText = new ConsoleRun(status.Text).WordWrap(size.Width);
				return Seq.Make(new ConsoleRun(status.User.ScreenName, ConsoleColor.Green)).Concat(statusText).ToArray();
			}
		}
	}
}
