using System;
using System.Net;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CatWalk.Twitter {
	public class WebRequestData{
		public WebRequest WebRequest{get; private set;}
		public byte[] RequestData{get; private set;}
		
		public WebRequestData(WebRequest req) : this(req, null){
		}
		
		public WebRequestData(WebRequest req, byte[] data){
			this.WebRequest = req;
			this.RequestData = data;
		}
		
		public void WriteRequestData(){
			if(this.RequestData == null){
				return;
			}
			using(Stream stream = this.WebRequest.GetRequestStream()){
				stream.Write(this.RequestData, 0, this.RequestData.Length);
			}
		}
	}
}
