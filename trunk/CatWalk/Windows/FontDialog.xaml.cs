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
using System.Windows.Markup;
using System.Threading;

namespace CatWalk.Windows {
	public partial class FontDialog : Window{
		public FontDialog() {
			InitializeComponent();

			this.sizeListBox.ItemsSource = new double[]{6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,28,32,48,64};
			this.weightListBox.ItemsSource = new FontWeight[]{
				FontWeights.ExtraLight, FontWeights.Light, FontWeights.Normal, FontWeights.Medium, FontWeights.DemiBold,
				FontWeights.Bold, FontWeights.ExtraBold, FontWeights.Black, FontWeights.ExtraBlack
			};
			this.styleListBox.ItemsSource = new FontStyle[]{FontStyles.Normal, FontStyles.Italic, FontStyles.Oblique};

			var lang = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentUICulture.IetfLanguageTag);
			this.fontListBox.ItemsSource = Fonts.SystemFontFamilies
				.Where(font => font.FamilyNames.Count > 0)
				.Select(font => new KeyValuePair<string, FontFamily>(
					(font.FamilyNames.ContainsKey(lang)) ? font.FamilyNames[lang] : font.FamilyNames.Values.First(),
					font))
				.OrderBy(pair => pair.Key)
				.ToArray();
		}

		#region プロパティ

		public readonly DependencyProperty SelectedFontFamilyProperty = DependencyProperty.Register("SelectedFontFamily", typeof(FontFamily), typeof(FontDialog));
		public FontFamily SelectedFontFamily{
			get{
				return (FontFamily)this.GetValue(SelectedFontFamilyProperty);
			}
			set{
				this.SetValue(SelectedFontFamilyProperty, value);
			}
		}

		public readonly DependencyProperty SelectedFontSizeProperty = DependencyProperty.Register("SelectedFontSize", typeof(double), typeof(FontDialog));
		public double SelectedFontSize{
			get{
				return (double)this.GetValue(SelectedFontSizeProperty);
			}
			set{
				this.SetValue(SelectedFontSizeProperty, value);
			}
		}

		public readonly DependencyProperty SelectedFontWeightProperty = DependencyProperty.Register("SelectedFontWeight", typeof(FontWeight), typeof(FontDialog));
		public FontWeight SelectedFontWeight{
			get{
				return (FontWeight)this.GetValue(SelectedFontWeightProperty);
			}
			set{
				this.SetValue(SelectedFontWeightProperty, value);
			}
		}

		public readonly DependencyProperty SelectedFontStyleProperty = DependencyProperty.Register("SelectedFontStyle", typeof(FontStyle), typeof(FontDialog));
		public FontStyle SelectedFontStyle{
			get{
				return (FontStyle)this.GetValue(SelectedFontStyleProperty);
			}
			set{
				this.SetValue(SelectedFontStyleProperty, value);
			}
		}

		public readonly DependencyProperty SampleTextProperty = DependencyProperty.Register("SampleText", typeof(string), typeof(FontDialog), new PropertyMetadata("Sample Text"));
		public string SampleText{
			get{
				return (string)this.GetValue(SampleTextProperty);
			}
			set{
				this.SetValue(SampleTextProperty, value);
			}
		}

		#endregion
	}
}
