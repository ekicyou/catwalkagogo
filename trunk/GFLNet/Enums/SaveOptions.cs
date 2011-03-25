/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GflNet {
	[Flags]
	public enum SaveOptions : uint{
		ReplaceExtension = 0x00000001,
		WantFilename     = 0x00000002,
		SaveAnyway       = 0x00000004,
		SaveIccProfile   = 0x00000008,
	}
}
