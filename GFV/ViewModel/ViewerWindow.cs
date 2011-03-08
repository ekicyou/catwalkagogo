using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace GFV.ViewModel{
	public class ViewerWindowViewModel : ViewModelBase{
		public ViewerWindowViewModel(){
		}

		private ViewerViewModel viewer = new ViewerViewModel();
		public ViewerViewModel Viewer{
			get{
				return this.viewer;
			}
		}
	}
}
