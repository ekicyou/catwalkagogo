using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace GFV.ViewModel{
	public class Viewer : ViewModelBase{
		public Viewer(){
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

		#region Command

		private ICommand nextFile;
		public ICommand NextFile{
			get{
				return (this.nextFile != null) ? this.nextFile : (this.nextFile = new RoutedCommand());
			}
		}

		private ICommand prevFile;
		public ICommand PrevFile{
			get{
				return (this.prevFile != null) ? this.prevFile : (this.prevFile = new RoutedCommand());
			}
		}

		private ICommand zoomIn;
		public ICommand ZoomIn{
			get{
				return (this.zoomIn != null) ? this.zoomIn : (this.zoomIn = new RoutedCommand());
			}
		}

		private ICommand zoomOut;
		public ICommand ZoomOut{
			get{
				return (this.zoomOut != null) ? this.zoomOut : (this.zoomOut = new RoutedCommand());
			}
		}

		private ICommand fitImage;
		public ICommand FitImage{
			get{
				return (this.fitImage != null) ? this.fitImage : (this.fitImage = new RoutedCommand());
			}
		}

		#endregion
	}
}
