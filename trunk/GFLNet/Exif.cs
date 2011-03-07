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
	public class Exif{
		private ReadOnlyCollection<ExifEntry> entries;
		
		internal Exif(Gfl.ExifData exif){
			ExifEntry[] entries = new ExifEntry[exif.NumberOfItems];
			for(int i = 0; i < exif.NumberOfItems; i++){
				entries[i] = new ExifEntry(exif.ItemList[i]);
			}
			this.entries = new ReadOnlyCollection<ExifEntry>(entries);
		}
		
		public ReadOnlyCollection<ExifEntry> Entries{
			get{
				return this.entries;
			}
		}
	}

}
