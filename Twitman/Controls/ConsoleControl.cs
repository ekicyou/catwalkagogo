using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CatWalk;
using CatWalk.Text;

namespace Twitman.Controls {
	public abstract class ConsoleControl {
		public Screen Screen{get; private set;}
		public Int32Point Position{get; private set;}
 		public Int32Size Size{get; private set;}
		internal bool _IsFocused;

		public ConsoleControl() : this(Int32Point.Empty, Int32Size.Empty){}
		public ConsoleControl(Int32Point position, Int32Size size){
			this.Position = position;
			this.Size = size;
		}

		#region Event

		internal void Attach(Screen screen){
			this.Screen = screen;
			if(screen.FocusedControl == null){
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
		}

		protected virtual void OnKeyPress(ConsoleKeyEventArgs e){
		}

		#endregion

		#region Focus

		public void Focus(){
			this.IsFocused = true;
		}

		public bool IsFocused{
			get{
				return this._IsFocused;
			}
			set{
				this._IsFocused = value;
				if(this.Screen != null){
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
			this.Screen.Write(y, x, text);
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
