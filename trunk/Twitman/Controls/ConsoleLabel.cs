using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatWalk;
using CatWalk.Text;

namespace Twitman.Controls {
	public class ConsoleLabel : ConsoleControl{
		public ConsoleLabel(Int32Point posision, Int32Size size) : this(posision, size, new ConsoleRun()){
		}
		public ConsoleLabel(Int32Point posision, Int32Size size, ConsoleRun text) : base(posision, size){
			this.Text = text;
		}

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			this.DrawText(this._Text);
		}

		protected override void OnUnload(EventArgs e) {
			base.OnUnload(e);
			var empty = new String(' ', this.Size.Width);
			for(var i = 0; i < this.Size.Height; i++){
				this.Write(i, 0, empty);
			}
		}

		private void DrawText(ConsoleRun text){
			if(this.Screen != null && this.Text.Texts != null){
				// Clear
				var empty = new String(' ', this.Size.Width);
				for(var i = 0; i < this.Size.Height; i++){
					this.Write(i, 0, empty);
				}

				var y = 0;
				foreach(var line in this.DisplayText){
					this.Write(y, 0, line);
					y++;
					if(y >= this.Size.Height){
						break;
					}
				}
			}
		}

		public ConsoleRun[] DisplayText{get; private set;}

		private ConsoleRun _Text;
		public ConsoleRun Text{
			get{
				return this._Text;
			}
			set{
				this._Text = value;
				if(value.Texts != null){
					this.DisplayText = value.WordWrap(this.Size.Width).ToArray();
				}
				this.DrawText(this._Text);
			}
		}
	}
}
