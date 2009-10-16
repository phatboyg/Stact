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
namespace Magnum.Reflection
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using CollectionExtensions;

	public static class ClassFactory
	{
		private const BindingFlags _bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		private static readonly Dictionary<Type, InstanceFactory> _builders = new Dictionary<Type, InstanceFactory>();

		public static void Reset()
		{
			_builders.Clear();
		}

		public static object New(Type type)
		{
			return GetInstanceFactory(type).New();
		}

		public static T New<T>()
		{
			return (T) GetInstanceFactory(typeof (T)).New();
		}

		public static T New<T>(params object[] args)
		{
			return (T) GetInstanceFactory(typeof (T)).New(args);
		}

		public static object New(Type type, params object[] args)
		{
			if (type.IsGenericType)
			{
				return NewGeneric(type, args);
			}

			if (args == null)
				throw new ArgumentNullException("args");

			return GetInstanceFactory(type).New(args);
		}

		private static object NewGeneric(Type type, object[] args)
		{
			ConstructorInfo ctor = type.GetConstructors(_bindingFlags)
				.FindBestMatch(args);

			var argumentTypes = type.GetGenericArguments()
				.GetGenericArgumentTypes(ctor.GetParameters(), args, null).ToArray();

			Type makeType = type.MakeGenericType(argumentTypes);

			return GetInstanceFactory(makeType).New(args);
		}

		private static InstanceFactory GetInstanceFactory(Type type)
		{
			return _builders.Retrieve(type, () =>
				{
					Type builderType = typeof (TypeInstanceFactory<>).MakeGenericType(type);

					var constructor = builderType.GetConstructors(_bindingFlags)
						.Where(x => x.GetParameters().Length == 0)
						.First();

					var func = Expression.Lambda<Func<InstanceFactory>>(Expression.New(constructor)).Compile();

					return func();
				});
		}
	}
}