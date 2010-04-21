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
namespace Magnum.TestFramework
{
	using System;
	using System.Collections.Generic;
	using System.Web;
	using System.Web.Routing;
	using Collections;
	using Extensions;
	using NUnit.Framework;
	using Rhino.Mocks;

	public static class AssertionsForRouting
	{
		private static readonly ObjectToDictionaryConverter _converter = new ObjectToDictionaryConverter();

		public static RouteCollection ShouldRoute(this RouteCollection routes, string url)
		{
			return ShouldRoute(routes, url, new {});
		}

		public static RouteCollection ShouldRoute(this RouteCollection routes, string url, object expectations)
		{
			var httpRequestMock = MockRepository.GenerateMock<HttpRequestBase>();
			httpRequestMock.Stub(x => x.AppRelativeCurrentExecutionFilePath).Return(url);
			httpRequestMock.Stub(x => x.PathInfo).Return("");
			httpRequestMock.Stub(x => x.HttpMethod).Return("GET");

			var httpContextMock = MockRepository.GenerateMock<HttpContextBase>();
			httpContextMock.Stub(x => x.Request).Return(httpRequestMock);

			RouteData routeData = routes.GetRouteData(httpContextMock);

			Assert.IsNotNull(routeData, "The route was not found: " + url);

			IDictionary<string, object> checkItems = _converter.Convert(expectations);

			checkItems.Each(item =>
				{
					Assert.IsTrue(string.Equals(item.Value.ToString(), routeData.Values[item.Key].ToString(), StringComparison.OrdinalIgnoreCase),
						"Expected '{0}', not '{1}' for '{2}'.".FormatWith(item.Value, routeData.Values[item.Key], item.Key));
				});

			return routes;
		}
	}
}