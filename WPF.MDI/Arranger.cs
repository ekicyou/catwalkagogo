﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WPF.MDI {
	public abstract class Arranger{
		public abstract IEnumerable<Rect> Arrange(Size containerSize, int count);
	}

	public class CascadeArranger : Arranger{
		public double WindowOffset{get; set;}

		public CascadeArranger() : this(25d){}
		public CascadeArranger(double windowOffset){
			this.WindowOffset = windowOffset;
		}
	
		public override IEnumerable<Rect> Arrange(Size containerSize, int count){
			double newWidth = containerSize.Width * 0.58, // should be non-linear formula here
				newHeight = containerSize.Height * 0.67,
				windowOffset = 0;
			for(var i = 0; i < count; i++){
				yield return new Rect(windowOffset, windowOffset, newWidth, newHeight);

				windowOffset += this.WindowOffset;
				if (windowOffset + newWidth > containerSize.Width)
					windowOffset = 0;
				if (windowOffset + newHeight > containerSize.Height)
					windowOffset = 0;
			}
		}
	}

	public class TileHorizontalArranger : Arranger{
		public override IEnumerable<Rect> Arrange(Size containerSize, int count){
			int rows = (int)Math.Sqrt(count),
				cols = count / rows;

			List<int> col_count = new List<int>(); // windows per column
			for (int i = 0; i < cols; i++)
			{
				if (count % cols > cols - i - 1)
					col_count.Add(rows + 1);
				else
					col_count.Add(rows);
			}

			double newWidth = containerSize.Width / cols,
				newHeight = containerSize.Height / col_count[0],
				offsetTop = 0,
				offsetLeft = 0;

			for (int i = 0, col_index = 0, prev_count = 0; i < count; i++)
			{
				if (i >= prev_count + col_count[col_index])
				{
					prev_count += col_count[col_index++];
					offsetLeft += newWidth;
					offsetTop = 0;
					newHeight = containerSize.Height / col_count[col_index];
				}

				yield return new Rect(offsetLeft, offsetTop, newWidth, newHeight);
				offsetTop += newHeight;
			}
		}
	}

	public class TileVerticalArranger : Arranger{
		public override IEnumerable<Rect> Arrange(Size containerSize, int count){
			int cols = (int)Math.Sqrt(count),
				rows = count / cols;

			List<int> col_count = new List<int>(); // windows per column
			for (int i = 0; i < cols; i++)
			{
				if (count % cols > cols - i - 1)
					col_count.Add(rows + 1);
				else
					col_count.Add(rows);
			}

			double newWidth = containerSize.Width / cols,
				newHeight = containerSize.Height / col_count[0],
				offsetTop = 0,
				offsetLeft = 0;

			for (int i = 0, col_index = 0, prev_count = 0; i < count; i++)
			{
				if (i >= prev_count + col_count[col_index])
				{
					prev_count += col_count[col_index++];
					offsetLeft += newWidth;
					offsetTop = 0;
					newHeight = containerSize.Height / col_count[col_index];
				}

				yield return new Rect(offsetLeft, offsetTop, newWidth, newHeight);
				offsetTop += newHeight;
			}
		}
	}

	public class StackVerticalArranger : Arranger{
		public override IEnumerable<Rect> Arrange(Size containerSize, int count) {
			double newWidth = containerSize.Width;
			double newHeight = containerSize.Height / count;
			double offsetTop = 0;
			double offsetLeft = 0;
			for(int i = 0; i < count; i++){
				yield return new Rect(offsetLeft, offsetTop, newWidth, newHeight);
				offsetTop += newHeight;
			}
		}
	}

	public class StackHorizontalArranger : Arranger{
		public override IEnumerable<Rect> Arrange(Size containerSize, int count) {
			double newWidth = containerSize.Width / count;
			double newHeight = containerSize.Height;
			double offsetTop = 0;
			double offsetLeft = 0;
			for(int i = 0; i < count; i++){
				yield return new Rect(offsetLeft, offsetTop, newWidth, newHeight);
				offsetLeft += newWidth;
			}
		}
	}
}
