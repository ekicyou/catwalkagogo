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
using System.Threading;
using CatWalk.SLGameLib;
using System.IO.IsolatedStorage;
using System.Linq;

namespace Nyoroge {
	public class GameOverScene : Scene{
		public GameScene GameScene{get; private set;}
		public GameResult Result{get; private set;}
		private UIElement _InputElement;

		public GameOverScene(UIElement inputElement, GameScene gameScene, Int32Point hitLocation){
			this._InputElement = inputElement;
			this.GameScene = gameScene;
			this.Result = new GameResult(gameScene.Snake.Length, gameScene.Duration);
		}

		public override void Start() {
		}

		private void IntroStoryboard_Completed(object sender, EventArgs e){
			this.OnExited(new SceneExitedEventArgs(new ScoresScene(this._InputElement, this.GameScene, this.Result)));
		}

		public override object Content{
			get{
				return this.GameScene.Content;
			}
		}

		public override object OverlayContent {
			get{
				var cont = (FrameworkElement)XamlLoader.LoadXaml(@"Scenes/GameOverOverlay.xaml");
				var storyboard = (Storyboard)cont.FindName("IntroStoryboard");
				storyboard.Completed += new EventHandler(IntroStoryboard_Completed);
				cont.DataContext = this;
				return cont;
			}
		}
	}
}
