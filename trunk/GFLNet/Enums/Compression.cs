/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GflNet {
	public enum Compression : ushort{
		None         = 0,
		RLE          = 1,
		LZW          = 2,
		JPEG         = 3,
		ZIP          = 4,
		SGIRLE       = 5,
		CCITTRLE     = 6,
		CCITTFAX3    = 7,
		CCITTFAX32D  = 8,
		CCITTFAX4    = 9,
		Wavelet      = 10,
		LZWPredictor = 11,
		Unknown      = 255,
	}
}
