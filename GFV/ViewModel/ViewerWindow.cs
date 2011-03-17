using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.ComponentModel;

namespace GFV.ViewModel{
	using Gfl = GflNet;

	public class ViewerWindowViewModel : ViewModelBase{
		public Gfl::Gfl Gfl{get; private set;}

		public ViewerWindowViewModel(Gfl::Gfl gfl){
			this.Gfl = gfl;
			this.viewer = new ViewerViewModel(this.Gfl);
		}

		private ViewerViewModel viewer;
		public ViewerViewModel Viewer{
			get{
				return this.viewer;
			}
		}

		#region OpenFile

		public void OpenFile(string file){
			this.Viewer.LoadFile(file);
		}

		public IOpenFileDialog OpenFileDialog{get; set;}

		private DelegateCommand _OpenFileCommand;
		public ICommand OpenFileCommand{
			get{
				if(this._OpenFileCommand == null){
					this._OpenFileCommand = new DelegateCommand(delegate{
						if(this.OpenFileDialog != null){
							var dlg = this.OpenFileDialog;
							if(dlg.ShowDialog().Value){
								var file = dlg.FileName;
								if(!String.IsNullOrEmpty(file)){
									this.OpenFile(file);
								}
							}
						}
					});
				}
				return this._OpenFileCommand;
			}
		}

		#endregion

		#region Close

		public event EventHandler RequestClose;

		private DelegateCommand _CloseCommand;
		public ICommand CloseCommand{
			get{
				if(this._CloseCommand == null){
					this._CloseCommand = new DelegateCommand(delegate{
						var handler = this.RequestClose;
						if(handler != null){
							handler(this, EventArgs.Empty);
						}
					});
				}
				return this._CloseCommand;
			}
		}

		#endregion
	}
}
