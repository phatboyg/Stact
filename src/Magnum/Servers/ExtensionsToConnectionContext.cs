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
namespace Magnum.Servers
{
	using System;
	using System.IO;
	using System.Text;
	using Extensions;


	public static class ExtensionsToConnectionContext
	{
		public static string GetRequestUri(this ConnectionContext context)
		{
			string localPath = context.Request.Url.LocalPath;
			string pathAndQuery = context.Server.BaseUri.PathAndQuery;

			if (pathAndQuery != "/" && localPath.StartsWith(pathAndQuery, StringComparison.InvariantCultureIgnoreCase))
			{
				localPath = localPath.Substring(pathAndQuery.Length);
				if (localPath.Length == 0)
					localPath = "/";
			}
			return localPath;
		}

		public static void WriteHtml(this ResponseContext response, string text)
		{
			const string header = @"<!DOCTYPE html><html>";
			const string footer = @"</html>";

			response.ContentType = "text/html; charset=\"utf-8\"";
			response.Write(header, text, footer);
		}

		public static void Write(this ResponseContext response, params string[] text)
		{
			using (var writer = new StreamWriter(response.OutputStream, Encoding.UTF8))
			{
				text.Each(writer.Write);
				writer.Flush();
			}
		}


		public static void SetStatusToNotModified(this ConnectionContext context)
		{
			context.Response.StatusCode = 304;
			context.Response.StatusDescription = "Not Modified";
		}

		/// <summary>
		/// 400 - Bad Request
		/// </summary>
		/// <param name="context"></param>
		public static void SetStatusToBadRequest(this ConnectionContext context)
		{
			context.Response.StatusCode = 400;
			context.Response.StatusDescription = "Bad Request";
		}

		/// <summary>
		/// 401 - Not Authorized
		/// </summary>
		/// <param name="context"></param>
		public static void SetStatusToUnauthorized(this ConnectionContext context)
		{
			context.Response.StatusCode = 401;
			context.Response.StatusDescription = "Unauthorized";
		}

		public static void SetStatusToForbidden(this ConnectionContext context)
		{
			context.Response.StatusCode = 403;
			context.Response.StatusDescription = "Forbidden";
		}

		public static void SetStatusToNotFound(this ConnectionContext context)
		{
			context.Response.StatusCode = 404;
			context.Response.StatusDescription = "Not Found";
		}
	}
}