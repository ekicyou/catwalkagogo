/*
	$Id: Account.cs 51 2010-01-28 21:55:33Z catwalk $
*/
using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Threading;
using CatWalk.Graph;
using MazeGenSL.Models;

namespace MazeGenSL.ViewModels {
	public class BoardViewModel : ViewModelBase{
		public BoardViewModel(){
			this.Size = new BoardSize(51, 51);
			this.RefreshBoard();
		}

		#region Properties

		private bool _IsBusy = false;
		public bool IsBusy{
			get{
				return this._IsBusy;
			}
			private set{
				this._IsBusy = value;
				this.GenerateCommand.RaiseCanExecuteChanged();
				this.SolveCommand.RaiseCanExecuteChanged();
			}
		}
		private bool IsNotBusy(){
			return !this._IsBusy;
		}

		private BoardSize _Size;
		public BoardSize Size{
			get{
				return this._Size;
			}
			set{
				this._Size = value;
				this.OnPropertyChanged("Size");
				this.RefreshBoard();
			}
		}

		private int _WaitTime = 0;
		public int WaitTime{
			get{
				return this._WaitTime;
			}
			set{
				this._WaitTime = value;
				this.OnPropertyChanged("WaitTime");
			}
		}

		#endregion

		#region Board

		private ObservableCollection<Cell> _Board = new ObservableCollection<Cell>();
		private ReadOnlyObservableCollection<Cell> _ReadOnlyBoard;

		public ReadOnlyObservableCollection<Cell> Board{
			get{
				return this._ReadOnlyBoard ?? (this._ReadOnlyBoard = new ReadOnlyObservableCollection<Cell>(this._Board));
			}
		}

		private int GetIndex(int x, int y){
			return x + y * this.Size.X;
		}

		private void RefreshBoard(){
			this._Board = new ObservableCollection<Cell>(new Cell[this.Size.X * this.Size.Y]);
			this._ReadOnlyBoard = null;
			this.OnPropertyChanged("Board");

			this._AllNodes = null;
			this.GenerateCommand.RaiseCanExecuteChanged();
			this.SolveCommand.RaiseCanExecuteChanged();
		}

		private Node<NodeInfo>[, ] _AllNodes;

		#endregion

		#region Generate

		private DelegateCommand _GenerateCommand;
		public DelegateCommand GenerateCommand{
			get{
				return this._GenerateCommand ?? (this._GenerateCommand = new DelegateCommand(this.Generate, this.CanGenerate));
			}
		}

		public bool CanGenerate(){
			return this.IsNotBusy() && this.Size.X > 2 && this.Size.Y > 2;
		}

