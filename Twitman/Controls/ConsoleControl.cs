﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Diagnostics;
using System.Text;
using CatWalk;
using CatWalk.Text;

namespace Twitman.Controls {
	public abstract class ConsoleControl {
		public virtual bool IsFocusable{get; private set;}
		public Screen Screen{get; private set;}
		public Int32Point Position{get; private set;}
 		public Int32Size Size{get; private set;}
		internal bool _IsFocused;

		public ConsoleControl() : this(Int32Point.Empty, Int32Size.Empty){}
		public ConsoleControl(Int32Point position, Int32Size size){
			this.Position = position;
			this.Size = size;
			this.IsFocusable = false;
		}

		#region Event

		internal void Attach(Screen screen){
			this.Screen = screen;
			if(screen.FocusedControl == null && this.IsFocusable){
				this.IsFocused = true;
			}else{
				this.IsFocused = false;
			}
			this.OnLoad(EventArgs.Empty);
		}

		internal void Dettach(){
			this.OnUnload(EventArgs.Empty);
		}

		internal void FireKeyPress(ConsoleKeyEventArgs e){
			this.OnKeyPress(e);
		}

		protected virtual void OnLoad(EventArgs e){
		}

		protected virtual void OnUnload(EventArgs e){
			var emp = new String(' ', this.Size.Width);
			for(var y = 0; y < this.Size.Height; y++){
				this.Write(y, 0, emp);
			}
		}

		protected virtual void OnKeyPress(ConsoleKeyEventArgs e){
			var handler = this.KeyPressed;
			if(handler != null){
				handler(this, e);
			}
		}

		public event ConsoleKeyEventHandler KeyPressed;


		#endregion

		public void SetCursorPosition(int x, int y){
			Screen.SetCursorPosition(this.Position.X + x, this.Position.Y + y);
		}

		#region Focus

		public void Focus(){
			if(!this.IsFocusable){
				throw new InvalidOperationException();
			}
			this.IsFocused = true;
		}

		public bool IsFocused{
			get{
				return this._IsFocused;
			}
			set{
				if(this.Screen != null){
					this._IsFocused = value;
					this.Screen.OnFocusedControlChanged(this, value);
				}
			}
		}

		#endregion

		#region Drawing

		protected void Write(int line, int column, string text){
			this.Write(line, column, new ConsoleRun(text));
		}

		/// <summary>
		/// text is automatically trimmed to fit control width
		/// </summary>
		/// <param name="line"></param>
		/// <param name="column"></param>
		/// <param name="text"></param>
		protected void Write(int line, int column, ConsoleRun text){
			var x = column + this.Position.X;
			var y = line + this.Position.Y;
			//text = text.FitTextWidth(this.Size.Width);
			Debug.Assert((x + text.Width) <= Screen.Size.Width);
			this.Screen.Write(y, x, text);
			//System.Threading.Thread.Sleep(100);
		}

		#endregion
	}

	public class ConsoleControlCollection : Collection<ConsoleControl>{
		private Screen _Screen;

		public ConsoleControlCollection(Screen screen){
			this._Screen = screen;
		}

		protected override void InsertItem(int index, ConsoleControl item) {
			item.Attach(this._Screen);
			base.InsertItem(index, item);
		}

		protected override void RemoveItem(int index) {
			var item = this[index];
			item.Dettach();
			base.RemoveItem(index);
		}

		protected override void SetItem(int index, ConsoleControl item) {
			var old = this[index];
			if(old != item){
				old.Dettach();
				item.Attach(this._Screen);
			}
			base.SetItem(index, item);
		}

		protected override void ClearItems() {
			foreach(var item in this){
				item.Dettach();
			}
			base.ClearItems();
		}
	}
}
