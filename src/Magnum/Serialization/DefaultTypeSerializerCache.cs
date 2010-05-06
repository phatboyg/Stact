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
	using System.Reflection;
	using Collections;

	public class DefaultTypeSerializerCache :
		TypeSerializerCache
	{
		private static readonly Cache<Type, TypeSerializer> _defaultSerializers;

		static DefaultTypeSerializerCache()
		{
			_defaultSerializers = new Cache<Type, TypeSerializer>(new TypeSerializerLoader().LoadBuiltInTypeSerializers());
		}

		public TypeSerializer<T> GetTypeSerializer<T>()
		{
			return _defaultSerializers[typeof (T)] as TypeSerializer<T>;
		}

		/// <summary>
		///   The default implmementation does not do any special handling of the property
		/// </summary>
		/// <typeparam name = "T"></typeparam>
		/// <param name = "property"></param>
		/// <returns></returns>
		public TypeSerializer<T> GetTypeSerializer<T>(PropertyInfo property)
		{
			return _defaultSerializers[typeof (T)] as TypeSerializer<T>;
		}

		public void Each(Action<Type, TypeSerializer> action)
		{
			_defaultSerializers.Each(action);
		}
	}
}