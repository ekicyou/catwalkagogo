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

namespace Nyoroge {
	public abstract class Scene : ViewModelBase{
		public abstract void Start();
		public abstract object Content{get;}
		public virtual object OverlayContent{
			get{
				return null;
			}
		}

		public event SceneExitedEventHandler Exited;
		protected virtual void OnExited(SceneExitedEventArgs e){
			var handler = this.Exited;
			if(handler != null){
				handler(this, e);
			}
		}

		public SceneExitedEventHandler SceneExitedEventHandler {
			get {
				throw new System.NotImplementedException();
			}
			set {
			}
		}
	}

	public delegate void SceneExitedEventHandler(object sender, SceneExitedEventArgs e);

	public class SceneExitedEventArgs : EventArgs{
		public Scene NextScene{get; private set;}

		public SceneExitedEventArgs(Scene nextScene){
			this.NextScene = nextScene;
		}
	}
}
