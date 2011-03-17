/*
	$Id$
*/
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace GflNet {
	public class ColorMap : ReadOnlyCollection<Color>{
		internal ColorMap(IntPtr hMap) : base(new List<Color>(256)){
			var map = new Gfl.GflColorMap();
			Marshal.PtrToStructure(hMap, map);
			for(var i = 0; i < 256; i++){
				this.Items.Add(new Color(map.Red[i], map.Green[i], map.Blue[i]));
			}
		}
	}

	public struct Color{
		public int Red{get; private set;}
		public int Green{get; private set;}
		public int Blue{get; private set;}
		public int Alpha{get; private set;}

		public Color(int r, int g, int b): this(r, g, b, 0){}
		public Color(int r, int g, int b, int a) : this(){
			this.Red = r;
			this.Green = g;
			this.Blue = b;
			this.Alpha = a;
		}
	}
}
