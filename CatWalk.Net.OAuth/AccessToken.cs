/*
	OAuthLib by 7k8m http://oauthlib.codeplex.com
	
	$Id$
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Security.Cryptography;

namespace CatWalk.Net.OAuth {
	/// <summary>
	/// Stands for access token
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(AccessTokenConverter))]
	public class AccessToken : Token {
		public AccessToken(String tokenValue, String tokenSecret) : base(tokenValue, tokenSecret) {
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
