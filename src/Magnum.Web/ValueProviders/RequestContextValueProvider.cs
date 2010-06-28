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
	using System.Web;
	using System.Web.Routing;
	using Abstractions;
	using Magnum.ValueProviders;


	/// <summary>
	///   Maps access to the RequestContext for model binding
	/// </summary>
	public class RequestContextValueProvider :
		ValueProvider
	{
		readonly ValueProvider _provider;

		public RequestContextValueProvider(RequestContext requestContext)
		{
			var providers = new[]
				{
					new DictionaryValueProvider(requestContext.RouteData.Values),
					GetCookieCollection(requestContext.HttpContext.Request),
					requestContext.HttpContext.Request.ContentType.StartsWith(MediaType.Json.ToString())
						? (ValueProvider)new JsonValueProvider(requestContext.HttpContext.Request.InputStream)
						: new EmptyValueProvider(),
					GetFormCollection(requestContext.HttpContext.Request),
					GetQueryStringCollection(requestContext.HttpContext.Request),
					GetFileCollection(requestContext.HttpContext.Request),
					GetHeaderCollection(requestContext.HttpContext.Request),
					new HttpRequestValueProvider(requestContext),
				};

			_provider = new MultipleValueProvider(providers);
		}

		public bool GetValue(string key, Func<object, bool> matchingValueAction)
		{
			return _provider.GetValue(key, matchingValueAction);
		}

		public bool GetValue(string key, Func<object, bool> matchingValueAction, Action missingValueAction)
		{
			return _provider.GetValue(key, matchingValueAction, missingValueAction);
		}

		public void GetAll(Action<string, object> valueAction)
		{
			_provider.GetAll(valueAction);
		}

		static AccessorValueProvider GetFileCollection(HttpRequestBase request)
		{
			return new AccessorValueProvider(x => request.Files[x], () => request.Files.AllKeys);
		}

		static AccessorValueProvider GetCookieCollection(HttpRequestBase request)
		{
			return new AccessorValueProvider(x => request.Cookies[x], () => request.Cookies.AllKeys);
		}

		static AccessorValueProvider GetFormCollection(HttpRequestBase request)
		{
			return new AccessorValueProvider(x => request.Form[x], () => request.Form.AllKeys);
		}

		static AccessorValueProvider GetQueryStringCollection(HttpRequestBase request)
		{
			return new AccessorValueProvider(x => request.QueryString[x], () => request.QueryString.AllKeys);
		}

		static AccessorValueProvider GetHeaderCollection(HttpRequestBase request)
		{
			return new AccessorValueProvider(x => request.Headers[x], () => request.Headers.AllKeys);
		}
	}
}