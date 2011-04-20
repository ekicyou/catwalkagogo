using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WPF.MDI {
	internal class MdiCanvas : Canvas{
		protected override Size MeasureOverride(Size constraint) {
			Size availableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);

			//double minLeft = 0;
			double maxRight = 0;
			//double minTop = 0;
			double maxBottom = 0;
			foreach(UIElement element in base.InternalChildren) {
				if(element != null) {
					element.Measure(availableSize);
					double left = Canvas.GetLeft(element);
					double top = Canvas.GetTop(element);
					double right = left + element.DesiredSize.Width;
					double bottom = top + element.DesiredSize.Height;
					/*
					if(left < minLeft){
						minLeft = left;
					}
					if(top < minTop){
						minTop = top;
					}
					*/
					if(maxRight < right){
						maxRight = right;
					}
					if(maxBottom < bottom){
						maxBottom = bottom;
					}
				}
			}

			return new Size(Math.Abs(0 - maxRight), Math.Abs(0 - maxBottom));
		}
		/*
		protected override Size ArrangeOverride(Size arrangeSize) {
			double minLeft = 0;
			double minTop = 0;
			foreach(UIElement element in base.InternalChildren) {
				if(element != null) {
					double left = Canvas.GetLeft(element);
					double top = Canvas.GetTop(element);
					if(left < minLeft){
						minLeft = left;
					}
					if(top < minTop){
						minTop = top;
					}
				}
			}
			foreach(UIElement element in base.InternalChildren) {
				double left = Canvas.GetLeft(element);
				double top = Canvas.GetRight(element);
				if(Double.IsNaN(left)){
					left = 0;
				}
				if(Double.IsNaN(top)){
					top = 0;
				}
				element.Arrange(new Rect(new Point(left - minLeft, top - minTop), element.DesiredSize));
			}
			return arrangeSize;
		}
		*/
	}
}
