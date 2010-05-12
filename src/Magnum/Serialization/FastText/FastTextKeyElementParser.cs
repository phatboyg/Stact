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
	using TypeSerializers;

	public class FastTextKeyElementParser<TKey, TElement> :
		FastTextElementParser<TElement>
	{
		public FastTextKeyElementParser(TypeSerializer<TKey> keyTypeSerializer, TypeSerializer<TElement> elementTypeSerializer)
			: base(elementTypeSerializer)
		{
			KeyTypeSerializer = keyTypeSerializer;
			KeyWriter = KeyTypeSerializer.GetWriter();
			KeyReader = KeyTypeSerializer.GetReader();
		}

		protected TypeSerializer<TKey> KeyTypeSerializer { get; private set; }
		protected TypeWriter<TKey> KeyWriter { get; private set; }
		protected TypeReader<TKey> KeyReader { get; private set; }

		protected Dictionary<TKey, TElement> DictionaryReader(string text)
		{
			var elements = new Dictionary<TKey, TElement>();

			if (text.IsEmpty())
				return elements;

			try
			{
				ReadMap(text, (keyText, valueText) =>
					{
						TKey key = KeyReader(keyText);
						TElement value = ElementReader(valueText);

						elements[key] = value;
					});
			}
			catch (Exception ex)
			{
				throw TypeSerializerException.New<IDictionary<TKey, TElement>>(text, ex);
			}

			return elements;
		}


		protected void DictionaryWriter(IDictionary<TKey, TElement> value, Action<string> output)
		{
			var sb = new StringBuilder(2048);

			sb.Append(MapStart);

			bool addSeparator = false;

			foreach (var obj in value)
			{
				KeyWriter(obj.Key, text =>
					{
						if (addSeparator)
							sb.Append(ItemSeparatorString);
						else
							addSeparator = true;

						sb.Append(text);
					});

				sb.Append(MapSeparatorString);

				ElementWriter(obj.Value, text =>
					{
						//
						sb.Append(text ?? MapNullValue);
					});
			}

			sb.Append(MapEnd);

			output(sb.ToString());
		}
	}
}