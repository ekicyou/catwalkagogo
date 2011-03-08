using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace GFV.ViewModel{
	public class ViewerViewModel : ViewModelBase{
		public ViewerViewModel(){
		}

		private ImageFile imageFile;
		public ImageFile ImageFile{
			get{
				return this.imageFile;
			}
			set{
				this.imageFile = value;
				this.OnPropertyChanged("ImageFile");
			}
		}


	}
}
