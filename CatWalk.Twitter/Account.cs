/*
	$Id: Account.cs 51 2010-01-28 21:55:33Z catwalk $
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
using CatWalk.OAuth;
using System.Threading;

namespace CatWalk.Twitter{
	public class Account{
		public AccessToken AccessToken{get; set;}
		public User User{get; private set;}
		public TwitterApi TwitterApi{get; private set;}

		public Account(TwitterApi api){
			this.TwitterApi = api;
		}
		
		public Account(TwitterApi api, AccessToken accessToken){
			this.AccessToken = accessToken;
			this.TwitterApi = api;
		}
		
		public void VerifyCredential(){
			this.VerifyCredential(CancellationToken.None);
		}

		public void VerifyCredential(CancellationToken token){
			this.User = null;
			var req = TwitterApi.VerifyCredential(this.AccessToken).WebRequest;
			token.Register(req.Abort);
			using(HttpWebResponse res = (HttpWebResponse)req.GetResponse())
			using(Stream stream = res.GetResponseStream()){
				var xml = XElement.Load(stream);
				this.User = new User(this.TwitterApi, xml);
			}
		}

		public bool IsVerified{
			get{
				return (this.User != null);
			}
		}

		#region Friends / Followers

		public IEnumerable<ulong> GetFriends(){
			return this.GetFriends(CancellationToken.None);
		}
		public IEnumerable<ulong> GetFriends(CancellationToken token){
			var req = TwitterApi.GetFriends(this.AccessToken, this.User.Id).WebRequest;
			token.Register(req.Abort);
			using(WebResponse res = req.GetResponse())
			using(Stream stream = res.GetResponseStream()){
				var xml = XDocument.Load(stream);
				var root = xml.Root;
				foreach(XElement user in root.Element("ids").Elements("id")){
					yield return UInt64.Parse(user.Value);
				}
			}
		}
		public ulong[] GetFriends(out Cursor<ulong> cursor){
			return this.GetFriends(CancellationToken.None, out cursor);
		}
		public ulong[] GetFriends(CancellationToken token, out Cursor<ulong> cursor){
			return this.GetFriends(token, -1, out cursor);
		}
		private ulong[] GetFriends(CancellationToken token, long cursorId, out Cursor<ulong> cursor){
			var req = TwitterApi.GetFriends(this.AccessToken, this.User.Id, cursorId).WebRequest;
			token.Register(req.Abort);
			var list = new List<ulong>();
			using(WebResponse res = req.GetResponse())
			using(Stream stream = res.GetResponseStream()){
				var xml = XDocument.Load(stream);
				var root = xml.Root;
				cursor = new Cursor<ulong>(root, this.GetFriendsCursor);
				foreach(XElement user in xml.Element("ids").Elements("id")){
					list.Add(UInt64.Parse(user.Value));
				}
			}
			return list.ToArray();
		}
		private CursorResult<ulong> GetFriendsCursor(long cursor, CancellationToken token){
			Cursor<ulong> outCursor;
			var result = this.GetFriends(token, cursor, out outCursor);
			return new CursorResult<ulong>(result, outCursor);
		}

		public IEnumerable<ulong> GetFollowers(){
			return this.GetFollowers(CancellationToken.None);
		}
		public IEnumerable<ulong> GetFollowers(CancellationToken token){
			var req = TwitterApi.GetFollowers(this.AccessToken, this.User.Id).WebRequest;
			token.Register(req.Abort);
			using(WebResponse res = req.GetResponse())
			using(Stream stream = res.GetResponseStream()){
				var xml = XDocument.Load(stream);
				var root = xml.Root;
				foreach(XElement user in root.Element("ids").Elements("id")){
					yield return UInt64.Parse(user.Value);
				}
			}
		}
		public ulong[] GetFollowers(out Cursor<ulong> cursor){
			return this.GetFollowers(CancellationToken.None, out cursor);
		}
		public ulong[] GetFollowers(CancellationToken token, out Cursor<ulong> cursor){
			return this.GetFollowers(token, -1, out cursor);
		}
		private ulong[] GetFollowers(CancellationToken token, long cursorId, out Cursor<ulong> cursor){
			var req = TwitterApi.GetFollowers(this.AccessToken, this.User.Id, cursorId).WebRequest;
			token.Register(req.Abort);
			var list = new List<ulong>();
			using(WebResponse res = req.GetResponse())
			using(Stream stream = res.GetResponseStream()){
				var xml = XDocument.Load(stream);
				var root = xml.Root;
				cursor = new Cursor<ulong>(root, this.GetFollowersCursor);
				foreach(XElement user in xml.Element("ids").Elements("id")){
					list.Add(UInt64.Parse(user.Value));
				}
			}
			return list.ToArray();
		}
		private CursorResult<ulong> GetFollowersCursor(long cursor, CancellationToken token){
			Cursor<ulong> outCursor;
			var result = this.GetFollowers(token, cursor, out outCursor);
			return new CursorResult<ulong>(result, outCursor);
		}

		#endregion

		#region Timeline

		public IEnumerable<Status> GetHomeTimeline(int count, int page, ulong sinceId, ulong maxId){
			return this.GetHomeTimeline(count, page, sinceId, maxId, CancellationToken.None);
		}

		public IEnumerable<Status> GetHomeTimeline(int count, int page, ulong sinceId, ulong maxId, CancellationToken token){
			var req = TwitterApi.GetHomeTimeline(this.AccessToken, count, page, sinceId, maxId).WebRequest;
			token.Register(req.Abort);
			using(HttpWebResponse res = (HttpWebResponse)req.GetResponse())
			using(Stream stream = res.GetResponseStream()){
				var xml = XDocument.Load(stream);
				foreach(XElement status in xml.Elements("status")){
					yield return new Status(this.TwitterApi, status);
				}
			}
		}

		#endregion

		#region Manipulation

		public void UpdateStatus(string status, ulong replyTo, string source){
			this.UpdateStatus(status, replyTo, source, CancellationToken.None);
		}
		public void UpdateStatus(string status, ulong replyTo, string source, CancellationToken token){
			WebRequestData data = TwitterApi.UpdateStatus(this.AccessToken, status, replyTo, source);
			token.Register(data.WebRequest.Abort);
			data.WriteRequestData();
		}
		
		public void DestroyStatus(Status status){
			this.DestroyStatus(status.Id, CancellationToken.None);
		}
		public void DestroyStatus(Status status, CancellationToken token){
			this.DestroyStatus(status.Id, token);
		}
		public void DestroyStatus(ulong id){
			this.DestroyStatus(id, CancellationToken.None);
		}
		public void DestroyStatus(ulong id, CancellationToken token){
			WebRequestData data = TwitterApi.DestroyStatus(this.AccessToken, id);
			token.Register(data.WebRequest.Abort);
			data.WriteRequestData();
		}
		
		public void CreateFavorite(Status status){
			this.CreateFavorite(status.Id, CancellationToken.None);
		}
		public void CreateFavorite(Status status, CancellationToken token){
			this.CreateFavorite(status.Id, token);
		}
		public void CreateFavorite(ulong id){
			this.CreateFavorite(id, CancellationToken.None);
		}
		public void CreateFavorite(ulong id, CancellationToken token){
			WebRequestData data = TwitterApi.CreateFavorite(this.AccessToken, id);
			token.Register(data.WebRequest.Abort);
			data.WriteRequestData();
		}
		
		public void CreateBlock(User user){
			this.CreateBlock(user.Id, CancellationToken.None);
		}
		public void CreateBlock(User user, CancellationToken token){
			this.CreateBlock(user.Id, token);
		}
		public void CreateBlock(ulong id){
			this.CreateBlock(id, CancellationToken.None);
		}
		public void CreateBlock(ulong id, CancellationToken token){
			WebRequestData data = TwitterApi.CreateBlock(this.AccessToken, id);
			token.Register(data.WebRequest.Abort);
			data.WriteRequestData();
		}
		
		public void DestroyFriendship(User user){
			this.DestroyFriendship(user.Id, CancellationToken.None);
		}
		public void DestroyFriendship(User user, CancellationToken token){
			this.DestroyFriendship(user.Id, token);
		}
		public void DestroyFriendship(ulong id){
			this.DestroyFriendship(id, CancellationToken.None);
		}
		public void DestroyFriendship(ulong id, CancellationToken token){
			WebRequestData data = TwitterApi.DestroyFriendship(this.AccessToken, id);
			token.Register(data.WebRequest.Abort);
			data.WriteRequestData();
		}

		#endregion
	}
}