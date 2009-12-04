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
namespace Magnum.Generator
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using InterfaceExtensions;
	using Linq;
	using Reflection;

	public static class TypeSpecializationExtensions
	{
		public static Type ToSpecializedType(this ConstructorInfo constructor, IEnumerable<object> args)
		{
			Guard.Against.Null(constructor, "constructor");

			Type type = constructor.DeclaringType;
			if (!type.IsGenericType)
				throw new ArgumentException("The argument must be for a generic type", "constructor");

			var parameters = constructor.GetParameters()
				.Merge(args, (x, y) => new { Parameter = x, Argument = y});

			Type[] genericArguments = type.GetGenericArguments()
				.Select(argumentType =>
					{
						Type result = parameters.Where(arg => arg.Parameter.ParameterType == argumentType && arg.Argument != null)
							.Select(arg => arg.Argument.GetType())
							.FirstOrDefault();

						if(result != null)
							return result;

						throw new ObjectGeneratorException(type, "Could not create a specialized type for argument: " + argumentType.Name);
					})
				.ToArray();

			return type.MakeGenericType(genericArguments);
		}


		public static IEnumerable<Type> GetGenericArgumentTypes(this IEnumerable<Type> arguments,
	IEnumerable<ParameterInfo> parameters,
	object[] args,
	Type[] argumentTypes)
		{
			var generics = new Dictionary<Type, Type>();

			arguments.Each(argument =>
			{
				parameters.Where(parameter => parameter.ParameterType == argument)
					.Each(parameter =>
					{
						Type parameterType = args[parameter.Position].GetType();

						generics[argument] = parameterType;

						argument.GetGenericParameterConstraints()
							.Where(x => x.IsGenericType)
							.Each(constraint =>
							{
								var declared = constraint.GetDeclaredGenericArguments().ToArray();
								var specified = parameterType
									.GetDeclaredTypesForGeneric(constraint.GetGenericTypeDefinition()).ToArray();

								if (declared.Length == specified.Length)
								{
									for (int i = 0; i < declared.Length; i++)
									{
										if (arguments.Contains(declared[i]))
											generics[declared[i]] = specified[i];
									}
								}
							});
					});

				parameters.Where(parameter => parameter.ParameterType != argument && parameter.ParameterType.IsGenericType)
					.Each(parameter =>
					{
						Type parameterType = args[parameter.Position].GetType();

						var declared = parameter.ParameterType.GetDeclaredGenericArguments().ToArray();
						var specified = parameterType.GetDeclaredTypesForGeneric(parameterType.GetGenericTypeDefinition()).ToArray();

						if (declared.Length == specified.Length)
						{
							for (int i = 0; i < declared.Length; i++)
							{
								if (arguments.Contains(declared[i]))
									generics[declared[i]] = specified[i];
							}
						}
					});
			});

			var methodTypes = arguments.Select(x => generics.ContainsKey(x) ? generics[x] : null).ToArray();

			if (argumentTypes != null)
			{
				for (int i = 0; i < methodTypes.Length && i < argumentTypes.Length; i++)
				{
					if (methodTypes[i] == null)
						methodTypes[i] = argumentTypes[i];
				}
			}

			return methodTypes;
		}

	}
}