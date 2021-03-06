﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using Newtonsoft.Json;
using System.IO;
using CatWalk;

namespace CatWalk.Heron.Configuration {
	public class DBStorage : Storage {
		#region Syntax

		private const string CREATE_TABLE = @"CREATE TABLE IF NOT EXISTS `%1` (key TEXT NOT NULL, value TEXT, PRIMARY KEY (key))";
		private const string INSERT = @"INSERT `%1` `key`,`value` VALUES (?,?)";
		private const string INSERT_OR_REPLACE = @"INSERT OR REPLACE `%1` `key`,`value` VALUES (?,?)";
		private const string SELECT = @"SELECT `value` FROM `%1` WHERE `key`=?";
		private const string SELECT_COUNT = @"SELECT COUNT(*) FROM `%1`";
		private const string SELECT_ALL = @"SELECT `key`,`value` FROM `%1`";
		private const string SELECT_ALL_KEYS = @"SELECT `key` FROM `%1`";
		private const string SELECT_ALL_VALUES = @"SELECT `value` FROM `%1`";
		private const string SELECT_ALL_LIMIT = @"SELECT `key`,`value` FROM `%1` LIMIT ?";
		private const string DELETE = @"DELETE FROM `%1` WHERE `%1`";
		private const string DELETE_ALL = @"DELETE FROM `%1`";

		private readonly string _Insert;
		private readonly string _InsertOrReplace;
		private readonly string _Select;
		private readonly string _SelectCount;
		private readonly string _SelectAll;
		private readonly string _SelectAllKeys;
		private readonly string _SelectAllValues;
		private readonly string _Delete;
		private readonly string _DeleteAll;
		private readonly string _SelectAllLimit;

		#endregion

		private const int MAX_CONNECTION = 4;
		private PooledConnection[] _Connections = new PooledConnection[MAX_CONNECTION];
		private DbProviderFactory _Factory;

		private string _ConnectionString;
		public DBStorage(string providerName, string connectionString, string tableName) : this(DbProviderFactories.GetFactory(providerName), connectionString, tableName){
		}
		public DBStorage(DbProviderFactory factory, string connectionString, string tableName) {
			factory.ThrowIfNull("factory");
			connectionString.ThrowIfNull("connectionString");
			tableName.ThrowIfNull("tableName");
			this._Factory = factory;
			this._ConnectionString = connectionString;

			this.Transaction((conn, tx) => {
				using(var com = conn.CreateCommand()) {
					com.CommandText = String.Format(CREATE_TABLE, tableName);
					com.ExecuteNonQuery();
				}
			});

			this._Insert = String.Format(INSERT, tableName);
			this._InsertOrReplace = String.Format(INSERT_OR_REPLACE, tableName);
			this._Select = String.Format(SELECT, tableName);
			this._SelectCount = String.Format(SELECT_COUNT, tableName);
			this._SelectAll = String.Format(SELECT_ALL, tableName);
			this._SelectAllValues = String.Format(SELECT_ALL_VALUES, tableName);
			this._SelectAllKeys = String.Format(SELECT_ALL_KEYS, tableName);
			this._SelectAllLimit = String.Format(SELECT_ALL_LIMIT, tableName);
			this._Delete = String.Format(DELETE, tableName);
			this._DeleteAll = String.Format(DELETE_ALL, tableName);
		}

		private void Transaction(Action<IDbConnection, IDbTransaction> action) {
			action.ThrowIfNull("action");
			using(var conn = this.GetConnection()) {
				using(var tx = conn.BeginTransaction()) {
					try {
						action(conn, tx);
						tx.Commit();
					} catch {
						tx.Rollback();
						throw;
					}
				}
			}
		}

