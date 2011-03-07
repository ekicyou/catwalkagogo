/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassNet2 {
	public class BassException : Exception{
		public BassErrorCode ErrorCode{get; private set;}
		
		public BassException(){
			this.ErrorCode = BassErrorCode.Unknown;
		}
		
		public BassException(BassErrorCode code){
			this.ErrorCode = code;
		}
		
		public BassException(string message) : base(message){
		}
		
		protected BassException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context){
		}
		
		public BassException(string message, Exception innerException) : base(message, innerException){
		}
	}
}
