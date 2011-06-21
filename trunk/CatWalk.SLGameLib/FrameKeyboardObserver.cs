/*
	$Id$
*/
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
	public sealed class FrameKeyboardObserver : KeyboardObserver{
		public GameTimer Timer{get; private set;}
		public FrameKeyboardObserver(GameTimer timer, UIElement element) : this(timer, element, false){}
		private Dictionary<Key, int> DownKeyFrameCount = new Dictionary<Key,int>();

		public FrameKeyboardObserver(GameTimer timer, UIElement element, bool isHandle) : base(element, isHandle){
			this.Timer = timer;
			this.Timer.Tick += this.OnTick;
		}

		private void OnTick(object sender, EventArgs e){
			var downHandler = this.FrameKeyDown;
			var holdHandler = this.FrameKeyHold;
			var upHandler = this.FrameKeyUp;
			var upKeys = new Dictionary<Key, int>(this.DownKeyFrameCount);

			foreach(var key in this.DownKeys){
				upKeys.Remove(key);

				int frameCount;
				if(this.DownKeyFrameCount.TryGetValue(key, out frameCount)){
					frameCount++;
					if(holdHandler != null){
						holdHandler(this, new FrameKeyEventArgs(key, frameCount));
					}
				}else{
					frameCount = 0;
					if(downHandler != null){
						downHandler(this, new FrameKeyEventArgs(key, frameCount));
					}
				}
				this.DownKeyFrameCount[key] = frameCount;
			}

			foreach(var upKeyEnt in upKeys){
				if(upHandler != null){
					upHandler(this, new FrameKeyEventArgs(upKeyEnt.Key, upKeyEnt.Value));
				}
				this.DownKeyFrameCount.Remove(upKeyEnt.Key);
			}
		}

		public void ClearFrameCount(Key key){
			this.SetFrameCount(key, 0);
		}

		public void SetFrameCount(Key key, int frameCount){
			if(this.DownKeyFrameCount.ContainsKey(key)){
				this.DownKeyFrameCount[key] = frameCount;
			}
		}

		/// <summary>
		/// Get key state of the specified key.
		/// </summary>
		/// <param name="key">key</param>
		/// <returns>Return frame count the key pushed. If the key is not pushed, returns -1.</returns>
		public int GetKeyState(Key key){
			int count;
			if(this.DownKeyFrameCount.TryGetValue(key, out count)){
				return count;
			}else{
				return -1;
			}
		}

		public event FrameKeyEventHandler FrameKeyDown;
		public event FrameKeyEventHandler FrameKeyHold;
		public event FrameKeyEventHandler FrameKeyUp;

		public override void Dispose() {
			base.Dispose();
			this.Timer.Tick -= this.OnTick;
		}
	}

	public delegate void FrameKeyEventHandler(object sender, FrameKeyEventArgs e);

	public class FrameKeyEventArgs : EventArgs{
		public int FrameCount{get; private set;}
		public Key Key{get; private set;}

		public FrameKeyEventArgs(Key key, int frameCount){
			this.Key = key;
			this.FrameCount = frameCount;
		}
	}
}
