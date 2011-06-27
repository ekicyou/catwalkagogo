using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatWalk;
using CatWalk.Text;

namespace Twitman.Controls {
	public class ConsoleLabel : ConsoleControl{
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
				var sb = new StringBuilder(text);
				var sbOut = new StringBuilder();
				var enm = text.GetViewChunk(this.Size.Width).GetEnumerator();
				var b = enm.MoveNext();
				for(var i = 0; i < this.Size.Height && b; i++, b = enm.MoveNext()){
					this.Write(i, 0, new String(' ', this.Size.Width));
					this.Write(i, 0, enm.Current);
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
