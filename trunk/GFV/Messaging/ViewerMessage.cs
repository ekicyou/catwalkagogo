using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatWalk.Mvvm;
using System.Windows;
using System.Windows.Media.Animation;

namespace GFV.Messaging {
	#region SizeMessage

	public class SizeMessage : MessageBase{
		public Size Size{get; private set;}
		public SizeMessage(object sender, Size size) : base(sender){
			this.Size = size;
		}
	}

	#endregion

	#region Scale Message

	public class ScaleMessage : MessageBase{
		public double Scale{get; private set;}

		public ScaleMessage(object sender, double scale) : base(sender){
			this.Scale = scale;
		}
	}

	#endregion

	#region RequestScaleMessage

	public class RequestScaleMessage : MessageBase{
		public double Scale{get; set;}

		public RequestScaleMessage(object sender) : base(sender){
		}
	}


	#endregion

	#region AnimationMessage

	public class AnimationMessage : MessageBase{
		public bool IsEnabled{get; private set;}
		public Storyboard Storyboard{get; private set;}

		public AnimationMessage(object sender, bool isEnabled, Storyboard storyboard) : base(sender){
			this.IsEnabled = isEnabled;
			this.Storyboard = storyboard;
		}
	}

	#endregion

	#region FrameIndexMessage

	public class FrameIndexMessage : MessageBase{
		public int FrameIndex{get; private set;}

		public FrameIndexMessage(object sender, int frame) : base(sender){
			this.FrameIndex = frame;
		}
	}

	#endregion
}
