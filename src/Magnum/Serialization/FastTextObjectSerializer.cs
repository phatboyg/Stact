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
	using System.Runtime.Serialization;
	using System.Text;
	using Reflection;

	public class FastTextObjectSerializer<T> :
		ObjectSerializer<T>
		where T : class
	{
		public FastTextObjectSerializer(PropertyTypeSerializerCache typeSerializerCache)
			: base(typeSerializerCache)
		{
		}

		public override TypeReader<T> GetReader()
		{
			return StringToInstance;
		}

		public override TypeWriter<T> GetWriter()
		{
			TypeWriter<T> baseWriter = base.GetWriter();

			return (value, output) =>
				{
					var sb = new StringBuilder(2048);

					sb.Append(FastTextSerializer.MapStartString);

					bool addSeparator = false;

					baseWriter(value, text =>
						{
							if (addSeparator)
								sb.Append(FastTextSerializer.ItemSeparatorString);
							else
								addSeparator = true;

							sb.Append(text);
						});

					sb.Append(FastTextSerializer.MapEndString);

					output(sb.ToString());
				};
		}

		private T StringToInstance(string value)
		{
			if (value[0] != FastTextSerializer.MapStartChar)
				throw new SerializationException(string.Format(
				                                 	"Type definitions should start with a '{0}', expecting serialized type '{1}', got string starting with: {2}",
				                                 	FastTextSerializer.MapStartChar, ObjectType.Name,
				                                 	value.Substring(0, value.Length < 50 ? value.Length : 50)));

			T instance = FastActivator<T>.Create();

			string propertyName;

			try
			{
				if (value == FastTextSerializer.EmptyMap)
					return instance;

				int length = value.Length;
				for (int index = 1; index < length; index++)
				{
					propertyName = ReadMapKey(value, ref index);
					index++;

					string propertyValueString = ReadMapValue(value, ref index);

					Properties.WithValue(propertyName, serializer => { serializer.Read(instance, propertyValueString); });
				}
			}
			catch (Exception ex)
			{
				throw;
			}
			return instance;
		}

		public static string ReadMapKey(string value, ref int index)
		{
			int start = index;
			int length = value.Length;
			while (value[++index] != FastTextSerializer.MapKeySeparator)
			{
			}
			return value.Substring(start, index - start);
		}

		public static string ReadMapValue(string value, ref int index)
		{
			int tokenStartPos = index;
			int valueLength = value.Length;
			if (index == valueLength) return null;

			char valueChar = value[index];

			//If we are at the end, return.
			if (valueChar == FastTextSerializer.ItemSeparator
			    || valueChar == FastTextSerializer.MapEndChar)
			{
				return null;
			}

			//Is List, i.e. [...]
			bool withinQuotes = false;
			if (valueChar == FastTextSerializer.ListStartChar)
			{
				int endsToEat = 1;
				while (++index < valueLength && endsToEat > 0)
				{
					valueChar = value[index];

					if (valueChar == FastTextSerializer.QuoteChar)
						withinQuotes = !withinQuotes;

					if (withinQuotes)
						continue;

					if (valueChar == FastTextSerializer.ListStartChar)
						endsToEat++;

					if (valueChar == FastTextSerializer.ListEndChar)
						endsToEat--;
				}
				return value.Substring(tokenStartPos, index - tokenStartPos);
			}

			//Is Type/Map, i.e. {...}
			if (valueChar == FastTextSerializer.MapStartChar)
			{
				int endsToEat = 1;
				while (++index < valueLength && endsToEat > 0)
				{
					valueChar = value[index];

					if (valueChar == FastTextSerializer.QuoteChar)
						withinQuotes = !withinQuotes;

					if (withinQuotes)
						continue;

					if (valueChar == FastTextSerializer.MapStartChar)
						endsToEat++;

					if (valueChar == FastTextSerializer.MapEndChar)
						endsToEat--;
				}
				return value.Substring(tokenStartPos, index - tokenStartPos);
			}


			//Is Within Quotes, i.e. "..."
			if (valueChar == FastTextSerializer.QuoteChar)
			{
				while (++index < valueLength)
				{
					valueChar = value[index];

					if (valueChar != FastTextSerializer.QuoteChar) continue;

					bool isLiteralQuote = index + 1 < valueLength && value[index + 1] == FastTextSerializer.QuoteChar;

					index++; //skip quote
					if (!isLiteralQuote)
						break;
				}
				return value.Substring(tokenStartPos, index - tokenStartPos);
			}

			//Is Value
			while (++index < valueLength)
			{
				valueChar = value[index];

				if (valueChar == FastTextSerializer.ItemSeparator
				    || valueChar == FastTextSerializer.MapEndChar)
				{
					break;
				}
			}

			return value.Substring(tokenStartPos, index - tokenStartPos);
		}
	}
}