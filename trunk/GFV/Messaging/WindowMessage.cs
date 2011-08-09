using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatWalk.Mvvm;

namespace GFV.Messaging {
	class WindowMessage {
	}	public class CloseMessage : MessageBase{
		public CloseMessage(object sender) : base(sender){}
	}

	public class AboutMessage : MessageBase{
		public AboutMessage(object sender) : base(sender){}
	}

	public enum ArrangeMode{
		Cascade,
		TileHorizontal,
		TileVertical,
		StackHorizontal,
		StackVertical,
	}

	public class ArrangeWindowsMessage : MessageBase{
		public ArrangeMode Mode{get; private set;}

		public ArrangeWindowsMessage(object sender, ArrangeMode mode) : base(sender){
			this.Mode = mode;
		}
	}

	public class ErrorMessage : MessageBase{
		public string Messsage{get; private set;}
		public Exception Exception{get; private set;}

		public ErrorMessage(object sender, string message, Exception ex) : base(sender){
			this.Messsage = message;
			this.Exception = ex;
		}
	}

	public class ShowSettingsMessage : MessageBase{
		public ShowSettingsMessage(object sender) : base(sender){}
	}
}
