/*
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
			return File.Open(path, FileMode.Open, FileAccess.Read).Use(stream => MatchImpl(regex, stream, path, null));
		}

		public static IEnumerable<GrepMatch> Match(Regex regex, string path, CancellationTokenSource tokenSource){
			if(path == null){
				throw new ArgumentNullException();
			}
			if(path == ""){
				throw new ArgumentException();
			}
			return File.Open(path, FileMode.Open, FileAccess.Read).Use(stream => MatchImpl(regex, stream, path, tokenSource));
		}
		
		public static IEnumerable<GrepMatch> Match(Regex regex, Stream stream){
			return MatchImpl(regex, stream, null, null);
		}

		public static IEnumerable<GrepMatch> Match(Regex regex, Stream stream, CancellationTokenSource tokenSource){
			return MatchImpl(regex, stream, null, tokenSource);
		}
		
		public static IEnumerable<GrepMatch> MatchImpl(Regex regex, Stream stream, string path, CancellationTokenSource tokenSource){
			if(regex == null){
				throw new ArgumentNullException("regex");
			}
			if(stream == null){
				throw new ArgumentNullException("stream");
			}
			
			var enc = EncodingDetector.GetEncodings(stream, tokenSource).FirstOrDefault();
			if((tokenSource != null) && tokenSource.Token.IsCancellationRequested){
				yield break;
			}

			long lineCount = 1;
			long lastStreamPosition = 0;
			var map = new List<IndexLinePair>();
			stream.Seek(0, SeekOrigin.Begin);
			foreach(var line in Seq.Use(() => new StreamReader(stream, enc, false))
				.Select(reader => reader.ReadLine())
				.TakeWhile(line => line != null)){
				if(lastStreamPosition != stream.Position){
					map.Add(new IndexLinePair(lastStreamPosition, lineCount));
					lastStreamPosition = stream.Position;
				}
				if((tokenSource != null) && tokenSource.Token.IsCancellationRequested){
					yield break;
				}
				foreach(var match in regex.Matches(line).Cast<Match>()){
					if((tokenSource != null) && tokenSource.Token.IsCancellationRequested){
						yield break;
					}
					yield return new GrepMatch(path, enc, lineCount, line, match, map);
				}
				lineCount++;
			}
			map.Add(new IndexLinePair(lastStreamPosition, lineCount));
		}
	}
}