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
namespace Magnum.Servers.ValueProviders
{
	using System;
	using System.Linq.Expressions;
	using System.Reflection;
	using Collections;
	using Extensions;
	using Magnum.ValueProviders;


	public class RequestContextValueProvider :
		ValueProvider
	{
		static readonly Cache<string, Func<RequestContext, object>> _values;
		readonly RequestContext _requestContext;

		static RequestContextValueProvider()
		{
			_values = new Cache<string, Func<RequestContext, object>>();

			AddRequestProperty(r => r.AcceptTypes);
			AddRequestProperty(r => r.ContentEncoding);
			AddRequestProperty(r => r.ContentLength);
			AddRequestProperty(r => r.ContentType);
			AddRequestProperty(r => r.Cookies);
			AddRequestProperty(r => r.HasEntityBody);
			AddRequestProperty(r => r.Headers);
			AddRequestProperty(r => r.HttpMethod);
			AddRequestProperty(r => r.IsAuthenticated);
			AddRequestProperty(r => r.IsLocal);
			AddRequestProperty(r => r.IsSecureConnection);
			AddRequestProperty(r => r.KeepAlive);
			AddRequestProperty(r => r.LocalEndpoint);
			AddRequestProperty(r => r.ProtocolVersion);
			AddRequestProperty(r => r.QueryString);
			AddRequestProperty(r => r.RawUrl);
			AddRequestProperty(r => r.RemoteEndpoint);
			AddRequestProperty(r => r.Url);
			AddRequestProperty(r => r.UrlReferrer);
			AddRequestProperty(r => r.UserAgent);
			AddRequestProperty(r => r.UserHostAddress);
			AddRequestProperty(r => r.UserHostName);
			AddRequestProperty(r => r.UserLanguages);
		}

		public RequestContextValueProvider(RequestContext requestContext)
		{
			_requestContext = requestContext;
		}

		public bool GetValue(string key, Func<object, bool> matchingValueAction)
		{
			if (_values.Has(key))
				return matchingValueAction(_values[key](_requestContext));

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
			_values.Each((key, getValue) => valueAction(key, getValue(_requestContext)));
		}

		static void AddRequestProperty(Expression<Func<RequestContext, object>> expression)
		{
			PropertyInfo property = expression.GetMemberPropertyInfo();

			_values[property.Name] = expression.Compile();
		}
	}
}