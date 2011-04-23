using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Collections.Generic;

namespace WPF.MDI {
	[ContentProperty("Content")]
	public class MdiChild : Control {
		#region Constants

		/// <summary>
		/// Width of minimized window.
		/// </summary>
		internal const int MinimizedWidth = 160;

		/// <summary>
		/// Height of minimized window.
		/// </summary>
		internal const int MinimizedHeight = 29;

		#endregion

		#region Dependency Properties

		public ContextMenu SystemMenu {
			get { return (ContextMenu)GetValue(SystemMenuProperty); }
			set { SetValue(SystemMenuProperty, value); }
		}

		// Using a DependencyProperty as the backing store for SystemMenu.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SystemMenuProperty =
			DependencyProperty.Register("SystemMenu", typeof(ContextMenu), typeof(MdiChild), new UIPropertyMetadata(null));

		/// <summary>
		/// Identifies the WPF.MDI.MdiChild.ContentProperty dependency property.
		/// </summary>
		/// <returns>The identifier for the WPF.MDI.MdiChild.ContentProperty property.</returns>
		public static readonly DependencyProperty ContentProperty =
			DependencyProperty.Register("Content", typeof(object), typeof(MdiChild));

		/// <summary>
		/// Identifies the WPF.MDI.MdiChild.TitleProperty dependency property.
		/// </summary>
		/// <returns>The identifier for the WPF.MDI.MdiChild.TitleProperty property.</returns>
		public static readonly DependencyProperty TitleProperty =
			DependencyProperty.Register("Title", typeof(string), typeof(MdiChild));

		/// <summary>
		/// Identifies the WPF.MDI.MdiChild.IconProperty dependency property.
		/// </summary>
		/// <returns>The identifier for the WPF.MDI.MdiChild.IconProperty property.</returns>
		public static readonly DependencyProperty IconProperty =
			DependencyProperty.Register("Icon", typeof(ImageSource), typeof(MdiChild));

		/// <summary>
		/// Identifies the WPF.MDI.MdiChild.ShowIconProperty dependency property.
		/// </summary>
		/// <returns>The identifier for the WPF.MDI.MdiChild.ShowIconProperty property.</returns>
		public static readonly DependencyProperty IsShowIconProperty =
			DependencyProperty.Register("IsShowIcon", typeof(bool), typeof(MdiChild),
			new UIPropertyMetadata(true));

		/// <summary>
		/// Identifies the WPF.MDI.MdiChild.ResizableProperty dependency property.
		/// </summary>
		/// <returns>The identifier for the WPF.MDI.MdiChild.ResizableProperty property.</returns>
		public static readonly DependencyProperty IsResizableProperty =
			DependencyProperty.Register("IsResizable", typeof(bool), typeof(MdiChild),
			new UIPropertyMetadata(true));

		/// <summary>
		/// Identifies the WPF.MDI.MdiChild.FocusedProperty dependency property.
		/// </summary>
		/// <returns>The identifier for the WPF.MDI.MdiChild.FocusedProperty property.</returns>
		public static readonly DependencyProperty IsSelectedProperty =
			DependencyProperty.Register("IsSelected", typeof(bool), typeof(MdiChild),
			new UIPropertyMetadata(false, new PropertyChangedCallback(IsSelectedValueChanged)));

		/// <summary>
		/// Identifies the WPF.MDI.MdiChild.MinimizeBoxProperty dependency property.
		/// </summary>
		/// <returns>The identifier for the WPF.MDI.MdiChild.MinimizeBoxProperty property.</returns>
		public static readonly DependencyProperty IsMinimizableProperty =
			DependencyProperty.Register("IsMinimizable", typeof(bool), typeof(MdiChild),
			new UIPropertyMetadata(true, new PropertyChangedCallback(IsMinimizableValueChanged)));

		/// <summary>
		/// Identifies the WPF.MDI.MdiChild.MaximizeBoxProperty dependency property.
		/// </summary>
		/// <returns>The identifier for the WPF.MDI.MdiChild.MaximizeBoxProperty property.</returns>
		public static readonly DependencyProperty IsMaximizableProperty =
			DependencyProperty.Register("IsMaximizable", typeof(bool), typeof(MdiChild),
			new UIPropertyMetadata(true, new PropertyChangedCallback(IsMaximizableValueChanged)));