		#region Connection Pool
		private IDbConnection GetConnection() {
			while(true) {
				PooledConnection conn = null;
				int idx;
				lock(this._Connections) {
					idx = this.FindPool();
					if(idx >= 0) {
						conn = this._Connections[idx];
						if(conn != null) {
							// reuse
							conn.Wait();
							if(conn.State == ConnectionState.Open) {
								return conn;
							} else {
								continue;
							}
						} else {
							// new
							conn = new PooledConnection(this._Factory.CreateConnection());
							try {
								conn.ConnectionString = this._ConnectionString;
								conn.Open();
								this._Connections[idx] = conn;
								conn.Wait();
								return conn;
							} catch {
								conn.Dispose();
							}
						}
					}
				}
				if(idx < 0) {
					// wait
					WaitHandle.WaitAny(this._Connections.Where(c => c != null).Select(c => c.WaitHandle).ToArray());
				}
			}
		}

		private int FindPool() {
			var emptyIdx = -1;
			for(var i = 0; i < MAX_CONNECTION; i++) {
				var conn = this._Connections[i];
				if(conn != null) {
					switch(conn.State) {
						case ConnectionState.Broken:
						case ConnectionState.Closed:
							conn.Destroy();
							conn = null;
							this._Connections[i] = null;
							break;
						case ConnectionState.Open:
							return i;
					}
				}

				if(conn == null) {
					emptyIdx = i;
				}
			}
			return emptyIdx;
		}

		private class PooledConnection : IDbConnection {
			private IDbConnection _Connection;
			private SemaphoreSlim _Semaphore = new SemaphoreSlim(1, 1);

			public PooledConnection(IDbConnection connection) {
				this._Connection = connection;
			}

			public void Destroy() {
				this._Connection.Close();
				this._Connection.Dispose();
				this._Semaphore.Dispose();
				GC.SuppressFinalize(this);
			}

			public void Wait() {
				this._Semaphore.Wait();
			}

			public void Wait(int ms) {
				this._Semaphore.Wait(ms);
			}

			public WaitHandle WaitHandle {
				get {
					return this._Semaphore.AvailableWaitHandle;
				}
			}

			#region IDbConnection Members

			public IDbTransaction BeginTransaction(IsolationLevel il) {
				return this._Connection.BeginTransaction(il);
			}

			public IDbTransaction BeginTransaction() {
				return this._Connection.BeginTransaction();
			}

			public void ChangeDatabase(string databaseName) {
				this._Connection.ChangeDatabase(databaseName);
			}

			public void Close() {
				// Do Noting
			}

			public string ConnectionString {
				get {
					return this._Connection.ConnectionString;
				}
				set {
					this._Connection.ConnectionString = value;
				}
			}

			public int ConnectionTimeout {
				get {
					return this._Connection.ConnectionTimeout;
				}
			}

			public IDbCommand CreateCommand() {
				return this._Connection.CreateCommand();
			}

			public string Database {
				get {
					return this._Connection.Database;
				}
			}

			public void Open() {
				this._Connection.Open();
			}

			public ConnectionState State {
				get {
					return this._Connection.State;
				}
			}

			#endregion

			#region IDisposable Members

			public void Dispose() {
				this._Semaphore.Release();
			}

			~PooledConnection() {
				this.Destroy();
			}

			#endregion
		}
		#endregion

		#region Storage

		private static readonly JsonSerializerSettings JSON_SETTINGS = new JsonSerializerSettings() {
			TypeNameHandling = TypeNameHandling.All,
			Formatting = Formatting.None,
		};

		protected virtual string Serialize(object value) {
			if(value == null) {
				return null;
			} else {
				return JsonConvert.SerializeObject(value, JSON_SETTINGS);
			}
		}

		protected virtual object Deserialize(string json) {
			if(json == null) {
				return null;
			} else {
				return JsonConvert.DeserializeObject(json, JSON_SETTINGS);
			}
		}

		protected override void AddItem(string key, object value) {
			this.Transaction((conn, tx) => {
				using(var com = conn.CreateCommand()) {
					com.CommandText = this._Insert;

					{
						var pKey = com.CreateParameter();
						pKey.Value = key;
						com.Parameters.Add(pKey);
					}
					{
						var pValue = com.CreateParameter();
						pValue.Value = this.Serialize(value);
						com.Parameters.Add(pValue);
					}

					com.ExecuteNonQuery();
				}
			});
		}

