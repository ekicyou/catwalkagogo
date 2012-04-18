using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatWalk.Mvvm;
using WPF.MDI;
using System.Windows;

namespace GFV.Messaging {
	public class CloseMessage : MessageBase{
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

	public class OpenFileMessage : MessageBase{
		public string File{get; private set;}

		public OpenFileMessage(object sender, string file) : base(sender){
			this.File = file;
		}
	}

	public class RequestActiveMdiChildMessage : MessageBase{
		public object ActiveMdiChild{get; set;}

		public RequestActiveMdiChildMessage(object sender) : base(sender){}
	}

	public class MdiChildClosedMessage : MessageBase{
		public MdiChildClosedMessage(object sender) : base(sender){}
	}

	public class ActiveMdiChildChangedMessage : MessageBase{
		public ActiveMdiChildChangedMessage(object sender) : base(sender){}
	}

	public class ActivateMdiChildMessage : MessageBase{
		public object MdiChild{get; private set;}

		public ActivateMdiChildMessage(object sender, object mdiChild) : base(sender){
			this.MdiChild = mdiChild;
		}
	}

	public class RequestRestoreBoundsMessage : MessageBase{
		public Rect Bounds{get; set;}

		public RequestRestoreBoundsMessage(object sender) : base(sender){}
	}
}
