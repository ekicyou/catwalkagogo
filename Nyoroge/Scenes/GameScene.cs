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
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using CatWalk.SLGameLib;
using CatWalk.SLGameLib.Views;

namespace Nyoroge {
	public class GameScene : Scene{
		private static WallObject _Wall = new WallObject();

		private GameTimer _Timer; public GameTimer Timer{get{return this._Timer;}}
		private Map _Map; public Map Map{get{return this._Map;}}
		private Snake _Snake; public Snake Snake{get{return this._Snake;}}
		private int _SnakeMovePerFrame = 5;
		private LinkedList<MapItem> _MapItems = new LinkedList<MapItem>(); public ICollection<MapItem> MapItems{get{return this._MapItems;}}
		private DateTime _StartTimestamp; public TimeSpan Duration{get{return DateTime.Now - this._StartTimestamp;}}
		public Player Player{get; private set;}
		private UIElement _InputElement;

		public GameScene(UIElement inputElement){
			this._InputElement = inputElement;
			this._Timer = new RenderTimer();
			this._Timer.FramesPerSecond = 30;
			this.Player = new HumanPlayer(inputElement);
			var mapSize = new Int32Size(32, 32);
			this._Map = new Map(mapSize);
			this._Snake = new Snake(this._Map, new Int32Rect(1, 1, mapSize.Width - 2, mapSize.Height - 2), new Int32Point(1, 1));
			this._Snake.HeadDirection = Direction.Right;
			this.DrawWall();

			this.Player.Initialize(this._Snake, this._Map);
			this._Timer.Tick += new EventHandler(this.Timer_Tick);
		}

		public override void Start(){
			if(this._Timer.IsEnabled){
				throw new InvalidOperationException();
			}
			this._Timer.Start();
			this.PutRandomScoreItem();
			this._StartTimestamp = DateTime.Now;
		}

		#region MapItem

		private Int32Point GetRandomLocation(){
			return new Int32Point(App.Random.Next(this._Snake.Bounds.Width) + 1, App.Random.Next(this._Snake.Bounds.Height) + 1);
		}

		private Int32Point GetRandomEmptyLocation(){
			var length = this._Snake.Bounds.Width * this._Snake.Bounds.Height;
			var emptyCount = length - this._Snake.Length - this._MapItems.Count;
			var index = App.Random.Next(emptyCount);
			var idx = 1;
			for(var x = this._Snake.Bounds.Left; x < this._Snake.Bounds.Right; x++){
				for(var y = this._Snake.Bounds.Top; y < this._Snake.Bounds.Bottom; y++){
					var i = x + this._Map.Size.Width * y;
					if(this._Map.Data[i] == null){
						idx++;
					}
					if(idx == index){
						return new Int32Point(x, y);
					}
				}
			}
			throw new InvalidOperationException();
		}

		private void PutRandomScoreItem(){
			var item = new ScoreItem(this.GetRandomEmptyLocation(), App.Random.Next(9) + 1);
			this._MapItems.AddLast(item);
			this._Map.PutObject(item, item.Location);
		}

		#endregion

		private void DrawWall(){
			var left = 0;
			var top = 0;
			var bottom = this._Map.Size.Width - 1;
			var right = this._Map.Size.Height - 1;
			for(var y = top; y <= bottom; y++){
				this._Map.PutObject(_Wall, left, y);
				this._Map.PutObject(_Wall, right, y);
			}
			for(var x = left; x <= right; x++){
				this._Map.PutObject(_Wall, x, top);
				this._Map.PutObject(_Wall, x, bottom);
			}
		}

		#region Content

		private object _Content;
		public override object Content {
			get{
				if(this._Content != null){
					return this._Content;
				}
				// <view:MapView x:Name="_MapView" Background="Azure" BorderBrush="Aqua" BorderThickness="1" HorizontalAlignment="Center" VerticalAlignment="Center"
				// Source="{Binding Map.Data}" ValueToPixelsConverter="{Binding Map.ValueToPixelsConverter}"
				// MapWidth="{Binding Map.Size.Width}" MapHeight="{Binding Map.Size.Height}" CellWidth="16" CellHeight="16" />
				var mapView = new MapView();
				mapView.Background = new SolidColorBrush(Colors.White);
				mapView.BorderBrush = new SolidColorBrush(Colors.Purple);
				mapView.BorderThickness = new Thickness(1);
				mapView.HorizontalAlignment = HorizontalAlignment.Left;
				mapView.VerticalAlignment = VerticalAlignment.Top;
				mapView.SetBinding(MapView.SourceProperty, new Binding("Map.Data"));
				mapView.SetBinding(MapView.ValueToPixelsConverterProperty, new Binding("Map.ValueToPixelsConverter"));
				mapView.SetBinding(MapView.MapWidthProperty, new Binding("Map.Size.Width"));
				mapView.SetBinding(MapView.MapHeightProperty, new Binding("Map.Size.Height"));
				mapView.CellHeight = mapView.CellWidth = 16;
				mapView.DataContext = this;
				return (this._Content = mapView);
			}
		}

