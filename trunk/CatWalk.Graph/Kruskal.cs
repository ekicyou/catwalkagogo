using System;
using System.Collections.Generic;
using CatWalk.Collections;
using System.Linq;

namespace CatWalk.Algorithms.Graph{
	public static class Kruskal{
		/*
		static void Main(string[] args){
			var nodes = Graph.ReadGraphFromFile<int>(args[0]);
			int i = 0;
			foreach(var node in nodes){
				node.Value = i;
				i++;
			}
			var t = DateTime.Now;
			foreach(var link in GetMinimumSpanningTree<int>(nodes)){
				Console.WriteLine("{0} to {1}, ", link.From.Value, link.To.Value);
			}
			Console.WriteLine("{0}", DateTime.Now - t);
		}
		*/
		public static IEnumerable<NodeLink<T>> GetMinimumSpanningTree<T>(Node<T>[] nodes){
			var heap = new Heap<NodeLink<T>>(new CustomComparer<NodeLink<T>>(delegate(NodeLink<T> x, NodeLink<T> y){
				return (x.Distance < y.Distance) ? -1 : (x.Distance < y.Distance) ? 1 : 0;
			}));
			foreach(var node in nodes){
				foreach(var link in node.Links){
					heap.Push(link);
				}
			}
			var forest = new HashSet<HashSet<Node<T>>>();
			
			int count = 0;
			int len = (nodes.Length - 1);
			while(count < len){
				var link = heap.Pop();
				var tree1 = forest.Where(tree => tree.Contains(link.From)).FirstOrDefault();
				var tree2 = forest.Where(tree => tree.Contains(link.To)).FirstOrDefault();
				if(tree1 != tree2){
					if(tree1 == null){
						yield return link; count++;
						tree2.Add(link.From);
					}else{
						if(tree2 == null){
							yield return link; count++;
							tree1.Add(link.To);
						}else{
							yield return link; count++;
							foreach(var node in tree2){
								tree1.Add(node);
							}
							forest.Remove(tree2);
						}
					}
				}else if((tree1 == null) && (tree2 == null)){
					yield return link; count++;
					var tree = new HashSet<Node<T>>();
					tree.Add(link.From);
					tree.Add(link.To);
					forest.Add(tree);
				}
			}
		}
	}
}