		/// <summary>
		/// Identifies the WPF.MDI.MdiChild.WindowStateProperty dependency property.
		/// </summary>
		/// <returns>The identifier for the WPF.MDI.MdiChild.WindowStateProperty property.</returns>
		public static readonly DependencyProperty WindowStateProperty =
			DependencyProperty.Register("WindowState", typeof(WindowState), typeof(MdiChild),
			new FrameworkPropertyMetadata(WindowState.Normal, new PropertyChangedCallback(WindowStateValueChanged)));

		// Using a DependencyProperty as the backing store for RestoreBounds.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty RestoreBoundsProperty =
			DependencyProperty.Register("RestoreBounds", typeof(Rect), typeof(MdiChild), new UIPropertyMetadata(new Rect(0, 0, 480, 320)));

		#endregion

		#region Property Accessors

		/// <summary>
		/// Gets or sets the content.
		/// This is a dependency property.
		/// </summary>
		/// <value>The content.</value>
		public object Content {
			get { return (object)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}

		/// <summary>
		/// Gets or sets the window title.
		/// This is a dependency property.
		/// </summary>
		/// <value>The window title.</value>
		public string Title {
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		/// <summary>
		/// Gets or sets the window icon.
		/// This is a dependency property.
		/// </summary>
		/// <value>The window icon.</value>
		public ImageSource Icon {
			get { return (ImageSource)GetValue(IconProperty); }
			set { SetValue(IconProperty, value); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether to [show the window icon].
		/// This is a dependency property.
		/// </summary>
		/// <value><c>true</c> if [show the window icon]; otherwise, <c>false</c>.</value>
		public bool IsShowIcon {
			get { return (bool)GetValue(IsShowIconProperty); }
			set { SetValue(IsShowIconProperty, value); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the [window is resizable].
		/// This is a dependency property.
		/// </summary>
		/// <value><c>true</c> if [window is resizable]; otherwise, <c>false</c>.</value>
		public bool IsResizable {
			get { return (bool)GetValue(IsResizableProperty); }
			set { SetValue(IsResizableProperty, value); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the [window is focused].
		/// This is a dependency property.
		/// </summary>
		/// <value><c>true</c> if [window is focused]; otherwise, <c>false</c>.</value>
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether to [show the minimize box button].
		/// This is a dependency property.
		/// </summary>
		/// <value><c>true</c> if [show the minimize box button]; otherwise, <c>false</c>.</value>
		public bool IsMinimizable {
			get { return (bool)GetValue(IsMinimizableProperty); }
			set { SetValue(IsMinimizableProperty, value); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether to [show the maximize box button].
		/// This is a dependency property.
		/// </summary>
		/// <value><c>true</c> if [show the maximize box button]; otherwise, <c>false</c>.</value>
		public bool IsMaximizable {
			get { return (bool)GetValue(IsMaximizableProperty); }
			set { SetValue(IsMaximizableProperty, value); }
		}

		/// <summary>
		/// Gets or sets the state of the window.
		/// This is a dependency property.
		/// </summary>
		/// <value>The state of the window.</value>
		public WindowState WindowState {
			get { return (WindowState)GetValue(WindowStateProperty); }
			set { SetValue(WindowStateProperty, value); }
		}

		public Rect RestoreBounds {
			get { return (Rect)GetValue(RestoreBoundsProperty); }
			set { SetValue(RestoreBoundsProperty, value); }
		}

		#endregion

		#region Location

		public double Top {
			get { return (double)GetValue(TopProperty); }
			set { SetValue(TopProperty, value); }
		}

		public static readonly DependencyProperty TopProperty =
			DependencyProperty.Register("Top", typeof(double), typeof(MdiChild), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsParentMeasure, OnLocationChanged));

		public double Left {
			get { return (double)GetValue(LeftProperty); }
			set { SetValue(LeftProperty, value); }
		}

		public static readonly DependencyProperty LeftProperty =
			DependencyProperty.Register("Left", typeof(double), typeof(MdiChild), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsParentMeasure, OnLocationChanged));

		private static void OnLocationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e){
			var self = (MdiChild)d;
			self.OnLocationChanged(EventArgs.Empty);
		}

		protected virtual void OnLocationChanged(EventArgs e){
			var eh = this.LocationChanged;
			if(eh != null){
				eh(this, EventArgs.Empty);
			}
		}

		public event EventHandler LocationChanged;

		#endregion

		/// <summary>
		/// Initializes the <see cref="MdiChild"/> class.
		/// </summary>
		static MdiChild() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(MdiChild), new FrameworkPropertyMetadata(typeof(MdiChild)));
			FocusableProperty.OverrideMetadata(typeof(MdiChild), new FrameworkPropertyMetadata(false));
			IsTabStopProperty.OverrideMetadata(typeof(MdiChild), new FrameworkPropertyMetadata(false));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MdiChild"/> class.
		/// </summary>
		public MdiChild() {
			FocusManager.SetIsFocusScope(this, true);
		}

		#region Control Events
		
		/// <summary>
		/// Handles the GotFocus event of the MdiChild control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		protected override void OnGotFocus(RoutedEventArgs e) {
			base.OnGotFocus(e);
			if(!this.IsSelected){
				this.IsSelected = true;
			}
		}

		protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e) {
			base.OnGotKeyboardFocus(e);
			//this.IsSelected = true;
		}
		
		#endregion

		#region Control Overrides

		/// <summary>
		/// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
		/// </summary>
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();

			var minimizeButton = (Button)Template.FindName("MinimizeButton", this);
			var maximizeButton = (Button)Template.FindName("MaximizeButton", this);
			var closeButton = (Button)Template.FindName("CloseButton", this);

			if(minimizeButton != null)
				minimizeButton.Click += new RoutedEventHandler(minimizeButton_Click);

			if(maximizeButton != null)
				maximizeButton.Click += new RoutedEventHandler(maximizeButton_Click);

			if(closeButton != null)
				closeButton.Click += new RoutedEventHandler(closeButton_Click);
			
			Thumb dragThumb = (Thumb)Template.FindName("DragThumb", this);
			
			if(dragThumb != null) {
				dragThumb.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);
				dragThumb.DragDelta += new DragDeltaEventHandler(dragThumb_DragDelta);
				dragThumb.DragCompleted += new DragCompletedEventHandler(Thumb_DragCompleted);
				
				dragThumb.MouseDoubleClick += (sender, e) => {
					if(WindowState == WindowState.Minimized)
						minimizeButton_Click(null, null);
					else if(WindowState == WindowState.Normal)
						maximizeButton_Click(null, null);
				};
			}

			Thumb resizeLeft = (Thumb)Template.FindName("ResizeLeft", this);
			Thumb resizeTopLeft = (Thumb)Template.FindName("ResizeTopLeft", this);
			Thumb resizeTop = (Thumb)Template.FindName("ResizeTop", this);
			Thumb resizeTopRight = (Thumb)Template.FindName("ResizeTopRight", this);
			Thumb resizeRight = (Thumb)Template.FindName("ResizeRight", this);
			Thumb resizeBottomRight = (Thumb)Template.FindName("ResizeBottomRight", this);
			Thumb resizeBottom = (Thumb)Template.FindName("ResizeBottom", this);
			Thumb resizeBottomLeft = (Thumb)Template.FindName("ResizeBottomLeft", this);

			if(resizeLeft != null) {
				resizeLeft.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);
				resizeLeft.DragDelta += new DragDeltaEventHandler(ResizeLeft_DragDelta);
				resizeLeft.DragCompleted += new DragCompletedEventHandler(Thumb_DragCompleted);
			}

			if(resizeTop != null) {
				resizeTop.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);
				resizeTop.DragDelta += new DragDeltaEventHandler(ResizeTop_DragDelta);
				resizeTop.DragCompleted += new DragCompletedEventHandler(Thumb_DragCompleted);
			}

