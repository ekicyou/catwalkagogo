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

		#region Register

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
			var messageType = typeof(TMessage);
#if SILVERLIGHT
			var entry = new TEntryValue(action, token);
#else
			var entry = new TEntryValue(new WeakDelegate(action), token);
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
			this.Unregister(action, null, false);
		}
		public void Unregister<TMessage>(Action<TMessage> action, object token) {
			this.Unregister(action, token, false);
		}
		public void Unregister<TMessage>(Action<TMessage> action, bool isReceiveDerivedMessages) {
			this.Unregister(action, null, isReceiveDerivedMessages);
		}
		public void Unregister<TMessage>(Action<TMessage> action, object token, bool isReceiveDerivedMessages) {
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
						if(!entry.Action.IsAlive){
							list.Remove(node);
						}else
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
				this.ProcessEntryList(list, token, d => {
					if(this._SynchronizeInvoke.InvokeRequired) {
						this._SynchronizeInvoke.Invoke(d, new object[] { message });
					} else {
						d.DynamicInvoke(new object[] { message });
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

		private void ProcessEntryList(TEntryList list, object token, Action<Delegate> callback) {
			var node = list.First;
			while(node != null) {
				var next = node.Next;
				var entry = node.Value;
				var target = entry.Action.Target.Target;
				var d = entry.Action.Delegate;
				if(target != null) {
					if(token == null || entry.Token == null || entry.Token == token) {
						callback(d);
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
				this.ProcessEntryList(list, token, d => {
					this._SynchronizeInvoke.BeginInvoke(d, new object[] { message });
				});
			}
		}

		#endregion

		/*
		private static void CleanUp(TDictionary entries){
			var keysToDelete = new List<TEntryKey>();
			foreach(var pair in entries){
				var list = pair.Value;
				var node = list.First;
				while(node != null){
					var next = node.Next;
					if(!node.Value.IsAlive){
						list.Remove(node);
					}
					node = next;
				}
				if(list.Count == 0){
					keysToDelete.Add(pair.Key);
				}
			}

			foreach(var key in keysToDelete){
				entries.Remove(key);
			}
		}
		*/
		internal struct Entry {
#if SILVERLIGHT
			public Delegate Action { get; private set; }
#else
			public WeakDelegate Action{get; private set;}
#endif
			public object Token { get; private set; }

#if SILVERLIGHT
			public Entry(Delegate action, object token)
				: this() {
#else
			public Entry(WeakDelegate action, object token) : this(){
#endif
				this.Action = action;
				this.Token = token;
			}
		}
	}

	public abstract class MessageBase {
		public object Sender { get; private set; }
		public MessageBase(object sender) {
			this.Sender = sender;
		}
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
	public class ReceiveMessageAttribute : Attribute {
		public Type MessageType { get; private set; }

		public ReceiveMessageAttribute(Type messageType) {
			this.MessageType = messageType;
		}
	}
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
	public class SendMessageAttribute : Attribute {
		public Type MessageType { get; private set; }

		public SendMessageAttribute(Type messageType) {
			this.MessageType = messageType;
		}
	}
}