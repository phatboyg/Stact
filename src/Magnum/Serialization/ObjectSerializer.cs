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
	using System.Collections.Generic;
	using System.Text;

	public class ObjectSerializer<T> :
		TypeSerializer<T>
		where T : class
	{
		private readonly Type _type;
		private readonly PropertySerializerCache<T> _properties;

		public ObjectSerializer(PropertyTypeSerializerCache typeSerializerCache)
		{
			_type = typeof (T);
			if (!_type.IsClass && !_type.IsInterface)
				throw new ArgumentException("Only classes and interfaces can be serialized by an object serializer, not: " + _type.FullName);

			_properties = new PropertySerializerCache<T>(typeSerializerCache);
		}

		public virtual TypeReader<T> GetReader()
		{
			// TODO
			return text => default(T);
		}

		public virtual TypeWriter<T> GetWriter()
		{
			return (value, output) =>
				{
					if (value == null)
						return;

					StringBuilder sb = new StringBuilder(1024);
					string separator = null;

					_properties.Each(serializer =>
						{
							serializer.Write(value, text => sb.Append(separator).Append(text));

							separator = separator ?? FastTextSerializer.ItemSeparatorString;
						});

					output(sb.ToString());
				};
		}
	}
}