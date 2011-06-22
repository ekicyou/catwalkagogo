/*
	$Id$
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
	public class TwitterApi{
		private static Lazy<TwitterApi> _Default;
		static TwitterApi(){
			_Default = new Lazy<TwitterApi>(() => new TwitterApi());
		}

		public static TwitterApi Default{
			get{
				return _Default.Value;
			}
		}
		
		protected TwitterApi(){
			ServicePointManager.Expect100Continue = false;
		}

		#region 通信プロパティ

		public int Timeout{get; set;}

		#endregion

		#region API
		
		public HttpWebRequest GetUserTimeline(string id, int count, int page, ulong sinceId, ulong maxId, bool trimUser){
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
			if(trimUser){
				prms.Add(new Parameter("trim_user", "1"));
			}
			return Get(url, prms.ToArray());
		}

		#endregion

		#region Tweets

		public HttpWebRequest ShowStatus(ulong id, bool trimUser, bool includeEntities){
			const string url = "http://api.twitter.com/1/statuses/show/";
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
		
		public HttpWebRequest ShowUser(ulong id){
			return ShowUser(id.ToString());
		}
		
		public HttpWebRequest ShowUser(string name){
			const string url = "http://api.twitter.com/1/users/show.xml";
			return Get(url, new Parameter[]{new Parameter("id", name)});
		}

		#endregion

		#region List

		public HttpWebRequest GetLists(ulong id){
			const string url = "http://api.twitter.com/1/lists.xml";
			return Get(url, new Parameter[]{
				new Parameter("user_id", id.ToString())});
		}

		public HttpWebRequest GetLists(ulong id, long cursor){
			const string url = "http://api.twitter.com/1/lists.xml";
			return Get(url, new Parameter[]{
				new Parameter("user_id", id.ToString()),
				new Parameter("cursor", cursor.ToString())});
		}

		public HttpWebRequest GetListStatuses(ulong id, ulong sinceId, ulong maxId, int perPage, int page, bool trimUser){
			const string url = "http://api.twitter.com/1/lists/statuses.xml";
			List<Parameter> prms = new List<Parameter>();
			if(perPage > 0){
				prms.Add(new Parameter("per_page", perPage.ToString()));
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
			if(trimUser){
				prms.Add(new Parameter("trim_user", "1"));
			}
			return Get(url, prms.ToArray());
		}

		#endregion

		#region 通信

		protected HttpWebRequest Get(string url, Parameter[] prms){
			HttpWebRequest req = GetWebRequest(url + ((prms.Length > 0) ? ("?" + Parameter.ConCat(prms)) : ""), prms);
			req.Method = "GET";
			
			return req;
		}
		
		protected PostingWebRequest Post(string url, Parameter[] prms){
			string query = Parameter.ConCat(prms);
			byte[] data = Encoding.ASCII.GetBytes(query);
			
			HttpWebRequest req = GetWebRequest(url, prms);
			req.Method = "POST";
			req.ContentType = "application/x-www-form-urlencoded";
			req.ContentLength = query.Length;
			
			return new PostingWebRequest(req, data);
		}
				
		protected HttpWebRequest GetWebRequest(string url, Parameter[] prms){
			HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
			req.KeepAlive = false;
			req.CookieContainer = null;
			req.Timeout = this.Timeout;
			return req;
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