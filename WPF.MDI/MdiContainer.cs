using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Data;

namespace WPF.MDI {
	[StyleTypedPropertyAttribute(Property = "ItemContainerStyle", StyleTargetType = typeof(MdiChild))]
	[TemplatePart(Name="PART_ScrollViewer", Type=typeof(ScrollViewer))]
	public partial class MdiContainer : Selector {
		#region Property Accessors

		internal double ContainerWidth{
			get{
				return this._ScrollViewer.ViewportWidth;
			}
		}

		internal double ContainerHeight{
			get{
				return this._ScrollViewer.ViewportHeight;
			}
		}

		#endregion

		#region Member Declarations

		private ScrollViewer _ScrollViewer;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="MdiContainer"/> class.
		/// </summary>
		public MdiContainer() {
			var template = new ControlTemplate(typeof(MdiContainer));
			var svFactory = new FrameworkElementFactory(typeof(ScrollViewer), "PART_ScrollViewer");
			svFactory.SetBinding(ScrollViewer.BackgroundProperty, new Binding("Background"){Source=this});
			svFactory.SetBinding(ScrollViewer.BorderBrushProperty, new Binding("BorderBrush"){Source=this});
			svFactory.SetBinding(ScrollViewer.BorderThicknessProperty, new Binding("BorderThickness"){Source=this});
			svFactory.SetBinding(ScrollViewer.PaddingProperty, new Binding("Padding"){Source=this});
			var itemsFactory = new FrameworkElementFactory(typeof(ItemsPresenter));
			svFactory.AppendChild(itemsFactory);
			template.VisualTree = svFactory;
			this.Template = template;

			var panelTemplate = new ItemsPanelTemplate();
			var canvasFactory = new FrameworkElementFactory(typeof(MdiCanvas));
			panelTemplate.VisualTree = canvasFactory;
			this.ItemsPanel = panelTemplate;

			Background = SystemColors.AppWorkspaceBrush;
			Focusable = IsTabStop = false;
		}
		
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			this._ScrollViewer = (ScrollViewer)this.Template.FindName("PART_ScrollViewer", this);
			this._ScrollViewer.VerticalScrollBarVisibility = this._ScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
			this._ScrollViewer.ScrollChanged += this.MdiContainer_SizeChanged;
		}

		#endregion

		#region Container Events

		/// <summary>
		/// Handles the SizeChanged event of the MdiContainer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
		private void MdiContainer_SizeChanged(object sender, ScrollChangedEventArgs e) {
			foreach(var mdiChild in this.Children){
				if(mdiChild.WindowState == WindowState.Maximized) {
					mdiChild.Width = this._ScrollViewer.ViewportWidth;
					mdiChild.Height = this._ScrollViewer.ViewportHeight;
				}
				if(mdiChild.WindowState == WindowState.Minimized) {
					mdiChild.Top += e.ViewportHeightChange;
				}
			}
		}

		#endregion

		#region Item Container

		internal IEnumerable<MdiChild> Children{
			get{
				var i = 0;
				foreach(var item in this.Items){
					var child = item as MdiChild;
					if(child != null){
						yield return child;
					}else{
						var cont = (MdiChild)this.ItemContainerGenerator.ContainerFromIndex(i);
						if(cont != null){
							yield return cont;
						}
					}
					i++;
				}
			}
		}

		private IEnumerable<MdiChild> GetChildren(System.Collections.IEnumerable items){
			if(items == null){
				yield break;
			}
			foreach(var item in items){
				var child = item as MdiChild;
				if(child != null){
					yield return child;
				}else{
					var cont = (MdiChild)this.ItemContainerGenerator.ContainerFromItem(item);
					if(cont != null){
						yield return cont;
					}
				}
			}
		}