		public override object OverlayContent {
			get {
				var panel = new Grid();
				var text2 = new TextBlock(){
					FontSize=16,
					FontWeight=FontWeights.ExtraBold,
					Foreground=new SolidColorBrush(Colors.Cyan),
					HorizontalAlignment=HorizontalAlignment.Right,
					VerticalAlignment=VerticalAlignment.Top
				};
				text2.SetBinding(TextBlock.TextProperty, new Binding("Snake.Length"){Source=this, StringFormat="長さ: {0} にょろ"});
				panel.Children.Add(text2);
				return panel;
			}
		}

		#endregion

		private int _SnakeWaitFrameCount = 0;
		private void Timer_Tick(object sender, EventArgs e) {
			this.Player.ProcessFrame();
			this._SnakeWaitFrameCount++;
			if(this._SnakeWaitFrameCount >= this._SnakeMovePerFrame){
				try{
					this._Snake.Move(this.Player.GetSnakeDirection());
				}catch(SnakeHitHisBodyEception ex){
					this.OnExited(new SceneExitedEventArgs(new GameOverScene(this._InputElement, this, ex.HitBody.Location)));
				}catch(SnakeOutOfBoundsException ex){
					this.OnExited(new SceneExitedEventArgs(new GameOverScene(this._InputElement, this, ex.Location)));
				}
				this._SnakeWaitFrameCount = 0;
				this.OnPropertyChanged("Duration");
			}

			// hit check between snake and items
			var node = this._MapItems.First;
			while(node != null){
				if(node.Value.Location == this._Snake.HeadLocation){
					node.Value.Take(this);
					this._MapItems.Remove(node);
					this.PutRandomScoreItem();
					break;
				}
				node = node.Next;
			}
		}

		protected override void OnExited(SceneExitedEventArgs e) {
			this._Timer.Tick -= this.Timer_Tick;
			base.OnExited(e);
		}
	}

	public class WallObject : MapObject{
		private static WriteableBitmap _WallBitmap = ImageLoader.LoadBitmap("Resource/Wall.png");

		public override int[] Pixels {
			get {
				return _WallBitmap.Pixels;
			}
		}
	}

	public abstract class MapItem : MapObject{
		public Int32Point Location{get; private set;}
		public MapItem(Int32Point location){
			this.Location = location;
		}

		public abstract void Take(GameScene scene);
	}

	public class ScoreItem : MapItem{
		public static Dictionary<int, WeakReference> _ScoreBitmapCache = new Dictionary<int,WeakReference>();
		public int Score{get; private set;}
		private int[] _Pixels;
		public override int[] Pixels{
			get{
				return this._Pixels;
			}
		}

		public ScoreItem(Int32Point location, int score) : base(location){
			if(score <= 0){
				throw new ArgumentOutOfRangeException("score");
			}
			this.Score = score;
			this._Pixels = GetBitmap(score).Pixels;
		}

		private static WriteableBitmap GetBitmap(int score){
			WeakReference wref;
			if(_ScoreBitmapCache.TryGetValue(score, out wref)){
				var bmp = (WriteableBitmap)wref.Target;
				if(bmp != null){
					return bmp;
				}
			}
			var newBmp = new WriteableBitmap(16, 16);
			wref = new WeakReference(newBmp);
			_ScoreBitmapCache[score] = wref;
			var text = new TextBlock(){
				Text = score.ToString(),
				FontSize = 16,
				Foreground = new SolidColorBrush(Colors.Red),
			};
			var trans = new ScaleTransform();
			trans.ScaleX = 16 / text.ActualWidth;
			trans.ScaleY = 16 / text.ActualHeight;
			newBmp.Render(text, trans);
			newBmp.Invalidate();
			return newBmp;
		}

		public override void Take(GameScene scene) {
			scene.Snake.Grow(this.Score);
		}
	}
}
