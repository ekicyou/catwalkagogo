using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatWalk;
using CatWalk.Graph;
using System.Threading.Tasks;

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

			var rnd = new Random();
			var maze = GenerateMaze(option.X, option.Y, rnd.Next(option.X - 1), rnd.Next(option.Y - 1), method);
			DisplayMaze(maze);
		}

		private static void ShowUsase(){
			Console.WriteLine("usase: [x] [y] (k|p)");
		}

		private enum Method{
			Prim,
			Kruskal,
		}

		static bool[,] GenerateMaze(int x, int y, int startX, int startY, Method method){
			// Initialize
			var x2 = x / 2;
			var y2 = y / 2;
			var allNodes = new Node<NodeInfo>[x2, y2];
			for(var i = 0; i < x2; i++){
				for(var j = 0; j < y2; j++){
					allNodes[i, j] = new Node<NodeInfo>(new NodeInfo(i, j));
				}
			}

			var x2_1 = x2 - 1;
			var y2_1 = y2 - 1;
			var rnd = new Random();
			for(var i = 0; i < x2; i++){
				for(var j = 0; j < y2; j++){
					var node = allNodes[i, j];
					node.Value.IsRoad = true;
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

			var board = new bool[x, y];
			/*
			for(var i = 0; i < x2; i++){
				for(var j = 0; j < y2; j++){
					board[(i << 1) + 1, (j << 1) + 1] = true;
				}
			}
			*/
			var startX2 = startX >> 1;
			var startY2 = startY >> 1;
			var startNode = allNodes[startX2, startY2];
			var mst = (method == Method.Prim) ? startNode.GetMinimumSpanningTree() : startNode.TraverseNodesPostorder().GetMinimumSpanningTree();
			Console.Clear();
			//Parallel.ForEach(mst, (link) => {
			foreach(var link in mst){
				var xFrom = (link.From.Value.X << 1) + 1;
				var yFrom = (link.From.Value.Y << 1) + 1;
				var xTo = (link.To.Value.X << 1) + 1;
				var yTo = (link.To.Value.Y << 1) + 1;
				var xWall = (xFrom + xTo) >> 1;
				var yWall = (yFrom + yTo) >> 1;
				lock(board){
					board[xFrom, yFrom] = true;
					board[xTo, yTo] = true;
					board[xWall, yWall] = true;
					DisplayMaze(board);
				}
			}
			//});

			return board;
		}

		private class NodeInfo{
			public bool IsRoad{get; set;}
			public int X{get; private set;}
			public int Y{get; private set;}

			public NodeInfo(int x, int y){
				this.X = x; this.Y = y;
			}
		}

		static void DisplayMaze(bool[,] maze){
			Console.SetCursorPosition(0, 0);
			var x = maze.GetLength(0);
			var y = maze.GetLength(1);
			for(var i = 0; i < y; i++){
				for(var j = 0; j < x; j++){
					Console.Write((maze[j, i]) ? "□" : "■");
				}
				Console.Write("\n");
			}
		}
	}
}
