/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace GFV.ViewModel{
	using Gfl = GflNet;
	using IO = System.IO;

	public class ViewerWindowViewModel : ViewModelBase{
		public Gfl::Gfl Gfl{get; private set;}

		public ViewerWindowViewModel(Gfl::Gfl gfl){
			this.Gfl = gfl;
		}

		private ViewerViewModel viewer;
		public ViewerViewModel Viewer{
			get{
				if(this.viewer == null){
					this.viewer = new ViewerViewModel(this.Gfl);
				}
				return this.viewer;
			}
		}

		#region Property

		private string _Title = "GFV";
		public string Title{
			get{
				return this._Title;
			}
			set{
				this._Title = value;
				this.OnPropertyChanged("Title");
			}
		}

		private Gfl::Bitmap _Icon;
		public Gfl::Bitmap Icon{
			get{
				return this._Icon;
			}
		}

		#endregion

		#region OpenFile

		private CancellationTokenSource _OpenFile_CancellationTokenSource;
		public void OpenFile(string file){
			file = IO.Path.GetFullPath(file);

			// Load bitmap;
			if(this._OpenFile_CancellationTokenSource != null){
				this._OpenFile_CancellationTokenSource.Cancel();
			}
			this._OpenFile_CancellationTokenSource = new CancellationTokenSource();
			var bitmap = this.Gfl.LoadMultiBitmap(file);
			bitmap.LoadParameters = this.Gfl.GetDefaultLoadParameters();
			bitmap.LoadParameters.BitmapType = Gfl::BitmapType.Bgra;
			bitmap.LoadParameters.Options = Gfl::LoadOptions.ForceColorModel | Gfl::LoadOptions.IgnoreReadError;

			var ui = TaskScheduler.FromCurrentSynchronizationContext();
			var task = new Task(delegate{
				var bmp = bitmap[0];
				double scaleW = (32d / (double)bmp.Width);
				double scaleH = (32d / (double)bmp.Height);
				var scale = Math.Min(scaleW, scaleH);
				this._Icon = bmp.Resize((int)Math.Round(bmp.Width * scale), (int)Math.Round(bmp.Height * scale), this.Viewer.ResizeMethod);
				this.OnPropertyChanged("Icon");
			}, this._OpenFile_CancellationTokenSource.Token);
			task.ContinueWith(delegate{
				this.Viewer.SourceBitmap = bitmap;
			}, this._OpenFile_CancellationTokenSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, ui);
			task.Start();
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
