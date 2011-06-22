/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Threading;
using System.IO;
using System.Net;

namespace CatWalk.Twitter {
	public class TwitterList {
		public TwitterApi TwitterApi{get; private set;}

		public ulong Id{get; private set;}
		public string Name{get; private set;}
		public string FullName{get; private set;}
		public string Slug{get; private set;}
		public string Description{get; private set;}
		public int SubscriberCount{get; private set;}
		public int MemberCount{get; private set;}
		public string Uri{get; private set;}
		public bool Following{get; private set;}
		public string Mode{get; private set;}
		public User User{get; private set;}

		public TwitterList(TwitterApi api, XElement elm){
			if(api == null){
				throw new ArgumentNullException("api");
			}
			if(elm == null){
				throw new ArgumentNullException("elm");
			}
			this.TwitterApi = api;

			this.Id = (ulong)elm.Element("id");
			this.Name = (string)elm.Element("name");
			this.FullName = (string)elm.Element("full_name");
			this.Slug = (string)elm.Element("slug");
			this.Description = (string)elm.Element("description");
			this.SubscriberCount = (int)elm.Element("subscriber_count");
			this.MemberCount = (int)elm.Element("member_count");
			this.Uri = (string)elm.Element("uri");
			this.Following = (bool)elm.Element("following");
			this.Mode = (string)elm.Element("mode");
			this.User = new User(api, elm.Element("user"));
		}

		#region API

		public IEnumerable<Status> GetTimeline(int count, int page, ulong sinceId, ulong maxId, bool trimUser, CancellationToken token){
			var req = this.TwitterApi.GetListStatuses(this.Id, sinceId, maxId, count, page, trimUser);
			using(Stream stream = req.Get(token)){
				var xml = XDocument.Load(stream);
				foreach(XElement status in xml.Root.Elements("status")){
					yield return new Status(this.TwitterApi, status);
				}
			}
		}

		#endregion
	}
}
