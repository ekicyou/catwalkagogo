using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatWalk.IOSystem;
using CatWalk.Net.Twitter;

namespace Twitman.IOSystem {
	public class StatusSystemEntry : SystemEntry{
		public Status Status{get; private set;}

		public StatusSystemEntry(ISystemDirectory parent, Status status) : base(parent, status.Id.ToString()){
			this.Status = status;
		}

		public void Delete(){
		}

		public void Favorite(){
		}

		public void Retweet(){
		}

		public void Reply(string status){
		}

		public override bool Exists {
			get {
				return true;
			}
		}
	}
}
