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

namespace MazeGenSL {
	public class ValidationErrorMessage : MessageBase{
		public bool HasError{
			get{
				return this.ErrorCount != 0;
			}
		}
		public int ErrorCount{get; private set;}
		public ValidationErrorEventAction Action{get; private set;}

		public ValidationErrorMessage(object sender, ValidationErrorEventAction action, int errorCount) : base(sender){
			this.Action = action;
			this.ErrorCount = errorCount;
		}
	}
}
