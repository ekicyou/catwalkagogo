/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GflNet {
	public delegate void GflProgressCallback(object sender, GflProgressEventArgs e);

	public class GflProgressEventArgs : EventArgs{
		public int ProgressPercentage{get; private set;}
		
		public GflProgressEventArgs(int percent){
			this.ProgressPercentage = percent;
		}
	}
}
