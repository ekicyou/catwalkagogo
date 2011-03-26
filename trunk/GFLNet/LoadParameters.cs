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
		public BitmapType BitmapType{get; set;}
		public LoadOptions Options{get; set;}
		public Origin Origin{get; set;}
		public Format Format{get; set;}

		internal LoadParameters(Gfl.GflLoadParams prms){
			this.BitmapType = prms.ColorModel;
			this.Options = prms.Options;
			this.Origin = prms.Origin;
			this.Format = Format.AnyFormats;
		}

		#region Callbacks

		public event ProgressEventHandler ProgressChanged;
		public event CancelEventHandler WantCancel;

		internal Gfl.ProgressCallback GetProgressCallback(object sender){
			if(this.ProgressChanged != null){
				return new Gfl.ProgressCallback(delegate(int percentage, IntPtr args){
					var eh = this.ProgressChanged;
					if(eh != null){
						eh(sender, new ProgressEventArgs(percentage));
					}
				});
			}else{
				return null;
			}
		}

		internal Gfl.WantCancelCallback GetWantCancelCallback(object sender){
			if(this.WantCancel != null){
				return new Gfl.WantCancelCallback(delegate(IntPtr args){
					var eh = this.WantCancel;
					if(eh != null){
						var e = new CancelEventArgs();
						eh(sender, e);
						return e.Cancel;
					}else{
						return false;
					}
				});
			}else{
				return null;
			}
		}

		#endregion
	}
}
