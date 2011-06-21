/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CatWalk;
using CatWalk.Collections;

namespace CatWalk.Graph {
	public static class AStar{
		public static Route<T> GetShortestPath<T>(this Node<T> start, Node<T> goal, Func<Node<T>, double> gstar, Func<Node<T>, double> hstar){
			var open = new Dictionary<Node<T>, Data<T>>();
			var close = new HashSet<Node<T>>();
			open.Add(start, new Data<T>(gstar(start) + hstar(start)));
			int openCount = 1;

			while(open.Count > 0){
				// Find least fs node data.
				var np = open.OrderBy(p => p.Value.Fs).First();
				var n = np.Key;
				var nd = np.Value;

				if(n == goal){
					var stack = new Stack<NodeLink<T>>();
					var data = nd;
					var distance = 0;
					while(data.ParentLink.From != null){
						distance += data.ParentLink.Distance;
						stack.Push(data.ParentLink);
						data = data.ParentData;
					}
					return new Route<T>(distance, stack.ToArray());
				}else{
					open.Remove(n);
					close.Add(n);
				}

				foreach(var link in n.Links){
					var m = link.To;
					if(close.Contains(m)){
						continue;
					}

					var fdm = gstar(n) + hstar(m) + link.Distance;
					Data<T> md;
					if(open.TryGetValue(m, out md)){
						var fsm = gstar(m) + hstar(m);
						if(fdm < fsm){
							md.ParentLink = link;
							md.ParentData = nd;
							md.Fs = fdm;
						}
					}else{
						open.Add(m, new Data<T>(fdm){ParentLink = link, ParentData=nd});
						openCount++;
					}
				}
			}
			return null;
		}

		private class Data<T>{
			public NodeLink<T> ParentLink{get; set;}
			public Data<T> ParentData{get; set;}
			public double Fs{get; set;}
			public Data(double fs){
				this.Fs = fs;
			}
		}
	}
}
