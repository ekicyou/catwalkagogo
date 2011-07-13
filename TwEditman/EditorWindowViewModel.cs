using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using CatWalk;
using CatWalk.Net.Twitter;
using CatWalk.Windows.ViewModel;

namespace TwEditman {
	public class EditorWindowViewModel : DataErrorInfoViewModelBase{
		private User _ReplyToUser;
		public User ReplyToUser {
			get {
				return this._ReplyToUser;
			}
			set {
				this.OnPropertyChanging("ReplyToUser");
				this._ReplyToUser = value;
				this.OnPropertyChanged("ReplyToUser");
			}
		}

		private string _StatusText;
		public string StatusText {
			get {
				return this._StatusText;
			}
			set {
				this.OnPropertyChanging("StatusText");
				if(value != null && StringInfo.GetTextElementEnumerator(value).ToSequence().Cast<string>().Count() > 140){
					this.SetError("StatusText", "Text is longer than 140 charactors.");
				}
				this._StatusText = value;
				this.OnPropertyChanged("StatusText");
			}
		}
		
	}
}
