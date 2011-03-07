/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassNet2.Channels{
	public enum ChannelState : uint{
		Stopped = 0,
		Playing = 1,
		Stalled = 2,
		Paused  = 3,
	}
}
