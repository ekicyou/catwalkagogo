/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GflNet {
	public delegate void ProgressEventHandler(object sender, ProgressEventArgs e);

	public class ProgressEventArgs : EventArgs{
		public int ProgressPercentage{get; private set;}
		
		public ProgressEventArgs(int percent){
			this.ProgressPercentage = percent;
		}
	}
}
