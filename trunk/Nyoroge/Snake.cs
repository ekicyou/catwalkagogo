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
using System.Linq;
using CatWalk.SLGameLib;
using System.Windows.Media.Imaging;

namespace Nyoroge {
	public class Snake : ViewModelBase{
		private LinkedList<SnakeBody> _Bodies = new LinkedList<SnakeBody>();
		private Int32Rect _Bounds; public Int32Rect Bounds{get{return this._Bounds;}}
		private Map _Map;
		private int _PendingGrowCount = 1;

		public Snake(Map map, Int32Rect bounds, Int32Point point){
			this._Map = map;
			this._Bounds = bounds;
			this._Bodies.AddFirst(new SnakeHead(){Location = point});
			this._Map.PutObject(this._Bodies.First.Value, point);
		}

		/// <summary>
		/// Move
		/// </summary>
		/// <param name="dir">Direction</param>
		/// <exception cref="SnakeHitHisBodyException">Snake failed to move toward the direction.</exception>
		public void Move(Direction dir){
			var head = this._Bodies.First.Value;
			Int32Vector vector = new Int32Vector();
			switch(dir){
				case Direction.Up: vector = new Int32Vector(0, -1); break;
				case Direction.Down: vector = new Int32Vector(0, 1); break;
				case Direction.Left: vector = new Int32Vector(-1, 0); break;
				case Direction.Right: vector = new Int32Vector(1, 0); break;
			}

			// move head and add body after head
			var oldHeadLocation = head.Location;
			var oldHeadDirection = head.Direction;
			var newHeadLocation = oldHeadLocation + vector;
			head.Location = newHeadLocation;
			head.Direction = dir;
			SnakeBody newBody = (this._Bodies.Count == 1) ? new SnakeTail() : new SnakeBody();
			newBody.Location = oldHeadLocation;
			newBody.Direction = oldHeadDirection;
			this._Bodies.AddAfter(this._Bodies.First, newBody);
			this.OnPropertyChanged("HeadDirection", "HeadLocation");

			this._Map.PutObject(newBody, newBody.Location);

			if(this._PendingGrowCount == 0){
				// remove tail
				var tail = this._Bodies.Last;
				var tailPos = tail.Value.Location;
				var tailBody = tail.Previous;
				this._Bodies.Remove(tailBody);
				tail.Value.Location = tailBody.Value.Location;
				tail.Value.Direction = tailBody.Value.Direction;

				this._Map.RemoveObject(tailPos);
				this._Map.PutObject(tail.Value, tail.Value.Location);
			}else{
				this.OnPropertyChanged("Length");
				this._PendingGrowCount--;
			}
			// hit check rect
			this._Map.PutObject(head, head.Location);
			if(!this._Bounds.Contains(head.Location)){
				throw new SnakeOutOfBoundsException(head.Location);
			}

			// hit check body
			var hitBody = this._Bodies.Skip(1).FirstOrDefault(body => body.Location == head.Location);
			if(hitBody != null){
				throw new SnakeHitHisBodyEception(hitBody);
			}
		}

		public void Grow(int n){
			this._PendingGrowCount += n;
		}

		public Direction HeadDirection{
			get{
				return this._Bodies.First.Value.Direction;
			}
			set{
				var head = this._Bodies.First.Value;
				head.Direction = value;
				this._Map.PutObject(head, head.Location);
			}
		}

		public Int32Point HeadLocation{
			get{
				return this._Bodies.First.Value.Location;
			}
		}

		public int Length{
			get{
				return this._Bodies.Count;
			}
		}
	}

	public class SnakeBody : MapObject{
		private static WriteableBitmap _SnakeBodyBitmap = ImageLoader.LoadBitmap(@"Resource/SnakeBody.png");

		public Int32Point Location{get; set;}
		public Direction Direction{get; set;}

		public SnakeBody(){}

		public override int[] Pixels {
			get { 
				return _SnakeBodyBitmap.Pixels;
			}
		}
	}

	public class SnakeHead : SnakeBody{
		private static WriteableBitmap _SnakeHeadBitmapUp = ImageLoader.LoadBitmap(@"Resource/SnakeHeadUp.png");
		private static WriteableBitmap _SnakeHeadBitmapLeft = ImageLoader.LoadBitmap(@"Resource/SnakeHeadLeft.png");
		private static WriteableBitmap _SnakeHeadBitmapRight = ImageLoader.LoadBitmap(@"Resource/SnakeHeadRight.png");
		private static WriteableBitmap _SnakeHeadBitmapDown = ImageLoader.LoadBitmap(@"Resource/SnakeHeadDown.png");

		public override int[] Pixels {
			get {
				switch(this.Direction){
					default:
					case Nyoroge.Direction.Up: return _SnakeHeadBitmapUp.Pixels;
					case Nyoroge.Direction.Down: return _SnakeHeadBitmapDown.Pixels;
					case Nyoroge.Direction.Left: return _SnakeHeadBitmapLeft.Pixels;
					case Nyoroge.Direction.Right: return _SnakeHeadBitmapRight.Pixels;
				}
			}
		}
	}
	public class SnakeTail : SnakeBody{
		private static WriteableBitmap _SnakeTailBitmapUp = ImageLoader.LoadBitmap(@"Resource/SnakeTailUp.png");
		private static WriteableBitmap _SnakeTailBitmapLeft = ImageLoader.LoadBitmap(@"Resource/SnakeTailLeft.png");
		private static WriteableBitmap _SnakeTailBitmapRight = ImageLoader.LoadBitmap(@"Resource/SnakeTailRight.png");
		private static WriteableBitmap _SnakeTailBitmapDown = ImageLoader.LoadBitmap(@"Resource/SnakeTailDown.png");

		public override int[] Pixels {
			get {
				switch(this.Direction){
					default:
					case Nyoroge.Direction.Up: return _SnakeTailBitmapUp.Pixels;
					case Nyoroge.Direction.Down: return _SnakeTailBitmapDown.Pixels;
					case Nyoroge.Direction.Left: return _SnakeTailBitmapLeft.Pixels;
					case Nyoroge.Direction.Right: return _SnakeTailBitmapRight.Pixels;
				}
			}
		}
	}

	public class SnakeHitHisBodyEception : Exception{
		public SnakeBody HitBody{get; private set;}

		public Snake Snake {
			get {
				throw new System.NotImplementedException();
			}
			set {
			}
		}

		public SnakeHitHisBodyEception(SnakeBody hitBody){
			this.HitBody = hitBody;
		}
	}

	public class SnakeOutOfBoundsException : Exception{
		public Int32Point Location{get; private set;}

		public Snake Snake {
			get {
				throw new System.NotImplementedException();
			}
			set {
			}
		}

		public SnakeOutOfBoundsException(Int32Point location){
			this.Location = location;
		}
	}

	public enum Direction{
		Left, Up, Right, Down
	}
}