		public void Generate(){
			if(this._IsBusy){
				throw new InvalidOperationException();
			}
			this.IsBusy = true;
			object job = App.Default.ProgressManager.AddJob();

			var x = this.Size.X;
			var y = this.Size.Y;

			ThreadPool.QueueUserWorkItem(new WaitCallback(delegate{
				Deployment.Current.Dispatcher.BeginInvoke(new Action(delegate{
					// reset
					for(var j = 0; j < y; j++){
						for(var i = 0; i < x; i++){
							if(this._Board[this.GetIndex(i, j)] != Cell.Wall){
								this._Board[this.GetIndex(i, j)] = Cell.Wall;
							}
						}
					}
				}));

				// Initialize
				var x2 = x / 2;
				var y2 = y / 2;
				var workingNodes = new Node<NodeInfo>[x2, y2];
				for(var i = 0; i < x2; i++){
					for(var j = 0; j < y2; j++){
						workingNodes[i, j] = new Node<NodeInfo>(new NodeInfo((i << 1) + 1, (j << 1) + 1));
					}
				}

				// Add links
				var x2_1 = x2 - 1;
				var y2_1 = y2 - 1;
				var rnd = new Random();
				for(var i = 0; i < x2; i++){
					for(var j = 0; j < y2; j++){
						var node = workingNodes[i, j];
						if(i > 0){
							node.AddLink(workingNodes[i - 1, j], rnd.Next());
						}
						if(i < x2_1){
							node.AddLink(workingNodes[i + 1, j], rnd.Next());
						}
						if(j > 0){
							node.AddLink(workingNodes[i, j - 1], rnd.Next());
						}
						if(j < y2_1){
							node.AddLink(workingNodes[i, j + 1], rnd.Next());
						}
					}
				}

				// Create road
				var startX2 = 0;
				var startY2 = 0;
				var startNode = workingNodes[startX2, startY2];
				var mst = startNode.GetMinimumSpanningTree();
				//var immediate = this.WaitTime == 0;
				const bool immediate = false;
				var board = immediate ? new List<Cell>(new Cell[this.Size.X * this.Size.Y]) : (IList<Cell>)this._Board;

				this._AllNodes = new Node<NodeInfo>[x2, y2];
				for(var i = 0; i < x2; i++){
					for(var j = 0; j < y2; j++){
						this._AllNodes[i, j] = new Node<NodeInfo>(new NodeInfo((i << 1) + 1, (j << 1) + 1));
					}
				}
				try{
					var context = new DispatcherSynchronizationContext(Deployment.Current.Dispatcher);
					var i = 0;
					var mstSize = (x2 - 1) * y2 + (y2 - 1);
					foreach(var link in mst){
						if(this._WaitTime > 0){
							Thread.Sleep(this._WaitTime);
						}
						this._AllNodes[link.From.Value.X >> 1, link.From.Value.Y >> 1].AddLink(
							this._AllNodes[link.To.Value.X >> 1, link.To.Value.Y >> 1], 1);
						this._AllNodes[link.To.Value.X >> 1, link.To.Value.Y >> 1].AddLink(
							this._AllNodes[link.From.Value.X >> 1, link.From.Value.Y >> 1], 1);
						var xFrom = link.From.Value.X;
						var yFrom = link.From.Value.Y;
						var xTo = link.To.Value.X;
						var yTo = link.To.Value.Y;
						var xWall = (xFrom + xTo) >> 1;
						var yWall = (yFrom + yTo) >> 1;
						var callback = new SendOrPostCallback(delegate{
							if(!immediate){
								App.Default.ProgressManager.ReportProgress(job, (double)i / (double)mstSize);
							}
							if(board[this.GetIndex(xFrom, yFrom)] != Cell.Road){
								board[this.GetIndex(xFrom, yFrom)] = Cell.Road;
							}
							if(board[this.GetIndex(xTo, yTo)] != Cell.Road){
								board[this.GetIndex(xTo, yTo)] = Cell.Road;
							}
							if(board[this.GetIndex(xWall, yWall)] != Cell.Road){
								board[this.GetIndex(xWall, yWall)] = Cell.Road;
							}
						});
						if(immediate){
							callback(null);
						}else if(this._WaitTime > 0){
							context.Send(callback, null);
						}else{
							context.Post(callback, null);
						}
						i++;
					}
				}finally{
					Deployment.Current.Dispatcher.BeginInvoke(new Action(delegate{
						if(immediate){
							this._Board = new ObservableCollection<Cell>(board);
							this._ReadOnlyBoard = null;
							this.OnPropertyChanged("Board");
						}
						this.IsBusy = false;
						App.Default.ProgressManager.Complete(job);
					}));
				}
			}));
		}

		#endregion

		#region Solve

		private DelegateCommand _SolveCommand;
		public DelegateCommand SolveCommand{
			get{
				return this._SolveCommand ?? (this._SolveCommand = new DelegateCommand(this.Solve, this.CanSolve));
			}
		}

		private bool CanSolve(){
			return this.IsNotBusy() && this._AllNodes != null;
		}

