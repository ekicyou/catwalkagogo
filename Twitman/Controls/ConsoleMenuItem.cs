using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Twitman.Controls {
	public class ConsoleMenuItem {
		public int Index{get; internal set;}
		internal ConsoleMenu Menu{get; set;}

		public ConsoleMenuItem(string text){
			this.Text = text;
		}

		#region Property

		private string _Text;
		public string Text{
			get{
				return this._Text;
			}
			set{
				this._Text = value;
				if(this.Menu != null){
					this.Menu.OnTextChanged(this.Index, value);
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
