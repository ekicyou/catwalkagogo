﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace CatWalk.Net {
	using Parameter = KeyValuePair<string, string>;

	public static class WebUtility {
		public static string EncodeQuery(this IEnumerable<Parameter> map){
			return EncodeQuery(map, false, "");
		}

		public static string EncodeQuery(this IEnumerable<Parameter> map, string quote){
			return EncodeQuery(map, false, quote);
		}

		public static string EncodeQuery(this IEnumerable<Parameter> map, bool normarize){
			return EncodeQuery(map, normarize, "");
		}

		public static string EncodeQuery(this IEnumerable<Parameter> map, bool normarize, string quote){
			if(map == null){
				throw new ArgumentNullException("map");
			}
			if(quote == null){
				throw new ArgumentNullException("quote");
			}
			var sep = quote + "&" + quote;
			if(normarize){
				return quote + String.Join(sep, map.Select(p => new Parameter(Uri.EscapeDataString(p.Key), Uri.EscapeDataString(p.Value)))
					.OrderBy(p => p.Key)
					.Select(p => p.Key + "=" + p.Value)) + quote;
			}else{
				return quote + String.Join(sep, map.Select(p => Uri.EscapeDataString(p.Key) + "=" + Uri.EscapeDataString(p.Value))) + quote;
			}
		}

		public static IEnumerable<Parameter> ParseQueryString(string query){
			if(query == null){
				throw new ArgumentNullException("query");
			}
			return query.Split('&').Select(s => {
				var t = s.Split('=');
				if(t.Length == 2){
					return new Parameter(t[0], t[1]);
				}else{
					throw new ArgumentException("query");
				}
			});
		}
	}
}
