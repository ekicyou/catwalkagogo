/*
	$Id: ViewModelBase.cs 190 2011-03-30 10:44:37Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace Nyoroge{
	public abstract class ViewModelBase : INotifyPropertyChanged{
		protected ViewModelBase(){
		}

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(params string[] names){
			CheckPropertyName(names);
			foreach(var name in names){
				this.OnPropertyChanged(new PropertyChangedEventArgs(name));
			}
		}

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e){
			var eh = this.PropertyChanged;
			if(eh != null){
				eh(this, e);
			}
		}

		[Conditional("DEBUG")]
		private void CheckPropertyName(params string[] names){
			var props = GetType().GetProperties();
			foreach(var name in names){
				var prop = props.Where(p => p.Name == name).SingleOrDefault();
				if(prop == null){
					throw new ArgumentException(name);
				}
			}
		}

		#endregion
	}

	public abstract class ViewModelDataErrorInfoBase : ViewModelBase, IDataErrorInfo{
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

		public string Error{
			get{
				return String.Join(
					Environment.NewLine,
					this._Errors.Where(err => !String.IsNullOrEmpty(err.Value)).Select(err => err.Value));
			}
		}

		public string this[string columnName] {
			get{
				string error;
				if(this.Errors.TryGetValue(columnName, out error)){
					return error;
				}else{
					return null;
				}
			}
		}

		protected void SetError(string propertyName, string error) {
			this.Errors[propertyName] = error;
		}

		protected void ClearError(string propertyName) {
			if(!this.Errors.ContainsKey(propertyName)) {
				return;
			}
			this.Errors.Remove(propertyName);
		}

		protected string GetError(string propertyName) {
			string error = null;
			this.Errors.TryGetValue(propertyName, out error);
			return error;
		}

		protected string[] GetErrorPropertyNames() {
			return this.Errors.Keys.ToArray();
		}

		public bool HasError{
			get{
				return this.Errors.Count != 0;
			}
		}

		#endregion
	}
}
