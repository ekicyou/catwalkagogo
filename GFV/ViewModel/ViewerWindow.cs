using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace GFV.ViewModel{
	public class ViewerWindow : ViewModelBase{
		public ViewerWindow(){
		}

		private Viewer viewer = new Viewer();
		public Viewer Viewer{
			get{
				return this.viewer;
			}
		}

		private ICommand openFile;
		public ICommand OpenFile{
			get{
				return (this.openFile != null) ? this.openFile : (this.openFile = new RoutedCommand());
			}
		}

		private ICommand close;
		public ICommand Close{
			get{
				return (this.close != null) ? this.close : (this.close = new RoutedCommand());
			}
		}
	}
}
