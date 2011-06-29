using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatWalk;
using CatWalk.Text;

namespace Twitman.Controls {
	public class ConsoleLabel : ConsoleControl{
		public ConsoleLabel(Int32Point posision, Int32Size size) : this(posision, size, ""){
		}
		public ConsoleLabel(Int32Point posision, Int32Size size, string text) : base(posision, size){
			this.Text = text;
		}

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			this.DrawText(this._Text);
		}

		protected override void OnUnload(EventArgs e) {
			base.OnUnload(e);
			this.DrawText("");
		}

		private void DrawText(string text){
			if(this.Screen != null){
				var empty = new String(' ', this.Size.Width);
				for(var i = 0; i < this.Size.Height; i++){
					this.Write(i, 0, empty);
				}
				var y = 0;
				while(text != String.Empty){
					var line = text.WidthSubstring(0, this.Size.Width);
					this.Write(y, 0, line);
					if(line.Length < text.Length){
						text = text.Substring(line.Length);
					}else{
						break;
					}
				}
			}
		}

		private string _Text;
		public string Text{
			get{
				return this._Text;
			}
			set{
				if(value == null){
					throw new ArgumentNullException();
				}
				this._Text = value;
				this.DrawText(this._Text);
			}
		}
	}
}
