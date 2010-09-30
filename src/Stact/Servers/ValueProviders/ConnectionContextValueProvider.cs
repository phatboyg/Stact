// Copyright 2007-2010 The Apache Software Foundation.
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
namespace Stact.Servers.ValueProviders
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using Stact.ValueProviders;


	/// <summary>
	///   Maps access to the RequestContext for model binding
	/// </summary>
	public class ConnectionContextValueProvider :
		ValueProvider
	{
		readonly ValueProvider _provider;

		public ConnectionContextValueProvider(ConnectionContext connectionContext)
		{
			_provider = new MultipleValueProvider(GetProvidersForConnection(connectionContext).ToArray());
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

		static IEnumerable<ValueProvider> GetProvidersForConnection(ConnectionContext connectionContext)
		{
			if (connectionContext.Request.Cookies.Count > 0)
				yield return GetCookieCollection(connectionContext.Request);

			if (connectionContext.Request.ContentType.StartsWith("application/json"))
				yield return new JsonValueProvider(connectionContext.Request.InputStream);

			if (connectionContext.Request.QueryString.Count > 0)
				yield return GetQueryStringCollection(connectionContext.Request);

			if (connectionContext.Request.Headers.Count > 0)
				yield return GetHeaderCollection(connectionContext.Request);

			yield return new RequestContextValueProvider(connectionContext.Request);
		}

		static AccessorValueProvider GetCookieCollection(RequestContext request)
		{
			return new AccessorValueProvider(x => request.Cookies[x],
			                                 () => request.Cookies.Cast<Cookie>().Select(x => x.Name).ToArray());
		}

		static AccessorValueProvider GetQueryStringCollection(RequestContext request)
		{
			return new AccessorValueProvider(x => request.QueryString[x], () => request.QueryString.AllKeys);
		}

		static AccessorValueProvider GetHeaderCollection(RequestContext request)
		{
			return new AccessorValueProvider(x => request.Headers[x], () => request.Headers.AllKeys);
		}
	}
}