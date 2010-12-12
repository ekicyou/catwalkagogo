/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using CatWalk;
using CatWalk.Text;

namespace Nekome.Search{
	public static class Grep{
		
		public static IEnumerable<GrepMatch> Match(Regex regex, string path){
			if(path == null){
				throw new ArgumentNullException();
			}
			if(path == ""){
				throw new ArgumentException();
			}
			return File.Open(path, FileMode.Open, FileAccess.Read).Use(stream => MatchImpl(regex, stream, path));
		}
		
		public static IEnumerable<GrepMatch> Match(Regex regex, Stream stream){
			return MatchImpl(regex, stream, null);
		}
		
		public static IEnumerable<GrepMatch> MatchImpl(Regex regex, Stream stream, string path){
			if(regex == null){
				throw new ArgumentNullException("regex");
			}
			if(stream == null){
				throw new ArgumentNullException("stream");
			}
			
			var enc = EncodingDetector.GetEncodings(stream).First();
			stream.Seek(0, SeekOrigin.Begin);
			
			long lineCount = 0;
			long lastStreamPosition = 0;
			var map = new List<IndexLinePair>();
			return Seq.Use(() => new StreamReader(stream, enc, false))
				.Select(reader => reader.ReadLine())
				.TakeWhile(line => line != null)
				.Select(new Func<string, IEnumerable<GrepMatch>>(delegate(string line){
					lineCount++;
					if(lastStreamPosition != stream.Position){
						map.Add(new IndexLinePair(lastStreamPosition, lineCount));
						lastStreamPosition = stream.Position;
					}
					var matches = regex.Matches(line).Cast<Match>().ToArray();
					if(matches.Length > 0){
						return matches.Select(match => new GrepMatch(path, enc, lineCount, line, match, map));
					}else{
						return Seq.ToSequence<GrepMatch>(null);
					}
				})).Select(match => {
					map.Add(new IndexLinePair(lastStreamPosition, lineCount));
					return match;
				}).Flatten();
		}
	}
}