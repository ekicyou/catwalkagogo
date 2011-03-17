/*
	$Id$
*/
using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace GflNet {
	public class Exif : ReadOnlyCollection<ExifEntry>{
		internal Exif(Gfl.GflExifData exif) : base(new List<ExifEntry>(exif.NumberOfItems)){
			for(int i = 0; i < exif.NumberOfItems; i++){
				this.Items.Add(new ExifEntry(exif.ItemList[i]));
			}
		}
	}
}
