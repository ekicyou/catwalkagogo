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
using System.Runtime.Serialization;

namespace Nyoroge {
	[DataContract]
	public struct GameResult {
		[DataMember]
		public int SnakeLength{get; set;}
		[DataMember]
		public TimeSpan Duration{get; set;}
		[DataMember]
		public DateTime PlayDate{get; set;}

		public GameResult(int snakeLength, TimeSpan duration) : this(snakeLength, duration, DateTime.Now){}
		public GameResult(int snakeLength, TimeSpan duration, DateTime playDate) : this(){
			this.SnakeLength = snakeLength;
			this.Duration = duration;
			this.PlayDate = playDate;
		}
	}
}
