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
	using System.Collections.Generic;
	using System.Text;
	using Extensions;

	public class FastTextAbstractEnumerableSerializer<T>
	{
		public FastTextAbstractEnumerableSerializer(TypeSerializer<T> elementTypeSerializer)
		{
			ElementTypeSerializer = elementTypeSerializer;
			ElementWriter = ElementTypeSerializer.GetWriter();
			ElementReader = ElementTypeSerializer.GetReader();

			StringReader = new FastTextStringSerializer().GetReader();
		}

		public TypeReader<string> StringReader { get; private set; }
		protected TypeSerializer<T> ElementTypeSerializer { get; private set; }
		protected TypeWriter<T> ElementWriter { get; private set; }
		protected TypeReader<T> ElementReader { get; private set; }

		protected List<T> ListReader(string value)
		{
			var elements = new List<T>();

			if (value.IsEmpty())
				return elements;

			if (value[0] == FastTextSerializer.MapStart)
			{
				int index = 0;
				int length = value.Length;
				do
				{
					//string elementText = ReadTypeValue();
					//elements.Add();

					throw new NotImplementedException();
				} while (++index < length);
			}
			else
			{
				int length = value.Length;
				for (int index = 0; index < length; index++)
				{
					string elementText = ReadToChar(value, ref index, FastTextSerializer.ItemSeparator);
					string clearText = StringReader(elementText);

					T element = clearText.IsEmpty() ? default(T) : ElementReader(clearText);

					elements.Add(element);
				}
			}

			return elements;
		}

		protected void ListWriter(IEnumerable<T> value, Action<string> output)
		{
			var sb = new StringBuilder(2048);

			sb.Append(FastTextSerializer.ListStart);

			bool addSeparator = false;

			foreach (T obj in value)
			{
				ElementWriter(obj, text =>
					{
						if (addSeparator)
							sb.Append(FastTextSerializer.ItemSeparatorString);
						else
							addSeparator = true;

						sb.Append(text);
					});
			}

			sb.Append(FastTextSerializer.ListEnd);

			output(sb.ToString());
		}


		private static string ReadToChar(string value, ref int index, char separator)
		{
			int start = index;
			int length = value.Length;

			if (value[start] != FastTextSerializer.Quote)
			{
				index = value.IndexOf(separator, start);
				if (index == -1)
					index = length;

				return value.Substring(start, index - start);
			}

			while (++index < length)
			{
				if (value[index] == FastTextSerializer.Quote
				    && (index + 1 >= length || value[index + 1] == separator))
				{
					index++;
					return value.Substring(start, index - start);
				}
			}

			throw new IndexOutOfRangeException("The ending quote character was not found.");
		}

		protected static string RemoveListChars(string value)
		{
			if (value.IsEmpty())
				return null;

			return value[0] == FastTextSerializer.ListStart ? value.Substring(1, value.Length - 2) : value;
		}
	}
}