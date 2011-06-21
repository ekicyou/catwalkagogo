/*
	OAuthLib by 7k8m http://oauthlib.codeplex.com
	
	$Id: Consumer.cs 14 2010-01-06 06:22:29Z catwalk $
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Security.Cryptography;
using System.IO;
using System.Linq;

namespace CatWalk.OAuth {
	/// <summary>
	/// Consumer object stands for consumer in OAuth protocol.
	/// </summary>
	/// <remarks>
	///  This object does follows.
	/// <list type="number">
	/// <item><description>Obtain unauthorized request token from Service Provider.</description></item>
	/// <item><description>Request access token responding authenticated request token.</description></item>
	/// <item><description>Access protected resource with access token</description></item>
	/// </list>
	/// </remarks>
	/// <example>
	/// <code>
	/// using System;
	///using System.Collections.Generic;
	///using System.Text;
	///using System.Diagnostics;
	///using System.Net;
	///
	///using OAuthLib;
	///
	///namespace OAuthDevSupport
	///{
	///    class Program
	///    {
	///        static void Main(string[] args)
	///        {
	///            try
	///          {
	///                Consumer c =
	///                   new Consumer("yourConsumerKey", "yourConsumerSecret");
	///
	///                RequestToken reqToken =
	///                    c.ObtainUnauthorizedRequestToken("http://twitter.com/oauth/request_token", "http://twitter.com/");
	///
	///                Process.Start(
	///                    Consumer.BuildUserAuthorizationURL(
	///                        "http://twitter.com/oauth/authorize",
	///                        reqToken
	///                    )
	///                );
	///
	///                Console.Out.WriteLine("Input verifier");
	///                String verifier = Console.In.ReadLine();
	///                verifier = verifier.TrimEnd('\r', '\n');
	///                AccessToken accessToken =
	///                   c.RequestAccessToken(verifier, reqToken, "http://twitter.com/oauth/access_token", "http://twitter.com/");
	///
	///                HttpWebResponse resp =
	///                    c.AccessProtectedResource(
	///                        accessToken,
	///                        "http://twitter.com/statuses/home_timeline.xml",
	///                        "GET",
	///                        "http://twitter.com/",
	///                            new Parameter[]{ 
	///                            new Parameter("since_id","your since id") 
	///                        }
	///                        );
	///
	///            }
	///            catch (Exception ex)
	///            {
	///                Console.WriteLine(ex.Message);
	///            }
	///        }
	///    }
	///}
	/// </code>
	/// </example>
	public class Consumer {
		#region Data

		private readonly String _consumerKey;
		private readonly String _consumerSecret;

		private IWebProxy _webProxy;

		/// <summary>
		/// HTTP Proxy to use when communicate with Service Provider.
		/// </summary>
		public IWebProxy Proxy {
			get {
				return _webProxy;
			}

			set {
				_webProxy = value;
			}
		}

		#endregion

		#region Constructor

		/// <summary>
		/// Construct new Consumer instance.
		/// </summary>
		/// <param name="consumerKey">Key of consumer</param>
		/// <param name="consumerSecret">Secret of consumer</param>
		public Consumer(
			String consumerKey,
			String consumerSecret
			) {
			_consumerKey = consumerKey;
			_consumerSecret = consumerSecret;
		}

		#endregion

		#region ObtainUnauthorizedRequestToken

		/// <summary>
		/// Obtain unauthorized request token from service provider
		/// </summary>
		/// <param name="requestTokenUrl">Request token URL</param>
		/// <param name="realm">Realm for obtaining request token</param>
		/// <returns>Obtained request token</returns>
		public RequestToken ObtainUnauthorizedRequestToken(
			String requestTokenUrl,
			String realm
			) {
			Parameter[] responseParameter = null;
			return ObtainUnauthorizedRequestToken(
				requestTokenUrl,
				null,
				realm,
				null,
				ref responseParameter
			);
		}

		/// <summary>
		/// Obtain unauthorized request token from service provider
		/// </summary>
		/// <param name="requestTokenUrl">Request token URL</param>
		/// <param name="realm">Realm for obtaining request token</param>
		/// <param name="responseParameter" >Parameters returned in response</param>
		/// <returns>Obtained request token</returns>
		public RequestToken ObtainUnauthorizedRequestToken(
			String requestTokenUrl,
			String realm,
			ref Parameter[] responseParameter
			) {
			return ObtainUnauthorizedRequestToken(
				requestTokenUrl,
				null,
				realm,
				null,
				ref responseParameter
			);
		}

		/// <summary>
		/// Obtain unauthorized request token from service provider
		/// </summary>
		/// <param name="requestTokenUrl">Request token URL</param>
		/// <param name="callbackURL">An absolute URL to which the Service Provider will redirect the User back when the Obtaining User Authorization step is completed.</param>
		/// <param name="realm">Realm for obtaining request token</param>
		/// <returns>Obtained request token</returns>
		public RequestToken ObtainUnauthorizedRequestToken(
			String requestTokenUrl,
			String callbackURL,
			String realm
			) {
			Parameter[] responseParameter = null;
			return ObtainUnauthorizedRequestToken(
				requestTokenUrl,
				callbackURL,
				realm,
				null,
				ref responseParameter
			);
		}

		/// <summary>
		/// Obtain unauthorized request token from service provider
		/// </summary>
		/// <param name="requestTokenUrl">Request token URL</param>
		/// <param name="callbackURL">An absolute URL to which the Service Provider will redirect the User back when the Obtaining User Authorization step is completed.</param>
		/// <param name="realm">Realm for obtaining request token</param>
		/// <param name="additionalParameters">Parameters added to Authorization header</param>
		/// <param name="responseParameters" >Parameters returned in response</param>
		/// <returns>Obtained request token</returns>
		public RequestToken ObtainUnauthorizedRequestToken(
			String requestTokenUrl,
			String callbackURL,
			String realm,
			Parameter[] additionalParameters,
			ref Parameter[] responseParameters
			) {

			if(additionalParameters == null)
				additionalParameters = new Parameter[0];

			String oauth_consumer_key = _consumerKey;
			String oauth_signature_method = "HMAC-SHA1";
			String oauth_timestamp =
				((DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1).Ticks) / (1000 * 10000)).ToString();
			String oauth_nonce =
				Guid.NewGuid().ToString();
			String oauth_callback =
				(callbackURL != null && callbackURL.Length > 0 ?
					callbackURL :
					"oob"
				);

			HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(requestTokenUrl);

			if(_webProxy != null)
				req.Proxy = _webProxy;

			req.Method = "POST";

			String oauth_signature =
				CreateHMACSHA1Signature(
					req.Method,
					requestTokenUrl,
					Parameter.ConCatAsArray(
						new Parameter[]{
                            new Parameter("oauth_consumer_key",oauth_consumer_key),
                            new Parameter ("oauth_signature_method",oauth_signature_method ),
                            new Parameter ("oauth_timestamp",oauth_timestamp),
                            new Parameter ("oauth_nonce",oauth_nonce ),
                            new Parameter ("oauth_callback",oauth_callback)
                        },
						additionalParameters
					),
					_consumerSecret,
					null
				);

			req.Headers.Add(
				"Authorization: OAuth " +
				"realm=\"" + realm + "\"," +
				"oauth_consumer_key=\"" + Parameter.EncodeParameterString(oauth_consumer_key) + "\"," +
				"oauth_signature_method=\"" + Parameter.EncodeParameterString(oauth_signature_method) + "\"," +
				"oauth_signature=\"" + Parameter.EncodeParameterString(oauth_signature) + "\"," +
				"oauth_timestamp=\"" + Parameter.EncodeParameterString(oauth_timestamp) + "\"," +
				"oauth_nonce=\"" + Parameter.EncodeParameterString(oauth_nonce) + "\"," +
				"oauth_callback=\"" + Parameter.EncodeParameterString(oauth_callback) + "\"" +
				(additionalParameters.Length > 0 ?
					"," + Parameter.ConCat(additionalParameters, "\"") :
					""
				)
			);

			HttpWebResponse resp = null;
			try {
				resp = (HttpWebResponse)req.GetResponse();
				StreamReader sr = new StreamReader(resp.GetResponseStream());
				responseParameters = Parameter.Parse(sr.ReadToEnd());

				String reqToken = null;
				String reqTokenSecret = null;

				foreach(Parameter param in responseParameters) {
					if(param.Name == "oauth_token")
						reqToken = param.Value;
					if(param.Name == "oauth_token_secret")
						reqTokenSecret = param.Value;
				}

				if(reqToken == null || reqTokenSecret == null)
					throw new InvalidOperationException();

				return new RequestToken(reqToken, reqTokenSecret);

			} finally {
				if(resp != null)
					resp.Close();
			}

		}

		#endregion

		#region RequestAccessToken

		/// <summary>
		/// Request access token responding to authenticated request token.
		/// </summary>
		/// <param name="verifier">Verifier string for authenticaed request token</param>
		/// <param name="requestToken">Authenticated request token</param>
		/// <param name="accessTokenUrl">Access token URL</param>
		/// <param name="realm">Realm for requesting access token</param>
		/// <returns>Responding access token</returns>
		public AccessToken RequestAccessToken(
			String verifier,
			RequestToken requestToken,
			String accessTokenUrl,
			String realm) {
			Parameter[] responseParameters = null;
			return RequestAccessToken(
				verifier,
				requestToken,
				accessTokenUrl,
				realm,
				null,
				ref responseParameters
			);
		}

		/// <summary>
		/// Request access token responding to authenticated request token.
		/// </summary>
		/// <param name="verifier">Verifier string for authenticaed request token</param>
		/// <param name="requestToken">Authenticated request token</param>
		/// <param name="accessTokenUrl">Access token URL</param>
		/// <param name="realm">Realm for requesting access token</param>
		/// <param name="responseParameters" >Parameters returned in response</param>       
		/// <returns>Responding access token</returns>
		public AccessToken RequestAccessToken(
			String verifier,
			RequestToken requestToken,
			String accessTokenUrl,
			String realm,
			ref Parameter[] responseParameters) {
			return RequestAccessToken(
				verifier,
				requestToken,
				accessTokenUrl,
				realm,
				null,
				ref responseParameters
			);
		}

		/// <summary>
		/// Request access token responding to authenticated request token.
		/// </summary>
		/// <param name="verifier">Verifier string for authenticaed request token</param>
		/// <param name="requestToken">Authenticated request token</param>
		/// <param name="accessTokenUrl">Access token URL</param>
		/// <param name="realm">Realm for requesting access token</param>
		/// <param name="additionalParameters">Parameters added to Authorization header</param>
		/// <param name="responseParameters" >Parameters returned in response</param>
		/// <returns>Responding access token</returns>
		public AccessToken RequestAccessToken(
			String verifier,
			RequestToken requestToken,
			String accessTokenUrl,
			String realm,
			Parameter[] additionalParameters,
			ref Parameter[] responseParameters) {

			if(additionalParameters == null)
				additionalParameters = new Parameter[0];

			String oauth_consumer_key = _consumerKey;
			String oauth_token = requestToken.TokenValue;
			String oauth_signature_method = "HMAC-SHA1";
			String oauth_timestamp =
				((DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1).Ticks) / (1000 * 10000)).ToString();
			String oauth_nonce =
				Guid.NewGuid().ToString();

			HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(accessTokenUrl);

			if(_webProxy != null)
				req.Proxy = _webProxy;

			req.Method = "POST";

			String oauth_signature =
				CreateHMACSHA1Signature(
					req.Method,
					accessTokenUrl,
					Parameter.ConCatAsArray(
						new Parameter[]{
                            new Parameter("oauth_consumer_key",oauth_consumer_key),
                            new Parameter("oauth_token",oauth_token ),
                            new Parameter ("oauth_signature_method",oauth_signature_method ),
                            new Parameter ("oauth_timestamp",oauth_timestamp),
                            new Parameter ("oauth_nonce",oauth_nonce ),
                            new Parameter ("oauth_verifier",verifier ),
                        },
						additionalParameters
					),
					_consumerSecret,
					requestToken.TokenSecret
				);

			req.Headers.Add(
				"Authorization: OAuth " +
				"realm=\"" + realm + "\"," +
				"oauth_consumer_key=\"" + Parameter.EncodeParameterString(oauth_consumer_key) + "\"," +
				"oauth_token=\"" + Parameter.EncodeParameterString(oauth_token) + "\"," +
				"oauth_signature_method=\"" + Parameter.EncodeParameterString(oauth_signature_method) + "\"," +
				"oauth_signature=\"" + Parameter.EncodeParameterString(oauth_signature) + "\"," +
				"oauth_timestamp=\"" + Parameter.EncodeParameterString(oauth_timestamp) + "\"," +
				"oauth_nonce=\"" + Parameter.EncodeParameterString(oauth_nonce) + "\"," +
				"oauth_verifier=\"" + Parameter.EncodeParameterString(verifier) + "\"" +
				(additionalParameters.Length > 0 ?
					"," + Parameter.ConCat(additionalParameters, "\"") :
					""
				)
			);

			HttpWebResponse resp = null;
			try {
				resp = (HttpWebResponse)req.GetResponse();
				StreamReader sr = new StreamReader(resp.GetResponseStream());

				responseParameters =
					Parameter.Parse(sr.ReadToEnd());

				String accessToken = null;
				String accessTokenSecret = null;
				foreach(Parameter param in responseParameters) {
					if(param.Name == "oauth_token")
						accessToken = param.Value;

					if(param.Name == "oauth_token_secret")
						accessTokenSecret = param.Value;
				}

				if(accessToken == null || accessTokenSecret == null)
					throw new InvalidOperationException();

				return new AccessToken(accessToken, accessTokenSecret);

			} finally {
				if(resp != null)
					resp.Close();
			}
		}

		#endregion

		#region AccessProtectedResource

		/// <summary>
		/// Access protected resource with access token
		/// </summary>
		/// <param name="accessToken">Access token</param>
		/// <param name="urlString">URL string for accessing protected resource</param>
		/// <param name="method">HTTP method to access</param>
		/// <param name="authorizationRealm">realm for accessing protected resource</param>
		/// <param name="queryParameters">Query parameter to be sent</param>
		/// <returns>HttpWebResponse from protected resource</returns>
		public void AccessProtectedResource(
			AccessToken accessToken,
			HttpWebRequest req,
			String urlString,
			String authorizationRealm,
			Parameter[] queryParameters
		) {
			AccessProtectedResource(
				accessToken,
				req,
				urlString,
				authorizationRealm,
				queryParameters,
				null);
		}

		/// <summary>
		/// Access protected resource with access token
		/// </summary>
		/// <param name="accessToken">Access token</param>
		/// <param name="urlString">URL string for accessing protected resource</param>
		/// <param name="method">HTTP method to access</param>
		/// <param name="authorizationRealm">realm for accessing protected resource</param>
		/// <param name="queryParameters">Query parameter to be sent</param>
		/// <param name="additionalParameters">Parameters added to Authorization header</param>
		/// <returns>HttpWebResponse from protected resource</returns>
		public void AccessProtectedResource(
			AccessToken accessToken,
			HttpWebRequest req,
			String urlString,
			String authorizationRealm,
			Parameter[] queryParameters,
			Parameter[] additionalParameters) {

			if(additionalParameters == null)
				additionalParameters = new Parameter[0];

			if(queryParameters == null)
				queryParameters = new Parameter[0];

			if(accessToken == null)
				accessToken = new AccessToken("", "");

			String oauth_consumer_key = _consumerKey;
			String oauth_token = accessToken.TokenValue;
			String oauth_signature_method = "HMAC-SHA1";
			String oauth_timestamp =
				((DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1).Ticks) / (1000 * 10000)).ToString();
			String oauth_nonce =
				Guid.NewGuid().ToString();

			if(_webProxy != null)
				req.Proxy = _webProxy;

			//Twitter service does not accept expect100continue
			req.ServicePoint.Expect100Continue = false;

			String oauth_signature =
				CreateHMACSHA1Signature(
					req.Method,
					urlString,
					Parameter.ConCatAsArray(
						new Parameter[]{
                            new Parameter("oauth_consumer_key",oauth_consumer_key),
                            new Parameter("oauth_token",oauth_token ),
                            new Parameter ("oauth_signature_method",oauth_signature_method ),
                            new Parameter ("oauth_timestamp",oauth_timestamp),
                            new Parameter ("oauth_nonce",oauth_nonce )
                        },
						additionalParameters,
						queryParameters
					),
					_consumerSecret,
					accessToken.TokenSecret
				);

			req.Headers.Add(
				"Authorization: OAuth " +
				"realm=\"" + authorizationRealm + "\"," +
				"oauth_consumer_key=\"" + Parameter.EncodeParameterString(oauth_consumer_key) + "\"," +
				"oauth_token=\"" + Parameter.EncodeParameterString(oauth_token) + "\"," +
				"oauth_signature_method=\"" + Parameter.EncodeParameterString(oauth_signature_method) + "\"," +
				"oauth_signature=\"" + Parameter.EncodeParameterString(oauth_signature) + "\"," +
				"oauth_timestamp=\"" + Parameter.EncodeParameterString(oauth_timestamp) + "\"," +
				"oauth_nonce=\"" + Parameter.EncodeParameterString(oauth_nonce) + "\"" +
				(additionalParameters.Length > 0 ?
					"," + Parameter.ConCat(additionalParameters, "\"") :
					""
				)
			);
		}

		#endregion

		#region CreateHMACSHA1Signature

		private static String CreateHMACSHA1Signature(
			String method,
			String url,
			Parameter[] parameterArray,
			String consumerSecret) {
			return CreateHMACSHA1Signature(method, url, parameterArray, consumerSecret, null);
		}

		private static String CreateHMACSHA1Signature(
			String method,
			String url,
			Parameter[] parameterArray,
			String consumerSecret,
			String tokenSecret) {

			if(consumerSecret == null)
				throw new NullReferenceException();

			if(tokenSecret == null)
				tokenSecret = "";

			method = method.ToUpper();

			url = url.ToLower();
			Uri uri = new Uri(url);
			url =
				uri.Scheme + "://" +
				uri.Host +
				((uri.Scheme.Equals("http") && uri.Port == 80 ||
							uri.Scheme.Equals("https") && uri.Port == 443) ?
							"" :
							uri.Port.ToString()
				) +
				uri.AbsolutePath;

			String concatenatedParameter =
				Parameter.ConcatToNormalize(parameterArray);

			HMACSHA1 alg = new HMACSHA1
				(
					Encode(
						Parameter.EncodeParameterString(consumerSecret) + "&" +
						Parameter.EncodeParameterString(tokenSecret)
					)
				);

			return
				System.Convert.ToBase64String(
					alg.ComputeHash(
						Encode(
							Parameter.EncodeParameterString(method) + "&" +
							Parameter.EncodeParameterString(url) + "&" +
							Parameter.EncodeParameterString(concatenatedParameter)
						)
					)
				);

		}

		#endregion

		#region BuildUserAuthorizationURL

		/// <summary>
		/// Build user authorization URL to authorize request token
		/// </summary>
		/// <param name="userAuthorizationUrl">User authorization URL served by Service Provider</param>
		/// <param name="requestToken">Request token</param>
		/// <returns>user authorization URL to authorize request token</returns>
		public static String BuildUserAuthorizationURL(
			String userAuthorizationUrl,
			RequestToken requestToken
			) {

			Uri uri = new Uri(userAuthorizationUrl);

			return
				uri.OriginalString +
				(uri.Query != null && uri.Query.Length > 0 ?
				"&" : "?") +
				"oauth_token=" + Parameter.EncodeParameterString(requestToken.TokenValue);

		}

		#endregion

		#region Encode / Decode

		private static byte[] Encode(String val) {
			MemoryStream ms = new MemoryStream();
			StreamWriter sw = new StreamWriter(ms, Encoding.ASCII);

			sw.Write(val);
			sw.Flush();

			return ms.ToArray();

		}

		private static String Decode(byte[] val) {
			MemoryStream ms = new MemoryStream(val);
			StreamReader sr = new StreamReader(ms, Encoding.ASCII);
			return sr.ReadToEnd();

		}

		#endregion

		#region RequestAccessToken XOuth

		public AccessToken RequestAccessToken(string url, string username, string password, string realm){
			return this.RequestAccessToken(url, username, password, realm, "client_auth");
		}

		public AccessToken RequestAccessToken(string url, string username, string password, string realm, string mode){
			Parameter[] resp;
			return this.RequestAccessToken(url, username, password, realm, "client_auth", out resp);
		}

		public AccessToken RequestAccessToken(string url, string username, string password, string realm, string mode, out Parameter[] responseParameters){
			String oauth_consumer_key = _consumerKey;
			String oauth_signature_method = "HMAC-SHA1";
			String oauth_timestamp =
				((DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1).Ticks) / (1000 * 10000)).ToString();
			String oauth_nonce =
				Guid.NewGuid().ToString();
	
			Parameter[] urlPrms = new Parameter[]{
				new Parameter ("x_auth_mode", mode),
				new Parameter ("x_auth_password", password),
				new Parameter ("x_auth_username", username),
			};
			var query = Parameter.ConCat(urlPrms);
			byte[] data = Encoding.ASCII.GetBytes(query);

			HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
			req.Method = "POST";
			req.ContentType = "application/x-www-form-urlencoded";
			req.ContentLength = query.Length;
			if(_webProxy != null)
				req.Proxy = _webProxy;

			using(Stream stream = req.GetRequestStream()){
				stream.Write(data, 0, data.Length);
			}

			Parameter[] prms = new Parameter[]{
				new Parameter ("oauth_consumer_key",oauth_consumer_key),
				new Parameter ("oauth_nonce",oauth_nonce ),
				new Parameter ("oauth_signature_method",oauth_signature_method ),
				new Parameter ("oauth_timestamp",oauth_timestamp),
				new Parameter ("oauth_version", "1.0"),
			};

			String oauth_signature =
				CreateHMACSHA1Signature(
					req.Method,
					url,
					Parameter.ConCatAsArray(urlPrms, prms),
					_consumerSecret
				);

			var headerString = String.Join("\",", prms.Select(prm => prm.Name + "=\"" + Parameter.EncodeParameterString(prm.Value))) + "\"";
			req.Headers.Add("Authorization: OAuth realm=\"" + realm + "\"," + headerString);

			HttpWebResponse resp = null;
			try {
				resp = (HttpWebResponse)req.GetResponse();
				StreamReader sr = new StreamReader(resp.GetResponseStream());

				responseParameters =
					Parameter.Parse(sr.ReadToEnd());

				String accessToken = null;
				String accessTokenSecret = null;
				foreach(Parameter param in responseParameters) {
					if(param.Name == "oauth_token")
						accessToken = param.Value;

					if(param.Name == "oauth_token_secret")
						accessTokenSecret = param.Value;
				}

				if(accessToken == null || accessTokenSecret == null)
					throw new InvalidOperationException();

				return new AccessToken(accessToken, accessTokenSecret);

			} finally {
				if(resp != null)
					resp.Close();
			}
		}

		#endregion
	}
}

