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
			stream.Seek(0, SeekOrigin.Begin);
			//System.Windows.MessageBox.Show(path + "\n" + enc.ToString());

			long lineCount = 1;
			long lastStreamPosition = 0;
			var map = new List<PositionLinePair>();
			var lineIndex = new List<int>();
			var buffer = new StringBuilder();
			foreach(var line in (new StreamReader(stream, enc, false, 1024)).Use()
				.Select(reader => reader.ReadLine())
				.TakeWhile(line => line != null)){
				if((tokenSource != null) && tokenSource.Token.IsCancellationRequested){
					yield break;
				}
				lineIndex.Add(buffer.Length);
				buffer.AppendLine(line);
				if(lastStreamPosition != stream.Position){
					map.Add(new PositionLinePair(lastStreamPosition, lineCount));
					lastStreamPosition = stream.Position;
				}
				lineCount++;
			}
			map.Add(new PositionLinePair(lastStreamPosition, lineCount));
			lineIndex.Add(buffer.Length);

			var text = buffer.ToString();
			foreach(var match in regex.Matches(text).Cast<Match>()){
				if((tokenSource != null) && tokenSource.Token.IsCancellationRequested){
					yield break;
				}
				var lineNum = lineIndex.BinarySearch(match.Index);
				if(lineNum < 0){
					lineNum = (~lineNum) - 1;
				}
				var lineHead = lineIndex[lineNum];
				var lineTail = ((lineNum + 1) < lineIndex.Count) ? lineIndex[lineNum+1] : text.Length;
				yield return new GrepMatch(path, enc, lineNum + 1, match.Index - lineHead, text.Substring(lineHead, lineTail - lineHead - Environment.NewLine.Length), match, map);
			}
		}
	}
}