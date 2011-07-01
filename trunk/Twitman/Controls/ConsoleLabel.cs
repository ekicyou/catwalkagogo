using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatWalk;
using CatWalk.Text;

namespace Twitman.Controls {
	public class ConsoleLabel : ConsoleControl{
		private ConsoleRun[] _Buffer;

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
			//this.DrawText(new ConsoleRun(""));
		}

		private void DrawText(ConsoleRun text){
			if(this.Screen != null){
				// Clear
				var empty = (ConsoleRun)(new String(' ', this.Size.Width));
				for(var i = 0; i < this.Size.Height; i++){
					this.Write(i, 0, empty);
				}
				var y = 0;
				for(var i = 0; i < this._Buffer.Length; i++){
					this.Write(i, 0, this._Buffer[0]);
				}
			}
		}

		private ConsoleRun _Text;
		public ConsoleRun Text{
			get{
				return this._Text;
			}
			set{
				this._Text = value;
				this.DrawText(this._Text);
			}
		}
	}
}
