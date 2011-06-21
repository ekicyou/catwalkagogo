/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk.Graph {
	public class Node<T>{
		private IList<NodeLink<T>> links;
		public IList<NodeLink<T>> Links{
			get{
				return this.links ?? (this.links = new List<NodeLink<T>>());
			}
		}
		public T Value{get; set;}

		public Node(){
		}

		public Node(IList<NodeLink<T>> links){
			this.links = links;
		}

		public Node(IList<NodeLink<T>> links, T value) : this(links){
			this.Value = value;
		}

		public Node(T value){
			this.Value = value;
		}

		public void AddLink(Node<T> to, int distance){
			this.Links.Add(new NodeLink<T>(this, to, distance));
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
}
