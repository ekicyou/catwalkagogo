using System;
using System.Net;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CatWalk.Twitter {
	public class GettingWebRequest{
		public WebRequest WebRequest{get; private set;}
		private Stream _ResponseStream;

		public GettingWebRequest(WebRequest req){
			if(req == null){
				throw new ArgumentNullException("req");
			}
			this.WebRequest = req;
		}

		public virtual Stream Get(){
			using(var response = this.WebRequest.GetResponse()){
				return response.GetResponseStream();
			}
		}
		public virtual Stream Get(CancellationToken token){
			token.Register(this.WebRequest.Abort);
			var result = this.WebRequest.BeginGetResponse(this.GetCallback, null);
			ThreadPool.RegisterWaitForSingleObject(result.AsyncWaitHandle, GetTimeoutCallback, null, this.WebRequest.Timeout, true);
			result.AsyncWaitHandle.WaitOne();
			return this._ResponseStream;
		}

		private virtual void GetCallback(IAsyncResult async){
			this._ResponseStream = this.WebRequest.EndGetResponse(async).GetResponseStream();
		}

		private void GetTimeoutCallback(object state, bool timedOut){
			if(timedOut){
				this.WebRequest.Abort();
			}
		}
	}

	public class PostingWebRequest : GettingWebRequest{
		public byte[] RequestData{get; private set;}

		public PostingWebRequest(WebRequest req, byte[] data) : base(req){
			if(data == null){
				throw new ArgumentNullException("data");
			}
			this.RequestData = data;
		}

		public override void Get() {
			// Not posted yet
			if(this.RequestData != null){
				throw new InvalidOperationException();
			}
			base.Get();
		}

		public override void Get(Action<Stream> callback, CancellationToken token) {
			// Not posted yet
			if(this.RequestData != null){
				throw new InvalidOperationException();
			}
			base.Get(callback, token);
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
			token.Register(this.WebRequest.Abort);
			var result = this.WebRequest.BeginGetRequestStream(this.PostCallback, this.RequestData);
			ThreadPool.RegisterWaitForSingleObject(result.AsyncWaitHandle, PostTimeoutCallback, null, this.WebRequest.Timeout, true);
			result.AsyncWaitHandle.WaitOne();
		}

		private void PostCallback(IAsyncResult async){
			var data = (byte[])async.AsyncState;
			using(Stream stream = this.WebRequest.EndGetRequestStream(async)){
				stream.Write(data, 0, data.Length);
			}
			this.RequestData = null;
		}

		private void PostTimeoutCallback(object state, bool timedOut){
			if(timedOut){
				this.WebRequest.Abort();
			}
		}
	}
}
