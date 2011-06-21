using System;

namespace CatWalk{
#if SILVERLIGHT
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
	}
#endif
}
