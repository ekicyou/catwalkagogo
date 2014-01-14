using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

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
			DependencyProperty.RegisterAttached("CollectionSynchronizer", typeof(MultiSelectorSynchronizer), typeof(MultiSelector));

		private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(e.OldValue != null) {
				var sync = d.GetValue(CollectionSynchronizerProperty) as MultiSelectorSynchronizer;
				sync.Stop();
			}

			if(e.NewValue != null) {
				var list = (IList)e.NewValue;
				var sync = new MultiSelectorSynchronizer((Selector)d, list);
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

		private class MultiSelectorSynchronizer {
			private IList _Collection;
			private Selector _Selector;

			public MultiSelectorSynchronizer(Selector selector, IList list) {
				this._Selector = selector;
				this._Collection = list;

				var selList = GetSelectedItemsList(selector);
				selList.Clear();
				foreach(var item in list) {
					selList.Add(item);
				}

			}

			void selector_SelectionChanged(object sender, SelectionChangedEventArgs e) {
				this.Stop();
				foreach(var item in e.RemovedItems) {
					this._Collection.Remove(item);
				}
				foreach(var item in e.AddedItems) {
					this._Collection.Add(item);
				}
				this.Start();
			}

			public void Start() {
				this._Selector.SelectionChanged += selector_SelectionChanged;
			}

			public void Stop() {
				this._Selector.SelectionChanged -= selector_SelectionChanged;

			}
		}
	}
}
