/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Threading;

namespace CatWalk.Twitter{
	public class Status : IEquatable<Status> {
		#region Data

		public TwitterApi TwitterApi{get; private set;}

		public DateTime CreatedAt{get; private set;}
		public ulong Id{get; private set;}
		public string Text{get; private set;}
		public string Source{get; private set;}
		public bool Trancated{get; private set;}
		public ulong InReplyToStatusId{get; private set;}
		public ulong InReplyToUserId{get; private set;}
		public bool Favorited{get; private set;}
		public string InReplyToScreenName{get; private set;}
		public User User{get; private set;}
		
		public Status(TwitterApi api, XElement element){
			if(element == null){
				throw new ArgumentNullException("element");
			}
			if(api == null){
				throw new ArgumentNullException("api");
			}
			DateTime dt;
			bool b;
			ulong dec;
			
			//status.Id = (ulong)element.Element("id");
			this.Id = (ulong)element.Element("id");
			if(TwitterApi.TryParseDateTime((string)element.Element("created_at"), out dt)){
				this.CreatedAt = dt;
			}
			this.Text = WebUtility.HtmlDecode((string)element.Element("text"));
			this.Source = (string)element.Element("source");
			if(Boolean.TryParse((string)element.Element("truncated"), out b)){
				this.Trancated = b;
			}
			if(UInt64.TryParse((string)element.Element("in_reply_to_status_id"), out dec)){
				this.InReplyToStatusId = dec;
			}
			if(UInt64.TryParse((string)element.Element("in_reply_to_user_id"), out dec)){
				this.InReplyToUserId = dec;
			}
			this.InReplyToScreenName = (string)element.Element("in_reply_to_screen_name");
			if(Boolean.TryParse((string)element.Element("favorited"), out b)){
				this.Favorited = b;
			}
			this.User = new User(api, element.Element("user"));
		}

		#endregion

		#region API

		public Status GetReplyStatus(bool trimUser, bool includeEntities){
			return this.GetReplyStatus(trimUser, includeEntities, CancellationToken.None);
		}

		public Status GetReplyStatus(bool trimUser, bool includeEntities, CancellationToken token){
			var req =  this.TwitterApi.ShowStatus(this.InReplyToStatusId, trimUser, includeEntities).WebRequest;
			token.Register(req.Abort);
			using(HttpWebResponse res = (HttpWebResponse)req.GetResponse())
			using(Stream stream = res.GetResponseStream()){
				var xml = XDocument.Load(stream);
				return new Status(this.TwitterApi, xml.Element("status"));
			}
		}

		public User GetReplyUser(){
			return this.GetReplyUser(CancellationToken.None);
		}

		public User GetReplyUser(CancellationToken token){
			var req =  this.TwitterApi.ShowUser(this.InReplyToUserId).WebRequest;
			token.Register(req.Abort);
			using(HttpWebResponse res = (HttpWebResponse)req.GetResponse())
			using(Stream stream = res.GetResponseStream()){
				var xml = XDocument.Load(stream);
				return new User(this.TwitterApi, xml.Element("user"));
			}
		}

		public WebRequestData GetRetweets(){
			throw new NotImplementedException();
		}

		public WebRequestData GetRetweetUsers(){
			throw new NotImplementedException();
		}

		#endregion

		#region IEquatable<Status> Members

		public override bool Equals(object obj){
			if(obj is Status){
				return this.Equals((Status)obj);
			}else{
				return false;
			}
		}
		
		public static bool operator ==(Status a, Status b){
			return (a.Id == b.Id);
		}
		
		public static bool operator !=(Status a, Status b){
			return (a.Id != b.Id);
		}
		
		public override int GetHashCode(){
			return this.Id.GetHashCode();
		}

		public bool Equals(Status other) {
			return this.Id.Equals(other.Id);
		}

		#endregion
	}
}