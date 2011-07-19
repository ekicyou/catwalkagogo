using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CatWalk.Collections;
using CatWalk.IOSystem;
using CatWalk.Net.Twitter;

namespace Twitman.IOSystem {
	public class TimelineSystemDirectory : SystemDirectory{
		public Account Account{get; private set;}
		private Timeline _NewestTimeline;
		private Timeline _OldestTimeline;
		private Func<CancellationToken, Timeline> _SeedTimeline;
		private ReadOnlyObservableList<StatusSystemEntry> _StatusReadOnlyList;
		private ObservableSortedSkipList<StatusSystemEntry> _StatusList;

		public TimelineSystemDirectory(ISystemDirectory parent, string name, Func<CancellationToken, Timeline> timeline) : base(parent, name){
			this._SeedTimeline = timeline;
			this._StatusReadOnlyList = new ReadOnlyObservableList<StatusSystemEntry>(this._StatusList);
		}

		public TimelineSystemDirectory(ISystemDirectory parent, string name, Account account, Func<CancellationToken, Timeline> timeline) : base(parent, name){
			this._SeedTimeline = timeline;
		}

		private void InitListIfFirst(CancellationToken token){
			if(this._SeedTimeline != null){
				var timeline = this._SeedTimeline(token);
				this._StatusList = new ObservableSortedSkipList<StatusSystemEntry>(new SortedSkipList<StatusSystemEntry>(timeline.Select(status => new StatusSystemEntry(this, status)), false));
				this._StatusReadOnlyList = new ReadOnlyObservableList<StatusSystemEntry>(this._StatusList);
				this._SeedTimeline = null;
			}
		}

		public void GetNewer(int count){
			this.GetNewer(count, CancellationToken.None);
		}
		public void GetNewer(int count, CancellationToken token){
			this.InitListIfFirst(token);
			this._NewestTimeline = this._NewestTimeline.GetNewer(count);
			foreach(var status in this._NewestTimeline){
				this._StatusList.Add(new StatusSystemEntry(this, status));
			}
		}

		public void GetOlder(int count){
			this.GetOlder(count, CancellationToken.None);
		}
		public void GetOlder(int count, CancellationToken token){
			this.InitListIfFirst(token);
			this._OldestTimeline = this._OldestTimeline.GetOlder(count);
			foreach(var status in this._NewestTimeline){
				this._StatusList.Add(new StatusSystemEntry(this, status));
			}
		}

		public bool CanUpdateStatus(){
			return this.Account != null;
		}

		public void UpdateStatus(string status){
			if(!this.CanUpdateStatus()){
				throw new NotSupportedException();
			}
		}

		public void UpdateStatus(string status, Status replyTo){
			this.UpdateStatus(status, replyTo, CancellationToken.None);
		}

		public void UpdateStatus(string status, Status replyTo, CancellationToken token){
			if(!this.CanUpdateStatus()){
				throw new NotSupportedException();
			}
			this.Account.UpdateStatus(status, replyTo.Id, "Twitman", token);
		}

		public override IEnumerable<ISystemEntry> Children {
			get {
				this.InitListIfFirst(CancellationToken.None);
				return this._StatusReadOnlyList;
			}
		}

		public override IEnumerable<ISystemEntry> GetChildren(CancellationToken token) {
			this.InitListIfFirst(token);
			return this._StatusReadOnlyList;
		}

		public override bool Exists {
			get {
				return true;
			}
		}
	}
}
