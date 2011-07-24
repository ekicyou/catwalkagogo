/*
	$Id: Account.cs 51 2010-01-28 21:55:33Z catwalk $
*/
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using MazeGenSL.Models;

namespace MazeGenSL.ViewModels {
	public class MainViewModel : ViewModelDataErrorInfoBase ,IDisposable{
		public MainViewModel(){
			this._BoardX = this.Board.Size.X;
			this._BoardY = this.Board.Size.Y;

			Messenger.Default.Register<ValidationErrorMessage>(this.RecieveValidationErrorMessage, this);
		}

		private void RecieveValidationErrorMessage(ValidationErrorMessage mes){
			this.HasViewError = mes.HasError;
		}
	
		#region Properties

		private BoardViewModel _Board = new BoardViewModel();
		public BoardViewModel Board{
			get{
				return this._Board;
			}
		}

		public ProgressManager ProgressManager{
			get{
				return App.Default.ProgressManager;
			}
		}

		private bool _HasViewError = false;
		private bool HasViewError{
			get{
				return this._HasViewError;
			}
			set{
				this._HasViewError = value;
				this.ResizeBoardCommand.RaiseCanExecuteChanged();
			}
		}

		#endregion

		#region Board

		private int _BoardX;
		public int BoardX{
			get{
				return this._BoardX;
			}
			set{
				if(value < 4){
					this.SetError("BoardX", "Board size must be larger than 3.");
				}else{
					this.ClearError("BoardX");
				}
				this._BoardX = value;
				this.OnPropertyChanged("BoardX");
				this.ResizeBoardCommand.RaiseCanExecuteChanged();
			}
		}
		private int _BoardY;
		public int BoardY{
			get{
				return this._BoardY;
			}
			set{
				if(value < 4){
					this.SetError("BoardY", "Board size must be larger than 3.");
				}else{
					this.ClearError("BoardY");
				}
				this._BoardY = value;
				this.OnPropertyChanged("BoardY");
				this.ResizeBoardCommand.RaiseCanExecuteChanged();
			}
		}

		#endregion

		#region Resize

		private DelegateCommand _ResizeBoardCommand;
		public DelegateCommand ResizeBoardCommand{
			get{
				return this._ResizeBoardCommand ?? (this._ResizeBoardCommand = new DelegateCommand(this.ResizeBoard, this.CanResizeBoard));
			}
		}

		public void ResizeBoard(){
			if(this.HasError){
				throw new InvalidOperationException();
			}
			this.Board.Size = new BoardSize(this.BoardX, this.BoardY);
		}

		public bool CanResizeBoard(){
			return !this.HasError && !this.HasViewError;
		}

		#endregion

		#region IDisposable Members

		~MainViewModel(){
			this.Dispose(false);
		}

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private bool _Disposed = false;
		protected virtual void Dispose(bool disposing){
			if(!this._Disposed){
				Messenger.Default.Unregister<ValidationErrorMessage>(this.RecieveValidationErrorMessage, this);
				this._Disposed = true;
			}
		}

		#endregion
	}
}
