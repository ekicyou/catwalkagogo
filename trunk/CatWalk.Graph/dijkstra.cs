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
			return GetShortestPath<T>(root, root.TraverseNodesPreorder());
		}
		public static IEnumerable<Route<T>> GetShortestPath<T>(Node<T> start, IEnumerable<Node<T>> nodes){
			var allNodes = new HashSet<Node<T>>(nodes);
			var routes = new Dictionary<Node<T>, WorkingRoute<T>>();

			routes[start] = new WorkingRoute<T>(0);
			foreach(var node in allNodes.Where(v => v != start)){
				routes[node] = new WorkingRoute<T>(Int32.MaxValue);
			}

			// ‘‚Ä–K–âÏ‚İ‚É‚È‚é‚Ü‚Å
			while(allNodes.Count() > 0){
				// –¢–K–â‚Å‹——£‚ªÅ¬‚Ìƒm[ƒh‚ğŒŸõ
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

				// –K–âÏ‚İ‚É‚·‚é
				allNodes.Remove(u);
				// ƒm[ƒhu‚©‚ç‚ÌƒŠƒ“ƒN‚Ådistances‚æ‚è‹——£‚Ì‹ß‚¢•¨‚ğ“o˜^
				var distU = routes[u];
				foreach(var link in u.Links){
					if(link.Distance < 0){
						throw new NegativeDistanceException();
					}
					// ƒŠƒ“ƒNæ‚Ìƒm[ƒh‚Ìƒ‹[ƒg‚Ì‘‹——£‚æ‚èAu‚Ü‚Å‚Ì‹——£‚Æu‚©‚ç‚Ì‹——£‚Ì˜a‚Ì•û‚ª’Z‚¢‚Æ‚«
					var distTo = routes[link.To];
					var dist = distU.TotalDistance + link.Distance;
					if(distTo.TotalDistance > dist){
						// Œo˜H‚ğXV
						distTo.TotalDistance = dist;
						distTo.Links.Clear();
						distTo.Links.AddRange(distU.Links.Concat(Seq.Make(link)));
					}
				}
			}
		}
	}

	internal class WorkingRoute<T>{
		public int TotalDistance{get; set;}
		public List<NodeLink<T>> Links{get; private set;}

		public WorkingRoute(int distance){
			this.TotalDistance = distance;
			this.Links = new List<NodeLink<T>>();
		}

		public WorkingRoute(int distance, IEnumerable<NodeLink<T>> links){
			this.TotalDistance = distance;
			this.Links = new List<NodeLink<T>>(links);
		}
	}

	public struct Route<T>{
		public int TotalDistance{get; private set;}
		public ReadOnlyCollection<NodeLink<T>> Links{get; private set;}

		public Route(int distance, IList<NodeLink<T>> links) : this(){
			this.TotalDistance = distance;
			this.Links = new ReadOnlyCollection<NodeLink<T>>(links);
		}

		public Node<T> StartNode{
			get{
				if(this.Links.Count == 0){
					return null;
				}else{
					return this.Links[0].From;
				}
			}
		}

		public Node<T> EndNode{
			get{
				if(this.Links.Count == 0){
					return null;
				}else{
					return this.Links[this.Links.Count - 1].To;
				}
			}
		}

		public IEnumerable<Node<T>> Nodes{
			get{
				if(this.Links.Count == 0){
					yield break;
				}else{
					yield return this.Links[0].From;
					foreach(var link in this.Links){
						yield return link.To;
					}
				}
			}
		}

		public IEnumerable<Node<T>> InterNodes{
			get{
				return this.Links.Skip(1).Select(link => link.From);
			}
		}
	}

	public class NegativeDistanceException : Exception{
		public NegativeDistanceException(){}
		public NegativeDistanceException(string message) : base(message){}
		public NegativeDistanceException(string message, Exception ex) : base(message, ex){}
	}
}