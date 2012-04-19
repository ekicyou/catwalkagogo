using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CatWalk.Mvvm;
using GFV.Messaging;

namespace GFV.ViewModel {
	public class ControlViewModel : ViewModelBase{
		public ViewModelBase Parent{ get; private set;}
		public ControlViewModelCollection Children{ get; private set;}

		public ControlViewModel(){
			this.Children = new ControlViewModelCollection(this);
		}

		public class ControlViewModelCollection : Collection<ControlViewModel>{

			public ControlViewModel ViewModel{get; private set;}

			public ControlViewModelCollection(ControlViewModel vm) : this(vm, new List<ControlViewModel>()){}

			public ControlViewModelCollection(ControlViewModel vm, IList<ControlViewModel> collection) : base(collection){
				this.ViewModel = vm;
			}

			protected override void  InsertItem(int index, ControlViewModel item){
				if(item == null){
					throw new ArgumentNullException("item");
				}
				item.Parent = this.ViewModel;
				base.InsertItem(index, item);
			}

			protected override void  RemoveItem(int index){
				var item = this[index];
				item.Parent = null;
				base.RemoveItem(index);
			}

			protected override void  ClearItems(){
				foreach(var item in this){
					item.Parent = null;
				}
				base.ClearItems();
			}

			protected override void  SetItem(int index, ControlViewModel item){
				if(item == null){
					throw new ArgumentNullException("item");
				}
				var old = this[index];
				old.Parent = null;
				item.Parent = this.ViewModel;
				base.SetItem(index, item);
			}
		}
	}
}
