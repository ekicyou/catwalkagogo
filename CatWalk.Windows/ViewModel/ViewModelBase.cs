﻿/*
	$Id: ViewModelBase.cs 190 2011-03-30 10:44:37Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace CatWalk.Windows.ViewModel{
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
}
