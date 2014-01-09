using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CatWalk.Mvvm;

namespace CatWalk.Heron.Windows {
	public static class MessageUtility {
		public static MessageReceiver<TMessage> RegisterMessageReceiver<TMessage>(this FrameworkElement element, Messenger messenger, Action<TMessage> receiver, bool receiveDeliveredMessage = false) {
			element.ThrowIfNull("element");
			messenger.ThrowIfNull("messenger");
			receiver.ThrowIfNull("receiver");

			return new MessageReceiver<TMessage>(element, messenger, receiver, receiveDeliveredMessage, false);
		}

		public static MessageReceiver<TMessage> RegisterMessageReceiver<TMessage>(this FrameworkElement element, Messenger messenger, Action<TMessage, object> receiver, bool receiveDeliveredMessage = false) {
			element.ThrowIfNull("element");
			messenger.ThrowIfNull("messenger");
			receiver.ThrowIfNull("receiver");

			return new MessageReceiver<TMessage>(element, messenger, receiver, receiveDeliveredMessage, true);
		}

		public static MessageReceiver<TMessage, TState> RegisterMessageReceiver<TMessage, TState>(this FrameworkElement element, Messenger messenger, Action<TMessage, TState> receiver, TState state, bool receiveDeliveredMessage = false) {
			element.ThrowIfNull("element");
			messenger.ThrowIfNull("messenger");
			receiver.ThrowIfNull("receiver");

			return new MessageReceiver<TMessage, TState>(element, messenger, receiver, state, receiveDeliveredMessage, false);
		}

		public static MessageReceiver<TMessage, TState> RegisterMessageReceiver<TMessage, TState>(this FrameworkElement element, Messenger messenger, Action<TMessage, object, TState> receiver, TState state, bool receiveDeliveredMessage = false) {
			element.ThrowIfNull("element");
			messenger.ThrowIfNull("messenger");
			receiver.ThrowIfNull("receiver");

			return new MessageReceiver<TMessage, TState>(element, messenger, receiver, state, receiveDeliveredMessage, true);
		}
		
	}

	public class MessageReceiver<TMessage> : DisposableObject {
		protected FrameworkElement View { get; private set; }
		protected Messenger Messenger { get; private set; }
		protected Delegate Receiver { get; private set; }
		protected bool IsReceiveDeliveredMessage { get; private set; }
		protected bool IsReceiveToken { get; private set; }

		public MessageReceiver(FrameworkElement element, Messenger messenger, Delegate receiver, bool receiveDeliveredMessage, bool receiveToken) {
			element.ThrowIfNull("element");
			messenger.ThrowIfNull("messenger");
			receiver.ThrowIfNull("receiver");

			this.View = element;
			this.Messenger = messenger;
			this.Receiver = receiver;
			this.IsReceiveDeliveredMessage = receiveDeliveredMessage;
			this.IsReceiveToken = receiveToken;

			if(element.DataContext != null) {
				this.Register(element.DataContext);
			}
			element.DataContextChanged += element_DataContextChanged;
			element.Unloaded += element_Unloaded;
		}

		void element_Unloaded(object sender, RoutedEventArgs e) {
			this.Dispose();
		}

		private void element_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			var context = e.OldValue;
			if(context != null) {
				this.Unregister(context);
			}
			context = e.NewValue;
			if(context != null) {
				this.Register(context);
			}

		}

		protected virtual void Register(object context) {
			if(this.IsReceiveToken) {
				this.Messenger.Register<TMessage>((Action<TMessage>)this.Receiver, context, this.IsReceiveDeliveredMessage);
			} else {
				this.Messenger.Register<TMessage>((Action<TMessage, object>)this.Receiver, context, this.IsReceiveDeliveredMessage);
			}
		}

		protected virtual void Unregister(object context) {
			if(this.IsReceiveToken) {
				this.Messenger.Unregister<TMessage>((Action<TMessage>)this.Receiver, context, this.IsReceiveDeliveredMessage);
			} else {
				this.Messenger.Unregister<TMessage>((Action<TMessage, object>)this.Receiver, context, this.IsReceiveDeliveredMessage);
			}
		}


		protected override void Dispose(bool disposing) {
			if(!this.IsDisposed) {
				var context = this.View.DataContext;
				if(context != null) {
					this.Unregister(context);
				}
				this.View.DataContextChanged -= this.element_DataContextChanged;
			}
			base.Dispose(disposing);
		}
	}


	public class MessageReceiver<TMessage, TState> : MessageReceiver<TMessage> {
		public TState State { get; private set; }

		public MessageReceiver(FrameworkElement element, Messenger messenger, Delegate receiver, TState state, bool receiveDeliveredMessage, bool receiveToken)
			: base(element, messenger, receiver, receiveDeliveredMessage, receiveToken){
			this.State = state;
		}

		protected override void Register(object context) {
			if(this.IsReceiveToken) {
				this.Messenger.Register<TMessage, TState>((Action<TMessage, object, TState>)this.Receiver, this.State, context, this.IsReceiveDeliveredMessage);
			} else {
				this.Messenger.Register<TMessage, TState>((Action<TMessage, TState>)this.Receiver, this.State, context, this.IsReceiveDeliveredMessage);
			}
		}

		protected override void Unregister(object context) {
			if(this.IsReceiveToken) {
				this.Messenger.Unregister<TMessage, TState>((Action<TMessage, object, TState>)this.Receiver, context, this.IsReceiveDeliveredMessage);
			} else {
				this.Messenger.Unregister<TMessage, TState>((Action<TMessage, TState>)this.Receiver, context, this.IsReceiveDeliveredMessage);
			}
		}
	}

}
