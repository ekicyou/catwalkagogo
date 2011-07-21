using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CatWalk;
using CatWalk.Text;

namespace Twitman.Controls {
	public class Screen {
		#region static

		public static ConsoleColor DefaultForegroundColor{get; private set;}
		public static ConsoleColor DefaultBackgroundColor{get; private set;}
		private static int _InitialY;
		public static Int32Size Size{get; private set;}

		static Screen(){
			Console.ResetColor();
			DefaultBackgroundColor = Console.BackgroundColor;
			DefaultForegroundColor = Console.ForegroundColor;
			_InitialY = Console.CursorTop;
			Size = new Int32Size(Console.WindowWidth - 1, Console.WindowHeight - 1);
			ConsoleApplication.Exited += delegate{
				InitializeScreen();
			};
			Console.CursorVisible = false;
		}

		internal static void SetCursorPosition(int x, int y){
			Console.SetCursorPosition(x, _InitialY + y);
		}

		private static void InitializeScreen(){
			for(var i = 0; i < Screen.Size.Height; i++){
				Console.WriteLine(new String(' ', Screen.Size.Width));
			}
			SetCursorPosition(0, 0);
		}

		private static void WriteInternal(int line, int column, ConsoleRun run){
			SetCursorPosition(column, line);
			var x = column;
			var defFore = run.ForegroundColor;
			var defBack = run.BackgroundColor;
			foreach(var text in run.Texts){
				if(defFore == null && text.ForegroundColor != null){
					Console.ForegroundColor = text.ForegroundColor.Value;
				}else if(defFore != null){
					Console.ForegroundColor = defFore.Value;
				}else{
					Console.ForegroundColor = DefaultForegroundColor;
				}
				if(defBack == null && text.BackgroundColor != null){
					Console.BackgroundColor = text.BackgroundColor.Value;
				}else if(defBack != null){
					Console.BackgroundColor = defBack.Value;
				}else{
					Console.BackgroundColor = DefaultBackgroundColor;
				}
				Console.Write(text.Text, x);
				x += text.Text.GetWidth();
			}
			Console.ResetColor();
		}

		public static string ReadLine(){
			Console.CursorVisible = true;
			try{
				return Console.ReadLine();
			}finally{
				Console.CursorVisible = false;
			}
		}

		#endregion

		private bool _IsAttached = false;
		private LinkedList<BufferItem>[] _Buffer;
		public ConsoleControlCollection Controls{get; private set;}
		public ConsoleControl FocusedControl{get; private set;}

		#region Init

		public Screen(){
			this._Buffer = Enumerable.Range(0, Size.Height)
				.Select(i => new LinkedList<BufferItem>(new BufferItem[]{
					new BufferItem(0, new ConsoleRun(new String(' ', Size.Width)))
				})).ToArray();
			this.Controls = new ConsoleControlCollection(this);
		}

		internal void Attach(){
			this._IsAttached = true;
			ConsoleApplication.KeyPressed += this.KeyPressHandler;
			var y = 0;
			foreach(var line in this._Buffer){
				foreach(var item in line){
					WriteInternal(y, item.Column, item.Run);
				}
				y++;
			}
			this.OnAttach(EventArgs.Empty);
		}

		internal void Detach(){
			var emp = new ConsoleRun(new String(' ', Size.Width));
			for(var y = 0; y < Size.Height; y++){
				WriteInternal(y, 0, emp);
			}
			this._IsAttached = false;
			ConsoleApplication.KeyPressed -= this.KeyPressHandler;
			this.OnDetach(EventArgs.Empty);
		}

		protected virtual void OnAttach(EventArgs e){
			var handler = this.Attached;
			if(handler != null){
				handler(this, EventArgs.Empty);
			}
		}

		protected virtual void OnDetach(EventArgs e){
			var handler = this.Detached;
			if(handler != null){
				handler(this, EventArgs.Empty);
			}
		}

		public event EventHandler Attached;
		public event EventHandler Detached;

		#endregion

		#region Event

		private void KeyPressHandler(object sender, ConsoleKeyEventArgs e){
			this.OnKeyPress(e);
		}

		protected virtual void OnKeyPress(ConsoleKeyEventArgs e){
			var handler = this.KeyPress;
			if(handler != null){
				handler(this, e);
			}
			if(!e.IsHandled){
				if(this.FocusedControl != null){
					this.FocusedControl.FireKeyPress(e);
				}
			}
		}

		public event ConsoleKeyEventHandler KeyPress;

		internal void OnFocusedControlChanged(ConsoleControl control, bool isFocused){
			if(isFocused){
				foreach(var ctrl in this.Controls){
					ctrl._IsFocused = false;
				}
				this.FocusedControl = control;
			}
		}

		protected virtual void OnCancelKeyPress(ConsoleCancelEventArgs e){
			e.Cancel = true;
			//ConsoleApplication.Exit();
		}

		internal void FireCancelKeyPress(ConsoleCancelEventArgs e){
			this.OnCancelKeyPress(e);
		}

		#endregion

		internal void Write(int line, int column, ConsoleRun run){
			if(line < 0 || Size.Height <= line){
				throw new ArgumentNullException("line");
			}
			if(column < 0 || Size.Width <= column){
				throw new ArgumentNullException("column");
			}
			var old = this._Buffer[line];

			var runItem = new BufferItem(column, run);
			var runWidth = runItem.Run.Width;
			var node = old.Last;
			while(node != null){
				var next = node.Previous;
				var item = node.Value;
				if(column <= item.Column && ((item.Column + item.Run.Width) <= (column + runWidth))){
					old.Remove(node);
				}
				node = next;
			}
			old.AddLast(runItem);

			// draw
			if(this._IsAttached){
				WriteInternal(line, column, run);
			}
		}

		private struct BufferItem{
			public int Column{get; private set;}
			public ConsoleRun Run{get; private set;}

			public BufferItem(int column, ConsoleRun run) : this(){
				this.Column = column;
				this.Run = run;
			}
		}
	}
}
