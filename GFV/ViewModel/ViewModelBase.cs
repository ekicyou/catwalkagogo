/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace GFV.ViewModel{
	public abstract class ViewModelBase : INotifyPropertyChanged, INotifyPropertyChanging{
		protected ViewModelBase(){
		}

		#region INotifyPropertyChanging

		public event PropertyChangingEventHandler PropertyChanging;

		protected void OnPropertyChanging(params string[] names){
			CheckPropertyName(names);
			foreach(var name in names){
				this.OnPropertyChanging(new PropertyChangingEventArgs(name));
			}
		}

		protected virtual void OnPropertyChanging(PropertyChangingEventArgs e){
			var eh = this.PropertyChanging;
			if(eh != null){
				eh(this, e);
			}
		}

		#endregion

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
				return this.Errors[columnName];
			}
		}

		#endregion
	}
}
