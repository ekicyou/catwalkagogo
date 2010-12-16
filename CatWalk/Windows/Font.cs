/*
	$Id$
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Markup;

namespace CatWalk.Windows {
	//using WinForms = System.Windows.Forms;
	//using Drawing = System.Drawing;

	[Serializable]
	public struct Font{
		public string FamilyName{get; set;}
		public double Size{get; set;}
		public string StyleName{get; set;}
		public string WeightName{get; set;}

		[XmlIgnore]
		public FontFamily Family{
			get{
				if(!this.FamilyName.IsNullOrEmpty()){
					var conv = new FontFamilyConverter();
					return (FontFamily)conv.ConvertFromString(this.FamilyName);
				}else{
					return null;
				}
			}
			set{
				var conv = new FontFamilyConverter();
				this.FamilyName = conv.ConvertToString(value);
			}
		}

		[XmlIgnore]
		public FontStyle Style{
			get{
				if(!this.StyleName.IsNullOrEmpty()){
					var conv = new FontStyleConverter();
					return (FontStyle)conv.ConvertFromString(this.StyleName);
				}else{
					return FontStyles.Normal;
				}
			}
			set{
				var conv = new FontStyleConverter();
				this.StyleName = conv.ConvertToString(value);
			}
		}

		[XmlIgnore]
		public FontWeight Weight{
			get{
				if(!this.WeightName.IsNullOrEmpty()){
					var conv = new FontWeightConverter();
					return (FontWeight)conv.ConvertFromString(this.WeightName);
				}else{
					return FontWeights.Normal;
				}
			}
			set{
				var conv = new FontWeightConverter();
				this.WeightName = conv.ConvertToString(value);
			}
		}

		public Font(string familyName, double size, string styleName, string weightName) : this(){
			this.FamilyName = familyName;
			this.Size = size;
			this.StyleName = styleName;
			this.WeightName = weightName;
		}

		public Font(FontFamily family, double size, FontStyle style, FontWeight weight) : this(){
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
