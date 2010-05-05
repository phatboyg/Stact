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
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using Collections;
	using Extensions;
	using Reflection;
	using TypeSerializers;

	public class FastTextTypeSerializerCache
	{
		private static readonly IDictionary<Type, FastTextTypeSerializer> _defaultSerializers;
		private readonly Cache<Type, FastTextTypeSerializer> _typeSerializers;

		static FastTextTypeSerializerCache()
		{
			_defaultSerializers = new TypeSerializerLoader().LoadBuiltInTypeSerializers();
		}

		public FastTextTypeSerializerCache()
		{
			_typeSerializers = new Cache<Type, FastTextTypeSerializer>(_defaultSerializers, CreateSerializerFor);

			_typeSerializers[typeof (string)] = new FastTextTypeSerializer<string>(new QuotedStringSerializer());
		}

		public FastTextTypeSerializer this[Type type]
		{
			get { return _typeSerializers[type]; }
		}

		private FastTextTypeSerializer CreateSerializerFor(Type type)
		{
			if (type.IsEnum)
			{
				return (FastTextTypeSerializer) FastActivator.Create(typeof (EnumSerializer<>).MakeGenericType(type));
			}

			if (typeof (IEnumerable).IsAssignableFrom(type))
			{
				return CreateEnumerableSerializerFor(type);
			}

			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>))
			{
				Type nullableType = type.GetGenericArguments().First();

				return _typeSerializers[nullableType];
			}

			Type serializerType = typeof (ObjectSerializer<>).MakeGenericType(type);

			var serializer = (FastTextTypeSerializer) FastActivator.Create(serializerType);

			return serializer;
		}

		private static FastTextTypeSerializer CreateEnumerableSerializerFor(Type type)
		{
			if (type.IsArray)
			{
				return (FastTextTypeSerializer) FastActivator.Create(typeof (ArraySerializer<>).MakeGenericType(type.GetElementType()));
			}

			Type[] genericArguments = type.GetDeclaredGenericArguments().ToArray();
			if (genericArguments == null || genericArguments.Length == 0)
			{
				Type elementType = type.IsArray ? type.GetElementType() : typeof (object);

				Type serializerType = typeof (ArraySerializer<>).MakeGenericType(elementType);

				return (FastTextTypeSerializer) FastActivator.Create(serializerType);
			}

			if (type.ImplementsGeneric(typeof (IDictionary<,>)))
			{
				Type serializerType = typeof (DictionarySerializer<,>).MakeGenericType(genericArguments);

				return (FastTextTypeSerializer) FastActivator.Create(serializerType);
			}

			if (type.ImplementsGeneric(typeof (IList<>)) || type.ImplementsGeneric(typeof (IEnumerable<>)))
			{
				Type serializerType = typeof (ListSerializer<>).MakeGenericType(genericArguments[0]);

				return (FastTextTypeSerializer) FastActivator.Create(serializerType);
			}

			throw new InvalidOperationException("The type of enumeration is not supported: " + type.FullName);
		}
	}
}