﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace GFV.Imaging {
	public class WicMultiBitmap : CachedMultiBitmap, IDisposable{
		public BitmapDecoder Decoder{get; private set;}

		public WicMultiBitmap(BitmapDecoder dec) : base(dec.Frames.Count){
			this.Decoder = dec;
		}

		protected override void DisposeWrappedDecoder() {
			this.Decoder.Dispatcher.InvokeShutdown();
			this.Decoder = null;
		}

		protected override BitmapSource LoadFrame(int index){
			this.OnLoadStarted(EventArgs.Empty);
			this.OnProgressChanged(new ProgressEventArgs(Double.NaN));
			try{
				BitmapSource frame = this.Decoder.Frames[index];
				if(frame.Format != PixelFormats.Bgr32 || frame.Format != PixelFormats.Bgra32 || frame.Format != PixelFormats.Pbgra32){
					frame = new FormatConvertedBitmap(frame, PixelFormats.Bgra32, frame.Palette, 0);
				}
				var cache = new WriteableBitmap(frame);
				cache.Freeze();
				this.Decoder.Dispatcher.InvokeShutdown();
				this.OnLoadCompleted(EventArgs.Empty);
				return cache;
			}catch(Exception ex){
				this.OnLoadFailed(new BitmapLoadFailedEventArgs(ex));
				throw ex;
			}
		}

		private void BitmapFrame_DownloadProgress(object sender, DownloadProgressEventArgs e) {
			this.OnProgressChanged(new ProgressEventArgs(e.Progress));
		}

		private void BitmapFrame_DownloadCompleted(object sender, EventArgs e) {
			this.OnLoadCompleted(e);
		}

		private void BitmapFrame_DecodeFailed(object sender, ExceptionEventArgs e) {
			this.OnLoadFailed(new BitmapLoadFailedEventArgs(e.ErrorException));
		}

		protected override BitmapSource LoadThumbnail() {
			try{
				return this.Decoder.Thumbnail ?? this[0];
			}catch(NotSupportedException){ // thrown by HD Photo Codec
				return this[0];
			}
		}

		public override bool IsLoading {
			get {
				return false;
			}
		}

		public override bool IsAnimated {
			get { return false; }
		}

		public override int[] DelayTimes {
			get { throw new NotImplementedException(); }
		}

		public override int LoopCount {
			get { throw new NotImplementedException(); }
		}

		#region IDisposable Members

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private bool _IsDisposed = false;
		protected virtual void Dispose(bool disposing){
			if(!this._IsDisposed){
				if(this.Decoder != null){
					this.Decoder.Dispatcher.InvokeShutdown();
				}
				this._IsDisposed = true;
			}
		}

		~WicMultiBitmap(){
			this.Dispose(false);
		}

		#endregion
	}
}
