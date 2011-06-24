using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatWalk;
using System.Collections.ObjectModel;

namespace Twitman {
	public static class ConsoleView {
		public static Int32Size Size{get; private set;}

		static ConsoleView(){
			Size = new Int32Size(Console.WindowWidth, Console.WindowHeight);
		}

		public static void Write(int line, int column, string text){
			var length = Size.Width - column;
			var strLength = (length < text.Length) ? length : text.Length;
			Console.SetCursorPosition(column, line);
			Console.Write(text.Substring(0, strLength));
		}

		public static void WriteLine(int line, string text){
			var minLength = (Size.Width < text.Length) ? Size.Width : text.Length;
			Console.SetCursorPosition(0, line);
			Console.WriteLine(text.Substring(0, minLength).PadRight(Size.Width));
		}
	}

	public class ConsoleMenu{
		private Int32Rect _Bounds;
		private int _OffsetX;
		private int _OffsetY;
		public ConsoleMenuItemCollection Items{get; private set;}

		public ConsoleMenu(){
			this.Items = new ConsoleMenuItemCollection(this);
		}
	}

	public class ConsoleMenuItem {
		internal ConsoleMenu Menu{get; private set;}

		#region Property

		private string _Text;
		public string Text{
			get{
				return this._Text;
			}
			set{
				this._Text = value;
			}
		}

		private bool _IsSelected;
		public bool IsSelected{
			get{
				return this._IsSelected;
			}
			set{
				this._IsSelected = value;
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

	public class ConsoleMenuItemCollection : Collection<ConsoleMenuItem>{
		internal ConsoleMenu Menu{get; private set;}

		public ConsoleMenuItemCollection(ConsoleMenu menu){
			this.Menu = menu;
		}
	}
}
