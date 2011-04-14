using System;
using System.Collections.Generic;
using System.Linq;

namespace CatWalk.Algorithms.Graph{
	public static class Dijkstra{
		/*
		static void Main(string[] args){
			var nodes = Graph.ReadGraphFromFile<int>(args[0]);
			int i = 0;
			foreach(var node in nodes){
				node.Value = i;
				i++;
			}
			var t = DateTime.Now;
			foreach(var node in nodes){
				Console.WriteLine("SP from {0}", node.Value);
				foreach(var tup in GetShortestPath(node).Skip(1)){
					Console.WriteLine("{0} to {1}", tup.Item2.Value, tup.Item1.Value);
				}
			}
			Console.WriteLine("{0}", DateTime.Now - t);
		}
		*/
		public static IEnumerable<Tuple<Node<T>, Node<T>>> GetShortestPath<T>(Node<T> root){
			var allNodes = new HashSet<Node<T>>();
			var visitedNodes = new HashSet<Node<T>>();
			var distances = new Dictionary<Node<T>, int>();
			var w = new Dictionary<Node<T>, Node<T>>();

			allNodes.Add(root);
			root.Walk(new Action<NodeLink<T>>(delegate(NodeLink<T> link){
				if(!allNodes.Contains(link.To)){
					allNodes.Add(link.To);
				}
			}));
			distances[root] = 0;
			foreach(var node in allNodes.Where(v => v != root)){
				distances[node] = Int32.MaxValue;
			}
			foreach(var node in allNodes){
				w[node] = null;
			}

			// ëçÇƒñKñ‚çœÇ›Ç…Ç»ÇÈÇ‹Ç≈
			while(allNodes.Except(visitedNodes).Count() > 0){
				// ñ¢ñKñ‚Ç≈ãóó£Ç™ç≈è¨ÇÃÉmÅ[ÉhÇåüçı
				Node<T> u = null;
				int min = Int32.MaxValue;
				foreach(var pair in distances){
					if(!visitedNodes.Contains(pair.Key) && pair.Value < min){
						min = pair.Value;
						u = pair.Key;
					}
				}
				yield return new Tuple<Node<T>, Node<T>>(u, w[u]);

				// ñKñ‚çœÇ›Ç…Ç∑ÇÈ
				visitedNodes.Add(u);
				foreach(var link in u.Links){
					if(distances[link.To] > (distances[u] + link.Distance)){
						distances[link.To] = distances[u] + link.Distance;
						w[link.To] = u;
					}
				}
			}
		}
	}
}