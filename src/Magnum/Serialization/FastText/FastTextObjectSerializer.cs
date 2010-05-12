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
		FastTextParser,
		TypeSerializer<T>
		where T : class
	{
		private ObjectSerializer<T> _objectSerializer;
		private PropertySerializerCache<T> _properties;

		public FastTextObjectSerializer(PropertyTypeSerializerCache typeSerializerCache)
		{
			_objectSerializer = new ObjectSerializer<T>(typeSerializerCache);
			_properties = _objectSerializer.Properties;
		}

		public TypeReader<T> GetReader()
		{
			return StringToInstance;
		}

		public TypeWriter<T> GetWriter()
		{
			return (value, output) =>
				{
					var sb = new StringBuilder(2048);

					sb.Append(MapStartString);

					bool addSeparator = false;

					_properties.Each(serializer =>
						{
							serializer.Write(value, text =>
								{
									if (addSeparator)
										sb.Append(ItemSeparatorString);
									else
										addSeparator = true;

									sb.Append(text);
								});
						});

					sb.Append(MapEndString);

					output(sb.ToString());
				};
		}

		private T StringToInstance(string text)
		{
			if (text[0] != MapStart)
			{
				string message =
					string.Format("Types should start with a '{0}', expecting serialized type '{1}', got string starting with: {2}",
					              MapStart, typeof (T).Name,
					              text.Substring(0, text.Length < 50 ? text.Length : 50));
				throw new SerializationException(message);
			}

			T instance = FastActivator<T>.Create();

			try
			{
				ReadMap(text, (key, value) => _properties.WithValue(key, serializer => serializer.Read(instance, value)));
			}
			catch (Exception ex)
			{
				throw TypeSerializerException.New(this, text, ex);
			}
			return instance;
		}
	}
}