		public void Solve(){
			if(this._IsBusy){
				throw new InvalidOperationException();
			}
			this.IsBusy = true;

			var x = this.Size.X;
			var y = this.Size.Y;
			var start = this._AllNodes[0, 0];
			var goal = this._AllNodes[this._AllNodes.GetLength(0) - 1, this._AllNodes.GetLength(1) - 1];
			this._Board[this.GetIndex(start.Value.X, start.Value.Y)] = Cell.Start;
			this._Board[this.GetIndex(goal.Value.X, goal.Value.Y)] = Cell.Goal;
			var job = App.Default.ProgressManager.AddJob();
			ThreadPool.QueueUserWorkItem(new WaitCallback(delegate{
				try{
					Deployment.Current.Dispatcher.BeginInvoke(new Action(delegate{
						// reset
						for(var j = 0; j < y; j++){
							for(var i = 0; i < x; i++){
								if(this._Board[this.GetIndex(i, j)] == Cell.Route){
									this._Board[this.GetIndex(i, j)] = Cell.Road;
								}
							}
						}
					}));

					var route = start.GetShortestPath(
						goal,
						n => GetDistance(n.Value.X, n.Value.Y, start.Value.X, start.Value.Y),
						n => GetDistance(n.Value.X, n.Value.Y, goal.Value.X, goal.Value.Y));
					//var route = start.GetShortestPaths().FirstOrDefault(r => r.EndNode == goal);

					if(route != null){
						var i = 0;
						var context = new DispatcherSynchronizationContext(Deployment.Current.Dispatcher);
						foreach(var link in route.Links){
							if(this._WaitTime > 0){
								Thread.Sleep(this._WaitTime);
							}
							var xFrom = link.From.Value.X;
							var yFrom = link.From.Value.Y;
							var xTo = link.To.Value.X;
							var yTo = link.To.Value.Y;
							var xWall = (xFrom + xTo) >> 1;
							var yWall = (yFrom + yTo) >> 1;
							var callback = new SendOrPostCallback(delegate{
								App.Default.ProgressManager.ReportProgress(job, (double)i / (double)route.Links.Count);
								if(this._Board[this.GetIndex(xFrom, yFrom)] == Cell.Road){
									this._Board[this.GetIndex(xFrom, yFrom)] = Cell.Route;
								}
								if(this._Board[this.GetIndex(xTo, yTo)] == Cell.Road){
									this._Board[this.GetIndex(xTo, yTo)] = Cell.Route;
								}
								if(this._Board[this.GetIndex(xWall, yWall)] == Cell.Road){
									this._Board[this.GetIndex(xWall, yWall)] = Cell.Route;
								}
							});
							if(this._WaitTime > 0){
								context.Send(callback, null);
							}else{
								context.Post(callback, null);
							}
							i++;
						}
					}
				}finally{
					Deployment.Current.Dispatcher.BeginInvoke(new Action(delegate{
						this.IsBusy = false;
						 App.Default.ProgressManager.Complete(job);
					}));
				}
			}));
		}

		private static double GetDistance(int x1, int y1, int x2, int y2){
			return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
		}

		#endregion

		private Node<NodeInfo>[,] ParseBoard(){
			var x = this.Size.X;
			var y = this.Size.Y;
			var x_1 = x - 1;
			var y_1 = y - 1;
			var nodes = new Node<NodeInfo>[this.Size.X, this.Size.Y];
			for(var j = 0; j < y; j++){
				for(var i = 0; i < x; i++){
					if(this._Board[this.GetIndex(i, j)] != Cell.Wall){
						var node = nodes[i, j];
						if(i > 0 && this._Board[this.GetIndex(i - 1, j)] != Cell.Wall){
							node.AddLink(nodes[i - 1, j], 1);
						}
						if(j > 0 && this._Board[this.GetIndex(i, j - 1)] != Cell.Wall){
							node.AddLink(nodes[i, j - 1], 1);
						}
						if(i < x_1 && this._Board[this.GetIndex(i + 1, j)] != Cell.Wall){
							node.AddLink(nodes[i + 1, j], 1);
						}
						if(j < y_1 && this._Board[this.GetIndex(i, j + 1)] != Cell.Wall){
							node.AddLink(nodes[i + 1, j], 1);
						}
					}
				}
			}
			return nodes;
		}

		private class NodeInfo{
			public int X{get; private set;}
			public int Y{get; private set;}

			public NodeInfo(int x, int y){
				this.X = x; this.Y = y;
			}
		}
	}
}