		protected override bool TryGetItem(string key, out object value) {
			var found = false;
			string v = null;
			this.Transaction((conn, tx) => {
				using(var com = conn.CreateCommand()) {
					com.CommandText = this._Select;

					var pKey = com.CreateParameter();
					pKey.Value = key;
					com.Parameters.Add(pKey);

					using(var reader = com.ExecuteReader()) {
						found = reader.Read();
						v = reader.GetString(0);
					}
				}
			});
			value = this.Deserialize(v);
			return found;
		}

		protected override ICollection<string> GetKeys() {
			var list = new List<string>();
			this.Transaction((conn, tx) => {
				using(var com = conn.CreateCommand()) {
					com.CommandText = this._SelectAllKeys;
					using(var reader = com.ExecuteReader()) {
						while(reader.Read()) {
							list.Add(reader.GetString(0));
						}
					}
				}
			});
			return list.AsReadOnly();
		}

		protected override bool RemoveItem(string key) {
			var count = 0;
			this.Transaction((conn, tx) => {
				using(var com = conn.CreateCommand()) {
					com.CommandText = this._Delete;

					var pKey = com.CreateParameter();
					pKey.Value = key;
					com.Parameters.Add(pKey);

					count = com.ExecuteNonQuery();
				}
			});
			return count > 0;
		}

		protected override void SetItem(string key, object value) {
			this.Transaction((conn, tx) => {
				using(var com = conn.CreateCommand()) {
					com.CommandText = this._InsertOrReplace;

					{
						var pKey = com.CreateParameter();
						pKey.Value = key;
						com.Parameters.Add(pKey);
					}
					{
						var pValue = com.CreateParameter();
						pValue.Value = this.Serialize(value);
						com.Parameters.Add(pValue);
					}

					com.ExecuteNonQuery();
				}
			});
		}

		protected override object GetItem(string key) {
			string v = null;
			this.Transaction((conn, tx) => {
				using(var com = conn.CreateCommand()) {
					com.CommandText = this._Select;

					var pKey = com.CreateParameter();
					pKey.Value = key;
					com.Parameters.Add(pKey);

					com.Prepare();
					v = (string)com.ExecuteScalar();
				}
			});
			return this.Deserialize(v);
		}

		protected override void ClearItems() {
			this.Transaction((conn, tx) => {
				using(var com = conn.CreateCommand()) {
					com.CommandText = this._DeleteAll;
					com.ExecuteNonQuery();
				}
			});
		}

		protected override int GetCount() {
			var count = 0;
			this.Transaction((conn, tx) => {
				using(var com = conn.CreateCommand()) {
					com.CommandText = this._SelectCount;
					count = (int)com.ExecuteScalar();
				}
			});
			return count;
		}

		public override IEnumerator<KeyValuePair<string, object>> GetEnumerator() {
			using(var conn = this.GetConnection()) {
				conn.Open();
				try {
					using(var com = conn.CreateCommand()) {
						com.CommandText = this._SelectAll;
						using(var reader = com.ExecuteReader()) {
							while(reader.Read()) {
								yield return new KeyValuePair<string, object>(reader.GetString(0), this.Deserialize(reader.GetString(1)));
							}
						}
					}
				} finally {
					conn.Close();
				}
			}
		}

		protected override ICollection<object> GetValues() {
			var list = new List<object>();
			this.Transaction((conn, tx) => {
				using(var com = conn.CreateCommand()) {
					com.CommandText = this._SelectAllValues;
					using(var reader = com.ExecuteReader()) {
						while(reader.Read()) {
							list.Add(this.Deserialize(reader.GetString(0)));
						}
					}
				}
			});
			return list.AsReadOnly();
		}

		#endregion

		protected override void Dispose(bool disposing) {
			foreach(var conn in this._Connections) {
				if(conn != null) {
					conn.Dispose();
				}
			}
			base.Dispose(disposing);
		}
	}
}
