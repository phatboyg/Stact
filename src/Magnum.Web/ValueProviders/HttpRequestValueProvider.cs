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
namespace Magnum.Web.ValueProviders
{
	using System;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Web;
	using System.Web.Routing;
	using Collections;

	public class HttpRequestValueProvider :
		ValueProvider
	{
		private static readonly Cache<string, Func<HttpRequestBase, object>> _values;
		private readonly HttpRequestBase _httpRequest;

		static HttpRequestValueProvider()
		{
			_values = new Cache<string, Func<HttpRequestBase, object>>();

			AddRequestProperty(r => r.AcceptTypes);
			AddRequestProperty(r => r.ApplicationPath);
			AddRequestProperty(r => r.AppRelativeCurrentExecutionFilePath);
			AddRequestProperty(r => r.Browser);
			AddRequestProperty(r => r.ClientCertificate);
			AddRequestProperty(r => r.ContentEncoding);
			AddRequestProperty(r => r.ContentLength);
			AddRequestProperty(r => r.ContentType);
			AddRequestProperty(r => r.Cookies);
			AddRequestProperty(r => r.CurrentExecutionFilePath);
			AddRequestProperty(r => r.FilePath);
			AddRequestProperty(r => r.Files);
			AddRequestProperty(r => r.Filter);
			AddRequestProperty(r => r.Form);
			AddRequestProperty(r => r.Headers);
			AddRequestProperty(r => r.HttpMethod);
			AddRequestProperty(r => r.IsAuthenticated);
			AddRequestProperty(r => r.IsLocal);
			AddRequestProperty(r => r.IsSecureConnection);
			AddRequestProperty(r => r.LogonUserIdentity);
			AddRequestProperty(r => r.Params);
			AddRequestProperty(r => r.Path);
			AddRequestProperty(r => r.PathInfo);
			AddRequestProperty(r => r.PhysicalApplicationPath);
			AddRequestProperty(r => r.PhysicalPath);
			AddRequestProperty(r => r.QueryString);
			AddRequestProperty(r => r.RawUrl);
			AddRequestProperty(r => r.RequestType);
			AddRequestProperty(r => r.ServerVariables);
			AddRequestProperty(r => r.TotalBytes);
			AddRequestProperty(r => r.Url);
			AddRequestProperty(r => r.UrlReferrer);
			AddRequestProperty(r => r.UserAgent);
			AddRequestProperty(r => r.UserHostAddress);
			AddRequestProperty(r => r.UserHostName);
			AddRequestProperty(r => r.UserLanguages);
		}

		public HttpRequestValueProvider(RequestContext requestContext)
			: this(requestContext.HttpContext)
		{
		}

		public HttpRequestValueProvider(HttpContextBase httpContext)
			: this(httpContext.Request)
		{
		}

		public HttpRequestValueProvider(HttpRequestBase httpRequest)
		{
			_httpRequest = httpRequest;
		}

		public bool GetValue(string key, Func<object, bool> matchingValueAction)
		{
			if (_values.Has(key))
			{
				return matchingValueAction(_values[key](_httpRequest));
			}

			return false;
		}

		public bool GetValue(string key, Func<object, bool> matchingValueAction, Action missingValueAction)
		{
			if (GetValue(key, matchingValueAction))
				return true;

			missingValueAction();
			return false;
		}

		public void GetAll(Action<string, object> valueAction)
		{
			_values.Each((key, getValue) => valueAction(key, getValue(_httpRequest)));
		}

		private static void AddRequestProperty(Expression<Func<HttpRequestBase, object>> expression)
		{
			PropertyInfo property = expression.GetMemberPropertyInfo();

			_values[property.Name] = expression.Compile();
		}
	}
}