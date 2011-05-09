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
using CatWalk.SLGameLib;

namespace Nyoroge {
	public class SceneManager : ViewModelBase{
		private Scene _CurrentScene;
		public Scene CurrentScene{
			get{
				return this._CurrentScene;
			}
			private set{
				this._CurrentScene = value;
				this.OnPropertyChanged("CurrentScene");
			}
		}

		public SceneManager(Scene initialScene){
			this.CurrentScene = initialScene;
		}

		public void Start(){
			this.CurrentScene.Exited += this.OnSceneExited;
			this.CurrentScene.Start();
		}

		public void OnSceneExited(object sender, SceneExitedEventArgs e){
			this.CurrentScene.Exited -= this.OnSceneExited;
			this.CurrentScene = e.NextScene;
			this.CurrentScene.Exited += this.OnSceneExited;
			this.CurrentScene.Start();
		}
	}
}
