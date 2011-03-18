/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GflNet {
	public enum ResizeMethod : int{
		Quick = 0,
		Bilinear = 1,
		Hermite = 2,
		Gaussian = 3,
		Bell = 4,
		BSpline = 5,
		Mitshell = 6,
		Lanczos = 7,
	}
}
