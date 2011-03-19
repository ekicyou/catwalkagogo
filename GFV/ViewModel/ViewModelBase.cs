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

		private FrameworkElement _View;
		public FrameworkElement View{
			get{
				return this._View;
			}
			set{
				var old = this._View;
				this._View = value;
				this.OnViewChanged(new ViewChangedEventArgs(old, this._View));
				this.OnPropertyChanged("View");
			}
		}

		public event ViewChangedEventHandler ViewChanged;

		protected virtual void OnViewChanged(ViewChangedEventArgs e){
			if(this.ViewChanged != null){
				this.ViewChanged(this, e);
			}
		}
	}

	public delegate void ViewChangedEventHandler(object sender, ViewChangedEventArgs e);

	public class ViewChangedEventArgs : EventArgs{
		public FrameworkElement OldView{get; private set;}
		public FrameworkElement NewView{get; private set;}

		public ViewChangedEventArgs(FrameworkElement oldView, FrameworkElement newView){
			this.OldView = oldView;
			this.NewView = newView;
	}
	}
}
