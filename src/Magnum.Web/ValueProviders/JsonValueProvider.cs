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
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Web;
	using System.Web.Script.Serialization;
	using Abstractions;
	using Extensions;
	using Magnum.ValueProviders;

	public class JsonValueProvider :
		ValueProvider
	{
		private readonly ValueProvider _provider;

		public JsonValueProvider(HttpRequestBase httpRequest)
		{
			if (httpRequest.ContentType.StartsWith(MediaType.Json.ToString()))
			{
				_provider = CreateDictionaryFromJson(httpRequest.InputStream);
			}
			else
			{
				_provider = new EmptyValueProvider();
			}
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

		private static DictionaryValueProvider CreateDictionaryFromJson(Stream inputStream)
		{
			object data = GetDeserializedObject(inputStream);

			var backingStore = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

			AddValueToDictionary(backingStore, "", data);

			return new DictionaryValueProvider(backingStore);
		}

		private static object GetDeserializedObject(Stream inputStream)
		{
			string body = inputStream.ReadToEnd().ToUtf8String();
			if (body.IsEmpty())
				return null;

			return new JavaScriptSerializer().DeserializeObject(body);
		}

		private static void AddValueToDictionary(IDictionary<string, object> backingStore, string prefix, object value)
		{
			var dictionary = value as IDictionary<string, object>;
			if (dictionary != null)
			{
				dictionary.Each(x => AddValueToDictionary(backingStore, MakePropertyKey(prefix, x.Key), x.Value));
				return;
			}

			var list = value as IList<object>;
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					AddValueToDictionary(backingStore, MakeArrayKey(prefix, i), list[i]);
				}
				return;
			}

			backingStore[prefix] = value;
		}

		private static string MakeArrayKey(string prefix, int index)
		{
			return prefix + "[" + index.ToString(CultureInfo.InvariantCulture) + "]";
		}

		private static string MakePropertyKey(string prefix, string propertyName)
		{
			return prefix.IsEmpty() ? propertyName : prefix + "." + propertyName;
		}
	}
}