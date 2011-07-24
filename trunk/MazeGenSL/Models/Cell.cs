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
	public enum Cell{
		Wall = 0,
		Road = 1,
		Route = 2,
		Start = 3,
		Goal = 4,
	}
}
