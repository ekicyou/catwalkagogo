﻿/*
	OAuthLib by 7k8m http://oauthlib.codeplex.com
	
	$Id$
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using System.Threading;

namespace CatWalk.Net.OAuth {
	using Parameter = KeyValuePair<string, string>;
	
	/// <summary>
	/// Stands for access token
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(AccessTokenConverter))]
	public class AccessToken : Token {
		public AccessToken(String tokenValue, String tokenSecret) : base(tokenValue, tokenSecret) {
		}

		public static AccessToken FromRequest(GettingWebRequest req, CancellationToken token){
			using(var res = req.GetResponse(token)){
				return FromResponse(res);
			}
		}

		public static AccessToken FromResponse(WebResponse resp){
			StreamReader sr = new StreamReader(resp.GetResponseStream());

			string accessToken = null;
			string accessTokenSecret = null;
			foreach(Parameter param in NetUtility.ParseQueryString(sr.ReadToEnd())) {
				if(param.Key == "oauth_token")
					accessToken = param.Value;

				if(param.Key == "oauth_token_secret")
					accessTokenSecret = param.Value;
			}

			if(accessToken == null || accessTokenSecret == null)
				throw new InvalidOperationException();

			return new AccessToken(accessToken, accessTokenSecret);
		}
	}

	public class AccessTokenConverter : TypeConverter{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType.Equals(typeof(string)) || sourceType.Equals(typeof(AccessToken));
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType.Equals(typeof(string)) || destinationType.Equals(typeof(AccessToken));
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			if(value is string){
				return ConvertFromStringInternal(value as string);
			}else if(value is AccessToken){
				return ConvertToString(value as AccessToken);
			}else{
				return value;
			}
		}

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(destinationType.Equals(value.GetType())){
				return value;
			}else if(value is string){
				if(destinationType.Equals(typeof(AccessToken))){
					return ConvertFromStringInternal(value as string);
				}
			}else if(value is AccessToken){
				if(destinationType.Equals(typeof(string))){
					return ConvertToString(value as AccessToken);
				}
			}
			return null;
		}

		private static string ConvertToString(AccessToken token){
			var text = token.TokenValue + "&" + token.TokenSecret;
			return Protect(text);
		}

		private static object ConvertFromStringInternal(string text){
			var tokens = Unprotect(text).Split('&');
			return new AccessToken(tokens[0], tokens[1]);
		}

		private static readonly byte[] optionalEntropy = new byte[]{0x01, 0xba, 0x33, 0x12, 0x9a};

		private static string Protect(string plain){
			if(plain == null){
				return "";
			}else{
				var data = Encoding.UTF8.GetBytes(plain);
				try{
					return Convert.ToBase64String(ProtectedData.Protect(data, optionalEntropy, DataProtectionScope.CurrentUser));
				}catch{
					return plain;
				}
			}
		}
		
		private static string Unprotect(string encrypted){
			if(encrypted == null){
				return "";
			}else{
				var data = Convert.FromBase64String(encrypted);
				try{
					return Encoding.UTF8.GetString(ProtectedData.Unprotect(data, optionalEntropy, DataProtectionScope.CurrentUser));
				}catch{
					return encrypted;
				}
			}
		}
	}
}
