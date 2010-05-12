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
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using Collections;
	using Extensions;
	using Reflection;
	using TypeSerializers;

	public class FastTextTypeSerializerCache :
		TypeSerializerCache
	{
		private static Cache<Type, FastTextTypeSerializer> _serializers;
		private readonly PropertyTypeSerializerCache _propertyTypeSerializerCache;

		public FastTextTypeSerializerCache(TypeSerializerCache typeSerializerCache)
		{
			_propertyTypeSerializerCache = new FastTextPropertyTypeSerializerCache(this);

			_serializers = new Cache<Type, FastTextTypeSerializer>(CreateSerializerFor);

			typeSerializerCache.Each((type, serializer) => _serializers.Add(type, CreateSerializerFor(type, serializer)));

			_serializers[typeof (string)] = new FastTextTypeSerializer<string>(new FastTextStringSerializer());
		}

		public FastTextTypeSerializer this[Type type]
		{
			get { return _serializers[type]; }
		}

		public TypeSerializer<T> GetTypeSerializer<T>()
		{
			return _serializers[typeof (T)] as TypeSerializer<T>;
		}

		public void Each(Action<Type, TypeSerializer> action)
		{
			_serializers.Each((type, serializer) => action(type, serializer as TypeSerializer));
		}

		private static FastTextTypeSerializer CreateSerializerFor(Type type, TypeSerializer serializer)
		{
			return (FastTextTypeSerializer) FastActivator.Create(typeof (FastTextTypeSerializer<>), new[] {type}, new object[] {serializer});
		}

		private FastTextTypeSerializer CreateSerializerFor(Type type)
		{
			type = Nullable.GetUnderlyingType(type) ?? type;

			if (type.IsEnum)
				return CreateEnumSerializerFor(type);

			if (typeof (IEnumerable).IsAssignableFrom(type))
				return CreateEnumerableSerializer(type);

			return CreateObjectSerializerFor(type);
		}

		private FastTextTypeSerializer CreateObjectSerializerFor(Type type)
		{
			var serializer =
				(TypeSerializer)
				FastActivator.Create(typeof (FastTextObjectSerializer<>), new[] {type}, new object[] {_propertyTypeSerializerCache});
			return CreateSerializerFor(type, serializer);
		}

		private static FastTextTypeSerializer CreateEnumSerializerFor(Type type)
		{
			return CreateSerializerFor(type, CreateGenericSerializer(typeof (EnumSerializer<>), type));
		}

		private static TypeSerializer CreateGenericSerializer(Type genericType, Type type)
		{
			return (TypeSerializer) FastActivator.Create(genericType.MakeGenericType(type));
		}

		private FastTextTypeSerializer CreateArraySerializer(Type type, Type elementType)
		{
			FastTextTypeSerializer elementSerializer = this[elementType];

			object serializer = FastActivator.Create(typeof (FastTextArraySerializer<>), new[] {elementType},
			                                         new object[] {elementSerializer});


			return CreateSerializerFor(type, (TypeSerializer) serializer);
		}

		private FastTextTypeSerializer CreateListSerializer(Type type, Type elementType)
		{
			FastTextTypeSerializer elementSerializer = this[elementType];

			object serializer = FastActivator.Create(typeof (FastTextListSerializer<>), new[] {elementType},
			                                         new object[] {elementSerializer});


			return CreateSerializerFor(type, (TypeSerializer) serializer);
		}

		private FastTextTypeSerializer CreateDictionarySerializer(Type type, Type keyType, Type elementType)
		{
			FastTextTypeSerializer keySerializer = this[keyType];
			FastTextTypeSerializer elementSerializer = this[elementType];

			object serializer = FastActivator.Create(typeof (FastTextDictionarySerializer<,>), new[] {keyType, elementType},
			                                         new object[] {keySerializer, elementSerializer});


			return CreateSerializerFor(type, (TypeSerializer) serializer);
		}

		private FastTextTypeSerializer CreateEnumerableSerializer(Type type)
		{
			if (type.IsArray)
				return CreateArraySerializer(type, type.GetElementType());

			Type[] genericArguments = type.GetDeclaredGenericArguments().ToArray();
			if (genericArguments == null || genericArguments.Length == 0)
			{
				Type elementType = type.IsArray ? type.GetElementType() : typeof (object);

				return CreateArraySerializer(type, elementType);
			}

			if (type.ImplementsGeneric(typeof (IDictionary<,>)))
			{
				return CreateDictionarySerializer(type, genericArguments[0], genericArguments[1]);
			}

			if (type.ImplementsGeneric(typeof (IList<>)) || type.ImplementsGeneric(typeof (IEnumerable<>)))
			{
				return CreateListSerializer(type, genericArguments[0]);
			}

			throw new InvalidOperationException("The type of enumeration is not supported: " + type.FullName);
		}
	}
}