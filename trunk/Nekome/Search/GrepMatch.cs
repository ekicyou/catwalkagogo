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
		public IList<IndexLinePair> Map{get; private set;}
		
		public GrepMatch(){
		}

		public GrepMatch(string path, Encoding enc, long line, string text, Match match, IList<IndexLinePair> map){
			if(enc == null){
				throw new ArgumentNullException();
			}
			this.Path = path;
			this.Encoding = enc;
			this.Line = line;
			this.Column = match.Index + 1;
			this.LineText = text;
			this.Match = match;
			this.Map = new ReadOnlyCollection<IndexLinePair>(map);
		}
		
		public IndexLinePair Block{
			get{
				int idx = ArrayList.Adapter((IList)this.Map).BinarySearch(new IndexLinePair(0, this.Line), new CustomComparer<IndexLinePair>(
					delegate(IndexLinePair x, IndexLinePair y){
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
	public struct IndexLinePair{
		public long Index{get; private set;}
		public long Line{get; private set;}
		
		public IndexLinePair(long index, long line) : this(){
			this.Index = index;
			this.Line = line;
		}
	}
}