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

namespace CatWalk.SLGameLib {
	public class RenderTimer : GameTimer, IDisposable{
		protected override void StartTimer() {
			if(this._IsDisposed){
				throw new ObjectDisposedException("this");
			}
			CompositionTarget.Rendering += this.OnRendering;
		}

		protected override void StopTimer() {
			CompositionTarget.Rendering -= this.OnRendering;
		}

		public override int FramesPerSecond {
			get {
				return Application.Current.Host.Settings.MaxFrameRate;
			}
			set {
				Application.Current.Host.Settings.MaxFrameRate = value;
			}
		}

		private void OnRendering(object sender, EventArgs e){
			base.OnTick(e);
		}

		~RenderTimer(){
			this.Dispose();
		}

		private bool _IsDisposed = false;
		public virtual void Dispose(){
			Deployment.Current.Dispatcher.BeginInvoke(new Action(delegate{
				this.Stop();
				GC.SuppressFinalize(this);
				this._IsDisposed = true;
			}));
		}
	}
}
