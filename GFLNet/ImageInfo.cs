﻿/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GflNet {
	public struct ImageInfo{
		private Format format;
		public int Width{get; private set;}
		public int Height{get; private set;}
		public int XDpi{get; private set;}
		public int YDpi{get; private set;}
		public int ImageCount{get; private set;}
		public string Description{get; private set;}
		public ColorModel ColorModel{get; private set;}
		public Compression Compression{get; private set;}
		public long Size{get; private set;}
		public int BitsPerComponent{get; private set;}
		public int ComponentsPerPixel{get; private set;}
		public string CompressionDescription{get; private set;}
		public int XOffset{get; private set;}
		public int YOffset{get; private set;}
		
		internal ImageInfo(Gfl gfl, Gfl.FileInformation info) : this(){
			this.format = gfl.GetGflFormat(info.FormatIndex);
			this.Width = info.Width;
			this.Height = info.Height;
			this.XDpi = info.Xdpi;
			this.YDpi = info.Ydpi;
			this.ImageCount = info.NumberOfImages;
			this.Description = info.Description;
			this.ColorModel = info.ColorModel;
			this.Compression = info.Compression;
			this.Size = info.FileSize;
			this.BitsPerComponent = info.BitsPerComponent;
			this.ComponentsPerPixel = info.ComponentsPerPixel;
			this.CompressionDescription = info.CompressionDescription;
			this.XOffset = info.XOffset;
			this.YOffset = info.YOffset;
		}
				
		public Format Format{
			get{
				return this.format;
			}
		}
	}
}