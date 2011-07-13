using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CatWalk.Windows.ViewModel {
	public abstract class DataErrorInfoViewModelBase : ViewModelBase, IDataErrorInfo{
		#region IDataErrorInfo

		private Dictionary<string, string> _Errors;
		protected Dictionary<string, string> Errors{
			get{
				if(this._Errors == null){
					this._Errors = new Dictionary<string,string>();
				}
				return this._Errors;
			}
		}

		public void SetError(string propertyName, string message){
			this.Errors[propertyName] = message;
		}

		public void RemoveError(string propertyName){
			this.Errors.Remove(propertyName);
		}

		public void ClearErrors(){
			this.Errors.Clear();
		}

		public string Error{
			get{
				return String.Join(
					Environment.NewLine,
					this._Errors.Where(err => !String.IsNullOrEmpty(err.Value)).Select(err => err.Value));
			}
		}

		public string this[string columnName] {
			get{
				return this.Errors[columnName];
			}
		}

		#endregion
	}
}
