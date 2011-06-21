/*
	OAuthLib by 7k8m http://oauthlib.codeplex.com
	
	$Id: Parameter.cs 14 2010-01-06 06:22:29Z catwalk $
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web;
using System.Globalization;

namespace CatWalk.OAuth{
	/// <summary>
	/// Parameter with name and value.
	/// </summary>
	public class Parameter
	{
		private String _name;
		private String _value;

		/// <summary>
		/// Name of parameter
		/// </summary>
		public String Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Value of parameter
		/// </summary>
		public String Value
		{
			get
			{
				return _value;
			}
		}

		/// <summary>
		/// Construct parameter
		/// </summary>
		/// <param name="name">Name of parameter</param>
		/// <param name="value">Value of parameter</param>
		public Parameter(String name, String value)
		{
			_name = name;
			_value = value;
		}

		private static Parameter[] SortToNormalize(Parameter[] parameterArray)
		{
			List<Parameter> l = new List<Parameter>(parameterArray);
			l.Sort(
				delegate(Parameter x, Parameter y)
				{
					if (!x._name.Equals(y._name))
						return EncodeParameterString(x._name).CompareTo(
							EncodeParameterString(y._name)
						);
					else
						return EncodeParameterString(x._value).CompareTo(
								EncodeParameterString(y._value)
							);
				}
			);

			return l.ToArray();

		}

		internal static String ConcatToNormalize(Parameter[] parameterArray)
		{
			parameterArray = SortToNormalize(parameterArray);
			return ConCat(parameterArray,"");

		}

		public static string ConCat(Parameter[] parameterArray)
		{
			return ConCat(parameterArray, "");
		}

		internal static string ConCat(Parameter[] parameterArray,String qutationMark)
		{

			if (parameterArray.Length == 0)
				return "";

			StringBuilder sb = new StringBuilder();
			foreach (Parameter parameter in parameterArray)
			{
				sb.Append(qutationMark + EncodeParameterString(parameter._name) + qutationMark);
				sb.Append("=");
				sb.Append(qutationMark + EncodeParameterString(parameter._value) + qutationMark);
				sb.Append("&");
			}

			sb.Remove(sb.Length - 1, 1);
			return sb.ToString();
		}

		internal static Parameter[] ConCatAsArray(
			params Parameter[][] parameterArrayArray
		)
		{
			List<Parameter> result = new List<Parameter>();
			foreach (Parameter[] array in parameterArrayArray)
			{
				result.AddRange(array);
			}

			return result.ToArray();

		}

		internal static Parameter[] Parse(String source)
		{

			List<Parameter > list =new List<Parameter> ();

			String[] nameAndValSetArray = source.Split('&');

			foreach (String nameAndValSet in nameAndValSetArray ){

				String[] nameAndVal = nameAndValSet.Split('=');

				nameAndVal[0] = Uri.UnescapeDataString(nameAndVal[0]);
				nameAndVal[1] = Uri.UnescapeDataString(nameAndVal[1]);

				list.Add(
					new Parameter(
						nameAndVal[0],
						nameAndVal[1]
					)
				);
							
			}

			return list.ToArray();

		}

		internal static String EncodeParameterString(String val){
            StringBuilder sb =new StringBuilder ();
            foreach (char c in val)
            {
                if (('\u0041' <= c && c <= '\u005a') || //ALPHA
                    ('\u0061' <= c && c <= '\u007a') || //ALPHA
                    ('\u0030' <= c && c <= '\u0039') || //DIGIT
                    c == '\u002d' ||//"-"
                    c == '\u002e' ||//"."
                    c == '\u005f' ||//"_"
                    c == '\u007e') //"~"
                {
                    sb.Append(c);
                }
                else
                {
                    byte[] encoded = Encoding.UTF8.GetBytes(new char[] { c });
                    for (int i = 0; i < encoded.Length; i ++ )
                    {
                        sb.Append('%');
                        sb.Append(encoded[i].ToString("X2"));
                    }
                }
            }

            return sb.ToString();
		}


		public override string ToString() {
			return String.Format("Name={0}, Value={1}", this.Name, this.Value);
		}
	}
}
