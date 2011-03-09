using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace GFV.ViewModel{
	using Gfl = GflNet;

	public class ViewerViewModel : ViewModelBase{
		public ViewerViewModel(){
		}

		private Gfl::MultiBitmap multiBitmap = null;
		public Gfl::MultiBitmap MultiBitmap{
			get{
				return this.multiBitmap;
			}
			set{
				this.multiBitmap = value;
				this.OnPropertyChanged("MultiBitmap");
			}
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
