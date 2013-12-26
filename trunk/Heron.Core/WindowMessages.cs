using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CatWalk.Mvvm;

namespace CatWalk.Heron {
	public static class WindowMessages {
		public static readonly DependencyProperty PROP = DependencyProperty.RegisterAttached("TEST", typeof(Nullable<bool>), typeof(Window), new PropertyMetadata((d, e) => {

		}));

		public abstract class RestoreBoundsMessage : MessageBase {
			public Rect Bounds { get; set; }

			public RestoreBoundsMessage(object sender) : base(sender) { }
		}

		public class RequestRestoreBoundsMessage : RestoreBoundsMessage {
			public RequestRestoreBoundsMessage(object sender) : base(sender) { }
		}

		public class SetRestoreBoundsMessage : RestoreBoundsMessage {

			public SetRestoreBoundsMessage(object sender, Rect rect)
				: base(sender) {
				this.Bounds = rect;
			}
		}

		public class CloseMessage : MessageBase {
			public CloseMessage(object sender) : base(sender){}
		}

		public class MessageBoxMessage : MessageBase {
			public MessageBoxResult Result { get; set; }
			public string Title { get; set; }
			public string Message { get; set; }
			public MessageBoxImage Image { get; set; }
			public MessageBoxButton Button{get;set;}
			public MessageBoxResult Default { get; set; }
			public MessageBoxOptions Options { get; set; }

			public MessageBoxMessage(object sender) : base(sender){
			}
		}

		public abstract class DialogResultMessage : MessageBase {
			public Nullable<bool> DialogResult { get; set; }

			public DialogResultMessage(object sender) : base(sender) {
			}
		}

		public class RequestDialogResultMessage : DialogResultMessage {
			public RequestDialogResultMessage(object sender)
				: base(sender) {
			}
		}

		public class SetDialogResultMessage : DialogResultMessage {
			public SetDialogResultMessage(object sender, Nullable<bool> result)
				: base(sender) {
					this.DialogResult = result;
			}
		}
	}
}
