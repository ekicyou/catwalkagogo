/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;

namespace Nekome.Search{
	[Serializable]
	public struct GrepMatch{
		public string Path{get; private set;}
		public Encoding Encoding{get; private set;}
		public long Line{get; private set;}
		public string LineText{get; private set;}
		public Match Match{get; private set;}
		public IList<IndexLinePair> Map{get; private set;}
		
		public GrepMatch(string path, Encoding enc, long line, string text, Match match, IList<IndexLinePair> map) : this(){
			if(enc == null){
				throw new ArgumentNullException();
			}
			this.Path = path;
			this.Encoding = enc;
			this.Line = line;
			this.LineText = text;
			this.Match = match;
			this.Map = new ReadOnlyCollection<IndexLinePair>(map);
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