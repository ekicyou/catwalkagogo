/*
	OAuthLib by 7k8m http://oauthlib.codeplex.com
	
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using System.Linq;

namespace CatWalk.Net.OAuth {
	using Parameter = KeyValuePair<string, string>;

	/// <summary>
	/// Stands for request token.
	/// </summary>
	[Serializable]
	public class RequestToken : Token {
		public RequestToken(String tokenValue, String tokenSecret)
			: base(tokenValue, tokenSecret) {
		}

		public static RequestToken FromRequest(GettingWebRequest req, CancellationToken token){
			using(var res = req.GetResponse(token)){
				return FromResponse(res);
			}
		}

		public static RequestToken FromResponse(WebResponse resp){
			StreamReader sr = new StreamReader(resp.GetResponseStream());

			string reqToken = null;
			string reqTokenSecret = null;

			foreach(Parameter param in NetUtility.ParseQueryString(sr.ReadToEnd())) {
				if(param.Key == "oauth_token")
					reqToken = param.Value;
				if(param.Key == "oauth_token_secret")
					reqTokenSecret = param.Value;
			}

			if(reqToken == null || reqTokenSecret == null)
				throw new InvalidOperationException();

			return new RequestToken(reqToken, reqTokenSecret);
		}
	}
}
