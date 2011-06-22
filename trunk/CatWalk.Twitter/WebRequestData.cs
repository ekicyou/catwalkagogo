using System;
using System.Net;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CatWalk.Twitter {
	public class PostingWebRequest{
		public WebRequest WebRequest{get; private set;}
		public byte[] RequestData{get; private set;}
				
		public PostingWebRequest(WebRequest req, byte[] data){
			if(req == null){
				throw new ArgumentNullException("req");
			}
			if(data == null){
				throw new ArgumentNullException("data");
			}
			this.WebRequest = req;
			this.RequestData = data;
		}
		
		public void Post(){
			if(this.RequestData == null){
				throw new InvalidOperationException();
			}
			using(Stream stream = this.WebRequest.GetRequestStream()){
				stream.Write(this.RequestData, 0, this.RequestData.Length);
			}
			this.RequestData = null;
		}

		public void Post(CancellationToken token){
			if(this.RequestData == null){
				throw new InvalidOperationException();
			}
			token.Register(this.WebRequest.Abort);
			using(Stream stream = this.WebRequest.GetRequestStream()){
				stream.Write(this.RequestData, 0, this.RequestData.Length);
			}
			this.RequestData = null;
		}
	}
}
