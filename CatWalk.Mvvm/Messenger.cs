/*
	$Id: Messenger.cs 195 2011-04-12 08:27:58Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using CatWalk;

namespace CatWalk.Mvvm{
	using TEntryKey = Type;
	using TEntryValue = Messenger.Entry;
	using TEntryList = LinkedList<Messenger.Entry>;
	using TDictionary = Dictionary<Type, LinkedList<Messenger.Entry>>;

	public class Messenger {
		private static Messenger _Default;
		public static Messenger Default {
			get {
				return _Default ?? (_Default = new Messenger());
			}
		}

		private TDictionary _DerivedEntries;
		private TDictionary DerivedEntries {
			get {
				return this._DerivedEntries ?? (this._DerivedEntries = new TDictionary());
			}
		}

		private TDictionary _StrictEntries;
		private TDictionary StrictEntries {
			get {
				return this._StrictEntries ?? (this._StrictEntries = new TDictionary());
			}
		}

		private ISynchronizeInvoke _SynchronizeInvoke;
		public ISynchronizeInvoke SynchronizeInvoke {
			get {
				return this._SynchronizeInvoke;
			}
			set {
				value.ThrowIfNull("value");
				this._SynchronizeInvoke = value;
			}
		}

		public Messenger() : this(new SynchronizeViewModel.DefaultSynchronizeInvoke()){

		}

		public Messenger(ISynchronizeInvoke invoke) {
			invoke.ThrowIfNull("invoke");
			this._SynchronizeInvoke = invoke;
		}

		#region Register<TMessage>

		public void Register<TMessage>(Action<TMessage> action) {
			this.Register(action, null, false);
		}
		public void Register<TMessage>(Action<TMessage> action, object token) {
			this.Register(action, token, false);
		}
		public void Register<TMessage>(Action<TMessage> action, bool isReceiveDerivedMessages) {
			this.Register(action, null, isReceiveDerivedMessages);
		}
		public void Register<TMessage>(Action<TMessage> action, object token, bool isReceiveDerivedMessages) {
			action.ThrowIfNull("action");

			var messageType = typeof(TMessage);
#if SILVERLIGHT
			var entry = new TEntryValue(action, token);
#else
			var entry = new TEntryValue(new WeakDelegate(action), token, false);
#endif
			// get list
			var entries = (isReceiveDerivedMessages) ? this.DerivedEntries : this.StrictEntries;
			TEntryList list;
			var key = messageType;
			if(!entries.TryGetValue(key, out list)) {
				list = new TEntryList();
				entries.Add(key, list);
			}

			list.AddLast(entry);
		}

		#endregion

		#region Register<TMessage, TState>

		public void Register<TMessage, TState>(Action<TMessage, TState> action, TState state) {
			this.Register(action, state, null, false);
		}
		public void Register<TMessage, TState>(Action<TMessage, TState> action, TState state, object token) {
			this.Register(action, state, token, false);
		}
		public void Register<TMessage, TState>(Action<TMessage, TState> action, TState state, bool isReceiveDerivedMessages) {
			this.Register(action, state, null, isReceiveDerivedMessages);
		}
		public void Register<TMessage, TState>(Action<TMessage, TState> action, TState state, object token, bool isReceiveDerivedMessages) {
			action.ThrowIfNull("action");

			var messageType = typeof(TMessage);
#if SILVERLIGHT
			var entry = new TEntryValue(action, token, state);
#else
			var entry = new TEntryValue(new WeakDelegate(action), token, false, state);
#endif
			// get list
			var entries = (isReceiveDerivedMessages) ? this.DerivedEntries : this.StrictEntries;
			TEntryList list;
			var key = messageType;
			if(!entries.TryGetValue(key, out list)) {
				list = new TEntryList();
				entries.Add(key, list);
			}

			list.AddLast(entry);
		}
		#endregion

		#region Register<TMessage> PassToken

		public void Register<TMessage>(Action<TMessage, object> action, object token) {
			this.Register(action, token, false);
		}
		public void Register<TMessage>(Action<TMessage, object> action, object token, bool isReceiveDerivedMessages) {
			action.ThrowIfNull("action");

			var messageType = typeof(TMessage);
#if SILVERLIGHT
			var entry = new TEntryValue(action, token);
#else
			var entry = new TEntryValue(new WeakDelegate(action), token, true);
#endif
			// get list
			var entries = (isReceiveDerivedMessages) ? this.DerivedEntries : this.StrictEntries;
			TEntryList list;
			var key = messageType;
			if(!entries.TryGetValue(key, out list)) {
				list = new TEntryList();
				entries.Add(key, list);
			}

			list.AddLast(entry);
		}

		#endregion

		#region Register<TMessage, TState> PassToken

		public void Register<TMessage, TState>(Action<TMessage, object, TState> action, TState state, object token) {
			this.Register(action, state, token, false);
		}
		public void Register<TMessage, TState>(Action<TMessage, object, TState> action, TState state, object token, bool isReceiveDerivedMessages) {
			action.ThrowIfNull("action");

			var messageType = typeof(TMessage);
#if SILVERLIGHT
			var entry = new TEntryValue(action, token, state);
#else
			var entry = new TEntryValue(new WeakDelegate(action), token, true, state);
#endif
			// get list
			var entries = (isReceiveDerivedMessages) ? this.DerivedEntries : this.StrictEntries;
			TEntryList list;
			var key = messageType;
			if(!entries.TryGetValue(key, out list)) {
				list = new TEntryList();
				entries.Add(key, list);
			}

			list.AddLast(entry);
		}
		#endregion

		#region Unregister

		public void Unregister<TMessage>(Action<TMessage> action) {
			this.UnregisterInternal<TMessage>(action, null, false);
		}
		public void Unregister<TMessage>(Action<TMessage> action, object token) {
			this.UnregisterInternal<TMessage>(action, token, false);
		}
		public void Unregister<TMessage>(Action<TMessage> action, bool isReceiveDerivedMessages) {
			this.UnregisterInternal<TMessage>(action, null, isReceiveDerivedMessages);
		}
		public void Unregister<TMessage>(Action<TMessage> action, object token, bool isReceiveDerivedMessages) {
			this.UnregisterInternal<TMessage>(action, token, isReceiveDerivedMessages);
		}

		public void Unregister<TMessage, TState>(Action<TMessage, TState> action) {
			this.UnregisterInternal<TMessage>(action, null, false);
		}
		public void Unregister<TMessage, TState>(Action<TMessage, TState> action, object token) {
			this.UnregisterInternal<TMessage>(action, token, false);
		}
		public void Unregister<TMessage, TState>(Action<TMessage, TState> action, bool isReceiveDerivedMessages) {
			this.UnregisterInternal<TMessage>(action, null, isReceiveDerivedMessages);
		}
		public void Unregister<TMessage, TState>(Action<TMessage, TState> action, object token, bool isReceiveDerivedMessages) {
			this.UnregisterInternal<TMessage>(action, token, isReceiveDerivedMessages);
		}

		public void Unregister<TMessage>(Action<TMessage, object> action) {
			this.UnregisterInternal<TMessage>(action, null, false);
		}
		public void Unregister<TMessage>(Action<TMessage, object> action, object token) {
			this.UnregisterInternal<TMessage>(action, token, false);
		}
		public void Unregister<TMessage>(Action<TMessage, object> action, bool isReceiveDerivedMessages) {
			this.UnregisterInternal<TMessage>(action, null, isReceiveDerivedMessages);
		}
		public void Unregister<TMessage>(Action<TMessage, object> action, object token, bool isReceiveDerivedMessages) {
			this.UnregisterInternal<TMessage>(action, token, isReceiveDerivedMessages);
		}

		public void Unregister<TMessage, TState>(Action<TMessage, object, TState> action) {
			this.UnregisterInternal<TMessage>(action, null, false);
		}
		public void Unregister<TMessage, TState>(Action<TMessage, object, TState> action, object token) {
			this.UnregisterInternal<TMessage>(action, token, false);
		}
		public void Unregister<TMessage, TState>(Action<TMessage, object, TState> action, bool isReceiveDerivedMessages) {
			this.UnregisterInternal<TMessage>(action, null, isReceiveDerivedMessages);
		}
		public void Unregister<TMessage, TState>(Action<TMessage, object, TState> action, object token, bool isReceiveDerivedMessages) {
			this.UnregisterInternal<TMessage>(action, token, isReceiveDerivedMessages);
		}

		private void UnregisterInternal<TMessage>(Delegate action, object token, bool isReceiveDerivedMessages) {
			action.ThrowIfNull("action");

			var messageType = typeof(TMessage);
			var key = messageType;

			var entries = (isReceiveDerivedMessages) ? this._DerivedEntries : this._StrictEntries;

			TEntryList list;
			lock(entries) {
				if(entries != null && entries.TryGetValue(key, out list)) {
					var node = list.First;
					while(node != null) {
						var next = node.Next;
						var entry = node.Value;
#if !SILVERLIGHT
						if(!entry.Action.IsAlive) {
							list.Remove(node);
						} else
#endif
							if(entry.Token == token &&
								entry.Action.Method.Equals(action.Method) &&
								entry.Action.Target == action.Target) {
								list.Remove(node);
							}
						node = next;
					}
					if(list.Count == 0) {
						entries.Remove(key);
					}
				}
			}
		}

		#endregion

		#region Send

		public void Send<TMessage>(TMessage message) {
			this.Send(message, null);
		}

		public void Send<TMessage>(TMessage message, object token) {
			foreach(var list in this.FindEntries(typeof(TMessage))) {
				this.ProcessEntryList(list, token, (entry, d) => {
					if(this._SynchronizeInvoke.InvokeRequired) {
						this._SynchronizeInvoke.Invoke(d, entry.GetParameters(message));
					} else {
						d.DynamicInvoke(entry.GetParameters(message));
					}
				});
			}
		}

		private IEnumerable<TEntryList> FindEntries(Type messageType) {
			// derived
			if(this._DerivedEntries != null) {
				var keysToDelete = new List<TEntryKey>();
				foreach(var pair in this._DerivedEntries
					.Where(pair => messageType.IsSubclassOf(pair.Key))) {
					var list = pair.Value;
					yield return list;
					if(list.Count == 0) {
						keysToDelete.Add(pair.Key);
					}
				}
				foreach(var key in keysToDelete) {
					this._DerivedEntries.Remove(key);
				}
			}

			// strict
			if(this._StrictEntries != null) {
				var key = messageType;
				TEntryList list;
				if(this._StrictEntries.TryGetValue(key, out list)) {
					yield return list;
					if(list.Count == 0) {
						this._StrictEntries.Remove(key);
					}
				}
			}
		}

		private void ProcessEntryList(TEntryList list, object token, Action<Entry, Delegate> callback) {
			var node = list.First;
			while(node != null) {
				var next = node.Next;
				var entry = node.Value;
				var target = entry.Action.Target.Target;
				var d = entry.Action.Delegate;
				if(target != null) {
					if(token == null || entry.Token == null || entry.Token == token) {
						callback(entry, d);
					}
				} else {
					list.Remove(node);
				}
				node = next;
			}
		}

		#endregion

		#region Post
		
		public void Post<TMessage>(TMessage message) {
			this.Post(message, null);
		}

		public void Post<TMessage>(TMessage message, object token) {
			foreach(var list in this.FindEntries(typeof(TMessage))) {
				this.ProcessEntryList(list, token, (entry, d) => {
					this._SynchronizeInvoke.BeginInvoke(d, entry.GetParameters(message));
				});
			}
		}
		
		#endregion

		#region Entry

		internal struct Entry {
#if SILVERLIGHT
			public Delegate Action { get; private set; }
#else
			public WeakDelegate Action { get; private set; }
#endif
			public object Token { get; private set; }
			public object State { get; private set; }
			public bool IsPassToken { get; private set; }
			public bool HasState { get; private set; }

#if SILVERLIGHT
			public Entry(Delegate action, object token)
				: this() {
#else
			public Entry(WeakDelegate action, object token, bool passToken) : this() {
#endif
				this.Action = action;
				this.Token = token;
				this.IsPassToken = passToken;
			}

#if SILVERLIGHT
			public Entry(Delegate action, object token, object state)
				: this(action, token) {
#else
			public Entry(WeakDelegate action, object token, bool passToken, object state)
				: this(action, token, passToken) {
#endif
				this.State = state;
				this.HasState = true;
			}

			public object[] GetParameters(object message) {
				if(this.IsPassToken) {
					if(this.HasState) {
						return new object[] { message, this.Token, this.State };
					} else {
						return new object[] { message, this.Token, null };
					}
				} else {
					if(this.HasState) {
						return new object[] { message, this.State };
					} else {
						return new object[] { message, null };
					}
				}
			}
		}

		#endregion

	}
	
	[Obsolete]
	public abstract class MessageBase {
		public object Sender { get; private set; }
		public MessageBase(object sender) {
			this.Sender = sender;
		}
	}
	
	[Obsolete]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
	public class ReceiveMessageAttribute : Attribute {
		public Type MessageType { get; private set; }

		public ReceiveMessageAttribute(Type messageType) {
			this.MessageType = messageType;
		}
	}

	[Obsolete]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
	public class SendMessageAttribute : Attribute {
		public Type MessageType { get; private set; }

		public SendMessageAttribute(Type messageType) {
			this.MessageType = messageType;
		}
	}
}