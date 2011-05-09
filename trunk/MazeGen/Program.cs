using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatWalk;
using CatWalk.Graph;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MazeGen {
	static class Program {
		private class CommandLineOption{
			public string Method{get; set;}
			[CommandLineParemeterOrder(0)]
			public int X{get; set;}
			[CommandLineParemeterOrder(1)]
			public int Y{get; set;}
		}

		static void Main(string[] args) {
			var option = new CommandLineOption();
			CommandLineParser.Parse(option, args, StringComparer.OrdinalIgnoreCase);
			if(option.X < 2 || option.Y < 2){
				ShowUsase();
				return;
			}
			var method = (option.Method != null && option.Method.StartsWith("k", StringComparison.OrdinalIgnoreCase)) ? Method.Kruskal : Method.Prim;

			//Console.Clear();

			var sw = new Stopwatch();
			sw.Start();
			Console.WriteLine("Generating Maze...");

			var rnd = new Random();
			var startX = rnd.Next(option.X - 1);
			var startY = rnd.Next(option.Y - 1);
			var t = GenerateMaze(option.X, option.Y, startX, startY, method);
			var maze = t.Item1;
			var board = t.Item2;
			//DisplayMaze(board);

			sw.Stop();
			Console.WriteLine("Finished {0} ms." + sw.ElapsedMilliseconds);
			sw.Restart();
			Console.WriteLine("Solving Maze...");

			//var start = maze[rnd.Next(option.X - 1) / 2, rnd.Next(option.Y - 1) / 2];
			//var goal = maze[rnd.Next(option.X - 1) / 2, rnd.Next(option.Y - 1) / 2];
			var start = maze[0, 0];
			var goal = maze[(option.X - 1) / 2 - 1, (option.Y - 1) / 2 - 1];
			board[start.Value.X, start.Value.Y] = '☆';
			board[goal.Value.X, goal.Value.Y] = '★';
			//DisplayMaze(board);
			var route = start.GetShortestPath(
				goal,
				n => GetDistance(n.Value.X, n.Value.Y, start.Value.X, start.Value.Y),
				n => GetDistance(n.Value.X, n.Value.Y, goal.Value.X, goal.Value.Y));
			//var route = start.GetShortestPaths().FirstOrDefault(r => r.EndNode == goal);

			if(route != null){
				foreach(var link in route.Links){
					var xFrom = link.From.Value.X;
					var yFrom = link.From.Value.Y;
					var xTo = link.To.Value.X;
					var yTo = link.To.Value.Y;
					var xWall = (xFrom + xTo) >> 1;
					var yWall = (yFrom + yTo) >> 1;
					board[xFrom, yFrom] = '＊';
					board[xTo, yTo] = '＊';
					board[xWall, yWall] = '＊';
				}
			}
			sw.Stop();
			Console.WriteLine("Finished {0} ms." + sw.ElapsedMilliseconds);
			//DisplayMaze(board);
			WriteBoard(board);
		}

		private static double GetDistance(int x1, int y1, int x2, int y2){
			return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
		}

		private static void ShowUsase(){
			Console.WriteLine("usase: [x] [y] (k|p)");
		}

		private enum Method{
			Prim,
			Kruskal,
		}

		static Tuple<Node<NodeInfo>[, ], char[, ]> GenerateMaze(int x, int y, int startX, int startY, Method method){
			// Initialize
			var x2 = x / 2;
			var y2 = y / 2;
			var allNodes = new Node<NodeInfo>[x2, y2];
			for(var i = 0; i < x2; i++){
				for(var j = 0; j < y2; j++){
					allNodes[i, j] = new Node<NodeInfo>(new NodeInfo((i << 1) + 1, (j << 1) + 1));
				}
			}

			var x2_1 = x2 - 1;
			var y2_1 = y2 - 1;
			var rnd = new Random();
			for(var i = 0; i < x2; i++){
				for(var j = 0; j < y2; j++){
					var node = allNodes[i, j];
					if(i > 0){
						node.AddLink(allNodes[i - 1, j], rnd.Next());
					}
					if(i < x2_1){
						node.AddLink(allNodes[i + 1, j], rnd.Next());
					}
					if(j > 0){
						node.AddLink(allNodes[i, j - 1], rnd.Next());
					}
					if(j < y2_1){
						node.AddLink(allNodes[i, j + 1], rnd.Next());
					}
				}
			}

			var board = new char[x, y];
			for(var i = 0; i < x; i++){
				for(var j = 0; j < y; j++){
					board[i, j] = '■';
				}
			}
			var startX2 = startX >> 1;
			var startY2 = startY >> 1;
			var startNode = allNodes[startX2, startY2];
			var mst = ((method == Method.Prim) ? startNode.GetMinimumSpanningTree() : startNode.TraverseNodesPostorder().GetMinimumSpanningTree()).ToArray();
			foreach(var node in allNodes){
				node.Links.Clear();
			}
			foreach(var link in mst){
				link.From.AddLink(link.To, 1);
				link.To.AddLink(link.From, 1);
				var xFrom = link.From.Value.X;
				var yFrom = link.From.Value.Y;
				var xTo = link.To.Value.X;
				var yTo = link.To.Value.Y;
				var xWall = (xFrom + xTo) >> 1;
				var yWall = (yFrom + yTo) >> 1;
				board[xFrom, yFrom] = '□';
				board[xTo, yTo] = '□';
				board[xWall, yWall] = '□';
			}

			return new Tuple<Node<NodeInfo>[, ], char[, ]>(allNodes, board);
		}

		private class NodeInfo{
			public int X{get; private set;}
			public int Y{get; private set;}

			public NodeInfo(int x, int y){
				this.X = x; this.Y = y;
			}
		}

		static void DisplayMaze(char[,] maze){
			Console.SetCursorPosition(0, 0);
			WriteBoard(maze);
		}

		static void WriteBoard(char[, ] maze){
			var x = maze.GetLength(0);
			var y = maze.GetLength(1);
			for(var i = 0; i < y; i++){
				for(var j = 0; j < x; j++){
					Console.Write(maze[j, i]);
				}
				Console.Write("\n");
			}
		}
	}
}
