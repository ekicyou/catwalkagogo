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
		
		public static GrepMatch[] Match(Regex regex, string path){
			if(path == null){
				throw new ArgumentNullException();
			}
			if(path == ""){
				throw new ArgumentException();
			}
			using(var stream = File.Open(path, FileMode.Open, FileAccess.Read)){
				return MatchImpl(regex, stream, path).ToArray();
			}
		}
		
		public static GrepMatch[] Match(Regex regex, Stream stream){
			return MatchImpl(regex, stream, null);
		}
		
		public static GrepMatch[] MatchImpl(Regex regex, Stream stream, string path){
			if(regex == null){
				throw new ArgumentNullException("regex");
			}
			if(stream == null){
				throw new ArgumentNullException("stream");
			}
			
			var list = new List<GrepMatch>(0);

			var enc = EncodingDetector.GetEncodings(stream).First();
			stream.Seek(0, SeekOrigin.Begin);
			
			long lineCount = 1;
			long lastStreamPosition = 0;
			var map = new List<IndexLinePair>();
			using(var reader = new StreamReader(stream, enc, false, 128)){
				string line;
				while((line = reader.ReadLine()) != null){
					if(lastStreamPosition != stream.Position){
						map.Add(new IndexLinePair(lastStreamPosition, lineCount));
						lastStreamPosition = stream.Position;
					}
					foreach(var match in regex.Matches(line).Cast<Match>()){
						list.Add(new GrepMatch(path, enc, lineCount, line, match, map));
					}
					lineCount++;
				}
				map.Add(new IndexLinePair(lastStreamPosition, lineCount));
			}
			return list.ToArray();
		}
	}
}