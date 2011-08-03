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

		/*
		private BitmapSource GetFrame(int index){
			return this.Decoder.Frames[index];
		}
		*/
		private void BitmapFrame_DownloadProgress(object sender, DownloadProgressEventArgs e) {
			this.OnProgressChanged(new ProgressEventArgs(e.Progress));
		}

		private void BitmapFrame_DownloadCompleted(object sender, EventArgs e) {
			this.OnLoadCompleted(e);
		}

		private void BitmapFrame_DecodeFailed(object sender, ExceptionEventArgs e) {
			this.OnLoadFailed(new BitmapLoadFailedEventArgs(e.ErrorException));
		}

		public override BitmapSource GetThumbnail() {
			return this.Decoder.Thumbnail;
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

		#region IDisposable Members

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private bool _IsDisposed = false;
		protected virtual void Dispose(bool disposing){
			if(!this._IsDisposed){
				this.Decoder.Dispatcher.InvokeShutdown();
				this._IsDisposed = true;
			}
		}

		~WicMultiBitmap(){
			this.Dispose(false);
		}

		#endregion
	}
}
