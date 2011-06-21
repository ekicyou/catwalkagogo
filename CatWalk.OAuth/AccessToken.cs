/*
	OAuthLib by 7k8m http://oauthlib.codeplex.com
	
	$Id: AccessToken.cs 14 2010-01-06 06:22:29Z catwalk $
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace CatWalk.OAuth{
    /// <summary>
    /// Stands for access token
    /// </summary>
    [Serializable]
    public class AccessToken : Token{
        public AccessToken(String tokenValue, String tokenSecret) : base(tokenValue, tokenSecret){
        }

    }
}
