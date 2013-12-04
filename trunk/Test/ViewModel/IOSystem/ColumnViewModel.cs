using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CatWalk;
using CatWalk.Mvvm;

namespace Test.ViewModel.IOSystem {
	public class ColumnViewModel : AppViewModelBase, IComparable<ColumnViewModel> {
		public SystemEntryViewModel SystemEntryViewModel { get; private set; }
		public ColumnDefinition ColumnProvider { get; private set; }
		private object _Value;

		public ColumnViewModel(ColumnDefinition provider, SystemEntryViewModel vm) {
			provider.ThrowIfNull("provider");
			vm.ThrowIfNull("vm");
			this.ColumnProvider = provider;
			this.SystemEntryViewModel = vm;
		}

		public void Refresh() {
			this.Refresh(CancellationToken.None);
		}

		public void Refresh(CancellationToken token) {
			this.OnPropertyChanging("Value");
			this.GetValue(true, token);
			this.OnPropertyChanged("Value");
		}

		private void GetValue(bool refresh, CancellationToken token) {
			token.ThrowIfCancellationRequested();
			this._Value = this.ColumnProvider.GetValue(this.SystemEntryViewModel.Entry, refresh, token);
			this.IsValueCreated = true;
		}

		public object Value {
			get{
				if(!this.IsValueCreated) {
					Task.Factory.StartNew(delegate {
						this.GetValue(false, this.SystemEntryViewModel.CancellationToken);
					});
				}
				return this._Value;
			}
		}

		public bool IsValueCreated {
			get;
			private set;
		}

		#region IComparable<ColumnViewModel>

		public int CompareTo(object obj) {
			var column = obj as ColumnViewModel;
			if(column != null){
				return this.CompareTo(column);
			}else if(this._Value == null){
				return obj != null ? Int32.MaxValue : 0;
			}else{
				var comparable = (IComparable)this._Value;
				if(comparable != null){
					return comparable.CompareTo(obj != null);
				}else{
					return Int32.MaxValue;
				}
			}
		}

		public int CompareTo(ColumnViewModel other) {
			if(this._Value == null) {
				return other != null ? -1 : 0;
			} else {
				var comparable = (IComparable)this._Value;
				if(comparable != null) {
					return comparable.CompareTo((other != null) ? other._Value : null);
				} else {
					return Int32.MaxValue;
				}
			}
		}


		#endregion
	}
}
