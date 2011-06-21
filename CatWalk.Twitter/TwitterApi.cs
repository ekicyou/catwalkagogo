/*
	$Id: TwitterAPI.cs 85 2010-06-09 13:51:04Z catwalk $
*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using CatWalk.OAuth;

namespace CatWalk.Twitter{
	/// <summary>
	/// TwitterAPIのプリミティブな関数群。
	/// </summary>
	public partial class TwitterApi{
		private static Lazy<TwitterApi> _Default;
		static TwitterApi(){
			_Default = new Lazy<TwitterApi>(() => new TwitterApi());
		}

		public static TwitterApi Default{
			get{
				return _Default.Value;
			}
		}
		
		public Consumer Consumer{get; private set;}

		private TwitterApi(){
		}

		public TwitterApi(Consumer consumer){
			ServicePointManager.Expect100Continue = false;
			Consumer = consumer;
		}

		#region 通信プロパティ

		public int Timeout{get; set;}

		#endregion

		#region API

		public WebRequestData UpdateStatus(AccessToken token, string status, ulong replyTo, string source){
			const string url = "http://api.twitter.com/1/statuses/update.xml";
			List<Parameter> prms = new List<Parameter>();
			prms.Add(new Parameter("status", status));
			prms.Add(new Parameter("source", source));
			if(replyTo > 0){
				prms.Add(new Parameter("in_reply_to_status_id", replyTo.ToString()));
			}
			return Post(url, prms.ToArray(), token);
		}
		
		public WebRequestData VerifyCredential(AccessToken token){
			const string url = "http://api.twitter.com/1/account/verify_credentials.xml";
			return Get(url, new Parameter[0], token);
		}
		
		public WebRequestData GetHomeTimeline(AccessToken token, int count, int page, ulong sinceId, ulong maxId){
			const string url = "http://api.twitter.com/1/statuses/home_timeline.xml";
			List<Parameter> prms = new List<Parameter>();
			if(count > 0){
				prms.Add(new Parameter("count", count.ToString()));
			}
			if(page > 0){
				prms.Add(new Parameter("page", page.ToString()));
			}
			if(sinceId > 0){
				prms.Add(new Parameter("since_id", sinceId.ToString()));
			}
			if(maxId > 0){
				prms.Add(new Parameter("max_id", maxId.ToString()));
			}
			return Get(url, prms.ToArray(), token);
		}
		
		public WebRequestData GetUserTimeline(string id, int count, int page, ulong sinceId, ulong maxId){
			const string url = "http://api.twitter.com/1/statuses/user_timeline.xml";
			List<Parameter> prms = new List<Parameter>();
			if(!String.IsNullOrEmpty(id)){
				prms.Add(new Parameter("id", id));
			}
			if(count > 0){
				prms.Add(new Parameter("count", count.ToString()));
			}
			if(page > 0){
				prms.Add(new Parameter("page", page.ToString()));
			}
			if(sinceId > 0){
				prms.Add(new Parameter("since_id", sinceId.ToString()));
			}
			if(maxId > 0){
				prms.Add(new Parameter("maxId", maxId.ToString()));
			}
			return Get(url, prms.ToArray());
		}

		#endregion

		#region Favorite / Block / Follow

		public WebRequestData DestroyStatus(AccessToken token, ulong id){
			const string url = "http://api.twitter.com/1/statuses/destroy";
			Parameter[] prms = new Parameter[1]{new Parameter("id", id.ToString())};
			return Post(url + "/" + id.ToString() + ".xml", prms, token);
		}
		
		public WebRequestData CreateFavorite(AccessToken token, ulong id){
			const string url = "http://api.twitter.com/1/favorites/create";
			Parameter[] prms = new Parameter[1]{new Parameter("id", id.ToString())};
			return Post(url + "/" + id.ToString() + ".xml", prms, token);
		}
		
		public WebRequestData CreateBlock(AccessToken token, ulong id){
			const string url = "http://api.twitter.com/1/blocks/create";
			Parameter[] prms = new Parameter[1]{new Parameter("id", id.ToString())};
			return Post(url + "/" + id.ToString() + ".xml", prms, token);
		}
		
		public WebRequestData DestroyBlock(AccessToken token, ulong id){
			const string url = "http://api.twitter.com/1/blocks/destroy";
			Parameter[] prms = new Parameter[1]{new Parameter("id", id.ToString())};
			return Post(url + "/" + id.ToString() + ".xml", prms, token);
		}
		
		public WebRequestData CreateFriendship(AccessToken token, ulong id){
			const string url = "http://api.twitter.com/1/friendships/create";
			Parameter[] prms = new Parameter[1]{new Parameter("id", id.ToString())};
			return Post(url + "/" + id.ToString() + ".xml", prms, token);
		}
		
		public WebRequestData DestroyFriendship(AccessToken token, ulong id){
			const string url = "http://api.twitter.com/1/friendships/destroy";
			Parameter[] prms = new Parameter[1]{new Parameter("id", id.ToString())};
			return Post(url + "/" + id.ToString() + ".xml", prms, token);
		}

		#endregion

		#region Tweets

		public WebRequestData Retweet(AccessToken token, ulong id){
			const string url = "http://api.twitter.com/1/statuses/retweet/";
			return Post(url + id.ToString() + ".xml", new Parameter[0], token);
		}

		public WebRequestData ShowStatus(ulong id, bool trimUser, bool includeEntities){
			const string url = "http://api.twitter.com/version/statuses/show/";
			var prms = new List<Parameter>();
			if(trimUser){
				prms.Add(new Parameter("trim_user", "1"));
			}
			if(includeEntities){
				prms.Add(new Parameter("include_entities", "1"));
			}
			return Get(url + id.ToString() + ".xml", prms.ToArray());
		}



		#endregion

		#region User

		public WebRequestData SearchUsers(AccessToken token, string searchWord, int count, int page){
			const string url = "http://api.twitter.com/1/users/search.xml";
			List<Parameter> prms = new List<Parameter>();
			prms.Add(new Parameter("q", searchWord));
			if(count > 0){
				prms.Add(new Parameter("per_page", count.ToString()));
			}
			if(page > 0){
				prms.Add(new Parameter("page", page.ToString()));
			}
			return Get(url, prms.ToArray(), token);
		}
		
		public WebRequestData ShowUser(ulong id){
			return ShowUser(id.ToString());
		}
		
		public WebRequestData ShowUser(string name){
			const string url = "http://api.twitter.com/1/users/show.xml";
			return Get(url, new Parameter[]{new Parameter("id", name)});
		}
		
		public WebRequestData GetFollowers(AccessToken token, ulong user_id){
			const string url = "http://api.twitter.com/1/followers/ids.xml";
			return Get(url, new Parameter[]{new Parameter("user_id", user_id.ToString())}, token);
		}
		
		public WebRequestData GetFollowers(AccessToken token, ulong user_id, long cursor){
			const string url = "http://api.twitter.com/1/followers/ids.xml";
			return Get(url, new Parameter[]{new Parameter("user_id", user_id.ToString()), new Parameter("cursor", cursor.ToString())}, token);
		}
		
		public WebRequestData GetFriends(AccessToken token, ulong user_id){
			const string url = "http://api.twitter.com/1/friends/ids.xml";
			return Get(url, new Parameter[]{new Parameter("user_id", user_id.ToString())}, token);
		}
		
		public WebRequestData GetFriends(AccessToken token, ulong user_id, long cursor){
			const string url = "http://api.twitter.com/1/friends/ids.xml";
			return Get(url, new Parameter[]{new Parameter("user_id", user_id.ToString()), new Parameter("cursor", cursor.ToString())}, token);
		}
		
		public WebRequestData GetBlocks(AccessToken token, int page){
			const string url = "http://api.twitter.com/1/blocks/blocking.xml";
			return Get(url, new Parameter[]{new Parameter("page", page.ToString())}, token);
		}

		#endregion

		#region User

		public WebRequestData GetLists(ulong id){
			return this.GetLists(id, -1);
		}

		public WebRequestData GetLists(ulong id, long cursor){
			const string url = "http://api.twitter.com/1/lists.xml";
			return Get(url, new Parameter[]{
				new Parameter("user_id", id.ToString()),
				new Parameter("cursor", cursor.ToString())});
		}

		#endregion

		#region 通信

		private WebRequestData Get(string url, Parameter[] prms){
			HttpWebRequest req = GetWebRequest(url + ((prms.Length > 0) ? ("?" + Parameter.ConCat(prms)) : ""), prms);
			req.Method = "GET";
			
			return new WebRequestData(req);
		}
		
		private WebRequestData Post(string url, Parameter[] prms){
			string query = Parameter.ConCat(prms);
			byte[] data = Encoding.ASCII.GetBytes(query);
			
			HttpWebRequest req = GetWebRequest(url, prms);
			req.Method = "POST";
			req.ContentType = "application/x-www-form-urlencoded";
			req.ContentLength = query.Length;
			
			return new WebRequestData(req, data);
		}
		
		private WebRequestData Get(string url, Parameter[] prms, AccessToken token){
			
			HttpWebRequest req = GetWebRequest(url + ((prms.Length > 0) ? ("?" + Parameter.ConCat(prms)) : ""), prms);
			req.Method = "GET";
			
			Consumer.AccessProtectedResource(token, req, url, "http://twitter.com/", prms);
			return new WebRequestData(req);
		}
		
		private WebRequestData Post(string url, Parameter[] prms, AccessToken token){
			string query = Parameter.ConCat(prms);
			byte[] data = Encoding.ASCII.GetBytes(query);
			
			HttpWebRequest req = GetWebRequest(url, prms);
			req.Method = "POST";
			req.ContentType = "application/x-www-form-urlencoded";
			req.ContentLength = query.Length;
			
			Consumer.AccessProtectedResource(token, req, url, "http://twitter.com/", prms);
			return new WebRequestData(req, data);
		}
		
		private HttpWebRequest GetWebRequest(string url, Parameter[] prms){
			HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
			req.KeepAlive = false;
			req.CookieContainer = null;
			req.Timeout = this.Timeout;
			return req;
		}

		#endregion

		#region OAuth

		public RequestToken ObtainUnauthorizedRequestToken(){
			return Consumer.ObtainUnauthorizedRequestToken("http://api.twitter.com/oauth/request_token", "http://twitter.com/");
		}
		
		public AccessToken GetAccessToken(RequestToken requestToken, string verifier){
			return Consumer.RequestAccessToken(
				verifier, requestToken, "http://api.twitter.com/oauth/access_token", "http://twitter.com/");
		}
		
		public string BuildUserAuthorizationURL(RequestToken reqToken){
			return Consumer.BuildUserAuthorizationURL("http://api.twitter.com/oauth/authorize", reqToken);
		}
		
		#endregion
		
		#region その他
		/*
		private string BuildUrl(string url, Parameter[] prms){
			return (prms.Length > 0) ? url + "?" + BuildQuery(prms) : url;
		}
		
		private string BuildQuery(Parameter[] prms){
			var query = from prm in prms
			            select prm.Name + "=" + prm.Value;
			return String.Join("&", query.ToArray());
		}
		*/
		public static bool TryParseDateTime(string s, out DateTime result){
			const string format = @"ddd MMM dd HH':'mm':'ss zz'00' yyyy";
			return DateTime.TryParseExact(
				s,
				format,
				System.Globalization.DateTimeFormatInfo.InvariantInfo,
				System.Globalization.DateTimeStyles.AllowWhiteSpaces,
				out result);
		}
		/*
		public bool TryParseColor(string s, out Color color){
			try{
				int n = Convert.ToInt32(s, 16);
				byte r = (byte)((n & 0xff0000) >> 16);
				byte g = (byte)((n & 0x00ff00) >> 8);
				byte b = (byte)((b & 0x0000ff));
				color = Color.FromRgb(r, g, b);
				return true;
			}catch{
				return false;
			}
		}
		*/
		
		public string GetErrorMessage(WebException ex){
			if(ex.Status == WebExceptionStatus.ProtocolError){
				HttpWebResponse req = ex.Response as HttpWebResponse;
				if(req != null){
					switch(req.StatusCode){
						case HttpStatusCode.BadRequest:
							return "400: リクエストが不正か、APIの使用制限を超えています。";
						case HttpStatusCode.Unauthorized:
							return "401: アカウントの認証を失敗しました。OAuth認証をやり直してください。";
						case HttpStatusCode.Forbidden:
							return "403: サーバーからアクセスが禁止されています。更新制限を超えている可能性があります。";
						case HttpStatusCode.BadGateway:
							return "501: Twitterのサーバーがダウンしているか、アップデート中です。";
						case HttpStatusCode.ServiceUnavailable:
							return "503: Twitterのサービスが使用できない状態にあります。しばらく後でやり直してください。";
						default:
							return String.Format("{0}: {1}", (int)req.StatusCode, ex.Message);
					}
				}
			}
			return ex.Message;
		}
		
		#endregion
	}
}