using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.Graph {
	public static class Traverser {
		#region Depth First

		public static IEnumerable<Node<T>> TraverseNodesDepthFirst<T>(this Node<T> root){
			var visited = new HashSet<Node<T>>();
			var stack = new Stack<Node<T>>();
			stack.Push(root);

			yield return root;
			visited.Add(root);

			while(stack.Count > 0){
			mainLoop:
				var node = stack.Peek();
				foreach(var link in node.Links.Where(link => !visited.Contains(link.To))){
					stack.Push(link.To);
					visited.Add(link.To);
					yield return link.To;
					goto mainLoop;
				}
				stack.Pop();
			}
		}

		public static IEnumerable<NodeLink<T>> TraverseLinksDepthFirst<T>(this Node<T> root){
			var visited = new HashSet<NodeLink<T>>();
			var stack = new Stack<Node<T>>();
			stack.Push(root);

			while(stack.Count > 0){
			mainLoop:
				var node = stack.Peek();
				foreach(var link in node.Links.Where(link => !visited.Contains(link))){
					stack.Push(link.To);
					visited.Add(link);
					yield return link;
					goto mainLoop;
				}
				stack.Pop();
			}
		}

		#endregion

		#region Preorder

		public static IEnumerable<Node<T>> TraverseNodesPreorder<T>(this Node<T> root){
			var visited = new HashSet<Node<T>>();
			var collection = new Queue<Node<T>>();
			collection.Enqueue(root);
			while(collection.Count > 0){
				var node = collection.Dequeue();
				yield return node;
				visited.Add(node);
				foreach(var link in node.Links.Where(link => !visited.Contains(link.To))){
					collection.Enqueue(link.To);
				}
			}
		}

		public static IEnumerable<NodeLink<T>> TraverseLinksPreorder<T>(this Node<T> root){
			var visited = new HashSet<NodeLink<T>>();
			var collection = new Queue<Node<T>>();
			collection.Enqueue(root);
			while(collection.Count > 0){
				var node = collection.Dequeue();
				foreach(var link in node.Links.Where(link => !visited.Contains(link))){
					yield return link;
					visited.Add(link);
					collection.Enqueue(link.To);
				}
			}
		}

		#endregion

		#region Postoreder

		public static IEnumerable<Node<T>> TraverseNodesPostorder<T>(this Node<T> root){
			var visited = new HashSet<Node<T>>();
			var stack = new Stack<Node<T>>();
			stack.Push(root);
			while(stack.Count > 0){
			mainLoop:
				var node = stack.Peek();
				visited.Add(node);
				foreach(var link in node.Links.Where(link => !visited.Contains(link.To))){
					stack.Push(link.To);
					goto mainLoop;
				}
				stack.Pop();
				yield return node;
			}
		}
		#endregion

		#region Walk

		public static void WalkParallel<T>(this Node<T> node, Action<Node<T>> action){
			action.ThrowIfNull("action");
			var visited = new HashSet<Node<T>>();
			WalkParallel(node, action, visited);
		}

		private static void WalkParallel<T>(Node<T> node, Action<Node<T>> action, HashSet<Node<T>> visited){
			IEnumerable<Action> tasks = null;
			lock(visited){
				visited.Add(node);
				tasks = node.Links.Where(link => !visited.Contains(link.To)).Select(link => {
					var node2 = link.To;
					return new Action(() => WalkParallel(node2, action, visited));
				});
			}
			Parallel.Invoke(tasks.Concat(Seq.Make(new Action(() => action(node)))).ToArray());
		}

		#endregion
	}
}
