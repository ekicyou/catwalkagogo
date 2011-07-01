using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatWalk;
using CatWalk.Text;

namespace Twitman.Controls {
	public struct ConsoleRun{
		public ConsoleText[] Texts{get; private set;}
		public ConsoleColor? ForegroundColor{get; private set;}
		public ConsoleColor? BackgroundColor{get; private set;}

		public ConsoleRun(string text) : this(text, null, null){}
		public ConsoleRun(string text, ConsoleColor? foreground) : this(text, foreground, null){}
		public ConsoleRun(string text, ConsoleColor? foreground, ConsoleColor? background) : this(){
			text.ThrowIfNull("texts");
			this.Texts = new ConsoleText[]{new ConsoleText(text)};
			this.ForegroundColor = foreground;
			this.BackgroundColor = background;
		}
	
		public ConsoleRun(IEnumerable<ConsoleText> texts) : this(texts, null, null){}
		public ConsoleRun(IEnumerable<ConsoleText> texts, ConsoleColor? foreground) : this(texts, foreground, null){}
		public ConsoleRun(IEnumerable<ConsoleText> texts, ConsoleColor? foreground, ConsoleColor? background) : this(){
			texts.ThrowIfNull("texts");
			this.Texts = texts.ToArray();
			this.ForegroundColor = foreground;
			this.BackgroundColor = background;
		}

		public override string ToString() {
			return String.Join("", this.Texts);
		}

		public int Length{
			get{
				return this.Texts.Sum(t => t.Text.Length);
			}
		}

		public int Width{
			get{
				return this.Texts.Sum(t => t.Width);
			}
		}

		public ConsoleRun WidthSubstring(int column, int width){
		}
	}

	public struct ConsoleText{
		public string Text{get; private set;}
		public ConsoleColor? ForegroundColor{get; private set;}
		public ConsoleColor? BackgroundColor{get; private set;}

		public ConsoleText(string text) : this(){
			text.ThrowIfNull("text");
			this.Text = text;
		}
		public ConsoleText(string text, ConsoleColor? foreground) : this(){
			text.ThrowIfNull("text");
			this.Text = text;
			this.ForegroundColor = foreground;
		}
		public ConsoleText(string text, ConsoleColor? foreground, ConsoleColor? background) : this(){
			text.ThrowIfNull("text");
			this.Text = text;
			this.ForegroundColor = foreground;
			this.BackgroundColor = background;
		}
	
		public override string ToString() {
			return this.Text;
		}

		private int? _Width;
		public int Width{
			get{
				if(this._Width == null){
					this._Width = this.Text.GetWidth();
				}
				return this._Width.Value;
			}
		}
	}
}
