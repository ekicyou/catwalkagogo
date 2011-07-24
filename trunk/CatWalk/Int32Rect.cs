/*
	$Id$
*/
using System;

namespace CatWalk{
//#if SILVERLIGHT
	public struct Int32Rect : IEquatable<Int32Rect>{
		public int X{get; private set;}
		public int Y{get; private set;}
		public int Width{get; private set;}
		public int Height{get; private set;}
		public int Left{get{return this.X;}}
		public int Top{get{return this.Y;}}
		public int Right{get{return this.X + this.Width;}}
		public int Bottom{get{return this.Y + this.Height;}}

		public Int32Rect(int x, int y, int width, int height) : this(){
			if(width < 0){
				throw new ArgumentOutOfRangeException("width");
			}
			if(height < 0){
				throw new ArgumentOutOfRangeException("height");
			}
			this.X = x;
			this.Y = y;
			this.Width = width;
			this.Height = height;
		}

		public bool Equals(Int32Rect rect){
			return this.X.Equals(rect.X) && this.Y.Equals(rect.Y) && this.Width.Equals(rect.Width) && this.Height.Equals(this.Height);
		}

		public override bool Equals(object obj) {
			if(obj == null){
				return false;
			}else if(obj is Int32Rect){
				return this.Equals((Int32Rect)obj);
			}else{
				return false;
			}
		}

		public static bool operator==(Int32Rect a, Int32Rect b){
			return a.Equals(b);
		}

		public static bool operator!=(Int32Rect a, Int32Rect b){
			return !a.Equals(b);
		}

		public override int GetHashCode() {
			return this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.Width.GetHashCode() ^ this.Height.GetHashCode();
		}

		public bool Contains(Int32Point point){
			return this.Left <= point.X && point.X < this.Right && this.Top <= point.Y && point.Y < this.Bottom;
		}
		
		public bool IsIntersect(Int32Rect rect){
			return !(this.Left > rect.Right || this.Right < rect.Left || this.Top > rect.Bottom || this.Bottom < rect.Top);
		}

		public Int32Rect Intersect(Int32Rect rect){
			if(this.Left > rect.Right || this.Right < rect.Left || this.Top > rect.Bottom || this.Bottom < rect.Top){
				return Empty;
			}else{
				var left = Math.Max(this.Left, rect.Left);
				var top = Math.Max(this.Top, rect.Top);
				var right = Math.Min(this.Right, rect.Right);
				var bottom = Math.Min(this.Bottom, rect.Bottom);
				var rect2 = new Int32Rect(left, top, bottom - top, right -left);
				return rect2;
			}
		}

		public long Area{
			get{
				return this.Width * this.Height;
			}
		}

		public static Int32Rect operator +(Int32Rect a, Int32Vector v){
			return new Int32Rect(a.Left + v.X, a.Top + v.Y, a.Width, a.Height);
		}

		public static Int32Rect operator +(Int32Vector v, Int32Rect a){
			return new Int32Rect(a.Left + v.X, a.Top + v.Y, a.Width, a.Height);
		}
	
		public static readonly Int32Rect Empty = new Int32Rect();
	}
//#endif
}
