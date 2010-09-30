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
namespace Stact.Servers
{
	using System.IO;
	using System.Text;
	using System.Text.RegularExpressions;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using Serialization;


	public static class ExtensionsToRequestContext
	{
		static readonly Regex _charsetPattern = new Regex(@"charset=([\w-]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		static readonly Encoding _defaultEncoding = Encoding.GetEncoding("ISO-8859-1");

		public static Encoding GetEncoding(this RequestContext context)
		{
			var contentType = context.Headers["Content-Type"];
			if (contentType == null)
				return _defaultEncoding;

			Match match = _charsetPattern.Match(contentType);
			if (match.Success == false)
				return _defaultEncoding;

			return Encoding.GetEncoding(match.Groups[1].Value);
		}

		public static JObject ReadJson(this RequestContext context)
		{
			using (var streamReader = new StreamReader(context.InputStream, context.GetEncoding()))
			using (var jsonReader = new JsonTextReader(streamReader))
				return JObject.Load(jsonReader);
		}

		public static T ReadJson<T>(this RequestContext context)
		{
			using (var streamReader = new StreamReader(context.InputStream, context.GetEncoding()))
			using (var jsonReader = new JsonTextReader(streamReader))
			{
				return (T)new JsonSerializer().Deserialize(jsonReader, typeof(T));
			}
		}

		public static object ReadJsonObject(this RequestContext context)
		{
			using (var streamReader = new StreamReader(context.InputStream, context.GetEncoding()))
			using (var jsonReader = new JsonTextReader(streamReader))
			{
				return new JsonSerializer().Deserialize(jsonReader);
			}
		}

		public static JArray ReadJsonArray(this RequestContext context)
		{
			using (var streamReader = new StreamReader(context.InputStream, context.GetEncoding()))
			using (var jsonReader = new JsonTextReader(streamReader))
				return JArray.Load(jsonReader);
		}

		public static string ReadString(this RequestContext context)
		{
			using (var streamReader = new StreamReader(context.InputStream, context.GetEncoding()))
				return streamReader.ReadToEnd();
		}
	}
}