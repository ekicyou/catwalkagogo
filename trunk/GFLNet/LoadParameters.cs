/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace GflNet{
	[Serializable]
	public class LoadParameters{
		internal LoadParameters(Gfl.GflLoadParams prms){
			this.BitmapType = prms.ColorModel;
			this.Options = prms.Options;
			this.Origin = prms.Origin;
		}
		
		public BitmapType BitmapType{get; set;}
		public LoadOptions Options{get; set;}
		public Origin Origin{get; set;}
		public event ProgressEventHandler ProgressChanged;
		public event CancelEventHandler WantCancel;

		internal Gfl.ProgressCallback ProgressCallback{
			get{
				if(this.ProgressChanged != null){
					return new Gfl.ProgressCallback(this.ProgressCallbackHandler);
				}else{
					return null;
				}
			}
		}

		private void ProgressCallbackHandler(int percentage, IntPtr args){
			var eh = this.ProgressChanged;
			if(eh != null){
				eh(this, new ProgressEventArgs(percentage));
			}
		}

		internal Gfl.WantCancelCallback WantCancelCallback{
			get{
				if(this.WantCancel != null){
					return new Gfl.WantCancelCallback(this.WantCancelCallbackHandler);
				}else{
					return null;
				}
			}
		}

		private bool WantCancelCallbackHandler(IntPtr args){
			var eh = this.WantCancel;
			if(eh != null){
				var e = new CancelEventArgs();
				eh(this, e);
				return e.Cancel;
			}else{
				return false;
			}
		}
	}
}
