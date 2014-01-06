using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CatWalk;

namespace CatWalk.Heron.IOSystem {
	public abstract class CacheColumnDefinition<T> : ColumnDefinition{
		public IColumnValueSource<T> Source { get; private set; }
		public CacheColumnDefinition(IColumnValueSource<T> source) {
			source.ThrowIfNull("source");
			this.Source = source;
		}

		public override object GetValue(CatWalk.IOSystem.ISystemEntry entry, bool noCache, System.Threading.CancellationToken token) {
			if(noCache) {
				this.Source.Reset();
			}
			return this.SelectValue(this.Source.GetValue(token));
		}

		protected virtual object SelectValue(T value) {
			return value;
		}
	}

	public interface IColumnValueSource<T> {
		void Reset();
		T GetValue(CancellationToken token);
	}

	public class ResetLazyColumnValueSource<T> : IColumnValueSource<T> {
		private ResetLazy<T> _Source;

		public ResetLazyColumnValueSource(Func<T> valueFactory) {
			this._Source = new ResetLazy<T>(valueFactory);
		}

		public void Reset() {
			this._Source.Reset();
		}

		public T GetValue(CancellationToken token) {
			return this._Source.Value;
		}
	}
}
