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
	using TickHandler = EventHandler;
	using TickArgs = EventArgs;

	public abstract class GameTimer{
		public uint CurrentFrame{get; private set;}

		public void Start(){
			this.StartTimer();
			this._IsEnabled = true;
		}
		public void Stop(){
			this.StopTimer();
			this._IsEnabled = false;
		}

		private bool _IsEnabled = false;
		public bool IsEnabled{
			get{
				return this._IsEnabled;
			}
			set{
				if(this._IsEnabled){
					this.Stop();
				}else{
					this.Start();
				}
			}
		}
		public abstract int FramesPerSecond{get; set;}

		public event TickHandler Tick;
		protected virtual void OnTick(TickArgs e){
			this.CurrentFrame++;
			var handler = this.Tick;
			if(handler != null){
				handler(this, e);
			}
			foreach(var info in this._TickPerFrameInfos){
				info.OnTick(this, e);
			}
		}

		private LinkedList<TickPerFrameInfo> _TickPerFrameInfos = new LinkedList<TickPerFrameInfo>();
		public void AddTickPerFramesHandler(TickHandler handler, int interval){
			if(handler == null){
				throw new ArgumentNullException("handler");
			}
			if(interval <= 0){
				throw new ArgumentOutOfRangeException("interval");
			}
			this._TickPerFrameInfos.AddLast(new TickPerFrameInfo(handler, interval));
		}
		public void RemoveTickPerFramesHandler(TickHandler handler, int interval){
			this._TickPerFrameInfos.Remove(new TickPerFrameInfo(handler, interval));
		}

		private sealed class TickPerFrameInfo : IEquatable<TickPerFrameInfo>{
			private int _CurrentFrames;
			private TickHandler _Tick;
			private int _Interval;

			public TickPerFrameInfo(TickHandler tick, int interval){
				this._Interval = interval;
				this._Tick = tick;
			}

			public void OnTick(object sender, TickArgs e){
				this._CurrentFrames++;
				if(this._CurrentFrames >= this._Interval){
					this._CurrentFrames = 0;
					this._Tick(sender, e);
				}
			}

			public override bool Equals(object obj) {
				return this.Equals(obj as TickPerFrameInfo);
			}

			public bool Equals(TickPerFrameInfo info){
				return this._Tick.Equals(info._Tick) && this._Interval.Equals(info._Interval);
			}

			public override int GetHashCode() {
				return this._Tick.GetHashCode() ^ this._Interval.GetHashCode();
			}
		}

		protected abstract void StartTimer();
		protected abstract void StopTimer();
	}
}
