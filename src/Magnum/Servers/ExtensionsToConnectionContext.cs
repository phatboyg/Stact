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
	using System.Web;
	using Extensions;
	using Internal;
	using Newtonsoft.Json;


	public static class ExtensionsToConnectionContext
	{
		const string XRequestedWithHeader = "X-Requested-With";
		const string XmlHttpRequestValue = "XMLHttpRequest";

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

		public static bool IsAjaxRequest(this ConnectionContext connectionContext)
		{
			string header = connectionContext.Request.Headers[XRequestedWithHeader];
			if (header.IsEmpty())
				return false;

			return XmlHttpRequestValue.Equals(header, StringComparison.InvariantCultureIgnoreCase);
		}

		/// <summary>
		/// Writes the object as a JSON response if the request is an Ajax request, otherwise the
		/// response is wrapped in an HTML text area for displaying to the user.
		/// </summary>
		/// <param name="obj"></param>
		public static void WriteObject(this ConnectionContext context, object obj)
		{
			if (context.IsAjaxRequest())
				context.Response.WriteJson(obj);
			else
			{
				string json = GetJsonString(obj);

				string html = "<body><textarea rows=\"10\" cols=\"80\">" + HttpUtility.HtmlEncode(json)
							  + "</textarea></body>";

				context.Response.WriteHtml(html);
			}
		}

		static string GetJsonString(object obj)
		{
			using (var memoryStream = new MemoryStream())
			using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
			{
				using (var jsonTextWriter = new JsonTextWriter(streamWriter)
					{
						Formatting = Formatting.None
					})
				{
					var jsonSerializer = new JsonSerializer
						{
							Converters =
								{
									new JsonEnumConverter()
								},
						};

					jsonSerializer.Serialize(jsonTextWriter, obj);
				}
				streamWriter.Flush();

				return Encoding.UTF8.GetString(memoryStream.ToArray());
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