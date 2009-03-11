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
namespace Magnum.Specs.Reflection
{
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using System.Reflection;
	using CollectionExtensions;

	public static class ObjectBuilder
	{
		private const BindingFlags _bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		private static readonly Dictionary<Type, InstanceBuilder> _builders = new Dictionary<Type, InstanceBuilder>();

		public static void Reset()
		{
			_builders.Clear();
		}

		public static object New(Type type)
		{
			InstanceBuilder builder = GetInstanceBuilder(type);

			return builder.New();
		}

		private static InstanceBuilder GetInstanceBuilder(Type type)
		{
			return _builders.Retrieve(type, () =>
				{
					Type builderType = typeof (TypeInstanceBuilder<>).MakeGenericType(type);

					ConstructorInfo[] constructors = builderType.GetConstructors(_bindingFlags);

					foreach (var constructor in constructors)
					{
						if (constructor.GetParameters().Length != 0) continue;

						Func<InstanceBuilder> e = Expression.Lambda<Func<InstanceBuilder>>(Expression.New(constructor)).Compile();

						return e();
					}

					throw new NotSupportedException("Unable to build object, no default constructor was found");
				});
		}

		private static InstanceBuilder GetInstanceBuilder(Type type, Type[] genericArguments)
		{
			return _builders.Retrieve(type, () =>
				{
					Type genericType = type.MakeGenericType(genericArguments);

					Type builderType = typeof(TypeInstanceBuilder<>).MakeGenericType(genericType);

					ConstructorInfo[] constructors = builderType.GetConstructors(_bindingFlags);

					foreach (var constructor in constructors)
					{
						if (constructor.GetParameters().Length != 0) continue;

						Func<InstanceBuilder> e = Expression.Lambda<Func<InstanceBuilder>>(Expression.New(constructor)).Compile();

						return e();
					}

					throw new NotSupportedException("Unable to build object, no default constructor was found");
				});
		}

		public static object New(Type type, params object[] args)
		{
			InstanceBuilder builder = GetInstanceBuilder(type, new[] {args[0].GetType()});

			return builder.New(args);




			
		}
	}
}