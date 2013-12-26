using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CatWalk.Heron.ViewModel.Windows {
	public class WindowViewModel : ViewViewModel {
		public WindowViewModel(ControlViewModel parent)
			: base(parent) {
		}

		private string _Title;
		public virtual string Title {
			get {
				return this._Title;
			}
			set {
				this._Title = value;
				this.OnPropertyChanged("Title");
			}
		}

		private double _Top;
		public virtual double Top {
			get {
				return this._Top;
			}
			set {
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
				this._Height = value;
				this.OnPropertyChanged("Height");
			}
		}

		private WindowState _WindowState = WindowState.Normal;
		public virtual WindowState WindowState {
			get {
				return this._WindowState;
			}
			set {
				this._WindowState = value;
				this.OnPropertyChanged("WindowState");
			}
		}

		public virtual Rect RestoreBounds {
			get {
				var m = new WindowMessages.RequestRestoreBoundsMessage(this);
				this.Messenger.Send(m, this);
				return m.Bounds;
			}
			set {
				var m = new WindowMessages.SetRestoreBoundsMessage(this, value);
				this.Messenger.Post(m, this);
			}
		}

		public Nullable<bool> DialogResult {
			get {
				var m = new WindowMessages.RequestDialogResultMessage(this);
				this.Messenger.Send(m, this);
				return m.DialogResult;
			}
			set {
				this.Messenger.Post(new WindowMessages.SetDialogResultMessage(this, value), this);
			}
		}
	}
}
