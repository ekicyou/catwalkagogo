/*
	OAuthLib by 7k8m http://oauthlib.codeplex.com
	
	$Id: RequestToken.cs 14 2010-01-06 06:22:29Z catwalk $
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace CatWalk.OAuth{
    /// <summary>
    /// Stands for request token.
    /// </summary>
    [Serializable]
    public class RequestToken : Token{
        public RequestToken(String tokenValue, String tokenSecret) : base(tokenValue, tokenSecret){
        }
    }
}
