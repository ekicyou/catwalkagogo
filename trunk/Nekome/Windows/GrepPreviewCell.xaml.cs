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
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Nekome.Search;
using System.Threading;

namespace Nekome.Windows {
	public partial class GrepPreviewCell : UserControl{
		public GrepPreviewCell(){
			InitializeComponent();
		}

		protected override void OnRender(DrawingContext drawingContext) {
			base.OnRender(drawingContext);
			if(this.ActualWidth > 0 && this.ActualHeight > 0){
			var formatedText = new FormattedText(
				this.Match.LineText,
				Thread.CurrentThread.CurrentUICulture,
				this.FlowDirection,
				new Typeface(this.FontFamily, this.FontStyle, this.FontWeight, this.FontStretch),
				this.FontSize,
				this.Foreground);
				if(this.VerticalContentAlignment == VerticalAlignment.Stretch){
					if(formatedText.Height != this.ActualHeight){
						this.Height = formatedText.Height + this.Margin.Top + this.Margin.Bottom;
					}
				}
				formatedText.MaxTextWidth = this.ActualWidth;
				formatedText.MaxTextHeight = this.ActualHeight;
				formatedText.SetForegroundBrush(SystemColors.HighlightBrush, (int)this.Match.Column, Math.Min(this.Match.Match.Length, this.Match.LineText.Length - (int)this.Match.Column));
				drawingContext.DrawText(formatedText, new Point(this.Margin.Left, this.Margin.Top));
			}
		}

		public static readonly DependencyProperty MatchProperty =
			DependencyProperty.Register("Match", typeof(GrepMatch), typeof(GrepPreviewCell));
		public GrepMatch Match{
			get{
				return (GrepMatch)this.GetValue(MatchProperty);
			}
			set{
				this.SetValue(MatchProperty, value);
			}
		}
	}
}
