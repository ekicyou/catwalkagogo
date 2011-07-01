using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatWalk.Collections;
using CatWalk.IOSystem;
using CatWalk.Net.Twitter;

namespace Twitman.IOSystem {
	public class TimelineSystemDirectory : SystemDirectory{
		private Timeline _NewestTimeline;
		private Timeline _OldestTimeline;
		private ReadOnlyObservableList<StatusSystemEntry> _StatusReadOnlyList;
		private ObservableSortedSkipList<StatusSystemEntry> _StatusList;

		public TimelineSystemDirectory(ISystemDirectory parent, string name, Timeline timeline) : base(parent, name){
			this._NewestTimeline = this._OldestTimeline = timeline;
			this._StatusList = new ObservableSortedSkipList<StatusSystemEntry>(new SortedSkipList<StatusSystemEntry>(timeline.Select(status => new StatusSystemEntry(this, status)), false));
			this._StatusReadOnlyList = new ReadOnlyObservableList<StatusSystemEntry>(this._StatusList);
		}

		public void GetNewer(int count){
			this._NewestTimeline = this._NewestTimeline.GetNewer(count);
			foreach(var status in this._NewestTimeline){
				this._StatusList.Add(new StatusSystemEntry(this, status));
			}
		}

		public void GetOlder(int count){
			this._OldestTimeline = this._OldestTimeline.GetOlder(count);
			foreach(var status in this._NewestTimeline){
				this._StatusList.Add(new StatusSystemEntry(this, status));
			}
		}

		public void UpdateStatus(string status){
		}

		public override IEnumerable<ISystemEntry> Children {
			get {
				return this._StatusReadOnlyList;
			}
		}

		public override bool Exists {
			get {
				return true;
			}
		}
	}
}
