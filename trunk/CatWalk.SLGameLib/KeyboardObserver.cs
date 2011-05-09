using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace CatWalk.SLGameLib {
	public class KeyboardObserver : IDisposable{
		public bool IsHandleEvents{get; set;}
		public UIElement SourceElement{get; private set;}
		protected HashSet<Key> DownKeys{get; private set;}

		public KeyboardObserver(UIElement element) : this(element, false){}
		public KeyboardObserver(UIElement element, bool isHandle){
			if(element == null){
				throw new ArgumentNullException("element");
			}
			this.IsHandleEvents = isHandle;
			this.SourceElement = element;
			this.SourceElement.KeyDown += this.OnKeyDown;
			this.SourceElement.KeyUp += this.OnKeyUp;
			this.SourceElement.LostFocus += this.OnLostFocus;
			this.DownKeys = new HashSet<Key>();
		}

		public bool IsKeyDown(Key key){
			if(this._IsDisposed){
				throw new ObjectDisposedException("this");
			}
			return this.DownKeys.Contains(key);
		}

		private void OnKeyDown(object sender, KeyEventArgs e){
			if(!this.DownKeys.Contains(e.Key)){
				this.DownKeys.Add(e.Key);
			}
			e.Handled = this.IsHandleEvents;
		}

		private void OnKeyUp(object sender, KeyEventArgs e){
			this.DownKeys.Remove(e.Key);
			e.Handled = this.IsHandleEvents;
		}

		private void OnLostFocus(object sender, EventArgs e){
			this.DownKeys.Clear();
		}

		~KeyboardObserver(){
			this.Dispose();
		}

		private bool _IsDisposed = false;
		public virtual void Dispose(){
			this.SourceElement.Dispatcher.BeginInvoke(new Action(delegate{
				this.SourceElement.KeyDown -= this.OnKeyDown;
				this.SourceElement.KeyUp -= this.OnKeyUp;
				this.SourceElement.LostFocus -= this.OnLostFocus;
			}));
			this._IsDisposed = true;
			GC.SuppressFinalize(this);
		}
	}
}
