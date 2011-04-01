/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GFV {
	public struct ViewViewModelPair<TView, TViewModel>{
		public TView View{get; private set;}
		public TViewModel ViewModel{get; private set;}

		public ViewViewModelPair(TView view, TViewModel viewModel) : this(){
			if(view == null){
				throw new ArgumentNullException("view");
			}
			if(viewModel == null){
				throw new ArgumentNullException("viewModel");
			}
			this.View = view;
			this.ViewModel = viewModel;
		}

		public override bool Equals(object obj){
			if(!(obj is ViewViewModelPair<TView, TViewModel>)){
				return false;
			}
			var other = (ViewViewModelPair<TView, TViewModel>)obj;
			return this.View.Equals(other.View) && this.ViewModel.Equals(other.ViewModel);
		}

		public static bool operator ==(ViewViewModelPair<TView, TViewModel> a, ViewViewModelPair<TView, TViewModel> b){
			return a.Equals(b);
		}

		public static bool operator !=(ViewViewModelPair<TView, TViewModel> a, ViewViewModelPair<TView, TViewModel> b){
			return !a.Equals(b);
		}

		public override int GetHashCode() {
			return this.View.GetHashCode() ^ this.ViewModel.GetHashCode();
		}
	}
}
