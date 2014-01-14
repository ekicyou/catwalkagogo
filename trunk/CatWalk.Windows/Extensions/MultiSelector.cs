using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;

namespace CatWalk.Windows.Extensions {
	public static class MultiSelector {


		public static IList GetSelectedItems(DependencyObject obj) {
			return (IList)obj.GetValue(SelectedItemsProperty);
		}

		public static void SetSelectedItems(DependencyObject obj, IList value) {
			obj.SetValue(SelectedItemsProperty, value);
		}

		// Using a DependencyProperty as the backing store for SelectedItems.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SelectedItemsProperty =
			DependencyProperty.RegisterAttached("SelectedItems", typeof(IList), typeof(MultiSelector), new PropertyMetadata(null, OnSelectedItemsChanged));

		private static readonly DependencyProperty CollectionSynchronizerProperty =
			DependencyProperty.RegisterAttached("CollectionSynchronizer", typeof(CollectionSynchronizer), typeof(MultiSelector));

		private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(e.OldValue != null) {
				var sync = d.GetValue(CollectionSynchronizerProperty) as CollectionSynchronizer;
				sync.Stop();
			}

			if(e.NewValue != null) {
				var list = (IList)e.NewValue;
				var selectedList = GetSelectedItemsList(d);
				var sync = new CollectionSynchronizer(list, selectedList);
				sync.Start();
				d.SetValue(CollectionSynchronizerProperty, sync);
			}
		}

		private static IList GetSelectedItemsList(DependencyObject d) {
			var sel = d as System.Windows.Controls.Primitives.MultiSelector;
			if(sel != null) {
				return sel.SelectedItems;
			} else {
				var listBox = d as ListBox;
				if(listBox != null) {
					return listBox.SelectedItems;
				} else {
					throw new ArgumentException("d");
				}
			}
		}

		private class CollectionSynchronizer {
			private IList _Collection1;
			private IList _Collection2;

			public CollectionSynchronizer(IList col1, IList col2) {
				col1.ThrowIfNull("col1");
				col2.ThrowIfNull("col2");
				this._Collection1 = col1;
				this._Collection2 = col2;
			}

			public void Start() {

			}

			public void Stop() {

			}
		}
	}
}
