/*
	$Id$
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Markup;

namespace CatWalk.Windows {
	//using WinForms = System.Windows.Forms;
	//using Drawing = System.Drawing;

	public class Font{
		public FontFamily Family{get; set;}
		public double Size{get; set;}
		public FontStyle Style{get; set;}
		public FontWeight Weight{get; set;}

		public Font(FontFamily family, double size) : this(family, size, FontStyles.Normal, FontWeights.Regular){}
		public Font(FontFamily family, double size, FontStyle style) : this(family, size, style, FontWeights.Regular){}
		public Font(FontFamily family, double size, FontStyle style, FontWeight weight){
			this.Family = family;
			this.Size = size;
			this.Style = style;
			this.Weight = weight;
		}
		/*
		public static Font FromGdiFont(Drawing.Font font){
			return new Font(
				new FontFamily(font.FontFamily.Name),
				(double)font.Size,
				(font.Style == Drawing.FontStyle.Italic) ? FontStyles.Italic : FontStyles.Normal,
				font.Bold ? FontWeights.Bold : FontWeights.Normal);
		}
		*/
	}
}
