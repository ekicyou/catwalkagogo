using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Twitman.Controls;

namespace Twitman.Screens {
	public abstract class TwitmanScreen : Screen{
		public virtual bool HasHelpScreen{
			get{
				return false;
			}
		}

		protected override void OnKeyPress(ConsoleKeyEventArgs e) {
			base.OnKeyPress(e);
			if(e.Modifiers == 0){
				switch(e.Key){
					case ConsoleKey.LeftArrow:
					case ConsoleKey.Q:{
						if(ConsoleApplication.ScreenHistory.Count > 0){
							ConsoleApplication.RestoreScreen();
							break;
						}
					}
				}
			}
			if(this.HasHelpScreen && e.KeyChar == '?'){
				ConsoleApplication.SetScreen(this.HelpScreen, true);
			}
		}

		public virtual Screen HelpScreen{
			get{
				if(!this.HasHelpScreen){
					throw new NotSupportedException();
				}
				return null;
			}
		}
	}
}
