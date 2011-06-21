/*
	OAuthLib by 7k8m http://oauthlib.codeplex.com
	
	$Id$
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
