using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CatWalk;

namespace Twitman.Controls {
	public class ConsoleMenu : ConsoleControl{
		private Int32Point _Position;
		private Int32Size _Size;
		private int _OffsetX;
		private int _OffsetY;
		private int _FocusedIndex = 0;
		public ConsoleMenuItemCollection Items{get; private set;}
		public SelectedConsoleMenuItemCollection SelectedItems{get; private set;}

		public ConsoleMenu(Int32Point point, Int32Size size){
			this._Position = point;
			this._Size = size;
			this.Items = new ConsoleMenuItemCollection(this);
			this.SelectedItems = new SelectedConsoleMenuItemCollection(this);
		}

		protected override void OnLoad(EventArgs e) {
			this.Redraw();
			base.OnLoad(e);
		}

		protected override void OnUnload(EventArgs e) {
			var x = this._Position.X;
			for(var i = 0; i < this._Size.Height; i++){
				var y = i + this._Position.Y;
				Screen.Write(y, x, new String(' ', this._Size.Width));
			}
			base.OnUnload(e);
		}

		private int GetViewIndex(int collectionIndex){
			return collectionIndex - this._OffsetX;
		}

		private int GetItemIndex(int viewIndex){
			return viewIndex + this._OffsetX;
		}

		protected override void OnKeyPress(ConsoleKeyEventArgs e) {
			switch(e.Key){
				case ConsoleKey.DownArrow: this.LineDown(); break;
				case ConsoleKey.UpArrow: this.LineUp(); break;
			}
			base.OnKeyPress(e);
		}

		#region Collection Event

		internal void OnInsertItem(int index){
			var viewIndex = this.GetViewIndex(index);
			if(viewIndex < this._Size.Height){
				this.RedrawBellow((viewIndex > 0) ? viewIndex : 0);
			}
		}

		internal void OnRemoveItem(int index){
			var viewIndex = this.GetViewIndex(index);
			if(viewIndex < this._Size.Height){
				this.RedrawBellow((viewIndex > 0) ? viewIndex : 0);
			}
		}

		internal void OnClearItem(){
			this.Redraw();
		}

		internal void OnSetItem(int index){
			var viewIndex = this.GetViewIndex(index);
			if(0 <= viewIndex && viewIndex < this._Size.Height){
				this.Draw(viewIndex);
			}
		}

		internal void OnSelectionChanged(int index, bool isSelected){
			var viewIndex = this.GetViewIndex(index);
			if(0 <= viewIndex && viewIndex < this._Size.Height){
				this.Draw(viewIndex);
			}
		}

		internal void OnTextChanged(int index, string text){
			var viewIndex = this.GetViewIndex(index);
			if(0 <= viewIndex && viewIndex < this._Size.Height){
				this.Draw(viewIndex);
			}
		}

		#endregion

		#region Drawing

		public void Redraw(){
			for(var i = 0; i < this._Size.Height; i++){
				this.Draw(i);
			}
		}

		private void RedrawBellow(int viewIndex){
			for(var i = viewIndex; i < this._Size.Height; i++){
				this.Draw(i);
			}
		}

		private void Draw(int viewIndex){
			var index = this.GetItemIndex(viewIndex);
			var y = viewIndex + this._Position.Y;
			var x = this._Position.X;
			if(index < this.Items.Count){
				var item = this.Items[index];
				var mark = (item.IsSelected) ? "*" : (this._FocusedIndex == index) ? "+" : " ";
				var text = mark + this.GetViewText(item.Text);
				Screen.Write(y, x, text.PadRight(this._Size.Width));
			}else{
				Screen.Write(y, x, new String(' ', this._Size.Width));
			}
		}

		private string GetViewText(string text){
			if(this._OffsetX < text.Length){
				var max = this._OffsetX + this._Size.Width - 1;
				if(max < text.Length){
					return text.Substring(this._OffsetX, this._Size.Width - 1);
				}else{
					return text.Substring(this._OffsetX);
				}
			}else{
				return String.Empty;
			}
		}

		#endregion

		#region Scroll

		public void ScrollDown(int line){
			this._OffsetY += line;
			if(this._OffsetY > this.Items.Count - this._Size.Height){
				this._OffsetY = this.Items.Count - this._Size.Height;
			}
			this.Redraw();
		}

		public void ScrollUp(int line){
			if(this._OffsetX > 0){
				this._OffsetY -= line;
				if(this._OffsetY < 0){
					this._OffsetY = 0;
				}
				this.Redraw();
			}
		}

		public int OffsetX{
			get{
				return this._OffsetX;
			}
			set{
				if(value < 0 && this._Size.Width <= value){
					throw new ArgumentOutOfRangeException();
				}
				this._OffsetX = value;
				this.Redraw();
			}
		}

		public int OffsetY{
			get{
				return this._OffsetY;
			}
			set{
				if(value < 0 && this._Size.Height <= value){
					throw new ArgumentOutOfRangeException();
				}
				this._OffsetY = value;
				this.Redraw();
			}
		}

		#endregion

		#region Focus

		public int FocusedIndex{
			get{
				return this._FocusedIndex;
			}
			set{
				if(value < 0 && this.Items.Count <= value){
					throw new ArgumentOutOfRangeException();
				}
				int viewIndex = 0;
				var old = this._FocusedIndex;
				this._FocusedIndex = value;

				if(0 <= old && old < this.Items.Count){
					viewIndex = this.GetViewIndex(old);
					if(0 <= viewIndex && viewIndex < this._Size.Height){
						this.Draw(viewIndex);
					}
				}

				viewIndex = this.GetViewIndex(value);
				if(0 <= viewIndex && viewIndex < this._Size.Height){
					this.Draw(viewIndex);
				}
			}
		}

		public ConsoleMenuItem FocusedItem{
			get{
				if(0 <= this._FocusedIndex && this._FocusedIndex < this.Items.Count){
					return this.Items[this._FocusedIndex];
				}else{
					return null;
				}
			}
		}

		public void LineDown(){
			if(this._FocusedIndex < (this.Items.Count - 1)){
				this.FocusedIndex++;
			}
		}

		public void LineUp(){
			if(0 < this._FocusedIndex){
				this.FocusedIndex--;
			}
		}

		#endregion
	}

	public class ConsoleMenuItemCollection : Collection<ConsoleMenuItem>{
		private ConsoleMenu Menu{get; set;}

		internal ConsoleMenuItemCollection(ConsoleMenu menu){
			this.Menu = menu;
		}

		protected override void InsertItem(int index, ConsoleMenuItem item) {
			item.Index = index;
			var sel = item.IsSelected;
			item.Menu = this.Menu;
			for(var i = index; i < this.Count; i++){
				this[i].Index = i + 1;
			}
			base.InsertItem(index, item);
			this.Menu.OnInsertItem(index);
		}

		protected override void RemoveItem(int index) {
			var item = this[index];
			item.Index = -1;
			item.Menu = null;
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
