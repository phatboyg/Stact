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
			InstanceFactory factory = GetInstanceFactory(type);

			return factory.New();
		}

		public static T New<T>()
		{
			InstanceFactory factory = GetInstanceFactory(typeof (T));

			return (T)factory.New();
		}

		public static object New(Type type, params object[] args)
		{
			if(type.IsGenericType)
			{
				return NewGeneric(type, args);
			}

			if (args == null)
				throw new ArgumentNullException("args");

			InstanceFactory factory = GetInstanceFactory(type);

			return factory.New(args);
		}

		private static object NewGeneric(Type type, object[] args)
		{
			Type[] arguments = type.GetGenericArguments();


			Type[] argTypes = new Type[args.Length];
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] == null)
					throw new ArgumentNullException("args[" + i + "]", "Argument cannot be null");

				argTypes[i] = args[i].GetType();
			}

			ConstructorInfo[] constructors = type.GetConstructors(_bindingFlags);

			foreach (var constructor in constructors)
			{
				ParameterInfo[] parameters = constructor.GetParameters();

				if (parameters.Length != args.Length) continue;

				int typeArgumentCount = 0;
				Type[] typeArguments = new Type[arguments.Length];
				for (int i = 0; i < parameters.Length; i++)
				{
					if (typeArgumentCount == arguments.Length)
						break;

					for (int j = 0; j < arguments.Length; j++)
					{
						if (typeArguments[j] != null)
							continue;

						if (parameters[i].ParameterType == arguments[j])
						{
							typeArguments[j] = argTypes[i];
							typeArgumentCount++;
							break;
						}
					}
				}

				for (int i = 0; i < arguments.Length; i++)
				{
					if (typeArgumentCount == arguments.Length)
						break;
					if (typeArguments[i] != null)
						continue;

					for (int j = 0; j < arguments.Length; j++)
					{
						if (i == j)
							continue;

						foreach (Type constraint in arguments[j].GetGenericParameterConstraints())
						{
							if (constraint.IsGenericType)
							{
								foreach (Type argument in constraint.GetGenericArguments())
								{
									if(argument == arguments[i])
									{
										/// need to pull the type from the argument that matches this position and get its implementation of that interface to get the generic type
									}
								}
							}
						}
					}
				}

				if(typeArgumentCount == arguments.Length)
				{
					Type makeType = type.MakeGenericType(typeArguments);

					InstanceFactory factory = GetInstanceFactory(makeType);

					return factory.New(args);

//					ConstantExpression[] constructorArgs = new ConstantExpression[arguments.Length];
//					for (int i = 0; i < arguments.Length; i++)
//					{
//						constructorArgs[i] = Expression.Constant(typeArguments[i], typeof (Type));
//					}
//
//					var e = Expression.Lambda<Func<InstanceFactory>>(Expression.New(constructor, constructorArgs)).Compile();
//					return e();
				}
			}

			throw new InvalidOperationException("Unable to create the object given the specific arguments");
		}

		private static InstanceFactory GetInstanceFactory(Type type)
		{
			return _builders.Retrieve(type, () =>
				{
					Type builderType = typeof (TypeInstanceFactory<>).MakeGenericType(type);

					ConstructorInfo[] constructors = builderType.GetConstructors(_bindingFlags);

					foreach (var constructor in constructors)
					{
						if (constructor.GetParameters().Length != 0) continue;

						var e = Expression.Lambda<Func<InstanceFactory>>(Expression.New(constructor)).Compile();

						return e();
					}

					throw new InvalidOperationException("For some reason, the TypeInstanceFactory for the specified type could not be created");
				});
		}
	}
}