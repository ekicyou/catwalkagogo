/*
	$Id: Account.cs 51 2010-01-28 21:55:33Z catwalk $
*/
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MazeGenSL.Models {
	public struct BoardSize{
		public int X{get; private set;}
		public int Y{get; private set;}

		public BoardSize(int x, int y) : this(){
			this.X = x;
			this.Y = y;
		}
	}
}
