using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Twitman.Controls {
	public class ConsoleMenuItem {
		public int Index{get; internal set;}
		public object Value{get; set;}
		internal ConsoleMenu Menu{get; set;}
		internal ConsoleRun[] DisplayText{get; set;}

		public ConsoleMenuItem(string text) : this(text, null){}
		public ConsoleMenuItem(string text, object value){
			this.Text = new ConsoleRun(text);
			this.Value = value;
		}

		public ConsoleMenuItem(ConsoleRun text) : this(text, null){}
		public ConsoleMenuItem(ConsoleRun text, object value){
			this.Text = text;
			this.Value = value;
		}

		#region Property

		private ConsoleRun _Text;
		public ConsoleRun Text{
			get{
				return this._Text;
			}
			set{
				this._Text = value;
				if(this.Menu != null){
					this.Menu.OnTextChanged(this.Index);
				}
			}
		}

		private bool _IsSelected;
		public bool IsSelected{
			get{
				return this._IsSelected;
			}
			set{
				if(this.Menu != null){
					this._IsSelected = value;
					this.Menu.OnSelectionChanged(this.Index, value);
				}
			}
		}

		#endregion

		#region Event

		internal void Execute(){
			this.OnExecuted();
		}

		public event EventHandler Executed;

		protected virtual void OnExecuted(){
			var handler = this.Executed;
			if(handler != null){
				handler(this, EventArgs.Empty);
			}
		}

		#endregion
	}

}
