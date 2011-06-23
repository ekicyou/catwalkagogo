/*
	OAuthLib by 7k8m http://oauthlib.codeplex.com
	
	$Id$
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Security.Cryptography;
using System.IO;
using System.Linq;

namespace CatWalk.Net.OAuth {
	using Parameter = KeyValuePair<string, string>;

	public class Consumer {
		#region Data

		private readonly String _consumerKey;
		private readonly String _consumerSecret;

		public int Timeout{get; set;}

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
			this.Timeout = 100 * 1000;
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

			req.Timeout = this.Timeout;

			req.Method = WebRequestMethods.Http.Post;

			String oauth_signature =
				CreateHMACSHA1Signature(
					req.Method,
					requestTokenUrl,
					(new Parameter[]{
						new Parameter("oauth_consumer_key",oauth_consumer_key),
						new Parameter ("oauth_signature_method",oauth_signature_method ),
						new Parameter ("oauth_timestamp",oauth_timestamp),
						new Parameter ("oauth_nonce",oauth_nonce ),
						new Parameter ("oauth_callback",oauth_callback)
					}).Concat(additionalParameters),
					_consumerSecret,
					null
				);

			req.Headers.Add(
				"Authorization: OAuth " +
				"realm=\"" + realm + "\"," +
				"oauth_consumer_key=\"" + Uri.EscapeDataString(oauth_consumer_key) + "\"," +
				"oauth_signature_method=\"" + Uri.EscapeDataString(oauth_signature_method) + "\"," +
				"oauth_signature=\"" + Uri.EscapeDataString(oauth_signature) + "\"," +
				"oauth_timestamp=\"" + Uri.EscapeDataString(oauth_timestamp) + "\"," +
				"oauth_nonce=\"" + Uri.EscapeDataString(oauth_nonce) + "\"," +
				"oauth_callback=\"" + Uri.EscapeDataString(oauth_callback) + "\"" +
				(additionalParameters.Length > 0 ? "," + additionalParameters.EncodeQuery("\"") : "")
			);

			HttpWebResponse resp = null;
			try {
				resp = (HttpWebResponse)req.GetResponse();
				StreamReader sr = new StreamReader(resp.GetResponseStream());
				responseParameters = NetUtility.ParseQueryString(sr.ReadToEnd()).ToArray();

				String reqToken = null;
				String reqTokenSecret = null;

				foreach(Parameter param in responseParameters) {
					if(param.Key == "oauth_token")
						reqToken = param.Value;
					if(param.Key == "oauth_token_secret")
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

			req.Timeout = this.Timeout;

			req.Method = WebRequestMethods.Http.Post;

			String oauth_signature =
				CreateHMACSHA1Signature(
					req.Method,
					accessTokenUrl,
					(new Parameter[]{
						new Parameter("oauth_consumer_key",oauth_consumer_key),
						new Parameter("oauth_token",oauth_token ),
						new Parameter ("oauth_signature_method",oauth_signature_method ),
						new Parameter ("oauth_timestamp",oauth_timestamp),
						new Parameter ("oauth_nonce",oauth_nonce ),
						new Parameter ("oauth_verifier",verifier ),
					}).Concat(additionalParameters),
					_consumerSecret,
					requestToken.TokenSecret
				);

			req.Headers.Add(
				"Authorization: OAuth " +
				"realm=\"" + realm + "\"," +
				"oauth_consumer_key=\"" + Uri.EscapeDataString(oauth_consumer_key) + "\"," +
				"oauth_token=\"" + Uri.EscapeDataString(oauth_token) + "\"," +
				"oauth_signature_method=\"" + Uri.EscapeDataString(oauth_signature_method) + "\"," +
				"oauth_signature=\"" + Uri.EscapeDataString(oauth_signature) + "\"," +
				"oauth_timestamp=\"" + Uri.EscapeDataString(oauth_timestamp) + "\"," +
				"oauth_nonce=\"" + Uri.EscapeDataString(oauth_nonce) + "\"," +
				"oauth_verifier=\"" + Uri.EscapeDataString(verifier) + "\"" +
				(additionalParameters.Length > 0 ? "," + additionalParameters.EncodeQuery("\"") : "")
			);

			HttpWebResponse resp = null;
			try {
				resp = (HttpWebResponse)req.GetResponse();
				StreamReader sr = new StreamReader(resp.GetResponseStream());

				responseParameters = NetUtility.ParseQueryString(sr.ReadToEnd()).ToArray();

				String accessToken = null;
				String accessTokenSecret = null;
				foreach(Parameter param in responseParameters) {
					if(param.Key == "oauth_token")
						accessToken = param.Value;

					if(param.Key == "oauth_token_secret")
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

			req.Timeout = this.Timeout;

			//Twitter service does not accept expect100continue
			req.ServicePoint.Expect100Continue = false;

			String oauth_signature =
				CreateHMACSHA1Signature(
					req.Method,
					urlString,
					(new Parameter[]{
						new Parameter("oauth_consumer_key",oauth_consumer_key),
						new Parameter("oauth_token",oauth_token ),
						new Parameter ("oauth_signature_method",oauth_signature_method ),
						new Parameter ("oauth_timestamp",oauth_timestamp),
						new Parameter ("oauth_nonce",oauth_nonce )
					}).Concat(additionalParameters).Concat(queryParameters),
					_consumerSecret,
					accessToken.TokenSecret
				);

			req.Headers.Add(
				"Authorization: OAuth " +
				"realm=\"" + authorizationRealm + "\"," +
				"oauth_consumer_key=\"" + Uri.EscapeDataString(oauth_consumer_key) + "\"," +
				"oauth_token=\"" + Uri.EscapeDataString(oauth_token) + "\"," +
				"oauth_signature_method=\"" + Uri.EscapeDataString(oauth_signature_method) + "\"," +
				"oauth_signature=\"" + Uri.EscapeDataString(oauth_signature) + "\"," +
				"oauth_timestamp=\"" + Uri.EscapeDataString(oauth_timestamp) + "\"," +
				"oauth_nonce=\"" + Uri.EscapeDataString(oauth_nonce) + "\"" +
				(additionalParameters.Length > 0 ? "," + additionalParameters.EncodeQuery("\"") : "")
			);
		}

		#endregion

		#region CreateHMACSHA1Signature

		private static String CreateHMACSHA1Signature(
			String method,
			String url,
			IEnumerable<Parameter> parameters,
			String consumerSecret) {
			return CreateHMACSHA1Signature(method, url, parameters, consumerSecret, null);
		}

		private static String CreateHMACSHA1Signature(
			String method,
			String url,
			IEnumerable<Parameter> parameters,
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

			String concatenatedParameter = parameters.EncodeQuery(true);

			HMACSHA1 alg = new HMACSHA1
				(
					Encode(
						Uri.EscapeDataString(consumerSecret) + "&" +
						Uri.EscapeDataString(tokenSecret)
					)
				);

			return
				System.Convert.ToBase64String(
					alg.ComputeHash(
						Encode(
							Uri.EscapeDataString(method) + "&" +
							Uri.EscapeDataString(url) + "&" +
							Uri.EscapeDataString(concatenatedParameter)
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
				"oauth_token=" + Uri.EscapeDataString(requestToken.TokenValue);

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
			var query = urlPrms.EncodeQuery();
			byte[] data = Encoding.ASCII.GetBytes(query);

			HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
			req.Method = WebRequestMethods.Http.Post;
			req.ContentType = "application/x-www-form-urlencoded";
			req.ContentLength = query.Length;
			req.Timeout = this.Timeout;

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
					urlPrms.Concat(prms),
					_consumerSecret
				);

			var headerString = String.Join("\",", prms.Select(prm => prm.Key + "=\"" + Uri.EscapeDataString(prm.Value))) + "\"";
			req.Headers.Add("Authorization: OAuth realm=\"" + realm + "\"," + headerString);

			HttpWebResponse resp = null;
			try {
				resp = (HttpWebResponse)req.GetResponse();
				StreamReader sr = new StreamReader(resp.GetResponseStream());

				responseParameters =
					NetUtility.ParseQueryString(sr.ReadToEnd()).ToArray();

				String accessToken = null;
				String accessTokenSecret = null;
				foreach(Parameter param in responseParameters) {
					if(param.Key == "oauth_token")
						accessToken = param.Value;

					if(param.Key == "oauth_token_secret")
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