			if(resizeRight != null) {
				resizeRight.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);
				resizeRight.DragDelta += new DragDeltaEventHandler(ResizeRight_DragDelta);
				resizeRight.DragCompleted += new DragCompletedEventHandler(Thumb_DragCompleted);
			}

			if(resizeBottom != null) {
				resizeBottom.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);
				resizeBottom.DragDelta += new DragDeltaEventHandler(ResizeBottom_DragDelta);
				resizeBottom.DragCompleted += new DragCompletedEventHandler(Thumb_DragCompleted);
			}

			if(resizeTopLeft != null) {
				resizeTopLeft.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);
				resizeTopLeft.DragCompleted += new DragCompletedEventHandler(Thumb_DragCompleted);

				resizeTopLeft.DragDelta += (sender, e) => {
					ResizeTop_DragDelta(null, e);
					ResizeLeft_DragDelta(null, e);
				};
			}

			if(resizeTopRight != null) {
				resizeTopRight.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);
				resizeTopRight.DragCompleted += new DragCompletedEventHandler(Thumb_DragCompleted);

				resizeTopRight.DragDelta += (sender, e) => {
					ResizeTop_DragDelta(null, e);
					ResizeRight_DragDelta(null, e);
				};
			}

			if(resizeBottomRight != null) {
				resizeBottomRight.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);
				resizeBottomRight.DragCompleted += new DragCompletedEventHandler(Thumb_DragCompleted);

				resizeBottomRight.DragDelta += (sender, e) => {
					ResizeBottom_DragDelta(null, e);
					ResizeRight_DragDelta(null, e);
				};
			}

			if(resizeBottomLeft != null) {
				resizeBottomLeft.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);
				resizeBottomLeft.DragCompleted += new DragCompletedEventHandler(Thumb_DragCompleted);

				resizeBottomLeft.DragDelta += (sender, e) => {
					ResizeBottom_DragDelta(null, e);
					ResizeLeft_DragDelta(null, e);
				};
			}
		}
		
		/// <summary>
		/// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseDown"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.</param>
		protected override void OnMouseDown(MouseButtonEventArgs e) {
			base.OnMouseDown(e);

			if(!this.IsSelected){
				IsSelected = true;
			}
		}
		
		#endregion

		#region Top Button Events

		/// <summary>
		/// Handles the Click event of the minimizeButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void minimizeButton_Click(object sender, RoutedEventArgs e) {
			if(WindowState == WindowState.Minimized)
				WindowState = WindowState.Normal;
			else
				WindowState = WindowState.Minimized;
			this.IsSelected = true;
		}

		/// <summary>
		/// Handles the Click event of the maximizeButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void maximizeButton_Click(object sender, RoutedEventArgs e) {
			if(WindowState == WindowState.Maximized)
				WindowState = WindowState.Normal;
			else
				WindowState = WindowState.Maximized;
			this.IsSelected = true;
		}

		/// <summary>
		/// Handles the Click event of the closeButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void closeButton_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}

		#endregion

		#region Thumb Events

		private static readonly DependencyPropertyKey IsDraggingPropertyKey = DependencyProperty.RegisterReadOnly("IsDragging", typeof(bool), typeof(MdiChild), new UIPropertyMetadata(false));

		public static readonly DependencyProperty IsDraggingProperty = IsDraggingPropertyKey.DependencyProperty;

		public bool IsDragging {
			get {
				return (bool)this.GetValue(IsDraggingProperty);
			}
			set {
				this.SetValue(IsDraggingPropertyKey, value);
			}
		}

		/// <summary>
		/// Handles the DragStarted event of the Thumb control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragStartedEventArgs"/> instance containing the event data.</param>
		private void Thumb_DragStarted(object sender, DragStartedEventArgs e) {
			if(!IsSelected)
				IsSelected = true;

			this.IsDragging = true;
		}

		private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e) {
			this.IsDragging = false;
		}

		/// <summary>
		/// Handles the DragDelta event of the ResizeLeft control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
		private void ResizeLeft_DragDelta(object sender, DragDeltaEventArgs e) {
			if(Width - e.HorizontalChange < MinWidth)
				return;

			double newLeft = e.HorizontalChange;

			if(Left + newLeft < 0)
				newLeft = 0 - Left;

			Width -= newLeft;
			this.Left += newLeft;
		}

		/// <summary>
		/// Handles the DragDelta event of the ResizeTop control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
		private void ResizeTop_DragDelta(object sender, DragDeltaEventArgs e) {
			if(Height - e.VerticalChange < MinHeight)
				return;

			double newTop = e.VerticalChange;

			if(Top + newTop < 0)
				newTop = 0 - Top;

			Height -= newTop;
			this.Top += newTop;

		}

		/// <summary>
		/// Handles the DragDelta event of the ResizeRight control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
		private void ResizeRight_DragDelta(object sender, DragDeltaEventArgs e) {
			if(Width + e.HorizontalChange < MinWidth)
				return;

			Width += e.HorizontalChange;
		}

		/// <summary>
		/// Handles the DragDelta event of the ResizeBottom control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
		private void ResizeBottom_DragDelta(object sender, DragDeltaEventArgs e) {
			if(Height + e.VerticalChange < MinHeight)
				return;

			Height += e.VerticalChange;
		}

		#endregion

		#region Control Drag Event

		/// <summary>
		/// Handles the DragDelta event of the dragThumb control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
		private void dragThumb_DragDelta(object sender, DragDeltaEventArgs e) {
			if(WindowState == WindowState.Maximized)
				return;

			double newLeft = Left + e.HorizontalChange,
				newTop = Top + e.VerticalChange;
			this.Left = newLeft;
			this.Top = newTop;
		}

		#endregion

		#region Dependency Property Events

		/// <summary>
		/// Dependency property event once the focused value has changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void IsSelectedValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
			var self = (MdiChild)sender;
			self.OnSelected(new RoutedEventArgs(SelectedEvent, sender));
		}

		/// <summary>
		/// Dependency property event once the minimize box value has changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void IsMinimizableValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
			MdiChild mdiChild = (MdiChild)sender;
			bool enable = (bool)e.NewValue;

			if(!enable && mdiChild.WindowState == WindowState.Minimized) {
				mdiChild.WindowState = WindowState.Normal;
			}
		}

		/// <summary>
		/// Dependency property event once the maximize box value has changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void IsMaximizableValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
			MdiChild mdiChild = (MdiChild)sender;
			bool enable = (bool)e.NewValue;

			if(!enable && mdiChild.WindowState == WindowState.Maximized) {
				mdiChild.WindowState = WindowState.Normal;
			}
		}

		/// <summary>
		/// Dependency property event once the windows state value has changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void WindowStateValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
			MdiChild mdiChild = (MdiChild)sender;

			WindowState previousWindowState = (WindowState)e.OldValue;
			WindowState windowState = (WindowState)e.NewValue;

			if(previousWindowState == windowState)
				return;

			switch(windowState) {
				case WindowState.Normal: {
						mdiChild.Top = mdiChild.RestoreBounds.Top;
						mdiChild.Left = mdiChild.RestoreBounds.Left;
						mdiChild.Width = mdiChild.RestoreBounds.Width;
						mdiChild.Height = mdiChild.RestoreBounds.Height;
					}
					break;
				case WindowState.Minimized: {
						if(previousWindowState == WindowState.Normal)
							mdiChild.RestoreBounds = new Rect(mdiChild.Left, mdiChild.Top, mdiChild.ActualWidth, mdiChild.ActualHeight);

						mdiChild.Width = MdiChild.MinimizedWidth;
						mdiChild.Height = MdiChild.MinimizedHeight;
					}
					break;
				case WindowState.Maximized: {
						if(previousWindowState == WindowState.Normal)
							mdiChild.RestoreBounds = new Rect(mdiChild.Left, mdiChild.Top, mdiChild.ActualWidth, mdiChild.ActualHeight);
						mdiChild.Focus();
					}
					break;
			}
		}

		#endregion

		/// <summary>
		/// Manually closes the child window.
		/// </summary>
		public void Close() {
			var eventArgs = new CancelEventArgs();
			this.OnClosing(eventArgs);

			if(eventArgs.Cancel)
				return;

			this.OnClosed(EventArgs.Empty);
		}

		protected virtual void OnClosing(CancelEventArgs e){
			var eh = this.Closing;
			if(eh != null){
				eh(this, e);
			}
		}

		public event CancelEventHandler Closing;

		protected virtual void OnClosed(EventArgs e){
			var eh = this.Closed;
			if(eh != null){
				eh(this, e);
			}
		}

		public event EventHandler Closed;

		public static readonly DependencyProperty DropDownMenuProperty =
			DependencyProperty.RegisterAttached("DropDownMenu", typeof(ContextMenu), typeof(MdiChild), new UIPropertyMetadata(null, DropDownMenuChanged));
		
		[AttachedPropertyBrowsableForType(typeof(UIElement))]
		public static ContextMenu GetDropDownMenu(DependencyObject obj){
			return (ContextMenu)obj.GetValue(DropDownMenuProperty);
		}
		
		[AttachedPropertyBrowsableForType(typeof(UIElement))]
		public static void SetDropDownMenu(DependencyObject obj, ContextMenu value){
			obj.SetValue(DropDownMenuProperty, value);
		}
		
		private static void DropDownMenuChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e){
			var button = (UIElement)sender;
			
			if(e.OldValue != null){
				button.MouseLeftButtonDown -= Button_Click;
			}
			if(e.NewValue != null){
				button.MouseLeftButtonDown += Button_Click;
			}
		}
		
		private static void Button_Click(object sender, RoutedEventArgs e){
			var button = (UIElement)sender;
			ContextMenu menu = GetDropDownMenu(button);
			if(menu != null){
				menu.PlacementTarget = button;
				menu.Placement = PlacementMode.Bottom; 
				menu.IsOpen = true;
				e.Handled = true;
			}
		}

		public static readonly RoutedEvent SelectedEvent = EventManager.RegisterRoutedEvent("Selected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MdiChild));

		public event RoutedEventHandler Selected{
			add{
				this.AddHandler(SelectedEvent, value);
			}
			remove{
				this.RemoveHandler(SelectedEvent, value);
			}
		}

		protected virtual void OnSelected(RoutedEventArgs e){
			this.RaiseEvent(e);
		}
	}
}