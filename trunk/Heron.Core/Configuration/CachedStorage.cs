using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using CatWalk;

namespace CatWalk.Heron.Configuration {
	public class CachedStorage : Storage{
		private OrderedDictionary _Cache = new OrderedDictionary();
		private int _CacheSize;
		private IStorage _Storage;

		public void Save() {
			this.ClearCache();
		}

		public int CacheSize {
			get {
				return this._CacheSize;
			}
			set {
				value.ThrowIfOutOfRange(1, "value");
				this.TrimCache();
			}
		}

		public CachedStorage(int cacheSize, IStorage storage) {
			cacheSize.ThrowIfOutOfRange(0, "cacheSize");
			storage.ThrowIfNull("storage");
			this._CacheSize = cacheSize;
			this._Storage = storage;
		}

		private void TrimCache() {
			var count = this._Cache.Count - this._CacheSize;
			if(count > 0) {
				var keys = this._Cache.Keys.Cast<string>().Take(count).ToArray();
				foreach(var key in keys) {
					var v = this._Cache[key];
					this._Storage[key] = v;
					this._Cache.Remove(key);
				}
			}
		}

		protected void ClearCache() {
			var keys = this._Cache.Keys.Cast<string>().ToArray();
			foreach(var key in keys) {
				var v = this._Cache[key];
				this._Storage[key] =  v;
			}
			this._Cache.Clear();
		}

		protected void PreloadCache() {
			foreach(var pair in this.GetItems(this._CacheSize)) {
				this._Cache.Add(pair.Key, pair.Value);
			}
		}

		protected virtual IEnumerable<KeyValuePair<string, object>> GetItems(int count) {
			return this.Take(count);
		}

		protected override void AddItem(string key, object value) {
			this._Cache.Add(key, value);
			this.TrimCache();
		}

		protected override bool TryGetItem(string key, out object value) {
			if(this._Cache.Contains(key)) {
				value = this._Cache[key];
				return true;
			} else {
				if(this._Storage.TryGetValue(key, out value)) {
					this._Cache[key] = value;
					this.TrimCache();
					return true;
				} else {
					return false;
				}
			}
		}

		protected override bool RemoveItem(string key) {
			this._Cache.Remove(key);
			return this._Storage.Remove(key);
		}

		protected override void SetItem(string key, object value) {
			this._Cache[key] = value;
			this.TrimCache();
		}

		protected override object GetItem(string key) {
			if(this._Cache.Contains(key)) {
				return this._Cache[key];
			} else {
				var v = this._Storage[key];
				this._Cache[key] = v;
				this.TrimCache();
				return v;
			}
		}

		protected override void ClearItems() {
			this._Cache.Clear();
			this._Storage.Clear();
		}

		protected override ICollection<string> GetKeys() {
			return this._Storage.Keys;
		}

		protected override ICollection<object> GetValues() {
			return this._Storage.Values;
		}

		protected override int GetCount() {
			return this._Storage.Count;
		}

		protected override bool ContainsItem(string key) {
			if(this._Cache.Contains(key)) {
				return true;
			} else {
				return this._Storage.ContainsKey(key);
			}
		}
	}
}
