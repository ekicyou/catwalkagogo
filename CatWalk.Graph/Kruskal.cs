using System;
using System.Collections.Generic;
using CatWalk.Collections;
using System.Linq;

namespace CatWalk.Graph{
	public static class Kruskal{

		public static IEnumerable<NodeLink<T>> GetMinimumSpanningTree<T>(this IEnumerable<Node<T>> nodes){
			// 一番短いリンクを取り出すためのヒープ
			var heap = new Heap<NodeLink<T>>(new CustomComparer<NodeLink<T>>(delegate(NodeLink<T> x, NodeLink<T> y){
				return x.Distance.CompareTo(y.Distance);
			}));
			// 全域木の大きさは全ノード数 - 1
			int len = -1;
			foreach(var node in nodes){
				len++;
				foreach(var link in node.Links){
					heap.Push(link);
				}
			}
			// 部分木の集合
			var forest = new HashSet<HashSet<Node<T>>>();

			int count = 0;
			while(count < len){ // 全域木の大きさになるまで
				// 一番距離の短いリンクを取り出す
				var link = heap.Pop();
				// リンク元を含む木を取り出す
				var tree1 = forest.FirstOrDefault(tree => tree.Contains(link.From));
				// リンク先を含む木を取り出す
				var tree2 = forest.FirstOrDefault(tree => tree.Contains(link.To));
				if(tree1 != tree2){ // 異なる木同士のとき
					if(tree1 == null){ // リンク元の木が見つからなかったとき
						yield return link; count++;
						tree2.Add(link.From); // リンク先の木にリンク元のノードを含める
					}else{
						if(tree2 == null){ // リンク先の木が見つからなかったとき
							yield return link; count++;
							tree1.Add(link.To); // リンク元の木にリンク先のノードを含める
						}else{ // 両方見つかったとき
							yield return link; count++;
							// 2つの木を結合する
							foreach(var node in tree2){
								tree1.Add(node);
							}
							forest.Remove(tree2);
						}
					}
				}else if((tree1 == null) && (tree2 == null)){ // どちらも見つからなかったとき
					yield return link; count++;
					// 新しい木を作る。
					var tree = new HashSet<Node<T>>();
					tree.Add(link.From);
					tree.Add(link.To);
					forest.Add(tree);
				} // 同じ木同士の場合は何もしない
			}
		}
	}
}