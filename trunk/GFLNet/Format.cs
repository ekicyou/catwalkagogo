/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GflNet {
	[Serializable]
	public struct Format : IEquatable<Format>{
		public string Name{get; private set;}
		public string DefaultSuffix{get; private set;}
		public bool Readable{get; private set;}
		public bool Writable{get; private set;}
		public string Description{get; private set;}
		
		internal Format(string name, string defaultSuffix, bool readable, bool writable, string description) : this(){
			this.Name = name;
			this.DefaultSuffix = defaultSuffix;
			this.Readable = readable;
			this.Writable = writable;
			this.Description = description;
		}

		#region IEquatable

		public bool Equals(Format other){
			return this.Name.Equals(other.Name) && this.DefaultSuffix.Equals(other.DefaultSuffix) &&
				this.Readable.Equals(other.Readable) && this.Writable.Equals(other.Writable) &&
				this.Description.Equals(other.Description);
		}

		public override bool Equals(object obj){
			if(!(obj is Format)) {
				return false;
			}
			return this.Equals((Format)obj);
		}

		public override int GetHashCode(){
			return this.Name.GetHashCode() ^ this.DefaultSuffix.GetHashCode() ^ this.Readable.GetHashCode() ^ this.Writable.GetHashCode() ^ this.Description.GetHashCode();
		}

		public static bool operator ==(Format a, Format b){
			return a.Equals(b);
		}

		public static bool operator !=(Format a, Format b){
			return !a.Equals(b);
		}

		#endregion
	}
}
