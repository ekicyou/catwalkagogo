using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CatWalk;
using CatWalk.Collections;
using CatWalk.Text;

namespace Twitman.Controls {
	public class ConsoleMenu : ConsoleControl{
		private int _OffsetX;
		private int _OffsetY;
		private int _FocusedIndex = 0;
		public ConsoleMenuItemCollection Items{get; private set;}
		public SelectedConsoleMenuItemCollection SelectedItems{get; private set;}

		public ConsoleMenu(Int32Point point, Int32Size size) : base(point, size){
			this.Items = new ConsoleMenuItemCollection(this);
			this.SelectedItems = new SelectedConsoleMenuItemCollection(this);
		}

		#region Function

		private int GetViewIndex(int collectionIndex){
			return collectionIndex - this._OffsetY;
		}

		private int GetItemIndex(int viewIndex){
			return viewIndex + this._OffsetY;
		}

		#endregion

		#region EventHandler

		protected override void OnLoad(EventArgs e) {
			this.Redraw();
			base.OnLoad(e);
		}

		protected override void OnUnload(EventArgs e) {
			var x = this.Position.X;
			for(var i = 0; i < this.Size.Height; i++){
				var y = i + this.Position.Y;
				if(this.Screen != null){
					this.Screen.Write(y, x, new String(' ', this.Size.Width));
				}
			}
			base.OnUnload(e);
		}

		protected override void OnKeyPress(ConsoleKeyEventArgs e) {
			switch(e.Key){
				case ConsoleKey.DownArrow: this.LineDown(); break;
				case ConsoleKey.UpArrow: this.LineUp(); break;
				case ConsoleKey.LeftArrow: this.ScrollLeft(1); break;
				case ConsoleKey.RightArrow: this.ScrollRight(1); break;
				case ConsoleKey.Spacebar: this.ToggleSelect(); this.LineDown(); break;
			}
			base.OnKeyPress(e);
		}

		#endregion

		#region Collection Event

		internal void OnInsertItem(int index){
			var viewIndex = this.GetViewIndex(index);
			if(viewIndex < this.Size.Height){
				this.RedrawBellow((viewIndex > 0) ? viewIndex : 0);
			}
		}

		internal void OnRemoveItem(int index){
			var viewIndex = this.GetViewIndex(index);
			if(viewIndex < this.Size.Height){
				this.RedrawBellow((viewIndex > 0) ? viewIndex : 0);
			}
		}

		internal void OnClearItem(){
			this.Redraw();
		}

		internal void OnSetItem(int index){
			var viewIndex = this.GetViewIndex(index);
			if(0 <= viewIndex && viewIndex < this.Size.Height){
				this.Draw(viewIndex);
			}
		}

		internal void OnSelectionChanged(int index, bool isSelected){
			var viewIndex = this.GetViewIndex(index);
			if(0 <= viewIndex && viewIndex < this.Size.Height){
				this.Draw(viewIndex);
			}
		}

		internal void OnTextChanged(int index){
			var viewIndex = this.GetViewIndex(index);
			if(0 <= viewIndex && viewIndex < this.Size.Height){
				this.Draw(viewIndex);
			}
		}

		#endregion

		#region Drawing

		public void Redraw(){
			for(var i = 0; i < this.Size.Height; i++){
				this.Draw(i);
			}
		}

		private void RedrawBellow(int viewIndex){
			for(var i = viewIndex; i < this.Size.Height; i++){
				this.Draw(i);
			}
		}

		private void Draw(int viewIndex){
			var index = this.GetItemIndex(viewIndex);
			var y = viewIndex;
			var x = 0;
			if(index < this.Items.Count){
				var item = this.Items[index];
				var mark =
					(item.IsSelected) ? (this._FocusedIndex == index) ? "*" : "+" :
					(this._FocusedIndex == index) ? "-" : " ";
				var text = this.GetViewText(item);
				if(this.Screen != null){
					this.Write(y, x, text);
				}
			}else{
				if(this.Screen != null){
					this.Write(y, x, new String(' ', this.Size.Width));
				}
			}
		}

		private ConsoleRun GetViewText(ConsoleMenuItem item){
			var trimed = this._ItemTemplate.GetText(item, this._OffsetX, this.Size.Width);
			return trimed;
		}

		#endregion

		#region Scroll

		public void ScrollDown(int line){
			this._OffsetY += line;
			if(this._OffsetY > this.Items.Count - this.Size.Height){
				this._OffsetY = this.Items.Count - this.Size.Height;
			}
			this.Redraw();
		}

