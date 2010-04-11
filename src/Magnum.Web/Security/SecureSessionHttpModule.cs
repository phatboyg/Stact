// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Magnum.Web.Security
{
	using System;
	using System.Configuration;
	using System.Globalization;
	using System.Security.Authentication;
	using System.Security.Cryptography;
	using System.Text;
	using System.Web;

	public class SecureSessionHttpModule :
		IHttpModule
	{
		public const string SecureSessionCookieName = "ASPSessionID";
		public const string SecureSessionSaltKey = "SecureSessionSalt";
		private string _cookieName;
		private string _salt;
		private byte[] _saltHash;

		public void Init(HttpApplication context)
		{
			_salt = ConfigurationManager.AppSettings[SecureSessionSaltKey];
			if (string.IsNullOrEmpty(_salt))
				throw new ConfigurationErrorsException("The SecureSessionHttpModule requires the " + SecureSessionSaltKey + " application setting");

			_saltHash = Encoding.UTF8.GetBytes(_salt);

			_cookieName = ConfigurationManager.AppSettings[SecureSessionCookieName];
			if (string.IsNullOrEmpty(_cookieName))
			{
				_cookieName = "asp.net_sessionid";
			}

			context.BeginRequest += OnBeginRequest;
			context.EndRequest += OnEndRequest;
		}

		public void Dispose()
		{
		}

		private void OnBeginRequest(object sender, EventArgs e)
		{
			var context = sender as HttpApplication;
			if (context == null) return;

			WithSessionCookie(context.Request.Cookies, cookie => { VerifySessionCookieHash(context.Request, cookie); });
		}

		private void OnEndRequest(object sender, EventArgs e)
		{
			var context = sender as HttpApplication;
			if (context == null) return;

			WithSessionCookie(context.Response.Cookies, cookie =>
				{
					cookie.HttpOnly = true;
					if (cookie.Value.Length == 24)
						cookie.Value += GetSessionIdCheck(cookie.Value, context.Request);
				});
		}

		private void VerifySessionCookieHash(HttpRequest request, HttpCookie cookie)
		{
			try
			{
				if (cookie.Value.Length <= 24)
					throw new AuthenticationException("Access denied");

				string id = cookie.Value.Substring(0, 24);
				string check = cookie.Value.Substring(24);

				string validation = GetSessionIdCheck(id, request);

				if (String.CompareOrdinal(check, validation) != 0)
					throw new AuthenticationException("Access denied");

				cookie.Value = id;
			}
			catch (Exception)
			{
				RemoveSessionCookie(request);
			}
		}

		private void RemoveSessionCookie(HttpRequest request)
		{
			WithSessionCookie(request.Cookies, cookie =>
				{
					cookie.Value = string.Empty;
					cookie.Expires = DateTime.Now - new TimeSpan(1, 0, 0, 0);
				});
		}

		private string GetSessionIdCheck(string id, HttpRequest request)
		{
			var check = new StringBuilder(id, 512);

			string ip = request.UserHostAddress;
			check.Append(ip.Substring(0, ip.IndexOf('.', ip.IndexOf('.') + 1)));

			check.Append(request.UserAgent);

			using (var hmac = new HMACSHA1(_saltHash))
			{
				return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(check.ToString())));
			}
		}

		private void WithSessionCookie(HttpCookieCollection cookies, Action<HttpCookie> cookieAction)
		{
			int count = cookies.Count;

			for (int i = 0; i < count; i++)
			{
				if (String.Compare(cookies[i].Name, _cookieName, true, CultureInfo.InvariantCulture) == 0)
				{
					cookieAction(cookies[i]);
					break;
				}
			}
		}
	}
}