using System;
using System.Net;
using System.Linq;
using System.IO;
using System.Threading;

namespace CatWalk.Net {
	public class GettingWebRequest{
		public WebRequest WebRequest{get; private set;}
		private Stream _ResponseStream;
		private bool _IsTimedout;
		protected Exception AsyncException{get; set;}
		private readonly object _SyncObject = new object();

		public GettingWebRequest(WebRequest req){
			if(req == null){
				throw new ArgumentNullException("req");
			}
			this.WebRequest = req;
			this._IsTimedout = false;
		}

		public virtual Stream Get(){
			return this.WebRequest.GetResponse().GetResponseStream();
		}
		public virtual Stream Get(CancellationToken token){
			if(token == CancellationToken.None){
				return Get();
			}
			token.Register(this.WebRequest.Abort);
			var result = this.WebRequest.BeginGetResponse(this.GetCallback, null);
			this.WaitAndTimeoutRequest(result);
			lock(this._SyncObject){
				if(this._ResponseStream == null){
					throw new WebException("Request was failed.", new IOException());
				}
				return this._ResponseStream;
			}
		}

		private void GetCallback(IAsyncResult async){
			try{
				lock(this._SyncObject){
					this._ResponseStream = this.WebRequest.EndGetResponse(async).GetResponseStream();
					// null was returned ?
					if(this._ResponseStream == null){
						throw new IOException();
					}
				}
			}catch(Exception ex){
				this.AsyncException = ex;
			}
		}

		private void TimeoutCallback(object state, bool timedOut){
			if(timedOut){
				this.WebRequest.Abort();
				this._IsTimedout = true;
			}
		}

		protected void WaitAndTimeoutRequest(IAsyncResult async){
			ThreadPool.RegisterWaitForSingleObject(async.AsyncWaitHandle, TimeoutCallback, null, this.WebRequest.Timeout, true);
			async.AsyncWaitHandle.WaitOne();
			if(this.AsyncException != null){
				throw new WebException("Request was failed", this.AsyncException);
			}
			if(this._IsTimedout){
				if(this.AsyncException is WebException){
					throw this.AsyncException;
				}else{
					throw new WebException("Request was timeout", WebExceptionStatus.Timeout);
				}
			}
		}
	}
}
