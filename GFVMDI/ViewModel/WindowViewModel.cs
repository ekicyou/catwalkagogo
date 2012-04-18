using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatWalk.Mvvm;
using GFV.Messaging;
using System.Windows;
using System.Windows.Media.Imaging;

namespace GFV.ViewModel {
	[SendMessage(typeof(RequestRestoreBoundsMessage))]
	public class WindowViewModel : ViewModelBase{
		private string _Title;
		public virtual string Title{
			get{
				return this._Title;
			}
			set{
				this.OnPropertyChanging("Title");
				this._Title = value;
				this.OnPropertyChanged("Title");
			}
		}

		private BitmapSource _Icon;
		public virtual BitmapSource Icon{
			get{
				return this._Icon;
			}
			set{
				this.OnPropertyChanging("Icon");
				this._Icon = value;
				this.OnPropertyChanged("Icon");
			}
		}

		private double _Top;
		public virtual double Top {
			get {
				return this._Top;
			}
			set {
				this.OnPropertyChanging("Top");
				this._Top = value;
				this.OnPropertyChanged("Top");
			}
		}
		

		private double _Left;
		public virtual double Left {
			get {
				return this._Left;
			}
			set {
				this.OnPropertyChanging("Left");
				this._Left = value;
				this.OnPropertyChanged("Left");
			}
		}

		private double _Width = 480;
		public virtual double Width {
			get {
				return this._Width;
			}
			set {
				this.OnPropertyChanging("Width");
				this._Width = value;
				this.OnPropertyChanged("Width");
			}
		}

		private double _Height = 320;
		public virtual double Height {
			get {
				return this._Height;
			}
			set {
				this.OnPropertyChanging("Height");
				this._Height = value;
				this.OnPropertyChanged("Height");
			}
		}

		private WindowState _WindowState = WindowState.Normal;
		public virtual WindowState WindowState{
			get{
				return this._WindowState;
			}
			set{
				this.OnPropertyChanging("WindowState");
				this._WindowState = value;
				this.OnPropertyChanged("WindowState");
			}
		}

		public virtual Rect RestoreBounds{
			get{
				var m = new RequestRestoreBoundsMessage(this);
				Messenger.Default.Send(m, this);
				return m.Bounds;
			}
			set{
				Messenger.Default.Send(new SetRestoreBoundsMessage(this, value), this);
			}
		}
	}
}
