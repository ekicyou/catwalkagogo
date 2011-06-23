/*
	OAuthLib by 7k8m http://oauthlib.codeplex.com
	
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace CatWalk.Net.OAuth{
    /// <summary>
    /// Stands for access token
    /// </summary>
    [Serializable]
    public class AccessToken : Token{
        public AccessToken(String tokenValue, String tokenSecret) : base(tokenValue, tokenSecret){
        }

    }
}