		public void ScrollUp(int line){
			if(this._OffsetY > 0){
				this._OffsetY -= line;
				if(this._OffsetY < 0){
					this._OffsetY = 0;
				}
				this.Redraw();
			}
		}

		public void ScrollLeft(int column){
			if(this._OffsetX > 0){
				this._OffsetX -= column;
				if(this._OffsetX < 0){
					this._OffsetX = 0;
				}
				this.Redraw();
			}
		}

		public void ScrollRight(int column){
			this._OffsetX += column;
			this.Redraw();
		}

		public int OffsetX{
			get{
				return this._OffsetX;
			}
			set{
				if(value < 0 && this.Size.Width <= value){
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
				if(value < 0 && this.Size.Height <= value){
					throw new ArgumentOutOfRangeException();
				}
				this._OffsetY = value;
				this.Redraw();
			}
		}

		public void EnsureVisible(int index){
			var viewIndex = this.GetViewIndex(index);
			if(viewIndex < 0){
				this.ScrollUp(-viewIndex);
			}else if(this.Size.Height <= viewIndex){
				this.ScrollDown(viewIndex - this.Size.Height + 1);
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
					if(0 <= viewIndex && viewIndex < this.Size.Height){
						this.Draw(viewIndex);
					}
				}

				viewIndex = this.GetViewIndex(value);
				if(0 <= viewIndex && viewIndex < this.Size.Height){
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
				this.EnsureVisible(this._FocusedIndex);
			}
		}

		public void LineUp(){
			if(0 < this._FocusedIndex){
				this.FocusedIndex--;
				this.EnsureVisible(this._FocusedIndex);
			}
		}

		private void ToggleSelect(){
			var item = this.FocusedItem;
			if(item != null){
				item.IsSelected = !item.IsSelected;
			}
		}

		#endregion

		#region ItemsSource

		private IEnumerable _ItemsSource;
		public IEnumerable ItemsSource{
			get{
				return this._ItemsSource;
			}
			set{
				if(this._ItemsSource != value){
					// Old
					if(this._ItemsSource != null){
						var notifier = this._ItemsSource as INotifyCollectionChanged;
						if(notifier != null){
							notifier.CollectionChanged += this.OnItemsSourceChanged;
						}
					}

					this._ItemsSource = value;

					// New
					if(value != null){
						var notifier = value as INotifyCollectionChanged;
						if(notifier != null){
							notifier.CollectionChanged += this.OnItemsSourceChanged;
						}
					}
					this.RefreshItems();
				}
			}
		}

		private ConsoleMenuItemTemplate _ItemTemplate = new ConsoleMenuItemTemplate();
		public ConsoleMenuItemTemplate ItemTemplate{
			get{
				return this._ItemTemplate;
			}
			set{
				if(value == null){
					throw new ArgumentNullException();
				}
				this._ItemTemplate = value;
				this.RefreshItems();
			}
		}

		private void RefreshItems(){
			this.Items.Clear();
			if(this._ItemsSource != null){
				foreach(var item in this._ItemsSource){
					this.Items.Add(this.GetConsoleMenuItem(item));
				}
			}
		}

		private void OnItemsSourceChanged(object sender, NotifyCollectionChangedEventArgs e){
			switch(e.Action){
				case NotifyCollectionChangedAction.Add:{
					for(var i = 0; i < e.NewItems.Count; i++){
						this.Items.Insert(e.NewStartingIndex + i, this.GetConsoleMenuItem(e.NewItems[i]));
					}
					break;
				}
				case NotifyCollectionChangedAction.Remove:{
					for(var i = 0; i < e.OldItems.Count; i++){
						this.Items.RemoveAt(e.OldStartingIndex);
					}
					break;
				}
				case NotifyCollectionChangedAction.Move:
				case NotifyCollectionChangedAction.Replace:{
					for(var i = 0; i < e.OldItems.Count; i++){
						this.Items.RemoveAt(e.OldStartingIndex);
					}
					for(var i = 0; i < e.NewItems.Count; i++){
						this.Items.Insert(e.NewStartingIndex + i, this.GetConsoleMenuItem(e.NewItems[i]));
					}
					break;
				}
				case NotifyCollectionChangedAction.Reset:{
					this.RefreshItems();
					break;
				}
			}
		}

		private ConsoleMenuItem GetConsoleMenuItem(object item){
			return this._ItemTemplate.GetMenuItem(item);
		}

		#endregion
	}

	public class ConsoleMenuItemCollection : Collection<ConsoleMenuItem>{
		private ConsoleMenu Menu{get; set;}

		internal ConsoleMenuItemCollection(ConsoleMenu menu) : base(new SkipList<ConsoleMenuItem>()){
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
