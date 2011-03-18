using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GflNet{
	public class LoadParameters{
		internal LoadParameters(Gfl.GflLoadParams prms){
			this.BitmapType = prms.ColorModel;
			this.Options = prms.Options;
			this.Origin = prms.Origin;
		}
		
		public BitmapType BitmapType{get; set;}
		public LoadOptions Options{get; set;}
		public Origin Origin{get; set;}
		public event GflProgressEventHandler ProgressChanged;

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
			if(this.ProgressChanged != null){
				this.ProgressChanged(this, new GflProgressEventArgs(percentage));
			}
		}
	}
}
