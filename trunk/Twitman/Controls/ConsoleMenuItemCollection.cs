using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CatWalk.Collections;

namespace Twitman.Controls {
	public class ConsoleMenuItemCollection : Collection<ConsoleMenuItem>{
		private ConsoleMenu Menu{get; set;}

		internal ConsoleMenuItemCollection(ConsoleMenu menu) : base(new SkipList<ConsoleMenuItem>()){
			if(menu == null){
				throw new ArgumentNullException("menu");
			}
			this.Menu = menu;
		}

		protected override void InsertItem(int index, ConsoleMenuItem item) {
			item.Index = index;
			var sel = item.IsSelected;
			item.Menu = this.Menu;
			for(var i = index; i < this.Count; i++){
				this[i].Index = i + 1;
			}
			item.IsFocused = (item.Menu.FocusedIndex == index);
			base.InsertItem(index, item);
			this.Menu.OnInsertItem(index);
		}

		protected override void RemoveItem(int index) {
			var item = this[index];
			item.Index = -1;
			item.Menu = null;
			item.IsFocused = false;
			base.RemoveItem(index);
			for(var i = index; i < this.Count; i++){
				this[i].Index = i;
			}
			this.Menu.OnRemoveItem(index);
		}

		protected override void ClearItems() {
			foreach(var item in this){
				item.Index = -1;
				item.Menu = null;
				item.IsSelected = false;
				item.IsFocused = false;
			}
			base.ClearItems();
		}

		protected override void SetItem(int index, ConsoleMenuItem item) {
			var old = this[index];
			old.Index = -1;
			old.Menu = null;
			item.Index = index;
			item.Menu = this.Menu;
			base.SetItem(index, item);
		}
	}

	public class SelectedConsoleMenuItemCollection : Collection<ConsoleMenuItem>{
		private ConsoleMenu Menu{get; set;}

		internal SelectedConsoleMenuItemCollection(ConsoleMenu menu){
			this.Menu = menu;
		}

		protected override void InsertItem(int index, ConsoleMenuItem item) {
			item.IsSelected = true;
			base.InsertItem(index, item);
		}

		protected override void RemoveItem(int index) {
			var item = this[index];
			item.IsSelected = false;
			base.RemoveItem(index);
		}

		protected override void ClearItems() {
			foreach(var item in this){
				item.IsSelected = false;
			}
			base.ClearItems();
		}

		protected override void SetItem(int index, ConsoleMenuItem item) {
			var old = this[index];
			if(old != item){
				old.IsSelected = false;
				item.IsSelected = true;
			}
			base.SetItem(index, item);
		}
	}
}
