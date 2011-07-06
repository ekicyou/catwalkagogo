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

		#region Property

		private int? _Length;
		public int Length{
			get{
				if(this._Length == null){
					this._Length = this.Texts.Sum(t => t.Text.Length);
				}
				return this._Length.Value;
			}
		}

		private int? _Width;
		public int Width{
			get{
				if(this._Width == null){
					this._Width = this.Texts.Sum(t => t.Width);
				}
				return this._Width.Value;
			}
		}

		#endregion

		#region Substring

		public ConsoleRun WidthSubstring(int column){
			if(column < 0){
				throw new ArgumentOutOfRangeException("column");
			}
			var width2 = 0;
			var list = new List<ConsoleText>(this.Texts.Length);
			bool started = false;
			foreach(var text in this.Texts){
				width2 += text.Width;
				if(started){
					list.Add(text);
				}else if(width2 > column){
					var len = width2 - column;
					list.Add(new ConsoleText(text.Text.WidthSubstring(text.Width - len), text.ForegroundColor, text.BackgroundColor));
					started = true;
				}
			}
			return new ConsoleRun(list, this.ForegroundColor, this.BackgroundColor);
		}

		public ConsoleRun WidthSubstring(int column, int width){
			bool b;
			return this.WidthSubstring(column, width, out b);
		}
		public ConsoleRun WidthSubstring(int column, int width, out bool sliced){
			if(column < 0){
				throw new ArgumentOutOfRangeException("column");
			}
			if(width < 0){
				throw new ArgumentOutOfRangeException("width");
			}
			var width2 = 0;
			var width3 = 0;
			var list = new List<ConsoleText>(this.Texts.Length);
			bool started = false;
			sliced = false;
			foreach(var text in this.Texts){
				width2 += text.Width;
				if(started){
					width3 += text.Width;
					if(width3 > width){
						var len = text.Width - (width3 - width);
						if(len > 0){
							list.Add(new ConsoleText(text.Text.WidthSubstring(0, len), text.ForegroundColor, text.BackgroundColor));
						}
						sliced = true;
						break;
					}else{
						list.Add(text);
						width3 += text.Width;
					}
				}else if(width2 > column){
					var len = width2 - column;
					if(len <= width){
						list.Add(new ConsoleText(text.Text.WidthSubstring(text.Width - len), text.ForegroundColor, text.BackgroundColor));
						width3 += len;
					}else{
						list.Add(new ConsoleText(text.Text.WidthSubstring(text.Width - len, width), text.ForegroundColor, text.BackgroundColor));
						sliced = true;
						break;
					}
					started = true;
				}
			}
			return new ConsoleRun(list, this.ForegroundColor, this.BackgroundColor);
		}

		public ConsoleRun Substring(int column){
			if(column < 0){
				throw new ArgumentOutOfRangeException("column");
			}
			var width2 = 0;
			var list = new List<ConsoleText>(this.Texts.Length);
			bool started = false;
			foreach(var text in this.Texts){
				width2 += text.Length;
				if(started){
					list.Add(text);
				}else if(width2 > column){
					var len = column - width2;
					list.Add(new ConsoleText(text.Text.Substring(text.Width - len), text.ForegroundColor, text.BackgroundColor));
					started = true;
				}
			}
			return new ConsoleRun(list, this.ForegroundColor, this.BackgroundColor);
		}

		#endregion

		#region Split

		public IEnumerable<ConsoleRun> Split(string sep){
			var part = new List<ConsoleText>();
			foreach(var text in this.Texts){
				var subTexts = text.Text.Split(new string[]{sep}, StringSplitOptions.None);
				if(subTexts.Length == 1){
					part.Add(text);
				}else{
					part.Add(new ConsoleText(subTexts[0], text.ForegroundColor, text.BackgroundColor));
					yield return new ConsoleRun(part, this.ForegroundColor, this.BackgroundColor);
					part.Clear();
					foreach(var sub in subTexts.Skip(1).Take(subTexts.Length - 2)){
						yield return new ConsoleRun(Seq.Make(new ConsoleText(sub, text.ForegroundColor, text.BackgroundColor)), this.ForegroundColor, this.BackgroundColor);
					}
					part.Add(new ConsoleText(subTexts[subTexts.Length - 1], text.ForegroundColor, text.BackgroundColor));
				}
			}
			if(part.Count > 0){
				yield return new ConsoleRun(part, this.ForegroundColor, this.BackgroundColor);
			}
		}

		#endregion

		public IEnumerable<ConsoleRun> WordWrap(int width){
			var lines = this.Split("\n");
			foreach(var line2 in lines){
				var line = line2;
				while(true){
					bool sliced;
					var sub = line.WidthSubstring(0, width, out sliced);
					yield return sub;
					if(sliced){
						line = line.WidthSubstring(width);
					}else{
						break;
					}
				}
			}
		}

		#region string

		public override string ToString() {
			return String.Join("", this.Texts);
		}

		public static implicit operator ConsoleRun(string text){
			return new ConsoleRun(text);
		}

		public static implicit operator ConsoleRun(ConsoleText text){
			return new ConsoleRun(Seq.Make(text));
		}

		public static ConsoleRun operator+(ConsoleRun a, ConsoleText b){
			return new ConsoleRun(a.Texts.Concat(Seq.Make(b)), a.ForegroundColor, b.BackgroundColor);
		}

		#endregion
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

		public int Length{
			get{
				return this.Text.Length;
			}
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

		public static implicit operator ConsoleText(string text){
			return new ConsoleText(text);
		}
	}
}
