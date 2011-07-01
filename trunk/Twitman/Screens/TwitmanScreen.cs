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
