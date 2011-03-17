using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace GFV.ViewModel{
	using Gfl = GflNet;
	using IO = System.IO;

	public class ViewerViewModel : ViewModelBase{
		public Gfl::Gfl Gfl{get; private set;}

		public ViewerViewModel(Gfl::Gfl gfl){
			this.Gfl = gfl;
		}

		private Gfl::MultiBitmap _MultiBitmap = null;
		public Gfl::MultiBitmap MultiBitmap{
			get{
				return this._MultiBitmap;
			}
			set{
				this._MultiBitmap = value;
				this.OnPropertyChanged("MultiBitmap");
			}
		}

		public void LoadFile(string file){
			file = IO.Path.GetFullPath(file);
			this.MultiBitmap = this.Gfl.LoadMultiBitmap(file);
		}

		private int frameIndex = 0;
		public int FrameIndex{
			get{
				return this.frameIndex;
			}
			set{
				if(this.MultiBitmap == null){
					throw new InvalidOperationException();
				}
				if(value < 0 || this.MultiBitmap.FrameCount <= value){
					throw new ArgumentOutOfRangeException();
				}
				this.frameIndex = value;
				this.OnPropertyChanged("FrameIndex");
			}
		}
	}
}
