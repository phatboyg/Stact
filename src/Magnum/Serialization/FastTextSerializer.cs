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
namespace Magnum.Serialization
{
	using System;
	using System.IO;
	using System.Text;
	using FastText;

	public class FastTextSerializer :
		Serializer
	{
		public const string DoubleQuoteString = "\"\"";
		public const string EmptyMap = "{}";
		public const char ItemSeparator = ',';
		public const string ItemSeparatorString = ",";

		public const char ListEnd = ']';
		public const char ListStart = '[';
		public const char MapEnd = '}';
		public const string MapEndString = "}";
		public const char MapSeparator = ':';
		public const string MapSeparatorString = ":";
		public const char MapStart = '{';
		public const string MapStartString = "{";
		public const char Quote = '"';
		public const string QuoteString = "\"";

		public static readonly char[] EscapeChars = new[] {Quote, ItemSeparator, MapStart, MapEnd, ListStart, ListEnd};

		[ThreadStatic]
		private static TypeSerializerCache _typeSerializerCache;

		[ThreadStatic]
		private static FastTextTypeSerializerCache _typeSerializers;

		public void Serialize<T>(T obj, TextWriter writer)
		{
			FastTextTypeSerializer serializer = GetTypeSerializer(typeof (T));

			serializer.Serialize(obj, writer);
		}

		public string Serialize<T>(T obj)
		{
			var sb = new StringBuilder(2048);
			using (var writer = new StringWriter(sb))
			{
				Serialize(obj, writer);
			}

			return sb.ToString();
		}

		public T Deserialize<T>(string text)
		{
			FastTextTypeSerializer serializer = GetTypeSerializer(typeof (T));

			return serializer.Deserialize<T>(text);
		}

		public T Deserialize<T>(TextReader reader)
		{
			FastTextTypeSerializer serializer = GetTypeSerializer(typeof (T));

			return serializer.Deserialize<T>(reader.ReadToEnd());
		}

		private static FastTextTypeSerializer GetTypeSerializer(Type type)
		{
			if (_typeSerializers == null)
				_typeSerializers = new FastTextTypeSerializerCache(GetTypeSerializerCache());

			return _typeSerializers[type];
		}

		private static TypeSerializerCache GetTypeSerializerCache()
		{
			if (_typeSerializerCache == null)
				_typeSerializerCache = new TypeSerializerCacheImpl();

			return _typeSerializerCache;
		}
	}
}