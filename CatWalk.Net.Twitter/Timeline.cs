using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
//using CatWalk.Collections;

namespace CatWalk.Net.Twitter {
	/*
	public class TimelineCollection : ReadOnlyObservableList<Status>{
		private Timeline _NewestTimeline;
		private Timeline _OldestTimeline;

		public TimelineCollection(Timeline timeline) : base(new SortedSkipList<Status>(timeline.Statuses)){
		}

		public void RetrieveOlder(int count, CancellationToken token){
			var timeline = this._OldestTimeline.GetOlder(count, token);
			foreach(var status in timeline.Statuses){
				this.List.Add(status);
			}
			this._OldestTimeline = timeline;
		}

		public void RetrieveNewer(int count, CancellationToken token){
			var timeline = this._NewestTimeline.GetNewer(count, token);
			foreach(var status in timeline.Statuses){
				this.List.Add(status);
			}
			this._NewestTimeline = timeline;
		}
	}
	*/
	public abstract class Timeline{
		private IEnumerable<Status> _Source;

		public Timeline(IEnumerable<Status> source){
			if(source == null){
				throw new ArgumentNullException("source");
			}
			this._Source = source;
		}

		public virtual IEnumerable<Status> Statuses{
			get{
				return this._Source;
			}
		}

		public abstract Timeline GetOlder(int count);
		public abstract Timeline GetOlder(int count, CancellationToken token);
		public abstract Timeline GetNewer(int count);
		public abstract Timeline GetNewer(int count, CancellationToken token);
	}

	public abstract class IdTimeline : Timeline{
		public ulong MaxId{get; private set;}
		public ulong MinId{get; private set;}

		public IdTimeline(IEnumerable<Status> source) : base(source){
			this.MaxId = UInt64.MinValue;
			this.MinId = UInt64.MaxValue;
		}

		public override IEnumerable<Status> Statuses{
			get{
				foreach(var status in base.Statuses){
					if(status.Id > this.MaxId){
						this.MaxId = status.Id;
					}
					if(status.Id < this.MinId){
						this.MinId = status.Id;
					}
					yield return status;
				}
			}
		}
	}
	/*
	public class UserTimeline : IdTimeline{
		public override Timeline GetOlder(int count) {
			throw new NotImplementedException();
		}

		public override Timeline GetOlder(int count, CancellationToken token) {
			throw new NotImplementedException();
		}

		public override Timeline GetNewer(int count) {
			throw new NotImplementedException();
		}

		public override Timeline GetNewer(int count, CancellationToken token) {
			throw new NotImplementedException();
		}
	}

	public class HomeTimeline : IdTimeline{
		public override Timeline GetOlder(int count) {
			throw new NotImplementedException();
		}

		public override Timeline GetOlder(int count, CancellationToken token) {
			throw new NotImplementedException();
		}

		public override Timeline GetNewer(int count) {
			throw new NotImplementedException();
		}

		public override Timeline GetNewer(int count, CancellationToken token) {
			throw new NotImplementedException();
		}
	}

	public class PublicTimeline : IdTimeline{
		public override Timeline GetOlder(int count) {
			throw new NotImplementedException();
		}

		public override Timeline GetOlder(int count, CancellationToken token) {
			throw new NotImplementedException();
		}

		public override Timeline GetNewer(int count) {
			throw new NotImplementedException();
		}

		public override Timeline GetNewer(int count, CancellationToken token) {
			throw new NotImplementedException();
		}
	}
	 * */
}
