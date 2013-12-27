using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.Collections {
	public class WeakDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TKey : class {
		private readonly IDictionary<WeakReference, TValue> data = new Dictionary<WeakReference, TValue>();

		private IEnumerable<KeyValuePair<TKey, TValue>> GetItems() {
			var garbageCollectedItems = from item in data where item.Key.IsAlive == false select item;

			foreach(var item in garbageCollectedItems.ToList()) data.Remove(item);

			return (from item in data select new KeyValuePair<TKey, TValue>(item.Key.Target as TKey, item.Value));
		}

		private KeyValuePair<TKey, TValue> GetItems(TKey key) {
			return (from item in GetItems() where ReferenceEquals(item.Key, key) select item).SingleOrDefault();
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
			return GetItems().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetItems().GetEnumerator();
		}

		public void Add(KeyValuePair<TKey, TValue> item) {
			Add(item.Key, item.Value);
		}

		public void Clear() {
			data.Clear();
		}

		public bool Contains(KeyValuePair<TKey, TValue> item) {
			return GetItems().Contains(item);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
			throw new NotImplementedException();
		}

		public bool Remove(KeyValuePair<TKey, TValue> item) {
			throw new NotImplementedException();
		}

		public int Count {
			get { return GetItems().Count(); }
		}

		public bool IsReadOnly {
			get { return false; }
		}

		public bool ContainsKey(TKey key) {
			TValue value;
			return TryGetValue(key, out value);
		}

		public void Add(TKey key, TValue value) {
			var reference = new WeakReference(key);
			data.Add(reference, value);
		}

		public bool Remove(TKey key) {
			throw new NotImplementedException();
		}

		public bool TryGetValue(TKey key, out TValue value) {
			value = default(TValue);
			var item = GetItems(key);

			if(Equals(item, default(KeyValuePair<TKey, TValue>))) return false;

			value = item.Value;
			return true;
		}

		public TValue this[TKey key] {
			get { return GetItems(key).Value; }
			set { throw new NotImplementedException(); }
		}

		public ICollection<TKey> Keys {
			get { return (from item in GetItems() select item.Key).ToList(); }
		}

		public ICollection<TValue> Values {
			get { return (from item in GetItems() select item.Value).ToList(); }
		}
	}
}
