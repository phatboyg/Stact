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
namespace Magnum.Serialization.TypeSerializers
{
	using System.Collections.Generic;

	public abstract class ArraySerializer<T, TElement> :
		EnumerableSerializerBase<T>
	{
		private readonly TypeSerializer<TElement> _elementTypeSerializer;

		public ArraySerializer(TypeSerializer<TElement> elementTypeSerializer)
			: base(typeof (T[]), elementTypeSerializer)
		{
			_elementTypeSerializer = elementTypeSerializer;
		}

		public override TypeWriter<T> GetWriter()
		{
			TypeWriter<TElement> elementWriter = _elementTypeSerializer.GetWriter();

			return (value, output) =>
				{
					foreach (TElement obj in ((IEnumerable<TElement>) value))
					{
						elementWriter(obj, output);
					}
				};
		}
	}
}