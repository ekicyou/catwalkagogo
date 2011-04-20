using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Data;

namespace WPF.MDI {
	[ContentProperty("Children")]
	public partial class MdiContainer : UserControl {
		#region Constants

		/// <summary>
		/// Offset for iniial placement of window, and for cascade mode.
		/// </summary>
		const int WindowOffset = 25;

		#endregion

		#region Dependency Properties

		private static readonly DependencyPropertyKey ActiveMdiChildPropertyKey =
			DependencyProperty.RegisterReadOnly("ActiveMdiChild", typeof(MdiChild), typeof(MdiContainer), new UIPropertyMetadata(null));

		public static readonly DependencyProperty ActiveMdiChildProperty = ActiveMdiChildPropertyKey.DependencyProperty;

		#endregion

		#region Property Accessors

		public MdiChild ActiveMdiChild {
			get { return (MdiChild)GetValue(ActiveMdiChildProperty); }
			internal set { SetValue(ActiveMdiChildPropertyKey, value); }
		}

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

		/// <summary>
		/// Gets or sets the child elements.
		/// </summary>
		/// <value>The child elements.</value>
		public ObservableCollection<MdiChild> Children { get; set; }

		private ScrollViewer _ScrollViewer;
		private Canvas _windowCanvas;

		/// <summary>
		/// Offset for new window.
		/// </summary>
		private double _windowOffset;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="MdiContainer"/> class.
		/// </summary>
		public MdiContainer() {
			/*
			var template = new ControlTemplate(typeof(UserControl));
			var svFactory = new FrameworkElementFactory(typeof(ScrollViewer), "PART_ScrollViewer");
			var canvasFactory = new FrameworkElementFactory(typeof(MdiCanvas), "PART_Canvas");
			svFactory.AppendChild(canvasFactory);
			template.VisualTree = svFactory;
			this.Template = template;
			*/
			Background = Brushes.DarkGray;
			Focusable = IsTabStop = false;

			Children = new ObservableCollection<MdiChild>();
			Children.CollectionChanged += Children_CollectionChanged;

			this._ScrollViewer = new ScrollViewer {
				Content = _windowCanvas = new MdiCanvas(),
				HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
				VerticalScrollBarVisibility = ScrollBarVisibility.Auto
			};
			Content = this._ScrollViewer;

			Loaded += MdiContainer_Loaded;
			SizeChanged += MdiContainer_SizeChanged;
			KeyDown += new System.Windows.Input.KeyEventHandler(MdiContainer_KeyDown);
		}
		/*
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			this._ScrollViewer = (ScrollViewer)this.Template.FindName("PART_ScrollViewer", this);
			this._windowCanvas = (Canvas)this.Template.FindName("PART_Canvas", this);
		}
		*/
		static void MdiContainer_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
			MdiContainer mdiContainer = (MdiContainer)sender;
			if(mdiContainer.Children.Count < 2)
				return;
			switch(e.Key) {
				case Key.Tab:
					if(e.KeyboardDevice.Modifiers == ModifierKeys.Control) {
						int minZindex = Panel.GetZIndex(mdiContainer.Children[0]);
						foreach(MdiChild mdiChild in mdiContainer.Children)
							if(Panel.GetZIndex(mdiChild) < minZindex)
								minZindex = Panel.GetZIndex(mdiChild);
						Panel.SetZIndex(mdiContainer.GetTopChild(), minZindex - 1);
						mdiContainer.GetTopChild().Focus();
						e.Handled = true;
					}
					break;
			}
		}

		#endregion

		#region Container Events