		protected override void ClearContainerForItemOverride(DependencyObject element, object item) {
			var mdiChild = element as MdiChild;
			this.RemoveEventHandlers(mdiChild);
		}

		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is MdiChild;
		}

		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			var mdiChild = element as MdiChild;
			if(element != item){
				if(mdiChild != null){
					mdiChild.Content = item;
					mdiChild.DataContext = item;
				}
			}
			this.AddEventHandlers(mdiChild);
		}

		protected override DependencyObject GetContainerForItemOverride() {
			return new MdiChild();
		}

		#endregion

		#region ObservableCollection Events

		protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e) {
			base.OnItemsChanged(e);
			this.Select(this.GetTopChild());
			foreach(var child in GetChildren(e.NewItems)){
				Canvas.SetTop(child, child.Top);
				Canvas.SetLeft(child, child.Left);
			}
		}

		private void AddEventHandlers(MdiChild mdiChild){
			var dpdWindowState = DependencyPropertyDescriptor.FromProperty(MdiChild.WindowStateProperty, typeof(MdiChild));
			var dpdIsSelected = DependencyPropertyDescriptor.FromProperty(MdiChild.IsSelectedProperty, typeof(MdiChild));
			var dpdIsDragging = DependencyPropertyDescriptor.FromProperty(MdiChild.IsDraggingProperty, typeof(MdiChild));
			dpdWindowState.AddValueChanged(mdiChild, this.MdiChild_WindowStateChanged);
			dpdIsSelected.AddValueChanged(mdiChild, this.MdiChild_IsSelectedChanged);
			dpdIsDragging.AddValueChanged(mdiChild, this.MdiChild_IsDraggingChanged);
			mdiChild.SizeChanged += this.MdiChild_SizeChanged;
			mdiChild.Closed += this.MdiChild_Closed;
			mdiChild.LocationChanged += this.MdiChild_LocationChanged;
		}

		private void RemoveEventHandlers(MdiChild mdiChild){
			var dpdWindowState = DependencyPropertyDescriptor.FromProperty(MdiChild.WindowStateProperty, typeof(MdiChild));
			var dpdIsSelected = DependencyPropertyDescriptor.FromProperty(MdiChild.IsSelectedProperty, typeof(MdiChild));
			var dpdIsDragging = DependencyPropertyDescriptor.FromProperty(MdiChild.IsDraggingProperty, typeof(MdiChild));
			dpdWindowState.RemoveValueChanged(mdiChild, this.MdiChild_WindowStateChanged);
			dpdIsSelected.RemoveValueChanged(mdiChild, this.MdiChild_IsSelectedChanged);
			dpdIsDragging.RemoveValueChanged(mdiChild, this.MdiChild_IsDraggingChanged);
			mdiChild.SizeChanged -= this.MdiChild_SizeChanged;
			mdiChild.Closed -= this.MdiChild_Closed;
			mdiChild.LocationChanged -= this.MdiChild_LocationChanged;
		}

		#endregion

		#region MdiChild Events

		private void MdiChild_Closed(object sender, EventArgs e){
			if(this.ItemsSource == null){
				this.Items.Remove((MdiChild)sender);
			}
		}

		private void MdiChild_WindowStateChanged(object sender, EventArgs e) {
			var mdiChild = (MdiChild)sender;
			switch(mdiChild.WindowState) {
				case WindowState.Maximized: {
					this._ScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
					this._ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
					this._ScrollViewer.ScrollToHorizontalOffset(0);
					this._ScrollViewer.ScrollToVerticalOffset(0);

					mdiChild.Left = mdiChild.Top = 0;
					mdiChild.Width = this.ContainerWidth;
					mdiChild.Height = this.ContainerHeight;

					this.Select(mdiChild);
					break;
				}
				case WindowState.Minimized:
					this.SetMinimizedPosition(mdiChild);
					goto case WindowState.Normal;
				case WindowState.Normal: {
					// Restore Maximized Windows
					var maximizedItems = this.Children.Where(child => child.WindowState == WindowState.Maximized).ToArray();
					foreach(var child in maximizedItems){
						child.WindowState = WindowState.Normal;
					}
					this._ScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
					this._ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
					break;
				}
			}
		}

		private void SetMinimizedPosition(MdiChild mdiChild){
			int capacity = (int)this.ContainerWidth / MdiChild.MinimizedWidth;
			var rect = new Rect();
			var minWindowRects = this.Children.Where(child => child != mdiChild && child.WindowState == WindowState.Minimized)
				.Select(child => new Rect(child.Left, child.Top, MdiChild.MinimizedWidth, MdiChild.MinimizedHeight)).ToArray();

			var length = minWindowRects.Length + 1;
			for(int i = 0; i < length; i++){
				int row = i / capacity + 1;
				int col = i % capacity;
				rect = new Rect(MdiChild.MinimizedWidth * col + 1, this.ActualHeight - MdiChild.MinimizedHeight * row,
					MdiChild.MinimizedWidth - 2, MdiChild.MinimizedHeight);
				if(!minWindowRects.Any(rect2 => rect2.IntersectsWith(rect))){
					break;
				}
			}
						
			mdiChild.Left = rect.Left - 1;
			mdiChild.Top = rect.Y;
		}

		private void MdiChild_LocationChanged(object sender, EventArgs e) {
			var mdiChild = (MdiChild)sender;
			Canvas.SetLeft(mdiChild, mdiChild.Left);
			Canvas.SetTop(mdiChild, mdiChild.Top);
		}

		private void MdiChild_SizeChanged(object sender, SizeChangedEventArgs e) {
		}

		private void MdiChild_IsSelectedChanged(object sender, EventArgs e) {
			var mdiChild =(MdiChild)sender;
			if(mdiChild.IsSelected){
				this.Select(mdiChild);
			}
		}

		private void MdiChild_IsDraggingChanged(object sender, EventArgs e){
			CursorClipping.SetIsEnabled(this, ((MdiChild)sender).IsDragging);
		}

		#endregion

		#region Selection

		private static readonly DependencyPropertyKey ActiveMdiChildPropertyKey =
			DependencyProperty.RegisterReadOnly("ActiveMdiChild", typeof(MdiChild), typeof(MdiContainer), new UIPropertyMetadata(null));

		public static readonly DependencyProperty ActiveMdiChildProperty = ActiveMdiChildPropertyKey.DependencyProperty;

		public MdiChild ActiveMdiChild {
			get { return (MdiChild)GetValue(ActiveMdiChildProperty); }
			internal set { SetValue(ActiveMdiChildPropertyKey, value); }
		}

		private void Select(MdiChild mdiChild){
			if(mdiChild != null){
				if(this.ItemsSource != null){
					var item = this.ItemContainerGenerator.ItemFromContainer(mdiChild);
					this.SelectedItem = item;
				}else{
					this.SelectedItem = mdiChild;
				}
			}else{
				this.SelectedItem = null;
			}
		}

		private void SetIsSelected(DependencyPropertyDescriptor dpdIsSelected, MdiChild mdiChild, bool isSelected) {
			dpdIsSelected.RemoveValueChanged(mdiChild, this.MdiChild_IsSelectedChanged);
			mdiChild.IsSelected = isSelected;
			dpdIsSelected.AddValueChanged(mdiChild, this.MdiChild_IsSelectedChanged);
		}

		protected override void OnSelectionChanged(SelectionChangedEventArgs e) {
			base.OnSelectionChanged(e);
			//var dpdIsSelected = DependencyPropertyDescriptor.FromProperty(MdiChild.IsSelectedProperty, typeof(MdiChild));
			foreach(var child in this.GetChildren(e.RemovedItems)){
				if(child.IsSelected){
					child.IsSelected = false;
				}
			}
			var mdiChild = this.GetChildren(e.AddedItems).FirstOrDefault();
			if(mdiChild != null){
				var title = mdiChild.Title;
				var isMaximized = false;
				var zIndex = 0;
				foreach(var child in this.Children.OrderBy(child => Panel.GetZIndex(child))){
					isMaximized |= child.WindowState == WindowState.Maximized;
					Panel.SetZIndex(child, zIndex);
					if(child != mdiChild) {
						//this.SetIsSelected(dpdIsSelected, child, false);
						zIndex++;
					}
				}
				if(!mdiChild.IsSelected){
					mdiChild.IsSelected = true;
				}
				this.ActiveMdiChild = mdiChild;
				//this.SelectedIndex = this.Items.IndexOf(mdiChild);
				Panel.SetZIndex(mdiChild, this.Items.Count);

				if(isMaximized){
					mdiChild.WindowState = WindowState.Maximized;
				}
			}
		}

		public void SelectNextMdiChild() {
			if(this.Items.Count >= 2) {
				var topChild = this.GetTopChild();
				if(topChild != null){
					Panel.SetZIndex(topChild, -1);
					this.Select(this.GetTopChild());
				}
			}
		}

		public void SelectPreviousMdiChild() {
			if(this.Items.Count >= 2) {
				var bottomChild = this.Children.OrderBy(child => Panel.GetZIndex(child)).FirstOrDefault();
				if(bottomChild != null){
					this.Select(bottomChild);
				}
			}
		}

		#endregion

		#region functions
		/*
		private static Panel GetItemsPanel(DependencyObject itemsControl) {
			ItemsPresenter itemsPresenter = GetVisualChild<ItemsPresenter>(itemsControl);
			if(itemsPresenter != null){
				Panel itemsPanel = GetVisualChild<MdiCanvas>(itemsPresenter);
				return itemsPanel;
			}else{
				return null;
			}
		}

		private static T GetVisualChild<T>(DependencyObject parent) where T : Visual {
			T child = default(T);
			int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
			for (int i = 0; i < numVisuals; i++) {
				Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
				child = v as T;
				if (child == null){
					child = GetVisualChild<T>(v);
				}
				if (child != null){
					break;
				}
			}
			return child; 
		}
		*/
		/// <summary>
		/// Gets MdiChild with maximum ZIndex.
		/// </summary>
		private MdiChild GetTopChild() {
			return this.Children.OrderByDescending(child => Panel.GetZIndex(child)).FirstOrDefault();
		}

		#endregion
	}
}