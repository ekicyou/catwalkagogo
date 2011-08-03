/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace GflNet{
	using IO = System.IO;

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

		internal void ToGflLoadParams(object sender, ref Gfl.GflLoadParams prms){
			prms.Options = this.Options;
			prms.ColorModel = this.BitmapType;
			prms.Origin = this.Origin;
			prms.FormatIndex = this.Format.Index;
			prms.Callbacks.Progress = this.GetProgressCallback(sender);
			prms.Callbacks.WantCancel = this.GetWantCancelCallback(sender);
			if(this.StreamToHandle != null){
				prms.Callbacks.Read += this.ReadCallback;
				prms.Callbacks.Tell += this.TellCallback;
				prms.Callbacks.Seek += this.SeekCallback;
			}
		}

		private int ReadCallback(IntPtr handle, byte[] buffer, int size){
			return this.StreamToHandle.Read(buffer, 0, size);
		}

		private int TellCallback(IntPtr handle){
			return (int)this.StreamToHandle.Position;
		}

		private int SeekCallback(IntPtr handle, int offset, SeekOrigin origin){
			switch(origin){
				case SeekOrigin.Begin: return (int)this.StreamToHandle.Seek(offset, IO::SeekOrigin.Begin);
				case SeekOrigin.Current: return (int)this.StreamToHandle.Seek(offset, IO::SeekOrigin.Current);
				case SeekOrigin.End: return (int)this.StreamToHandle.Seek(offset, IO::SeekOrigin.End);
				default: throw new ArgumentException("origin");
			}
		}

		internal IO::Stream StreamToHandle{get; set;}

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