		/// <summary>
		/// Handles the Loaded event of the MdiContainer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void MdiContainer_Loaded(object sender, RoutedEventArgs e) {
			Window wnd = Window.GetWindow(this);
			if(wnd != null) {
				wnd.Activated += MdiContainer_Activated;
				wnd.Deactivated += MdiContainer_Deactivated;
			}
			/*

			_windowCanvas.Width = _windowCanvas.ActualWidth;
			_windowCanvas.Height = _windowCanvas.ActualHeight;

			_windowCanvas.VerticalAlignment = VerticalAlignment.Top;
			_windowCanvas.HorizontalAlignment = HorizontalAlignment.Left;
			*/
			InvalidateSize();
		}

		/// <summary>
		/// Handles the Activated event of the MdiContainer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void MdiContainer_Activated(object sender, EventArgs e) {
			if(Children.Count == 0)
				return;

			int index = 0, maxZindex = Panel.GetZIndex(Children[0]);
			for(int i = 0; i < Children.Count; i++) {
				int zindex = Panel.GetZIndex(Children[i]);
				if(zindex > maxZindex) {
					maxZindex = zindex;
					index = i;
				}
			}
			Children[index].IsSelected = true;
		}

		/// <summary>
		/// Handles the Deactivated event of the MdiContainer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void MdiContainer_Deactivated(object sender, EventArgs e) {
			if(Children.Count == 0)
				return;

			for(int i = 0; i < _windowCanvas.Children.Count; i++)
				Children[i].IsSelected = false;
		}

		/// <summary>
		/// Handles the SizeChanged event of the MdiContainer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
		private void MdiContainer_SizeChanged(object sender, SizeChangedEventArgs e) {
			if(Children.Count == 0)
				return;

			for(int i = 0; i < Children.Count; i++) {
				MdiChild mdiChild = Children[i];
				if(mdiChild.WindowState == WindowState.Maximized) {
					mdiChild.Width = this._ScrollViewer.ViewportWidth;
					mdiChild.Height = this._ScrollViewer.ViewportHeight;
				}
				if(mdiChild.WindowState == WindowState.Minimized) {
					mdiChild.Top += e.NewSize.Height - e.PreviousSize.Height;
				}
			}
		}

		#endregion

		#region ObservableCollection Events

		/// <summary>
		/// Handles the CollectionChanged event of the Children control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
		private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			switch(e.Action) {
				case NotifyCollectionChangedAction.Add: {
					MdiChild mdiChild = Children[e.NewStartingIndex],
						topChild = GetTopChild();

					if(topChild != null && topChild.WindowState == WindowState.Maximized)
						mdiChild.Loaded += (s, a) => mdiChild.WindowState = WindowState.Maximized;

					mdiChild.Left = mdiChild.Top = _windowOffset;

					_windowCanvas.Children.Add(mdiChild);
					mdiChild.Loaded += (s, a) => Activate(mdiChild);

					_windowOffset += WindowOffset;
					if(_windowOffset + mdiChild.Width > this.ContainerWidth)
						_windowOffset = 0;
					if(_windowOffset + mdiChild.Height > this.ContainerHeight)
						_windowOffset = 0;

					var dpdWindowState = DependencyPropertyDescriptor.FromProperty(MdiChild.WindowStateProperty, typeof(MdiChild));
					var dpdIsSelected = DependencyPropertyDescriptor.FromProperty(MdiChild.IsSelectedProperty, typeof(MdiChild));
					var dpdIsDragging = DependencyPropertyDescriptor.FromProperty(MdiChild.IsDraggingProperty, typeof(MdiChild));
					foreach(var child in e.NewItems.Cast<MdiChild>()) {
						dpdWindowState.AddValueChanged(child, this.MdiChild_WindowStateChanged);
						dpdIsSelected.AddValueChanged(child, this.MdiChild_IsSelectedChanged);
						dpdIsDragging.AddValueChanged(child, this.MdiChild_IsDraggingChanged);
						child.SizeChanged += this.MdiChild_SizeChanged;
						child.Closed += this.MdiChild_Closed;
						child.LocationChanged += this.MdiChild_LocationChanged;
					}
					break;
				}
				case NotifyCollectionChangedAction.Remove: {
					_windowCanvas.Children.Remove((MdiChild)e.OldItems[0]);
					Activate(GetTopChild());
					var dpdWindowState = DependencyPropertyDescriptor.FromProperty(MdiChild.WindowStateProperty, typeof(MdiChild));
					var dpdIsSelected = DependencyPropertyDescriptor.FromProperty(MdiChild.IsSelectedProperty, typeof(MdiChild));
					var dpdIsDragging = DependencyPropertyDescriptor.FromProperty(MdiChild.IsDraggingProperty, typeof(MdiChild));
					foreach(var child in e.OldItems.Cast<MdiChild>()) {
						dpdWindowState.RemoveValueChanged(child, this.MdiChild_WindowStateChanged);
						dpdIsSelected.RemoveValueChanged(child, this.MdiChild_IsSelectedChanged);
						dpdIsDragging.RemoveValueChanged(child, this.MdiChild_IsDraggingChanged);
						child.SizeChanged -= this.MdiChild_SizeChanged;
						child.Closed -= this.MdiChild_Closed;
						child.LocationChanged -= this.MdiChild_LocationChanged;
					}
					break;
				}
				case NotifyCollectionChangedAction.Reset:
					_windowCanvas.Children.Clear();
					break;
			}
			InvalidateSize();
		}

		#endregion

		#region MdiChild Events

		private void MdiChild_Closed(object sender, EventArgs e){
			this.Children.Remove((MdiChild)sender);
		}

		private void MdiChild_WindowStateChanged(object sender, EventArgs e) {
			var mdiChild = (MdiChild)sender;
			var state = mdiChild.WindowState;
			switch(state) {
				case WindowState.Maximized: {
					this._ScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
					this._ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

					mdiChild.Left = mdiChild.Top = 0;
					mdiChild.Width = this.ContainerWidth;
					mdiChild.Height = this.ContainerHeight;

					this.Activate(mdiChild);
					break;
				}
				case WindowState.Minimized:
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
					goto case WindowState.Normal;
				case WindowState.Normal: {
					// Restore Maximized Windows
					var maximizedChildren = this.Children.Where(child => child.WindowState == WindowState.Maximized).ToArray();
					foreach(var child in maximizedChildren){
						child.WindowState = WindowState.Normal;
					}
					if(maximizedChildren.Length == 0) {
						this._ScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
						this._ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
					}
					break;
				}
			}
			this.InvalidateSize();
		}

		private void MdiChild_LocationChanged(object sender, EventArgs e) {
			var mdiChild = (MdiChild)sender;
			Canvas.SetLeft(mdiChild, mdiChild.Left);
			Canvas.SetTop(mdiChild, mdiChild.Top);
			this.InvalidateSize();
		}

		private void MdiChild_SizeChanged(object sender, SizeChangedEventArgs e) {
			this.InvalidateSize();
		}

		private void MdiChild_IsSelectedChanged(object sender, EventArgs e) {
			this.Activate((MdiChild)sender);
		}

		private void MdiChild_IsDraggingChanged(object sender, EventArgs e){
			CursorClipping.SetIsEnabled(this, ((MdiChild)sender).IsDragging);
		}

		#endregion

		/// <summary>
		/// Focuses a child and brings it into view.
		/// </summary>
		/// <param name="mdiChild">The MDI child.</param>
		private void Activate(MdiChild mdiChild) {
			if(mdiChild == null){
				this.ActiveMdiChild = null;
				return;
			}

			var dpdIsSelected = DependencyPropertyDescriptor.FromProperty(MdiChild.IsSelectedProperty, typeof(MdiChild));
			int maxZindex = 0;
			for(int i = 0; i < this.Children.Count; i++) {
				int zindex = Panel.GetZIndex(this.Children[i]);
				if(zindex > maxZindex)
					maxZindex = zindex;
				if(this.Children[i] != mdiChild) {
					this.SetIsSelected(dpdIsSelected, this.Children[i], false);
				} else {
					this.SetIsSelected(dpdIsSelected, mdiChild, true);
				}
			}
			this.ActiveMdiChild = mdiChild;
			Panel.SetZIndex(mdiChild, maxZindex + 1);

			if(this.Children.Any(child => child.WindowState == WindowState.Maximized)){
				mdiChild.WindowState = WindowState.Maximized;
			}
		}

		private void SetIsSelected(DependencyPropertyDescriptor dpdIsSelected, MdiChild mdiChild, bool isSelected) {
			dpdIsSelected.RemoveValueChanged(mdiChild, this.MdiChild_IsSelectedChanged);
			mdiChild.IsSelected = isSelected;
			dpdIsSelected.AddValueChanged(mdiChild, this.MdiChild_IsSelectedChanged);
		}

		public void SelectNextMdiChild() {
			if(this.Children.Count >= 2) {
				int minZindex = Panel.GetZIndex((UIElement)this.Children[0]);
				foreach(MdiChild mdiChild in this.Children)
					if(Panel.GetZIndex(mdiChild) < minZindex)
						minZindex = Panel.GetZIndex(mdiChild);
				Panel.SetZIndex(this.GetTopChild(), minZindex - 1);
				this.GetTopChild().IsSelected = true;
			}
		}

		public void SelectPreviousMdiChild() {
			if(this.Children.Count >= 2) {
				int minZindex = Panel.GetZIndex((UIElement)this.Children[0]);
				MdiChild minChild = null;
				foreach(MdiChild mdiChild in this.Children) {
					if(Panel.GetZIndex(mdiChild) < minZindex) {
						minZindex = Panel.GetZIndex(mdiChild);
						minChild = mdiChild;
					}
				}
				Panel.SetZIndex(minChild, Panel.GetZIndex(this.GetTopChild()) + 1);
				this.GetTopChild().IsSelected = true;
			}
		}

		/// <summary>
		/// Invalidates the size checking to see if the furthest
		/// child point exceeds the current height and width.
		/// </summary>
		private void InvalidateSize() {
			this._windowCanvas.InvalidateMeasure();
		}

		/// <summary>
		/// Gets MdiChild with maximum ZIndex.
		/// </summary>
		private MdiChild GetTopChild() {
			if(_windowCanvas.Children.Count < 1)
				return null;

			int index = 0, maxZindex = Panel.GetZIndex(_windowCanvas.Children[0]);
			for(int i = 1, zindex; i < _windowCanvas.Children.Count; i++) {
				zindex = Panel.GetZIndex(_windowCanvas.Children[i]);
				if(zindex > maxZindex) {
					maxZindex = zindex;
					index = i;
				}
			}
			return (MdiChild)_windowCanvas.Children[index];
		}

		#region Nested MdiChild comparerer

		internal class MdiChildComparer : IComparer<MdiChild> {
			#region IComparer<MdiChild> Members

			public int Compare(MdiChild x, MdiChild y) {
				return Canvas.GetZIndex(y).CompareTo(Canvas.GetZIndex(x));
			}

			#endregion
		}
		#endregion
	}
}