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

namespace Nyoroge{
	public class TitleScene : Scene{
		public UIElement InputElement{get; private set;}
		private GameScene _GameScene;

		public TitleScene(UIElement inputElement){
			this.InputElement = inputElement;
			this._GameScene = new GameScene(this.InputElement);
		}

		public override void Start() {
			this.InputElement.KeyDown += new KeyEventHandler(this.InputElement_KeyDown);
		}

		private void InputElement_KeyDown(object sender, KeyEventArgs e) {
			this.InputElement.KeyDown -= this.InputElement_KeyDown;
			var storyboard = (Storyboard)((FrameworkElement)this._OverlayContent).Resources["ExitAnimation"];
			storyboard.Completed += delegate{
				this.OnExited(new SceneExitedEventArgs(this._GameScene));
			};
			storyboard.Begin();
		}

		public override object Content {
			get {
				return this._GameScene.Content;
			}
		}

		private object _OverlayContent;
		public override object OverlayContent {
			get{
				return this._OverlayContent ?? (this._OverlayContent = XamlLoader.LoadXaml("Scenes/TitleOverlay.xaml"));
			}
		}
	}
}
