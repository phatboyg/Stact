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
namespace Magnum.Serialization.FastText
{
	using System;
	using System.Runtime.Serialization;
	using System.Text;
	using Reflection;
	using TypeSerializers;

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
			return (value, output) =>
				{
					var sb = new StringBuilder(2048);

					sb.Append(FastTextSerializer.MapStartString);

					bool addSeparator = false;

					Properties.Each(serializer =>
						{
							serializer.Write(value, text =>
								{
									if (addSeparator)
										sb.Append(FastTextSerializer.ItemSeparatorString);
									else
										addSeparator = true;

									sb.Append(text);
								});
						});

					sb.Append(FastTextSerializer.MapEndString);

					output(sb.ToString());
				};
		}

		private T StringToInstance(string value)
		{
			if (value[0] != FastTextSerializer.MapStart)
				throw new SerializationException(string.Format(
					"Type definitions should start with a '{0}', expecting serialized type '{1}', got string starting with: {2}",
					FastTextSerializer.MapStart, ObjectType.Name,
					value.Substring(0, value.Length < 50 ? value.Length : 50)));

			T instance = FastActivator<T>.Create();

			try
			{
				if (value == FastTextSerializer.EmptyMap)
					return instance;

				int length = value.Length;
				for (int index = 1; index < length; index++)
				{
					string propertyName = ReadMapKey(value, ref index);
					index++;

					string propertyValueString = ReadMapValue(value, ref index);

					Properties.WithValue(propertyName, serializer => { serializer.Read(instance, propertyValueString); });
				}
			}
			catch (Exception ex)
			{
				throw TypeSerializerException.New(this, value, ex);
			}
			return instance;
		}

		private static string ReadMapKey(string value, ref int index)
		{
			int start = index;
			while (value[++index] != FastTextSerializer.MapSeparator)
			{
			}
			return value.Substring(start, index - start);
		}

		private static string ReadMapValue(string value, ref int index)
		{
			int start = index;
			int length = value.Length;
			if (index == length)
				return null;

			string result;

			char ch = value[index];
			if (ch == FastTextSerializer.ItemSeparator || ch == FastTextSerializer.MapEnd)
				return null;

			if (TryReadListValue(value, ref index, out result))
				return result;

			if (TryReadMapValue(value, ref index, out result))
				return result;

			if (TryReadQuotedValue(value, ref index, out result))
				return result;

			while (++index < length)
			{
				ch = value[index];

				if (ch == FastTextSerializer.ItemSeparator || ch == FastTextSerializer.MapEnd)
					break;
			}

			return value.Substring(start, index - start);
		}

		private static bool TryReadListValue(string value, ref int index, out string result)
		{
			result = null;

			char ch = value[index];
			if (ch != FastTextSerializer.ListStart)
				return false;

			bool inQuote = false;
			int depth = 1;

			int start = index;
			int length = value.Length;
			while (++index < length && depth > 0)
			{
				ch = value[index];

				if (ch == FastTextSerializer.Quote)
					inQuote = !inQuote;

				if (inQuote)
					continue;

				if (ch == FastTextSerializer.ListStart)
					depth++;

				if (ch == FastTextSerializer.ListEnd)
					depth--;
			}

			result = value.Substring(start, index - start);
			return true;
		}

		private static bool TryReadMapValue(string value, ref int index, out string result)
		{
			result = null;

			char ch = value[index];
			if (ch != FastTextSerializer.MapStart)
				return false;

			bool inQuote = false;
			int depth = 1;

			int start = index;
			int length = value.Length;
			while (++index < length && depth > 0)
			{
				ch = value[index];

				if (ch == FastTextSerializer.Quote)
					inQuote = !inQuote;

				if (inQuote)
					continue;

				if (ch == FastTextSerializer.MapStart)
					depth++;

				if (ch == FastTextSerializer.MapEnd)
					depth--;
			}

			result = value.Substring(start, index - start);
			return true;
		}

		private static bool TryReadQuotedValue(string value, ref int index, out string result)
		{
			result = null;

			if (value[index] != FastTextSerializer.Quote)
				return false;

			int start = index;
			int length = value.Length;
			while (++index < length)
			{
				if (value[index] != FastTextSerializer.Quote)
					continue;

				bool isDoubleQuote = index + 1 < length && value[index + 1] == FastTextSerializer.Quote;

				++index; // skip quote/escaped quote
				if (!isDoubleQuote)
					break;
			}

			result = value.Substring(start, index - start);
			return true;
		}
	}
}