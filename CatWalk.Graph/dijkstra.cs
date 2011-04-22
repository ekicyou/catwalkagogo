using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CatWalk.Collections;

namespace CatWalk.Graph{
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
		
		public static IEnumerable<Route<T>> GetShortestPath<T>(this Node<T> root){
			var allNodes = new HashSet<Node<T>>(root.TraverseNodesPreorder());
			var routes = new Dictionary<Node<T>, WorkingRoute<T>>();

			routes[root] = new WorkingRoute<T>(0);
			foreach(var node in allNodes.Where(v => v != root)){
				routes[node] = new WorkingRoute<T>(Int32.MaxValue);
			}

			// 総て訪問済みになるまで
			while(allNodes.Count() > 0){
				// 未訪問で距離が最小のノードを検索
				Node<T> u = null;
				var min = new WorkingRoute<T>(Int32.MaxValue);
				foreach(var pair in routes){
					var node = pair.Key;
					var route = pair.Value;
					if(allNodes.Contains(node) && route.TotalDistance < min.TotalDistance){
						min = route;
						u = node;
					}
				}
				if(min.Links.Count > 0){
					yield return new Route<T>(min.TotalDistance, min.Links);
				}

				// 訪問済みにする
				allNodes.Remove(u);
				// ノードuからのリンクでdistancesより距離の近い物を登録
				var distU = routes[u];
				foreach(var link in u.Links){
					// リンク先のノードのルートの総距離より、uまでの距離とuからの距離の和の方が短いとき
					var distTo = routes[link.To];
					if(distTo.TotalDistance > (distU.TotalDistance + link.Distance)){
						// 経路を更新
						distTo.TotalDistance = distU.TotalDistance + link.Distance;
						distTo.Links.Clear();
						distTo.Links.AddRange(distU.Links.Concat(Seq.Make(link)));
					}
				}
			}
		}

		private class WorkingRoute<T>{
			public int TotalDistance{get; set;}
			public List<NodeLink<T>> Links{get; private set;}

			public WorkingRoute(int distance){
				this.TotalDistance = distance;
				this.Links = new List<NodeLink<T>>();
			}
		}
	}

	public struct Route<T>{
		public int TotalDistance{get; private set;}
		public ReadOnlyCollection<NodeLink<T>> Links{get; private set;}

		public Route(int distance, IList<NodeLink<T>> links) : this(){
			this.TotalDistance = distance;
			this.Links = new ReadOnlyCollection<NodeLink<T>>(links);
		}
	}
}