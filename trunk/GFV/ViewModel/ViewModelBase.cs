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
	public abstract class ViewModelBase : INotifyPropertyChanged{
		protected ViewModelBase(){
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(params string[] names){
			if(this.PropertyChanged == null){
				return;
			}

			CheckPropertyName(names);

			foreach(var name in names){
				this.PropertyChanged(this, new PropertyChangedEventArgs(name));
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
	}
}
