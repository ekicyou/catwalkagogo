﻿using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CatWalk;
using CatWalk.Collections;
using CatWalk.Text;
using System.Diagnostics;

namespace Twitman.Controls {
	public class ConsoleMenu : ConsoleControl{
		private int _OffsetX;
		private int _OffsetY;
		private int _FocusedIndex = 0;
		public ConsoleMenuItemCollection Items{get; private set;}
		public SelectedConsoleMenuItemCollection SelectedItems{get; private set;}
		public override bool IsFocusable {get {return true;}}

		public ConsoleMenu(Int32Point point, Int32Size size) : base(point, size){
			this.Items = new ConsoleMenuItemCollection(this);
			this.SelectedItems = new SelectedConsoleMenuItemCollection(this);
		}

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
			base.OnKeyPress(e);
			if(!e.IsHandled){
				switch(e.Key){
					case ConsoleKey.DownArrow: this.LineDown(); break;
					case ConsoleKey.UpArrow: this.LineUp(); break;
					case ConsoleKey.LeftArrow: this.ScrollLeft(1); break;
					case ConsoleKey.RightArrow: this.ScrollRight(1); break;
					case ConsoleKey.Spacebar: this.ToggleSelect(); this.LineDown(); break;
				}
			}
		}

		#endregion

		#region Collection Event

		internal void OnInsertItem(int index){
			this.RefreshDisplayText(this.Items[index]);
			this.Redraw();
		}

		internal void OnRemoveItem(int index){
			this.Redraw();
		}

		internal void OnClearItem(){
			this.Redraw();
		}

		internal void OnSetItem(int index){
			this.RefreshDisplayText(this.Items[index]);
			this.Redraw();
		}

		internal void OnSelectionChanged(int index, bool isSelected){
			this.RefreshDisplayText(this.Items[index]);
			this.Redraw();
		}

		internal void OnTextChanged(int index){
			this.RefreshDisplayText(this.Items[index]);
			this.Redraw();
		}

		#endregion

		#region Drawing

		private void RefreshDisplayText(ConsoleMenuItem item){
			item.DisplayText = this.ItemTemplate.GetDisplayText(item, this.Size);
		}

		public void Redraw(){
			if(this.Screen != null){
				var endY = this.OffsetY + this.Size.Height;
				var y = 0;
				foreach(var item in this.Items){
					if((y + item.DisplayText.Length) < this.OffsetY){
						y += item.DisplayText.Length;
					}else{
						for(var i = 0; i < item.DisplayText.Length; i++){
							if(this.OffsetY <= y){
								this.Draw(item, i, y - this.OffsetY);
							}
							y++;
							if(endY < y){
								break;
							}
						}
					}
					if(endY < y){
						break;
					}
				}
			}
		}

		private void Draw(ConsoleMenuItem item, int line, int y){
			var text = this.ItemTemplate.GetText(item, line, this._OffsetX, this.Size.Width);
			this.Write(y, 0, text);
		}

		#endregion

		#region Scroll

		public void ScrollDown(int line){
			this._OffsetY += line;
			this.CheckOffset();
			this.Redraw();
		}

		public void ScrollUp(int line){
			this._OffsetY -= line;
			this.CheckOffset();
			this.Redraw();
		}

		private void CheckOffset(){
			var all = this.Items.Sum(item => item.DisplayText.Length);
			if(all <= this._OffsetY){
				this._OffsetY = all - 1;
			}
			if(this._OffsetY < 0){
				this._OffsetY = 0;
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
			var h = 0;
			for(var i = 0; i < index; i++){
				h += this.Items[i].DisplayText.Length;
			}
			if(h < this._OffsetY){
				this.ScrollUp(this._OffsetY - h);
			}else if((this._OffsetY + this.Size.Height) < h){
				this.ScrollDown(h - (this._OffsetY + this.Size.Height));
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
				var fi = this.FocusedItem;
				if(fi != null){
					fi.IsFocused = false;
				}
				this._FocusedIndex = value;
				this.Items[value].IsFocused = true;
				this.Redraw();
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
