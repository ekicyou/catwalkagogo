using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CatWalk.SLGameLib {
	public struct Int32Vector : IEquatable<Int32Vector>{
		public int X{get; private set;}
		public int Y{get; private set;}

		public Int32Vector(int x, int y) : this(){
			this.X = x;
			this.Y = y;
		}

		public bool Equals(Int32Vector point){
			return this.X.Equals(point.X) && this.Y.Equals(point.Y);
		}

		public override bool Equals(object obj) {
			if(obj == null){
				return false;
			}else if(obj is Int32Vector){
				return this.Equals((Int32Vector)obj);
			}else{
				return false;
			}
		}

		public static bool operator==(Int32Vector a, Int32Vector b){
			return a.Equals(b);
		}

		public static bool operator!=(Int32Vector a, Int32Vector b){
			return !a.Equals(b);
		}

		public override int GetHashCode() {
			return this.X.GetHashCode() ^ this.Y.GetHashCode();
		}
	}
}
