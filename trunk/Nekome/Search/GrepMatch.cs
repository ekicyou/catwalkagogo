/*
	$Id$
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using CatWalk.Collections;

namespace Nekome.Search{
	[Serializable]
	public class GrepMatch{
		public string Path{get; private set;}
		public Encoding Encoding{get; private set;}
		public long Line{get; private set;}
		public long Column{get; private set;}
		public string LineText{get; private set;}
		public Match Match{get; private set;}
		public IList<PositionLinePair> Map{get; private set;}

		public GrepMatch(string path, Encoding enc, long line, long column, string text, Match match, IList<PositionLinePair> map){
			if(enc == null){
				throw new ArgumentNullException();
			}
			this.Path = path;
			this.Encoding = enc;
			this.Line = line;
			this.Column = column;
			this.LineText = text;
			this.Match = match;
			this.Map = new ReadOnlyCollection<PositionLinePair>(map);
		}
		
		public PositionLinePair Block{
			get{
				int idx = ArrayList.Adapter((IList)this.Map).BinarySearch(new PositionLinePair(0, this.Line), new CustomComparer<PositionLinePair>(
					delegate(PositionLinePair x, PositionLinePair y){
						return x.Line.CompareTo(y.Line);
					}
				));
				if(idx < 0){
					idx = (~idx) - 1;
				}
				return this.Map[idx];
			}
		}
	}

	[Serializable]
	public struct PositionLinePair : IComparable<PositionLinePair>{
		public long Position{get; private set;}
		public long Line{get; private set;}

		public PositionLinePair(long position, long line) : this(){
			this.Line = line;
			this.Position = position;
		}
	
		public int CompareTo(PositionLinePair other){
			return this.Position.CompareTo(other.Position);
		}
	}
}