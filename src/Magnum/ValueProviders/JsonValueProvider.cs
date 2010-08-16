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
namespace Magnum.ValueProviders
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using Extensions;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;


	public class JsonValueProvider :
		ValueProvider
	{
		readonly ValueProvider _provider;
		readonly IDictionary<string, object> _dictionary = new Dictionary<string, object>();

		public JsonValueProvider(Stream bodyStream)
		{
			LoadJsonObject(bodyStream);

			_provider = new DictionaryValueProvider(_dictionary);
		}

		public JsonValueProvider(string text)
		{
			LoadJsonObject(text);

			_provider = new DictionaryValueProvider(_dictionary);
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

		static DictionaryValueProvider CreateDictionaryFromJson(object data)
		{
			var backingStore = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

			AddValueToDictionary(backingStore, "", data);

			return new DictionaryValueProvider(backingStore);
		}

		static object GetDeserializedObject(Stream inputStream)
		{
			return inputStream.ReadJsonObject();
		}

		static void AddValueToDictionary(IDictionary<string, object> backingStore, string prefix, object value)
		{
			var dictionary = value as IDictionary<string,object>;
			if (dictionary != null)
			{
				dictionary.Each(x => AddValueToDictionary(backingStore, MakePropertyKey(prefix, x.Key), x.Value));
				return;
			}

			var list = value as IList<object>;
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
					AddValueToDictionary(backingStore, MakeArrayKey(prefix, i), list[i]);
				return;
			}

			backingStore[prefix] = value;
		}

		void LoadJsonObject(string text)
		{
			using (var stringReader = new StringReader(text))
				LoadJsonObject(stringReader);
		}

		void LoadJsonObject(Stream stream)
		{
			using (var reader = new StreamReader(stream))
				LoadJsonObject(reader);
		}

		void LoadJsonObject(TextReader textReader)
		{
			using (var jsonReader = new JsonTextReader(textReader))
			{
				ReadObject(jsonReader, (k,i) => k);
			}
		}

		void ReadObject(JsonReader reader, Func<string, int, string> keyFormatter)
		{
			int index = 0;
            while (reader.Read())
            {
				if (reader.TokenType == JsonToken.EndObject)
					return;

				if (reader.TokenType == JsonToken.EndArray)
					return;

            	string key = null;
				if (reader.TokenType == JsonToken.PropertyName)
				{
					key = reader.Value.ToString();
					reader.Read();
				}

            	if (reader.TokenType == JsonToken.StartObject)
            	{
					ReadObject(reader, (k, i) =>
						{
							string prefix = keyFormatter(key, index);

							return prefix == null ? k : prefix + "." + k;
						});
            	}
				else if (reader.TokenType == JsonToken.StartArray)
				{
					ReadObject(reader, (k, i) =>
					{
						string prefix = keyFormatter(key, index);

						if (prefix.IsEmpty())
							prefix = "";

						prefix = prefix + "[" + i.ToString(CultureInfo.InvariantCulture) + "]";
						if (k.IsNotEmpty())
							prefix = prefix + "." + k;

						return prefix;
					});
				}
				else
				{
					_dictionary.Add(keyFormatter(key, index), reader.Value);
				}

            	index++;
            }
        }

		static string MakeArrayKey(string prefix, int index)
		{
			return prefix + "[" + index.ToString(CultureInfo.InvariantCulture) + "]";
		}

		static string MakePropertyKey(string prefix, string propertyName)
		{
			return prefix.IsEmpty() ? propertyName : prefix + "." + propertyName;
		}
	}
}