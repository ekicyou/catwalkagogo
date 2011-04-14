using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CatWalk.Algorithms.Graph{
	public class Node<T>{
		private List<NodeLink<T>> links;
		public IList<NodeLink<T>> Links{
			get{
				return this.links;
			}
		}
		public T Value{get; set;}

		public Node(){
			this.links = new List<NodeLink<T>>();
		}

		public Node(NodeLink<T>[] links){
			this.links = new List<NodeLink<T>>(links);
		}

		public Node(NodeLink<T>[] links, T value) : this(links){
			this.Value = value;
		}

		public Node(T value) : this(){
			this.Value = value;
		}

		public void AddLink(Node<T> to, int dist){
			this.links.Add(new NodeLink<T>(this, to, dist));
		}
	}

	public struct NodeLink<T>{
		public Node<T> From{get; private set;}
		public Node<T> To{get; private set;}
		public int Distance{get; private set;}

		internal NodeLink(Node<T> from, Node<T> to, int distance) : this(){
			this.From = from;
			this.To = to;
			this.Distance = distance;
		}
	}

	public static class Graph{
		public static Node<T>[] ReadGraphFromFile<T>(string file){
			var lines = File.ReadAllLines(file, Encoding.UTF8);
			var n = Int32.Parse(lines[0]);
			Node<T>[] nodes = new Node<T>[n];
			for(int i = 0; i < n; i++){
				var elms = lines[i + 1].Split(new char[]{' '}, n, StringSplitOptions.RemoveEmptyEntries);
				for(int j = 0; j < elms.Length; j++){
					try{
					int d = Int32.Parse(elms[j]);
					if(nodes[i] == null){
						nodes[i] = new Node<T>();
					}
					if(nodes[j] == null){
						nodes[j] = new Node<T>();
					}
					if(d != 0){
						nodes[i].AddLink(nodes[j], d);
					}
					}catch{
						Console.WriteLine("{0}", elms[j]);
					}
				}
			}
			return nodes.Where(node => node != null).ToArray();
		}

		public static int[,] ReadMatrixFromFile<T>(string file){
			var lines = File.ReadAllLines(file, Encoding.UTF8);
			var n = Int32.Parse(lines[0]);
			int[,] matrix = new int[n,n];
			for(int i = 0; i < n; i++){
				var elms = lines[i + 1].Split(new char[]{' ', '\t'}, n, StringSplitOptions.RemoveEmptyEntries);
				for(int j = 0; j < elms.Length; j++){
					int d = Int32.Parse(elms[j]);
					matrix[i,j] = d;
				}
			}
			return matrix;
		}

		public static int[,] GetGraphMatrix<T>(Node<T> root){
			var trMap = new Dictionary<Tuple<int, int>, int>();
			var nodeDic = new Dictionary<Node<T>, int>();

			Walk(root, new Action<NodeLink<T>>(delegate(NodeLink<T> link){
				int a, b;
				if(!nodeDic.TryGetValue(link.From, out a)){
					a = nodeDic.Count;
					nodeDic.Add(link.From, a);
				}
				if(!nodeDic.TryGetValue(link.To, out b)){
					b = nodeDic.Count;
					nodeDic.Add(link.To, b);
				}
				trMap.Add(new Tuple<int, int>(a, b), link.Distance);
			}));

			return MakeGraphMatrix(trMap, nodeDic.Count);
		}

		public static int[,] GetGraphMatrix<T>(Node<T>[] nodes){
			var trMap = new Dictionary<Tuple<int, int>, int>();
			var nodeDic = new Dictionary<Node<T>, int>();

			foreach(var node in nodes){
				nodeDic.Add(node, nodeDic.Count);
			}
			foreach(var node in nodes){
				foreach(var link in node.Links){
					int a, b;
					if(!nodeDic.TryGetValue(node, out a)){
						a = nodeDic.Count;
						nodeDic.Add(node, a);
					}
					if(!nodeDic.TryGetValue(link.To, out b)){
						b = nodeDic.Count;
						nodeDic.Add(link.To, b);
					}
					trMap.Add(new Tuple<int, int>(a, b), link.Distance);
				}
			}
			return MakeGraphMatrix(trMap, nodeDic.Count);
		}

		private static int[,] MakeGraphMatrix(IDictionary<Tuple<int, int>, int> trMap, int n){
			var matrix = new int[n, n];
			for(int i = 0; i < n; i++){
				for(int j = 0; j < n; j++){
					int d;
					if(trMap.TryGetValue(new Tuple<int, int>(i, j), out d)){
						matrix[i, j] = d;
					}else{
						matrix[i, j] = Int32.MaxValue;
					}
				}
			}
			return matrix;
		}

		/// <summary>
		/// Walk all node links ignoring duplicates
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="node"></param>
		/// <param name="action"></param>
		public static void Walk<T>(this Node<T> node, Action<NodeLink<T>> action){
			var visited = new HashSet<NodeLink<T>>();
			Walk(node, action, visited);
		}
		private static void Walk<T>(this Node<T> node, Action<NodeLink<T>> action, HashSet<NodeLink<T>> visited){
			foreach(var link in node.Links){
				if(!visited.Contains(link)){
					visited.Add(link);
					action(link);
					if(link.To.Links.Count > 0){
						Walk(link.To, action, visited);
					}
				}
			}
		}
	}
}