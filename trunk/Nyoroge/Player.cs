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
using CatWalk.SLGameLib;

namespace Nyoroge {
	public abstract class Player {
		public virtual void Initialize(Snake snake, Map map){}
		public virtual void ProcessFrame(){}
		public abstract Direction GetSnakeDirection();
	}

	public class HumanPlayer : Player{
		private Direction _CurrentDirection;

		public HumanPlayer(UIElement inputElement){
			inputElement.KeyDown += new KeyEventHandler(InputElement_KeyDown);
		}

		public override void Initialize(Snake snake, Map map) {
			this._CurrentDirection = snake.HeadDirection;
		}

		private void InputElement_KeyDown(object sender, KeyEventArgs e) {
			switch(e.Key){
				case Key.Up: this._CurrentDirection = Direction.Up; return;
				case Key.Down: this._CurrentDirection = Direction.Down; return;
				case Key.Left: this._CurrentDirection = Direction.Left; return;
				case Key.Right: this._CurrentDirection = Direction.Right; return;
				default: return;
			}
		}

		public override Direction GetSnakeDirection() {
			return this._CurrentDirection;
		}
	}
}
