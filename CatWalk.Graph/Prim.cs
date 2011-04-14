using System;
using System.Collections.Generic;
using System.Linq;

namespace CatWalk.Algorithms.Graph{
	public static class Prim{
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
				Console.WriteLine("MST from {0}", node.Value);
				foreach(var link in GetMinimumSpanningTree(node)){
					Console.WriteLine("{0} to {1}", link.From.Value, link.To.Value);
				}
			}
			Console.WriteLine("{0}", DateTime.Now - t);
		}
		*/
		public static IEnumerable<NodeLink<T>> GetMinimumSpanningTree<T>(Node<T> root){

			var visited = new Dictionary<Node<T>, bool>();
			var current = root;
			var nexts = new Dictionary<Node<T>, NodeLink<T>>();

			visited.Add(current, true);
			foreach(var link in current.Links){
				nexts.Add(link.To, link);
				visited[link.To] = false;
			}

			while(!visited.All(pair => pair.Value)){ // 全て訪問済みなら終了
				// 隣接リンクから一番距離の短いものを選ぶ
				NodeLink<T> next = default(NodeLink<T>);
				int min = Int32.MaxValue;
				foreach(var link in nexts.Values){
					if(min > link.Distance){
						min = link.Distance;
						next = link;
					}
				}
				nexts.Remove(next.To);
				yield return next; // 全域木に追加
				visited[next.To] = true; // 訪問済みにする

				// 次の候補リンクをnextsへ追加
				foreach(var link in next.To.Links){
					// 新しく出現したノードをvisitedに追加
					if(!visited.ContainsKey(link.To)){
						visited.Add(link.To, false);
					}
					if(!visited[link.To]){
						NodeLink<T> link2;
						if(nexts.TryGetValue(link.To, out link2)){
							// 距離の短いほうを候補のリンクにする
							if(link.Distance < link2.Distance){
								nexts[link.To] = link;
							}
						}else{
							nexts.Add(link.To, link);
						}
					}
				}
			}
		}
	}
}