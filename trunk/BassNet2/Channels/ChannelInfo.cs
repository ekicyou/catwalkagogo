/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassNet2.Channels{
	public class BassChannelInfo{
		public int Frequency{get; private set;}
		public int Channels{get; private set;}
		public ChannelType ChannelType{get; private set;}
		public int OriginalResolution{get; private set;}
		public string Filename{get; private set;}
		
		internal BassChannelInfo(Bass.ChannelInfo info){
			this.Frequency = info.Frequency;
			this.Channels = info.Channels;
			this.ChannelType = info.ChannelType;
			this.OriginalResolution = info.OriginalResolution;
			this.Filename = info.Filename;
		}
	}
}
