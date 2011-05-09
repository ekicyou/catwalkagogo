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
using System.Collections.ObjectModel;

namespace Nyoroge {
	public class ScoresScene : Scene{
		public GameScene GameScene{get; private set;}
		public ObservableCollection<HighScoreItem> HighScores{get; private set;}
		private UIElement _InputElement;
		private const int HighScoreCount = 3;

		public ScoresScene(UIElement inputElement, GameScene gameScene, GameResult result){
			this._InputElement = inputElement;
			this.GameScene = gameScene;
			this.HighScores = new ObservableCollection<HighScoreItem>();

			GameResult[] scores;
			if(!IsolatedStorageSettings.SiteSettings.TryGetValue("HighScores", out scores)){
				scores = new GameResult[0];
			}
			var highScores = scores.Concat(new GameResult[]{result}).OrderByDescending(score => score.SnakeLength).Take(HighScoreCount).ToArray();
			foreach(var score in highScores.Select((score, idx) => new HighScoreItem(idx + 1, score))){
				this.HighScores.Add(score);
			}
			IsolatedStorageSettings.SiteSettings["HighScores"] = highScores;
			IsolatedStorageSettings.SiteSettings.Save();
		}

		public override void Start() {
		}

		private void IntroStoryboard_Completed(object sender, EventArgs e){
			this._InputElement.KeyDown += this.InputElement_KeyDown;
		}

		private void InputElement_KeyDown(object sender, KeyEventArgs e) {
			this._InputElement.KeyDown -= this.InputElement_KeyDown;
			//this.OnExited(new SceneExitedEventArgs(new GameScene(this.GameScene.KeyboardObserver.SourceElement)));
			this.OnExited(new SceneExitedEventArgs(new GameScene(this._InputElement)));
			e.Handled = true;
		}

		public override object Content{
			get{
				return this.GameScene.Content;
			}
		}

		public override object OverlayContent {
			get{
				var cont = (FrameworkElement)XamlLoader.LoadXaml(@"Scenes/ScoresOverlay.xaml");
				var storyboard = (Storyboard)cont.FindName("IntroStoryboard");
				storyboard.Completed += new EventHandler(IntroStoryboard_Completed);
				cont.DataContext = this;
				return cont;
			}
		}

		public class HighScoreItem{
			public int Rank{get; private set;}
			public GameResult Result{get; private set;}

			public HighScoreItem(int rank, GameResult result){
				this.Rank = rank;
				this.Result = result;
			}
		}
	}
}
